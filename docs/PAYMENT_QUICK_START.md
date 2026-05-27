# Payment Integration - Quick Start Guide

## 🚀 Bắt Đầu Nhanh

### 1. Cấu Hình Sepay API Key

**File:** `src/Anonwork.API/appsettings.Development.json`

```json
{
  "Sepay": {
    "ApiKey": "SP-LIVE-YOUR-API-KEY",
    "ApiSecret": "spsk_live_YOUR-API-SECRET",
    "ApiUrl": "https://api.sepay.vn/v3",
    "ReturnUrl": "https://yourapp.com/payment/success",
    "NotifyUrl": "https://yourapp.com/api/v1/payments/webhook"
  }
}
```

### 2. Tạo Subscription Plans

```sql
INSERT INTO subscription_plans (id, name, slug, price, duration_days, is_active, created_at)
VALUES 
  (gen_random_uuid(), 'Premium', 'premium', 99000, 30, true, NOW()),
  (gen_random_uuid(), 'Pro', 'pro', 199000, 30, true, NOW()),
  (gen_random_uuid(), 'Enterprise', 'enterprise', 499000, 30, true, NOW());
```

### 3. Test API Endpoints

#### Create Order
```bash
curl -X POST http://localhost:5000/api/v1/payments/create-order \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "planId": "YOUR_PLAN_UUID"
  }'
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "planId": "550e8400-e29b-41d4-a716-446655440002",
  "orderCode": "ORD1234567890",
  "amount": 99000,
  "currency": "VND",
  "status": "pending",
  "paymentMethod": "bank_transfer",
  "sepayTransactionId": "SEP123456",
  "expiresAt": "2026-05-27T10:00:00Z",
  "paidAt": null,
  "createdAt": "2026-05-26T10:00:00Z",
  "updatedAt": "2026-05-26T10:00:00Z"
}
```

#### Get Order Status
```bash
curl -X GET http://localhost:5000/api/v1/payments/orders/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### Renew Subscription
```bash
curl -X POST http://localhost:5000/api/v1/payments/subscriptions/550e8400-e29b-41d4-a716-446655440003/renew \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 📊 Payment Flow

```
┌─────────────────────────────────────────────────────────┐
│ 1. User chọn subscription plan                          │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│ 2. POST /api/v1/payments/create-order                   │
│    - Validate plan                                      │
│    - Create order (status: pending)                     │
│    - Call Sepay API                                     │
│    - Return payment link                                │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│ 3. User redirected to Sepay payment page                │
│    - User enters payment info                           │
│    - Sepay processes payment                            │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│ 4. Sepay calls webhook                                  │
│    POST /api/v1/payments/webhook                        │
│    - Verify signature                                   │
│    - Update order (status: paid)                        │
│    - Create user subscription                           │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│ 5. Client polls order status                            │
│    GET /api/v1/payments/orders/{orderId}                │
│    - Check if status = paid                             │
│    - Show subscription activated                        │
└─────────────────────────────────────────────────────────┘
```

---

## 🔑 Key Features

### ✅ Order Management
- Tạo order với unique order code
- Auto-generate Sepay transaction ID
- Order expiration sau 24 giờ
- Track payment status

### ✅ Webhook Handling
- Verify HMAC-SHA256 signature
- Atomic transaction (order + subscription)
- Error handling & logging
- Idempotent processing

### ✅ Subscription Management
- Auto-create subscription khi thanh toán thành công
- Track subscription status (active, expired, cancelled)
- Calculate expiration date based on plan duration
- Support multiple subscriptions per user

### ✅ Auto-Renewal
- Background service chạy mỗi 1 giờ
- Tự động gia hạn subscriptions hết hạn
- Logging & error handling
- Graceful shutdown

---

## 🔐 Security

### Webhook Signature Verification
```csharp
// Sepay sử dụng HMAC-SHA256
// Signature string: {transactionId}|{orderCode}|{amount}|{status}|{timestamp}

var signatureString = $"{transactionId}|{orderCode}|{amount}|{status}|{timestamp}";
using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
{
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
    var computedSignature = Convert.ToHexString(hash).ToLower();
    
    bool isValid = computedSignature == receivedSignature.ToLower();
}
```

### Authorization
- Tất cả endpoints (trừ webhook) yêu cầu JWT token
- Chỉ user tạo order mới có thể xem status
- Webhook không cần authorization (verify signature thay thế)

---

## 📝 Database Schema

### Orders
```sql
CREATE TABLE orders (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  plan_id UUID,
  order_code VARCHAR(50) UNIQUE,
  amount BIGINT,
  currency VARCHAR(10),
  status VARCHAR(20),
  payment_method VARCHAR(30),
  sepay_transaction_id VARCHAR(100),
  expires_at TIMESTAMPTZ,
  paid_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ,
  updated_at TIMESTAMPTZ
);
```

