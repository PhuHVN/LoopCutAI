# LoopCutAI - Hệ Thống Quản Lý Đăng Ký Gói Dịch Vụ Tích Hợp AI

## 📋 Giới Thiệu

**LoopCutAI** là một hệ thống quản lý đăng ký gói dịch vụ hiện đại được xây dựng với công nghệ .NET/C# và tích hợp trí tuệ nhân tạo (AI). Hệ thống cung cấp các tính năng quản lý tài khoản, gói dịch vụ, thanh toán, đăng ký, và tương tác AI (chatbot) cho phép người dùng quản lý subscription một cách dễ dàng và hiệu quả.

Dự án được phát triển theo nguyên tắc **Clean Architecture**, đảm bảo code có thể mở rộng, bảo trì, và dễ kiểm thử.

---

## ✨ Tính Năng Chính

### 1. **Xác Thực & Bảo Mật (Authentication)**

- Đăng nhập/Đăng ký với JWT (JSON Web Token)
- Xác thực OAuth 2.0 với Google
- Quản lý token an toàn và refresh token
- Phân quyền người dùng (Role-based Access)

### 2. **Quản Lý Tài Khoản (Account Management)**

- Tạo, cập nhật, xóa tài khoản người dùng
- Quản lý hồ sơ cá nhân
- Xem thông tin cá nhân (Me endpoint)
- Xác thực email

### 3. **Quản Lý Gói Dịch Vụ (Service & Membership Management)**

- Tạo và quản lý các gói dịch vụ (Service Plans)
- Định nghĩa các dịch vụ riêng lẻ (Service Definitions)
- Thiết lập giá cả và các thuộc tính của gói
- Quản lý membership người dùng

### 4. **Quản Lý Đăng Ký (Subscription Management)**

- Đăng ký gói dịch vụ
- Hủy hoặc gia hạn đăng ký
- Theo dõi trạng thái subscription
- Quản lý lịch sử thanh toán
- **Gửi email thông báo tự động** khi gói sắp hết hạn

### 5. **Xử Lý Thanh Toán (Payment Processing)**

- Tích hợp cổng thanh toán **PayOS**
- Tạo đơn thanh toán
- Theo dõi trạng thái giao dịch
- Quản lý lịch sử thanh toán và hoàn tiền
- Webhook xử lý callback từ payment gateway

### 6. **Chatbot AI (Gemini Integration)**

- Chat real-time với AI assistant
- Sử dụng Google Gemini API
- Hỗ trợ người dùng 24/7
- Quản lý lịch sử cuộc hội thoại

### 7. **Quản Lý Logs & Monitoring**

- Ghi log chi tiết hoạt động hệ thống
- Lọc và tìm kiếm logs
- Theo dõi lỗi và sự cố
- Hỗ trợ audit trail

### 8. **Dịch Vụ Hỗ Trợ Khác**

- Gửi email (Email Service)
- Lưu trữ file (Storage Service)
- Xử lý tác vụ nền (Background Tasks)
- Seeding dữ liệu khởi tạo

---

## 🏗️ Kiến Trúc & Công Nghệ

### Kiến Trúc Clean Architecture

Dự án được chia thành **4 layer chính**:

```
LoopCut.API (Presentation Layer)
    ↓
LoopCut.Application (Application/Business Logic Layer)
    ↓
LoopCut.Domain (Domain/Business Rules Layer)
    ↓
LoopCut.Infrastructure (Data Access & External Services Layer)
```

#### **Tại sao chọn Clean Architecture?**

- ✅ **Tách biệt trách nhiệm**: Mỗi layer có trách nhiệm riêng, dễ hiểu
- ✅ **Dễ bảo trì**: Thay đổi logic không ảnh hưởng đến presentation
- ✅ **Dễ kiểm thử**: Có thể test business logic độc lập
- ✅ **Có thể mở rộng**: Thêm feature mới mà không phá vỡ code hiện tại
- ✅ **Độc lập framework**: Có thể thay đổi framework mà không ảnh hưởng logic

### Các Thư Viện & Công Nghệ Chính

