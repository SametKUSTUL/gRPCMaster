using Grpc.Core;
using PaymentService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.MapGrpcService<PaymentServiceEndpoints>();
app.MapGrpcReflectionService();

var port = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5055";
Console.WriteLine($"Payment Service çalışıyor: {port}");
app.Run();

class PaymentServiceEndpoints : Payment.PaymentBase
{
    public override Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Ödeme işleniyor: OrderId={request.OrderId}, Tutar={request.Amount}₺");
        
        return Task.FromResult(new PaymentResponse
        {
            Success = true,
            TransactionId = Guid.NewGuid().ToString()
        });
    }

    public override Task<BalanceResponse> CheckBalance(BalanceRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Bakiye sorgulanıyor: UserId={request.UserId}");
        
        return Task.FromResult(new BalanceResponse
        {
            Balance = 1500.50
        });
    }

    public override Task<ReversalPaymentResponse> ReversalPayment(ReversalPaymentRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Ödeme iptal ediliyor: TransactionId={request.TransactionId}");
        
        return Task.FromResult(new ReversalPaymentResponse
        {
            Success = true,
            Message = "Ödeme başarıyla iptal edildi"
        });
    }
}
