# gRPC Mikroservis Örneği

.NET 10 ile gRPC kullanarak mikroservis mimarisi örneği. Order ve Payment servisleri arasında gRPC iletişimi.

## 📋 Proje Yapısı

```
gRPCMaster/
├── PaymentService/          # Ödeme servisi (Port: 5055)
│   ├── Protos/
│   │   └── payment.proto   # gRPC servis tanımı
│   └── Program.cs          # Minimal API implementasyonu
│
├── OrderService/           # Sipariş servisi (Port: 5056)
│   ├── Protos/
│   │   ├── order.proto    # gRPC servis tanımı
│   │   └── payment.proto  # Payment client için
│   └── Program.cs         # Minimal API implementasyonu
│
└── docker-compose.yml     # Docker orchestration
```

## 🚀 Servisler

### PaymentService (Port: 5055)
Ödeme işlemlerini yöneten servis.

**Metodlar:**
- `ProcessPayment` - Ödeme işleme
- `CheckBalance` - Bakiye sorgulama
- `ReversalPayment` - Ödeme iptali

### OrderService (Port: 5056)
Sipariş işlemlerini yöneten servis. PaymentService'e gRPC ile bağlanır.

**Metodlar:**
- `CreateOrder` - Sipariş oluşturma (Payment'a gRPC çağrısı yapar)
- `DeleteOrder` - Sipariş silme (Payment'ta ödeme iptali yapar)

## 🛠️ Gereksinimler

- .NET 10 SDK
- Docker Desktop (opsiyonel)
- grpcui (test için, opsiyonel)

## 📦 Kurulum

```bash
# Projeyi klonla
cd /Users/sametkustul/Repos/gRPCMaster

# Paketleri geri yükle
dotnet restore
```

## ▶️ Çalıştırma

### 1. Local Çalıştırma

**Terminal 1 - PaymentService:**
```bash
cd PaymentService
dotnet run
# http://localhost:5055 adresinde çalışır
```

**Terminal 2 - OrderService:**
```bash
cd OrderService
dotnet run
# http://localhost:5056 adresinde çalışır
```

### 2. Docker ile Çalıştırma

```bash
# Servisleri başlat
docker-compose up --build

# Arka planda çalıştır
docker-compose up -d

# Logları izle
docker-compose logs -f

# Durdur
docker-compose down
```

### 3. Rider/Visual Studio ile Debug

1. Solution'ı aç: `gRPCMaster.sln`
2. Run Configurations'dan `PaymentService` seç → Debug (F5)
3. Run Configurations'dan `OrderService` seç → Debug (F5)
4. Breakpoint koyarak debug yapabilirsin

## 🧪 Test Etme

### grpcui ile Test (Tavsiye Edilen)

```bash
# grpcui kur (macOS)
brew install grpcui

# PaymentService test
grpcui -plaintext localhost:5055

# OrderService test
grpcui -plaintext localhost:5056
```
<img width="544" height="866" alt="image" src="https://github.com/user-attachments/assets/e8c308a5-fb43-47da-a142-e911720c43db" />


Tarayıcıda otomatik açılır, Swagger benzeri UI ile test edebilirsin.

### Postman ile Test

1. New → gRPC Request
2. URL: `localhost:5055` veya `localhost:5056`
3. Method seç ve JSON gönder

### Örnek İstekler

**CreateOrder (OrderService):**
```json
{
  "product_name": "Laptop",
  "price": 15000
}
```

**ProcessPayment (PaymentService):**
```json
{
  "order_id": "abc-123",
  "amount": 15000
}
```

**DeleteOrder (OrderService):**
```json
{
  "product_name": "Laptop",
  "transaction_id": "xyz-789"
}
```

## 🔧 Teknolojiler

- **.NET 10** - Framework
- **gRPC** - Servisler arası iletişim
- **Protocol Buffers** - Veri serileştirme
- **Docker** - Containerization
- **Minimal API** - Basit ve hızlı API geliştirme

## 📝 gRPC Nasıl Çalışır?

1. **Proto Dosyası Yaz** (`payment.proto`)
   ```protobuf
   service Payment {
     rpc ProcessPayment (PaymentRequest) returns (PaymentResponse);
   }
   ```

2. **Build Et** - Otomatik C# kodu üretilir
   ```bash
   dotnet build
   ```

3. **Implement Et** - Üretilen base class'ı override et
   ```csharp
   class PaymentServiceEndpoints : Payment.PaymentBase
   {
       public override Task<PaymentResponse> ProcessPayment(...)
       {
           // İş mantığı
       }
   }
   ```

4. **Çağır** - Client tarafından kullan
   ```csharp
   var client = new Payment.PaymentClient(channel);
   var response = await client.ProcessPaymentAsync(request);
   ```

## 🔍 Servisler Arası İletişim

```
Client → OrderService (5056) → PaymentService (5055)
         [gRPC Request]         [gRPC Request]
         [gRPC Response] ←      [gRPC Response]
```

OrderService, PaymentService'e gRPC ile bağlanır:
- Docker'da: `http://payment-service:5055`
- Local'de: `http://localhost:5055`

## 📚 Daha Fazla Bilgi

- [gRPC Resmi Dokümantasyon](https://grpc.io/docs/)
- [.NET gRPC](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [Protocol Buffers](https://protobuf.dev/)

## 🐛 Sorun Giderme

**Port zaten kullanımda:**
```bash
# Port'u kullanan process'i bul
lsof -i :5055
lsof -i :5056

# Process'i durdur
kill -9 <PID>
```

**Docker build hatası (ARM64):**
Dockerfile'larda `--platform=linux/amd64` zaten ekli.

**grpcui bağlanamıyor:**
Servislerin `0.0.0.0` adresinde dinlediğinden emin ol (appsettings.json).
