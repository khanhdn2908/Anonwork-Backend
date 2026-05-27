# Payment Integration - Implementation Summary

## ✅ Hoàn thành

Tôi đã tạo hoàn chỉnh hệ thống thanh toán tích hợp Sepay Gateway với các tính năng:

### 1. **DTOs** (Data Transfer Objects)
- `CreateOrderRequest.cs` - Request tạo order
- `OrderResponse.cs` - Response order
- `SepayPaymentResponse.cs` - Response từ Sepay API
- `SepayWebhookRequest.cs` - Webhook từ Sepay

### 2. **Interfaces** (Application Layer)
- `ISepayService.cs` - Interface cho Sepay service
- `IOrderRepository.cs` - Repository cho Order
- `ISubscriptionPlanRepository.cs` - Repository cho SubscriptionPlan
- `IUserSubscriptionRepository.cs` - Repository cho UserSubscription

### 3. **Use Cases** (Business Logic)
- `CreateOrderUseCase.cs` - Tạo order & gọi Sepay API
- `GetOrderStatusUseCase.cs` - Lấy trạng thái order
- `HandleSepayWebhookUseCase.cs` - Xử lý webhook từ Sepay (verify signature + tạo subscription)
- `RenewSubscriptionUseCase.cs` - Gia hạn subscription

### 4. **Services** (Infrastructure Layer)
- `SepayService.cs` - Gọi Sepay API + verify webhook signature
- `SubscriptionRenewalService.cs` - Background job auto-renew subscriptions

### 5. **Repositories** (Data Access)
- `OrderRepository.cs` - CRUD Order
- `SubscriptionPlanRepository.cs` - CRUD SubscriptionPlan
- `UserSubscriptionRepository.cs` - CRUD UserSubscription

### 6. **API Controller**
- `PaymentController.cs` - 4 endpoints:
  - `POST /api/v1/payments/create-order` - Tạo order
  - `GET /api/v1/payments/orders/{orderId}` - Lấy trạng thái
  - `POST /api/v1/payments/webhook` - Webhook từ Sepay
  - `POST /api/v1/payments/subscriptions/{subscriptionId}/renew` - Gia hạn

### 7. **Configuration**
- `SepayOptions.cs` - Config class
- `appsettings.json` - Updated với Sepay config
- `appsettings.Development.json` - Updated với Sepay API key/secret

### 8. **Dependency Injection**
- Updated `Application/DependencyInjection.cs` - Register use cases
- Updated `Infrastructure/DependencyInjection.cs` - Register services, repositories, background job

### 9. **Documentation**
- `docs/PAYMENT_SEPAY_INTEGRATION.md` - Hướng dẫn chi tiết

---

## 🎯 Tính năng

### ✅ Tạo Order
- Validate subscription plan
- Generate unique order code
- Gọi Sepay API để tạo payment link
- Lưu order vào database

### ✅ Verify Webhook Signature
- Sử dụng HMAC-SHA256
- Verify signature từ Sepay
- Reject nếu signature không hợp lệ

### ✅ Xử lý Webhook
- Verify signature
- Update order status (paid/failed)
- Tự động tạo user subscription khi thanh toán thành công
- Tính toán expiration date dựa trên plan duration

### ✅ Auto-Renewal
- Background service chạy mỗi 1 giờ
- Tìm subscriptions hết hạn
- Tự động gia hạn (update expires_at)

### ✅ Lấy Trạng thái Order
- Kiểm tra authorization
- Return order details

---

## 📋 API Endpoints

### 1. Create Order
```
POST /api/v1/payments/create-order
Authorization: Bearer {token}

Request:
{
  "planId": "uuid"
}

Response (201):
{
  "id": "uuid",
  "userId": "uuid",
  "planId": "uuid",
  "orderCode": "ORD...",
  "amount": 99000,
  "currency": "VND",
  "status": "pending",
  "sepayTransactionId": "SEP...",
  "expiresAt": "2026-05-27T...",
  "createdAt": "2026-05-26T...",
  "updatedAt": "2026-05-26T..."
}
```

### 2. Get Order Status
```
GET /api/v1/payments/orders/{orderId}
Authorization: Bearer {token}

Response (200):
{
  "id": "uuid",
  "status": "paid",
  ...
}
```

### 3. Webhook (từ Sepay)
```
POST /api/v1/payments/webhook

Request:
{
  "transactionId": "SEP...",
  "orderCode": "ORD...",
  "amount": 99000,
  "status": "success",
  "timestamp": 1234567890,
  "signature": "hex..."
}

Response (200):
{
  "success": true
}
```

### 4. Renew Subscription
```
POST /api/v1/payments/subscriptions/{subscriptionId}/renew
Authorization: Bearer {token}

Response (200):
{
  "success": true
}
```

---

## 🔧 Configuration

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

### Environment Variables (Optional)
```
SEPAY_API_KEY=your-api-key
SEPAY_API_SECRET=your-api-secret
```

---

## 🔐 Security

✅ **Webhook Signature Verification**
- Sử dụng HMAC-SHA256
- Verify mỗi webhook từ Sepay
- Reject nếu signature không hợp lệ

✅ **Authorization**
- Tất cả endpoints (trừ webhook) yêu cầu JWT token
- Chỉ user tạo order mới có thể xem status

✅ **Order Expiration**
- Order pending tự động expire sau 24 giờ
- Cleanup job có thể được thêm sau