| Công Nghệ                   | Mục Đích                                                |
| --------------------------- | ------------------------------------------------------- |
| **.NET 8 / C#**             | Nền tảng phát triển                                     |
| **Entity Framework Core**   | ORM (Object-Relational Mapping) cho database            |
| **FluentValidation**        | Validation dữ liệu đầu vào                              |
| **AutoMapper**              | Mapping giữa DTO và Entity                              |
| **MediatR**                 | CQRS pattern (Command Query Responsibility Segregation) |
| **PayOS**                   | Tích hợp cổng thanh toán                                |
| **Google Gemini API**       | AI chatbot                                              |
| **JWT Authentication**      | Xác thực token                                          |
| **CORS**                    | Cross-Origin Resource Sharing                           |
| **Swagger/OpenAPI**         | Tài liệu API tương tác                                  |
| **Docker & Docker Compose** | Container hóa ứng dụng                                  |

### Các Pattern & Best Practices

- **Repository Pattern**: Trừu tượng hóa data access
- **Unit of Work Pattern**: Quản lý transaction
- **Dependency Injection**: Quản lý dependencies
- **DTOs (Data Transfer Objects)**: Truyền dữ liệu giữa layer
- **Global Exception Handling**: Xử lý lỗi tập trung
- **Background Services**: Xử lý tác vụ không đồng bộ

---

## 🚀 Hướng Dẫn Chạy Dự Án

### Yêu Cầu Hệ Thống

- **Docker & Docker Compose** (Phương pháp khuyên dùng)
- Hoặc:
  - .NET 8 SDK
  - SQL Server (hoặc database được cấu hình trong `appsettings.json`)
  - Node.js (nếu có frontend)

### Cách 1: Chạy với Docker Compose (Nhanh nhất ⚡)

#### Bước 1: Chuẩn bị file `.env`

Tạo file `.env` ở thư mục gốc với tất cả các biến cần thiết:

```bash
# ===== DATABASE =====
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=sql_server;Database=LoopCutAI;User Id=sa;Password=YourSaPassword123;TrustServerCertificate=true;

# ===== JWT AUTHENTICATION =====
JWT__SECRETKEY=your_jwt_secret_key_min_32_characters_long
JWT__ISSUER=LoopCutAI
JWT__AUDIENCE=LoopCutAI_Users
JWT__EXPIRATIONINMINUTES=60

# ===== GOOGLE OAUTH =====
AUTHENTICATION__GOOGLE__CLIENTID=your_google_client_id.apps.googleusercontent.com
AUTHENTICATION__GOOGLE__CLIENTSECRET=your_google_client_secret

# ===== SEEDER DATA (ADMIN ACCOUNT) =====
SEEDERDATA__DEFAULTADMINEMAIL=xx@xxxx.com
SEEDERDATA__DEFAULTADMINPASSWORD=xxxxx!

# ===== PAYOS PAYMENT GATEWAY =====
PAYOS__CLIENTID=your_payos_client_id
PAYOS__APIKEY=your_payos_api_key
PAYOS__CHECKSUMKEY=your_payos_checksum_key
PAYOS__BASEURL=https://api.payos.com/v1
PAYOS__URL=your_return_url_after_payment

# ===== GOOGLE GEMINI AI =====
GEMINI__APIKEY=your_google_gemini_api_key
GEMINI__BASEURL=https://generativelanguage.googleapis.com

# ===== EMAIL SETTINGS =====
EMAILSETTINGS__HOST=smtp.gmail.com
EMAILSETTINGS__PORT=587
EMAILSETTINGS__USESSL=true
EMAILSETTINGS__USERNAME=your_email@gmail.com
EMAILSETTINGS__PASSWORD=your_email_app_password
EMAILSETTINGS__FROM=noreply@loopcutai.com
EMAILSETTINGS__FROMNAME=LoopCutAI

# ===== GOOGLE CLOUD STORAGE =====
GOOGLESTORAGE__BUCKETNAME=your_gcs_bucket_name
GOOGLESTORAGE__CREDENTIALSJSON=your_gcs_credentials_json_path_or_content
```

#### Bước 1.1: Giải thích các biến cấu hình