### Subscription Plans
```sql
CREATE TABLE subscription_plans (
  id UUID PRIMARY KEY,
  name VARCHAR(100),
  slug VARCHAR(50) UNIQUE,
  price BIGINT,
  duration_days INT,
  features JSONB,
  is_active BOOLEAN,
  created_at TIMESTAMPTZ
);
```

### User Subscriptions
```sql
CREATE TABLE user_subscriptions (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  plan_id UUID NOT NULL,
  order_id UUID NOT NULL,
  status VARCHAR(20),
  started_at TIMESTAMPTZ,
  expires_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ
);
```

---

## 🧪 Testing

### Test Create Order
```bash
# 1. Get JWT token
TOKEN=$(curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}' \
  | jq -r '.accessToken')

# 2. Get plan ID
PLAN_ID=$(curl -X GET http://localhost:5000/api/v1/subscriptions \
  | jq -r '.[0].id')

# 3. Create order
curl -X POST http://localhost:5000/api/v1/payments/create-order \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"planId\":\"$PLAN_ID\"}"
```

### Test Webhook (Mock)
```bash
# Compute signature
TRANSACTION_ID="SEP123456"
ORDER_CODE="ORD1234567890"
AMOUNT="99000"
STATUS="success"
TIMESTAMP="1234567890"
API_SECRET="your-api-secret"

SIGNATURE=$(echo -n "$TRANSACTION_ID|$ORDER_CODE|$AMOUNT|$STATUS|$TIMESTAMP" | \
  openssl dgst -sha256 -hmac "$API_SECRET" -hex | cut -d' ' -f2)

# Call webhook
curl -X POST http://localhost:5000/api/v1/payments/webhook \
  -H "Content-Type: application/json" \
  -d "{
    \"transactionId\":\"$TRANSACTION_ID\",
    \"orderCode\":\"$ORDER_CODE\",
    \"amount\":$AMOUNT,
    \"status\":\"$STATUS\",
    \"timestamp\":$TIMESTAMP,
    \"signature\":\"$SIGNATURE\"
  }"
```

---

## 🐛 Troubleshooting

### Order không được tạo
- ❌ Plan ID không tồn tại → Kiểm tra plan ID
- ❌ Plan không active → Kiểm tra `is_active` trong database
- ❌ Sepay API error → Kiểm tra API key/secret
- ❌ Network error → Kiểm trap firewall/proxy

### Webhook không được gọi
- ❌ NotifyUrl sai → Kiểm tra config
- ❌ Firewall blocking → Kiểm tra inbound rules
- ❌ Sepay API down → Kiểm tra Sepay status

### Signature verification failed
- ❌ API secret sai → Kiểm tra config
- ❌ Signature format sai → Kiểm tra format string
- ❌ Timestamp mismatch → Kiểm tra server time

---

## 📚 Files Structure

```
src/
├── Anonwork.Application/
│   ├── Features/Payments/
│   │   ├── DTOs/
│   │   │   ├── CreateOrderRequest.cs
│   │   │   ├── OrderResponse.cs
│   │   │   ├── SepayPaymentResponse.cs
│   │   │   └── SepayWebhookRequest.cs
│   │   ├── CreateOrderUseCase.cs
│   │   ├── GetOrderStatusUseCase.cs
│   │   ├── HandleSepayWebhookUseCase.cs
│   │   └── RenewSubscriptionUseCase.cs
│   └── Interfaces/
│       ├── ISepayService.cs
│       ├── IOrderRepository.cs
│       ├── ISubscriptionPlanRepository.cs
│       └── IUserSubscriptionRepository.cs
├── Anonwork.Infrastructure/
│   ├── Services/
│   │   ├── SepayService.cs
│   │   └── SubscriptionRenewalService.cs
│   ├── Repositories/
│   │   ├── OrderRepository.cs
│   │   ├── SubscriptionPlanRepository.cs
│   │   └── UserSubscriptionRepository.cs
│   └── Common/
│       └── SepayOptions.cs
└── Anonwork.API/
    └── Controllers/
        └── PaymentController.cs
```

---

## 🔗 References

- [Sepay API Documentation](https://sepay.vn/docs)
- [HMAC-SHA256 Verification](https://en.wikipedia.org/wiki/HMAC)
- [ASP.NET Core Background Tasks](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

## ✨ Next Steps

1. ✅ Cấu hình Sepay API key
2. ✅ Tạo subscription plans
3. ✅ Test create order endpoint
4. ✅ Test webhook handling
5. ⏳ Implement recurring billing (auto-charge)
6. ⏳ Add refund handling
7. ⏳ Add payment history
8. ⏳ Add invoice generation
