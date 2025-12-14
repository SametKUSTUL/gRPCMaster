using Grpc.Core;
using Grpc.Net.Client;
using OrderService;
using PaymentService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Payment client'ı DI'a ekle
builder.Services.AddSingleton(sp =>
{
    var paymentUrl = builder.Configuration["PaymentService__Url"] ?? "http://localhost:5055";
    var channel = GrpcChannel.ForAddress(paymentUrl);
    return new Payment.PaymentClient(channel);
});

var app = builder.Build();

app.MapGrpcService<OrderServiceEndpoints>();
app.MapGrpcReflectionService();

var port = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5056";
Console.WriteLine($"Order Service çalışıyor: {port}");
app.Run();

class OrderServiceEndpoints : Order.OrderBase
{
    private readonly Payment.PaymentClient _paymentClient;

    public OrderServiceEndpoints(Payment.PaymentClient paymentClient)
    {
        _paymentClient = paymentClient;
    }

    public override async Task<OrderResponse> CreateOrder(OrderRequest request, ServerCallContext context)
    {
        var orderId = Guid.NewGuid().ToString();
        Console.WriteLine($"Sipariş oluşturuluyor: {request.ProductName} - {request.Price}₺");

        var paymentResponse = await _paymentClient.ProcessPaymentAsync(new PaymentRequest
        {
            OrderId = orderId,
            Amount = request.Price
        });

        var status = paymentResponse.Success ? "Ödeme Başarılı" : "Ödeme Başarısız";
        Console.WriteLine($"Ödeme sonucu: {status}, TransactionId: {paymentResponse.TransactionId}");

        return new OrderResponse
        {
            OrderId = orderId,
            Status = status
        };
    }

    public override async Task<DeleteOrderResponse> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Sipariş siliniyor: {request.ProductName}");
        Console.WriteLine($"Ödeme İptal Ediliyor: TransactionId: {request.TransactionId}");
        
        var reversalPaymentResponse = await _paymentClient.ReversalPaymentAsync(new ReversalPaymentRequest
        {
            TransactionId = request.TransactionId
        });
        
        Console.WriteLine($"İptal sonucu: {reversalPaymentResponse.Message}");
        
        return new DeleteOrderResponse
        {
            IsDeleted = reversalPaymentResponse.Success
        };
    }
}
