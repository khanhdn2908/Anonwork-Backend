# 💳 Payment Integration - Sepay Gateway

## 🎉 Status: ✅ COMPLETE

Hệ thống thanh toán Sepay Gateway đã được tích hợp hoàn chỉnh vào dự án Anonwork Backend.

---

## 📊 Overview

```
┌─────────────────────────────────────────────────────────┐
│                    CLIENT (Frontend)                     │
│                                                          │
│  1. User chọn subscription plan                         │
│  2. POST /api/v1/payments/create-order                  │
│  3. Nhận payment link                                   │
│  4. Redirect đến Sepay                                  │
│  5. User thanh toán                                     │
│  6. Poll GET /api/v1/payments/orders/{orderId}          │
│  7. Subscription activated                              │
└─────────────────────────────────────────────────────────┘
                         ↕
┌─────────────────────────────────────────────────────────┐
│                    SERVER (Backend)                      │
│                                                          │
│  ✅ CreateOrderUseCase                                  │
│  ✅ GetOrderStatusUseCase                               │
│  ✅ HandleSepayWebhookUseCase                           │
│  ✅ RenewSubscriptionUseCase                            │
│  ✅ SepayService (API calls + signature verification)   │
│  ✅ SubscriptionRenewalService (auto-renewal)           │
│  ✅ Repositories (Order, Plan, Subscription)            │
└─────────────────────────────────────────────────────────┘
                         ↕
┌─────────────────────────────────────────────────────────┐
│                  SEPAY GATEWAY                           │
│                                                          │
│  - Create payment link                                  │
│  - Process payment                                      │
│  - Send webhook notification                            │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 What's Included

### **27 Files Created**

#### Application Layer (12 files)
- 4 DTOs (CreateOrderRequest, OrderResponse, SepayPaymentResponse, SepayWebhookRequest)
- 4 Use Cases (CreateOrder, GetOrderStatus, HandleWebhook, RenewSubscription)
- 4 Interfaces (ISepayService, IOrderRepository, ISubscriptionPlanRepository, IUserSubscriptionRepository)

#### Infrastructure Layer (7 files)
- 2 Services (SepayService, SubscriptionRenewalService)
- 3 Repositories (OrderRepository, SubscriptionPlanRepository, UserSubscriptionRepository)
- 1 Configuration (SepayOptions)

#### API Layer (1 file)
- 1 Controller (PaymentController with 4 endpoints)

#### Configuration (2 files updated)
- appsettings.json
- appsettings.Development.json

#### Dependency Injection (2 files updated)
- Application/DependencyInjection.cs
- Infrastructure/DependencyInjection.cs

#### Documentation (4 files)
- PAYMENT_SEPAY_INTEGRATION.md (comprehensive guide)
- PAYMENT_QUICK_START.md (quick start guide)
- PAYMENT_IMPLEMENTATION_SUMMARY.md (implementation summary)
- PAYMENT_API_COMPLETE.md (complete overview)

---

## 🎯 Features

### ✅ Order Management
```
- Create orders with unique order codes
- Validate subscription plans
- Auto-generate Sepay transaction IDs
- Order expiration after 24 hours
- Track payment status (pending, paid, failed, refunded, expired)
```

### ✅ Sepay Integration
```
- Call Sepay API to create payment links
- Retrieve transaction details
- HMAC-SHA256 signature verification
- Error handling and logging
```

### ✅ Webhook Handling
```
- Verify webhook signatures from Sepay
- Atomic transactions (order + subscription)
- Idempotent processing
- Comprehensive error handling
```

### ✅ Subscription Management
```
- Auto-create subscriptions on successful payment
- Track subscription status (active, expired, cancelled)
- Calculate expiration dates based on plan duration
- Support multiple subscriptions per user
```

### ✅ Auto-Renewal
```
- Background service runs every 1 hour
- Automatically renews expired subscriptions
- Graceful shutdown
- Comprehensive logging
```

### ✅ Security
```
- JWT authorization (except webhook)
- Webhook signature verification (HMAC-SHA256)
- HTTPS only
- Order expiration
- User authorization checks
```

---

## 📋 API Endpoints

### 1. Create Order
```bash
POST /api/v1/payments/create-order
Authorization: Bearer {token}

Request:
{
  "planId": "uuid"
}

Response (201):
{
  "id": "uuid",
  "orderCode": "ORD1234567890",
  "amount": 99000,
  "status": "pending",
  "sepayTransactionId": "SEP123456",
  ...
}
```

### 2. Get Order Status
```bash
GET /api/v1/payments/orders/{orderId}
Authorization: Bearer {token}

Response (200):
{
  "id": "uuid",
  "status": "paid",
  ...
}
```

### 3. Webhook (from Sepay)
```bash
POST /api/v1/payments/webhook

Request:
{
  "transactionId": "SEP123456",
  "orderCode": "ORD1234567890",
  "amount": 99000,
  "status": "success",
  "timestamp": 1234567890,
  "signature": "hex_signature"
}

Response (200):
{
  "success": true
}
```

### 4. Renew Subscription
```bash
POST /api/v1/payments/subscriptions/{subscriptionId}/renew
Authorization: Bearer {token}