| Biến                                   | Mô Tả                                         | Ví Dụ                                       |
| -------------------------------------- | --------------------------------------------- | ------------------------------------------- |
| `CONNECTIONSTRINGS__DEFAULTCONNECTION` | Connection string kết nối database SQL Server | `Server=sql_server;Database=LoopCutAI;...`  |
| `JWT__SECRETKEY`                       | Khóa bí mật JWT (tối thiểu 32 ký tự)          | `MySecretKey123456789012345678901234`       |
| `JWT__ISSUER`                          | Công ty/ứng dụng phát hành token              | `LoopCutAI`                                 |
| `JWT__AUDIENCE`                        | Đối tượng sử dụng token                       | `LoopCutAI_Users`                           |
| `JWT__EXPIRATIONINMINUTES`             | Thời gian hết hạn token (phút)                | `60`                                        |
| `AUTHENTICATION__GOOGLE__CLIENTID`     | Google OAuth Client ID từ Google Console      | `xxx.apps.googleusercontent.com`            |
| `AUTHENTICATION__GOOGLE__CLIENTSECRET` | Google OAuth Client Secret                    | `GOCSP...`                                  |
| `SEEDERDATA__DEFAULTADMINEMAIL`        | Email tài khoản admin mặc định                | `xxxx@x.com`                       |
| `SEEDERDATA__DEFAULTADMINPASSWORD`     | Mật khẩu admin mặc định                       | `xxx!`                         |
| `PAYOS__CLIENTID`                      | Client ID từ PayOS                            | `xxxxx`                                     |
| `PAYOS__APIKEY`                        | API Key từ PayOS                              | `xxxxx`                                     |
| `PAYOS__CHECKSUMKEY`                   | Checksum Key từ PayOS                         | `xxxxx`                                     |
| `PAYOS__BASEURL`                       | Base URL API PayOS                            | `https://api.payos.com/v1`                  |
| `PAYOS__URL`                           | URL callback sau thanh toán                   | `https://yourdomain.com/payment/callback`   |
| `GEMINI__APIKEY`                       | API Key Google Gemini AI                      | `AIzaS...`                                  |
| `GEMINI__BASEURL`                      | Base URL Google Gemini API                    | `https://generativelanguage.googleapis.com` |
| `EMAILSETTINGS__HOST`                  | SMTP server email                             | `smtp.gmail.com`                            |
| `EMAILSETTINGS__PORT`                  | SMTP port (thường 587 hoặc 465)               | `587`                                       |
| `EMAILSETTINGS__USESSL`                | Sử dụng SSL/TLS                               | `true`                                      |
| `EMAILSETTINGS__USERNAME`              | Email account để gửi thư                      | `your_email@gmail.com`                      |
| `EMAILSETTINGS__PASSWORD`              | Mật khẩu ứng dụng email (Google)              | `xxxx xxxx xxxx xxxx`                       |
| `EMAILSETTINGS__FROM`                  | Email noreply                                 | `noreply@loopcutai.com`                     |
| `EMAILSETTINGS__FROMNAME`              | Tên hiển thị email                            | `LoopCutAI`                                 |
| `GOOGLESTORAGE__BUCKETNAME`            | Tên bucket Google Cloud Storage               | `loopcutai-bucket`                          |
| `GOOGLESTORAGE__CREDENTIALSJSON`       | Path hoặc content JSON credentials GCS        | `/path/to/credentials.json`                 |

#### Bước 2: Chạy Docker Compose

```bash
docker-compose up -d
```

Lệnh này sẽ:

- Build Docker image cho API
- Khởi chạy container ASP.NET Core API
- Expose API tại `http://localhost:5000`

#### Bước 3: Kiểm tra API

```bash
# Kiểm tra API đang chạy
curl http://localhost:5000/swagger

# Hoặc mở browser và truy cập
http://localhost:5000/swagger/index.html
```

#### Bước 4: Dừng services

```bash
docker-compose down
```

---

### Cách 2: Chạy Local (chỉ máy development)

#### Bước 1: Clone repository

```bash
git clone <repository-url>
cd LoopCutAI
```

#### Bước 2: Restore NuGet packages

```bash
dotnet restore
```

#### Bước 3: Cấu hình Database

