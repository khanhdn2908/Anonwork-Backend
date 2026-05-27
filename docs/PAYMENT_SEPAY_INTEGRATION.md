# Payment Integration - Sepay Gateway

## Overview

Hệ thống thanh toán tích hợp Sepay Gateway cho phép người dùng mua các gói subscription.

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    API Controller                        │
│              (PaymentController.cs)                      │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Application Layer (Use Cases)               │
│  - CreateOrderUseCase                                   │
│  - GetOrderStatusUseCase                                │
│  - HandleSepayWebhookUseCase                            │
│  - RenewSubscriptionUseCase                             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Infrastructure Layer                        │
│  - ISepayService / SepayService                         │
│  - IOrderRepository / OrderRepository                   │
│  - ISubscriptionPlanRepository / SubscriptionPlanRepo   │
│  - IUserSubscriptionRepository / UserSubscriptionRepo   │
│  - SubscriptionRenewalService (Background Job)          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              External API                                │
│              Sepay Gateway                              │
└─────────────────────────────────────────────────────────┘
```

## Database Schema

### Orders Table
```sql
CREATE TABLE orders (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  plan_id UUID,
  order_code VARCHAR(50) UNIQUE,
  amount BIGINT,
  currency VARCHAR(10),
  status VARCHAR(20), -- pending, paid, failed, refunded, expired
  payment_method VARCHAR(30),
  sepay_transaction_id VARCHAR(100),
  metadata JSONB,
  expires_at TIMESTAMPTZ,
  paid_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ,
  updated_at TIMESTAMPTZ
);
```

### Subscription Plans Table
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

### User Subscriptions Table
```sql
CREATE TABLE user_subscriptions (
  id UUID PRIMARY KEY,
  user_id UUID NOT NULL,
  plan_id UUID NOT NULL,
  order_id UUID NOT NULL,
  status VARCHAR(20), -- active, expired, cancelled
  started_at TIMESTAMPTZ,
  expires_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ
);
```

## API Endpoints

### 1. Create Order (Tạo đơn hàng)
```
POST /api/v1/payments/create-order
Authorization: Bearer {token}

Request:
{
  "planId": "uuid"
}