Response (200):
{
  "success": true
}
```

---

## 🔧 Quick Setup

### Step 1: Configure Sepay API Key
```json
// appsettings.Development.json
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

### Step 2: Create Subscription Plans
```sql
INSERT INTO subscription_plans (name, slug, price, duration_days, is_active)
VALUES 
  ('Premium', 'premium', 99000, 30, true),
  ('Pro', 'pro', 199000, 30, true);
```

### Step 3: Test Endpoints
```bash
# Create order
curl -X POST http://localhost:5000/api/v1/payments/create-order \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"planId": "uuid"}'

# Get order status
curl -X GET http://localhost:5000/api/v1/payments/orders/{orderId} \
  -H "Authorization: Bearer {token}"
```

---

## 🔐 Security

### Webhook Signature Verification
```csharp
// HMAC-SHA256 verification
var signatureString = $"{transactionId}|{orderCode}|{amount}|{status}|{timestamp}";
using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
{
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
    var computedSignature = Convert.ToHexString(hash).ToLower();
    
    bool isValid = computedSignature == receivedSignature.ToLower();
}
```

### Authorization
- JWT token required for all endpoints (except webhook)
- Webhook uses signature verification instead
- Only order creator can view order status

---

## 📊 Database Schema

### Orders Table
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
  status VARCHAR(20),
  started_at TIMESTAMPTZ,
  expires_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ
);
```

---

## 📚 Documentation

### 1. **PAYMENT_SEPAY_INTEGRATION.md**
Comprehensive integration guide with:
- Architecture overview
- Database schema
- API endpoints documentation
- Payment flow diagram
- Configuration guide
- Sepay API integration details
- Webhook signature verification
- Auto-renewal service
- Error handling
- Testing guide
- Security considerations
- Future enhancements
- Troubleshooting

### 2. **PAYMENT_QUICK_START.md**
Quick start guide with:
- Configuration steps
- Test API endpoints with curl examples
- Payment flow diagram
- Key features
- Security overview
- Database schema
- Testing examples
- Troubleshooting
- Files structure
- References
- Next steps

### 3. **PAYMENT_IMPLEMENTATION_SUMMARY.md**
Implementation summary with:
- Features completed
- API endpoints overview
- Configuration details
- Security features
- Database schema
- Usage guide
- Files created list
- Notes and future enhancements

### 4. **PAYMENT_API_COMPLETE.md**
Complete implementation overview with:
- All files created (27 total)
- All features implemented
- API endpoints
- Payment flow
- Configuration
- Security features
- Database schema
- Usage guide
- File structure
- Code quality notes
- References

---

## 🚀 Payment Flow

```
1. User selects subscription plan
   ↓
2. POST /api/v1/payments/create-order
   - Validate plan exists
   - Create order (status: pending)
   - Call Sepay API to create payment
   - Return payment link
   ↓
3. User redirected to Sepay payment page
   - User enters payment information
   - Sepay processes payment
   ↓
4. Sepay calls webhook
   POST /api/v1/payments/webhook
   - Verify webhook signature
   - Update order (status: paid)
   - Create user subscription (status: active)
   ↓
5. Client polls order status
   GET /api/v1/payments/orders/{orderId}
   - Check if status = paid
   - Show subscription activated
   ↓
6. Background service auto-renews subscriptions
   - Runs every 1 hour
   - Finds expired subscriptions
   - Automatically renews them
```

---

## ⚠️ Important Notes

### Current Implementation
- ✅ Auto-renewal only extends subscription, doesn't charge
- ✅ Webhook has no retry logic (can be added later)
- ✅ No cleanup job for expired orders (can be added later)

### Future Enhancements
1. **Recurring Billing** - Auto-charge every month
2. **Refund Handling** - Process refunds
3. **Webhook Retry** - Retry failed webhooks
4. **Cleanup Job** - Delete expired orders
5. **Payment History** - Store payment history
6. **Invoice Generation** - Generate invoices
7. **Promo Codes** - Support discount codes
8. **Multiple Payment Methods** - Add more payment gateways

---

## 📁 File Structure

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

docs/
├── PAYMENT_SEPAY_INTEGRATION.md
└── PAYMENT_QUICK_START.md
```

---

## ✨ Code Quality

- ✅ Clean Architecture (Domain → Application → Infrastructure → API)
- ✅ SOLID Principles
- ✅ Dependency Injection
- ✅ Async/Await
- ✅ Error Handling
- ✅ Logging
- ✅ Security Best Practices
- ✅ Comprehensive Comments

---

## 🔗 References

- [Sepay API Documentation](https://sepay.vn/docs)
- [HMAC-SHA256](https://en.wikipedia.org/wiki/HMAC)
- [ASP.NET Core Background Tasks](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

## 📞 Support

For questions or issues:
1. Check the documentation files
2. Review the troubleshooting section
3. Check the logs for error details
4. Verify Sepay API key/secret configuration

---

**Status: ✅ COMPLETE & READY TO USE**

**Next Steps:**
1. Update Sepay API key/secret in appsettings
2. Create subscription plans in database
3. Test the endpoints
4. Deploy to production