Cập nhật `appsettings.Development.json` với tất cả các biến:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=xxxxxxx;Database=LoopCutAI;User Id=xxxxxx;Password=xxxxxx;"
  },
  "Jwt": {
    "SecretKey": "MySecretKey123456789012345678901234",
    "Issuer": "LoopCutAI",
    "Audience": "LoopCutAI_Users",
    "ExpirationInMinutes": 60
  },
  "Authentication": {
    "Google": {
      "ClientId": "your_google_client_id.apps.googleusercontent.com",
      "ClientSecret": "your_google_client_secret"
    }
  },
  "SeederData": {
    "DefaultAdminEmail": "xx@xxx.com",
    "DefaultAdminPassword": "xxxxx!"
  },
  "PayOs": {
    "ClientId": "your_payos_client_id",
    "ApiKey": "your_payos_api_key",
    "ChecksumKey": "your_payos_checksum_key",
    "BaseUrl": "https://api.payos.com/v1",
    "Url": "https://yourdomain.com/payment/callback"
  },
  "Gemini": {
    "ApiKey": "your_google_gemini_api_key",
    "BaseUrl": "https://generativelanguage.googleapis.com"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "UseSSL": true,
    "Username": "your_email@gmail.com",
    "Password": "your_email_app_password",
    "From": "noreply@loopcutai.com",
    "FromName": "LoopCutAI"
  },
  "GoogleStorage": {
    "BucketName": "your_gcs_bucket_name",
    "CredentialsJson": "/path/to/credentials.json"
  }
}
```

#### Bước 4: Migration Database

```bash
dotnet ef database update --project LoopCut.Infrastructure --startup-project LoopCut.API
```

#### Bước 5: Chạy API

```bash
dotnet run --project LoopCut.API
```

API sẽ chạy tại `https://localhost:5001` hoặc `http://localhost:5000`

---

## 📚 Hướng Dẫn Cấu Hình Các Dịch Vụ Bên Ngoài

### 1️⃣ Google OAuth (Xác thực)