Response (201 Created):
{
  "id": "uuid",
  "userId": "uuid",
  "planId": "uuid",
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

### 2. Get Order Status (Lấy trạng thái đơn hàng)
```
GET /api/v1/payments/orders/{orderId}
Authorization: Bearer {token}

Response (200 OK):
{
  "id": "uuid",
  "userId": "uuid",
  "planId": "uuid",
  "orderCode": "ORD1234567890",
  "amount": 99000,
  "currency": "VND",
  "status": "paid",
  "paymentMethod": "bank_transfer",
  "sepayTransactionId": "SEP123456",
  "expiresAt": "2026-05-27T10:00:00Z",
  "paidAt": "2026-05-26T10:05:00Z",
  "createdAt": "2026-05-26T10:00:00Z",
  "updatedAt": "2026-05-26T10:05:00Z"
}
```

### 3. Sepay Webhook (Nhận thông báo từ Sepay)
```
POST /api/v1/payments/webhook
Content-Type: application/json

Request:
{
  "transactionId": "SEP123456",
  "orderCode": "ORD1234567890",
  "amount": 99000,
  "status": "success",
  "description": "Payment for subscription",
  "timestamp": 1234567890,
  "signature": "hex_signature"
}

Response (200 OK):
{
  "success": true
}
```

### 4. Renew Subscription (Gia hạn subscription)
```
POST /api/v1/payments/subscriptions/{subscriptionId}/renew
Authorization: Bearer {token}

Response (200 OK):
{
  "success": true
}
```

## Payment Flow

### 1. User tạo order
```
Client → POST /api/v1/payments/create-order
  ↓
Server:
  - Validate plan exists
  - Generate unique order code
  - Create order (status: pending)
  - Call Sepay API to create payment
  - Save Sepay transaction ID
  ↓
Response: Order details + Payment link
```

### 2. User thanh toán qua Sepay
```
Client → Open payment link
  ↓
Sepay Gateway → Process payment
  ↓
Payment success/failed
```

### 3. Sepay gọi webhook
```
Sepay → POST /api/v1/payments/webhook
  ↓
Server:
  - Verify webhook signature
  - Find order by order code
  - If status = success:
    - Update order (status: paid)
    - Create user subscription (status: active)
  - If status = failed:
    - Update order (status: failed)
  ↓
Response: 200 OK
```

### 4. Client poll order status
```
Client → GET /api/v1/payments/orders/{orderId}
  ↓
Server: Return order status
  ↓
If status = paid:
  - Show subscription activated
  - Redirect to dashboard
```

## Configuration

### appsettings.json
```json
{
  "Sepay": {
    "ApiKey": "your-sepay-api-key",
    "ApiSecret": "your-sepay-api-secret",
    "ApiUrl": "https://api.sepay.vn/v3",
    "ReturnUrl": "https://yourapp.com/payment/success",
    "NotifyUrl": "https://yourapp.com/api/v1/payments/webhook"
  }
}
```

### Environment Variables
```
SEPAY_API_KEY=your-api-key
SEPAY_API_SECRET=your-api-secret
```

## Sepay API Integration

### Create Payment
```
POST https://api.sepay.vn/v3/payment/create
Authorization: Bearer {ApiKey}

Request:
{
  "amount": 99000,
  "description": "Thanh toán gói Premium - 30 ngày",
  "orderCode": "ORD1234567890",
  "returnUrl": "https://yourapp.com/payment/success",
  "notifyUrl": "https://yourapp.com/api/v1/payments/webhook",
  "buyerName": "Customer",
  "buyerEmail": "customer@example.com"
}

Response:
{
  "success": true,
  "paymentLink": "https://sepay.vn/pay/...",
  "transactionId": "SEP123456"
}
```

### Get Transaction Detail
```
GET https://api.sepay.vn/v3/payment/detail/{transactionId}
Authorization: Bearer {ApiKey}

Response:
{
  "success": true,
  "transactionId": "SEP123456",
  "status": "success",
  "amount": 99000
}
```

## Webhook Signature Verification

Sepay sử dụng HMAC-SHA256 để ký webhook:

```csharp
// Signature string format:
// {transactionId}|{orderCode}|{amount}|{status}|{timestamp}

// Compute signature:
var signatureString = $"{transactionId}|{orderCode}|{amount}|{status}|{timestamp}";
using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
{
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
    var computedSignature = Convert.ToHexString(hash).ToLower();
    
    bool isValid = computedSignature == receivedSignature.ToLower();
}
```

## Auto-Renewal Service

Background service `SubscriptionRenewalService` chạy mỗi 1 giờ để:
1. Tìm các subscription hết hạn (status = active, expires_at < now)
2. Tự động gia hạn (update expires_at = now + duration_days)

**Lưu ý:** Hiện tại auto-renewal không tính tiền. Bạn cần thêm logic để:
- Tạo order mới
- Gọi Sepay API để tính tiền
- Hoặc sử dụng subscription lặp lại (recurring billing)

## Error Handling

### Order Creation Errors
- Plan not found → 404 Not Found
- Plan not active → 400 Bad Request
- Sepay API error → 400 Bad Request

### Webhook Errors
- Invalid signature → 401 Unauthorized
- Order not found → 404 Not Found
- Plan not found → 404 Not Found

### Subscription Renewal Errors
- Subscription not found → 404 Not Found
- Plan not found → 404 Not Found

## Testing

### Test Create Order
```bash
curl -X POST http://localhost:5000/api/v1/payments/create-order \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"planId": "uuid"}'
```

### Test Get Order Status
```bash
curl -X GET http://localhost:5000/api/v1/payments/orders/{orderId} \
  -H "Authorization: Bearer {token}"
```

### Test Webhook (Mock)
```bash
curl -X POST http://localhost:5000/api/v1/payments/webhook \
  -H "Content-Type: application/json" \
  -d '{
    "transactionId": "SEP123456",
    "orderCode": "ORD1234567890",
    "amount": 99000,
    "status": "success",
    "description": "Payment for subscription",
    "timestamp": 1234567890,
    "signature": "computed_signature"
  }'
```

## Security Considerations

1. **Webhook Signature Verification** - Luôn verify signature từ Sepay
2. **HTTPS Only** - Tất cả API calls phải dùng HTTPS
3. **API Key Protection** - Không commit API key vào git, dùng environment variables
4. **Order Expiration** - Order pending tự động expire sau 24 giờ
5. **User Authorization** - Chỉ user tạo order mới có thể xem status

## Future Enhancements

1. **Recurring Billing** - Tự động tính tiền hàng tháng
2. **Refund Handling** - Xử lý hoàn tiền
3. **Payment History** - Lưu lịch sử thanh toán
4. **Invoice Generation** - Tạo hóa đơn
5. **Multiple Payment Methods** - Thêm các cổng thanh toán khác
6. **Subscription Tiers** - Nhiều mức subscription khác nhau
7. **Promo Codes** - Mã giảm giá
8. **Payment Analytics** - Thống kê doanh thu

## Troubleshooting

### Webhook không được gọi
- Kiểm tra NotifyUrl trong config
- Kiểm tra firewall/network
- Kiểm tra logs của Sepay

### Signature verification failed
- Kiểm tra ApiSecret có đúng không
- Kiểm tra format signature string
- Kiểm tra timestamp

### Order không được tạo
- Kiểm tra plan ID có tồn tại không
- Kiểm tra plan có active không
- Kiểm tra Sepay API key/secret

## References

- [Sepay API Documentation](https://sepay.vn/docs)
- [HMAC-SHA256 Verification](https://en.wikipedia.org/wiki/HMAC)
- [ASP.NET Core Background Tasks](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
