using Grpc.Net.Client;
using OrderService;

var channel = GrpcChannel.ForAddress("http://localhost:5002");
var client = new Order.OrderClient(channel);

var reply = await client.CreateOrderAsync(new OrderRequest 
{ 
    ProductName = "Laptop", 
    Price = 15000 
});

Console.WriteLine($"Sipariş Sonucu: OrderId={reply.OrderId}, Status={reply.Status}");