**Bước 1**: Truy cập [Google Cloud Console](https://console.cloud.google.com/)

**Bước 2**: Tạo project mới

- Click "Select a project" → "NEW PROJECT"
- Nhập tên project: `LoopCutAI`

**Bước 3**: Kích hoạt Google+ API

- Tìm "Google+ API" → Click "Enable"

**Bước 4**: Tạo OAuth 2.0 Credentials

- Vào "Credentials" → Click "Create Credentials" → "OAuth client ID"
- Chọn "Web application"
- Thêm "Authorized redirect URIs": `http://localhost:5000/signin-google`
- Copy `Client ID` và `Client Secret` vào `.env`

### 2️⃣ Google Gemini API (AI Chatbot)

**Bước 1**: Truy cập [Google AI Studio](https://aistudio.google.com/app/apikey)

**Bước 2**: Click "Create API Key"

**Bước 3**: Copy API Key vào `GEMINI__APIKEY` trong `.env`

### 3️⃣ PayOS (Cổng Thanh Toán)

**Bước 1**: Đăng ký tài khoản trên [PayOS](https://payos.vn/)

**Bước 2**: Truy cập Dashboard → Lấy thông tin:

- `Client ID`
- `API Key`
- `Checksum Key`

**Bước 3**: Cấu hình Return URL

- Trên Dashboard PayOS, thiết lập "Return URL": `https://yourdomain.com/payment/callback`
- Cập nhật vào `PAYOS__URL` trong `.env`

**Bước 4**: Cập nhật các biến vào `.env`:

```
PAYOS__CLIENTID=xxx
PAYOS__APIKEY=xxx
PAYOS__CHECKSUMKEY=xxx
PAYOS__URL=https://yourdomain.com/payment/callback
```

### 4️⃣ Email Configuration (Gmail)

**Để sử dụng Gmail SMTP:**

**Bước 1**: Bật "Less secure app access"

- Truy cập [myaccount.google.com](https://myaccount.google.com/)
- Vào "Security" → Bật "Less secure app access"

**Hoặc (Khuyên dùng)** - Sử dụng App Password:

- Truy cập [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)
- Chọn App: "Mail" → Device: "Windows Computer" (hoặc tùy chọn khác)
- Copy mật khẩu ứng dụng (16 ký tự)

**Bước 2**: Cập nhật vào `.env`:

```
EMAILSETTINGS__HOST=smtp.gmail.com
EMAILSETTINGS__PORT=587
EMAILSETTINGS__USERNAME=your_email@gmail.com
EMAILSETTINGS__PASSWORD=xxxx xxxx xxxx xxxx
EMAILSETTINGS__FROM=noreply@loopcutai.com
EMAILSETTINGS__FROMNAME=LoopCutAI
```

### 5️⃣ Google Cloud Storage (Lưu trữ File)

**Bước 1**: Tạo GCS Bucket trên [Google Cloud Console](https://console.cloud.google.com/)

- Navigate to Cloud Storage → Buckets → Create
- Nhập tên bucket: `loopcutai-bucket`
- Chọn location và storage class phù hợp

**Bước 2**: Tạo Service Account

- Vào "IAM & Admin" → "Service Accounts" → "Create Service Account"
- Nhập tên: `LoopCutAI-Storage`
- Click "Create and Continue"

**Bước 3**: Cấp quyền

- Click vào service account vừa tạo
- Vào tab "Keys" → "Add Key" → "Create new key"
- Chọn "JSON" → "Create"
- File JSON sẽ được tải xuống

**Bước 4**: Cập nhật vào `.env`:

```
GOOGLESTORAGE__BUCKETNAME=loopcutai-bucket
GOOGLESTORAGE__CREDENTIALSJSON=/path/to/downloaded/credentials.json
```

### 6️⃣ JWT Secret Key Generation

Để tạo JWT Secret Key an toàn (tối thiểu 32 ký tự):

**PowerShell:**

```powershell
-join ((0..31) | ForEach-Object { [char](Get-Random -Minimum 33 -Maximum 127) })
```

**Bash/Linux:**

```bash
openssl rand -base64 32
```

## 📁 Cấu Trúc Thư Mục

```
LoopCutAI/
├── LoopCut.API/                 # Presentation Layer (Controllers, Middleware)
│   ├── Controllers/             # API endpoints
│   ├── Middleware/              # Global exception handling, etc.
│   ├── Program.cs               # Cấu hình startup
│   └── appsettings.json         # Configuration
│
├── LoopCut.Application/         # Business Logic Layer
│   ├── Interfaces/              # Service interfaces
│   ├── Services/                # Service implementations
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Validators/              # FluentValidation rules
│   └── DependencyInjection.cs   # DI configuration
│
├── LoopCut.Domain/              # Business Rules Layer
│   ├── Entities/                # Domain entities
│   ├── Enums/                   # Enumerations
│   ├── IRepository/             # Repository interfaces
│   └── Abstractions/            # Base classes & interfaces
│
├── LoopCut.Infrastructure/      # Data Access Layer
│   ├── Repository/              # Repository implementations
│   ├── Migrations/              # Entity Framework migrations
│   ├── Seeder/                  # Database seeding
│   └── DependencyInjection.cs   # DI configuration
│
├── docker-compose.yml           # Docker Compose configuration
├── Dockerfile                   # Docker configuration
└── README.md                    # Tài liệu này
```

---

## 🔌 API Endpoints Chính

| Endpoint                 | Method | Mô Tả                     |
| ------------------------ | ------ | ------------------------- |
| `/api/auth/login`        | POST   | Đăng nhập                 |
| `/api/auth/register`     | POST   | Đăng ký tài khoản         |
| `/api/account/profile`   | GET    | Lấy thông tin cá nhân     |
| `/api/subscription`      | POST   | Tạo đăng ký               |
| `/api/subscription/{id}` | GET    | Lấy chi tiết đăng ký      |
| `/api/payment/create`    | POST   | Tạo thanh toán            |
| `/api/service-plans`     | GET    | Lấy danh sách gói dịch vụ |
| `/api/chat/send`         | POST   | Gửi tin nhắn đến AI       |
| `/api/logs`              | GET    | Lấy logs                  |

Chi tiết tất cả endpoints: `http://localhost:5000/swagger`

---

## 🛠️ Development

### Cài đặt IDE

**Visual Studio Code:**

```bash
code .
```

**Visual Studio 2022:**

- Mở `LoopCutAI.sln`

### Build Project

```bash
dotnet build
```

### Run Tests (nếu có)

```bash
dotnet test
```

---

## 📚 Tài Liệu Bổ Sung

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 📝 License

Project LoopCutAI được cấp phép dưới MIT License.

---

## 👥 Contributors

- **PhuHVN**: [PhuHo](https://github.com/PhuHVN)
- **Long9904**: [Long9904](https://github.com/Long9904)
---

## 📞 Liên Hệ & Hỗ Trợ

Nếu có câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ hoặc mở issue trên repository.

---

**Happy Coding! 🎉**