✅ **HTTPS Only**
- Tất cả API calls phải dùng HTTPS

---

## 📊 Database

### Orders Table
- `id` - UUID
- `user_id` - Reference to users
- `plan_id` - Reference to subscription_plans
- `order_code` - Unique order code
- `amount` - Price in VND
- `status` - pending, paid, failed, refunded, expired
- `sepay_transaction_id` - Transaction ID từ Sepay
- `expires_at` - Order expiration time
- `paid_at` - Payment time
- `created_at`, `updated_at`

### Subscription Plans Table
- `id` - UUID
- `name` - Plan name
- `slug` - URL slug
- `price` - Price in VND
- `duration_days` - Subscription duration
- `features` - JSONB features
- `is_active` - Active status

### User Subscriptions Table
- `id` - UUID
- `user_id` - Reference to users
- `plan_id` - Reference to subscription_plans
- `order_id` - Reference to orders
- `status` - active, expired, cancelled
- `started_at` - Start date
- `expires_at` - Expiration date
- `created_at`

---

## 🚀 Cách Sử Dụng

### 1. Cấu hình Sepay API Key
```
Cập nhật appsettings.Development.json:
- ApiKey: your-sepay-api-key
- ApiSecret: your-sepay-api-secret
- NotifyUrl: https://yourapp.com/api/v1/payments/webhook
```

### 2. Tạo Subscription Plans
```sql
INSERT INTO subscription_plans (name, slug, price, duration_days, is_active)
VALUES 
  ('Premium', 'premium', 99000, 30, true),
  ('Pro', 'pro', 199000, 30, true);
```

### 3. Client Flow
```
1. User chọn plan
2. POST /api/v1/payments/create-order
3. Nhận payment link từ response
4. Redirect user đến payment link
5. User thanh toán trên Sepay
6. Sepay gọi webhook
7. Client poll GET /api/v1/payments/orders/{orderId}
8. Khi status = paid, show subscription activated
```

### 4. Auto-Renewal
```
- Background service chạy mỗi 1 giờ
- Tự động gia hạn subscriptions hết hạn
- Không tính tiền (chỉ update expires_at)
```

---

## ⚠️ Lưu Ý

### Hiện tại
- Auto-renewal chỉ gia hạn subscription, không tính tiền
- Webhook không có retry logic (nên thêm sau)
- Không có cleanup job cho expired orders

### Cần thêm sau
1. **Recurring Billing** - Tự động tính tiền hàng tháng
2. **Refund Handling** - Xử lý hoàn tiền
3. **Webhook Retry** - Retry nếu webhook fail
4. **Cleanup Job** - Xóa expired orders
5. **Payment History** - Lưu lịch sử thanh toán
6. **Invoice Generation** - Tạo hóa đơn
7. **Promo Codes** - Mã giảm giá
8. **Multiple Payment Methods** - Thêm cổng thanh toán khác

---

## 📚 Files Created

### Application Layer
- `src/Anonwork.Application/Features/Payments/DTOs/CreateOrderRequest.cs`
- `src/Anonwork.Application/Features/Payments/DTOs/OrderResponse.cs`
- `src/Anonwork.Application/Features/Payments/DTOs/SepayPaymentResponse.cs`
- `src/Anonwork.Application/Features/Payments/DTOs/SepayWebhookRequest.cs`
- `src/Anonwork.Application/Features/Payments/CreateOrderUseCase.cs`
- `src/Anonwork.Application/Features/Payments/GetOrderStatusUseCase.cs`
- `src/Anonwork.Application/Features/Payments/HandleSepayWebhookUseCase.cs`
- `src/Anonwork.Application/Features/Payments/RenewSubscriptionUseCase.cs`
- `src/Anonwork.Application/Interfaces/ISepayService.cs`
- `src/Anonwork.Application/Interfaces/IOrderRepository.cs`
- `src/Anonwork.Application/Interfaces/ISubscriptionPlanRepository.cs`
- `src/Anonwork.Application/Interfaces/IUserSubscriptionRepository.cs`

### Infrastructure Layer
- `src/Anonwork.Infrastructure/Services/SepayService.cs`
- `src/Anonwork.Infrastructure/Services/SubscriptionRenewalService.cs`
- `src/Anonwork.Infrastructure/Common/SepayOptions.cs`
- `src/Anonwork.Infrastructure/Repositories/OrderRepository.cs`
- `src/Anonwork.Infrastructure/Repositories/SubscriptionPlanRepository.cs`
- `src/Anonwork.Infrastructure/Repositories/UserSubscriptionRepository.cs`

### API Layer
- `src/Anonwork.API/Controllers/PaymentController.cs`

### Configuration
- `src/Anonwork.API/appsettings.json` (updated)
- `src/Anonwork.API/appsettings.Development.json` (updated)

### Dependency Injection
- `src/Anonwork.Application/DependencyInjection.cs` (updated)
- `src/Anonwork.Infrastructure/DependencyInjection.cs` (updated)

### Documentation
- `docs/PAYMENT_SEPAY_INTEGRATION.md`

---

## ✨ Tóm tắt

Hệ thống thanh toán Sepay đã được tích hợp hoàn chỉnh với:
- ✅ 4 API endpoints
- ✅ Webhook signature verification
- ✅ Auto-renewal service
- ✅ Full error handling
- ✅ Comprehensive documentation

**Tiếp theo:** Cập nhật Sepay API key/secret trong appsettings, test các endpoints, và deploy!
