# TopCV Backend

Backend API cho hệ thống TopCV, xây dựng với `.NET 8`, `ASP.NET Core Web API`, `Entity Framework Core` và `SQL Server`.

## Yêu cầu môi trường

- `Windows/Linux/macOS`
- `.NET SDK 8.x` (theo `global.json`)
- `SQL Server` (local hoặc Docker)
- `Git`
- Khuyến nghị: cài `dotnet-ef` để migrate database

## Cấu trúc chính

- `topCv/topCv.Api`: API project (startup project)
- `topCv/topCv.Application`: business/application layer
- `topCv/topCv.Domain`: domain entities
- `topCv/topCv.Infrastructure`: EF Core, persistence, migrations

## Setup nhanh

1. Clone repo và vào thư mục dự án:

```powershell
cd e:\code\topcv\topcv-be
```

2. Cấu hình connection string và JWT:
- Sửa file `topCv/topCv.Api/appsettings.json`
- Hoặc dùng biến môi trường để override:
  - `ConnectionStrings__DefaultConnection`
  - `Jwt__Issuer`
  - `Jwt__Audience`
  - `Jwt__Key`
  - `Jwt__AccessTokenMinutes`
  - `Jwt__RefreshTokenDays`

Ví dụ connection string local trong `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SPCSQLSERVER;Database=topCv;User Id=sa;Password=IntelliSys123!;TrustServerCertificate=True;"
}
```

## Build

Chạy từ thư mục root:

```powershell
dotnet restore topCv/topCv.Api/topCv.Api.csproj
dotnet build topCv/topCv.Api/topCv.Api.csproj
```

## Run

Chạy API:

```powershell
dotnet run --project topCv/topCv.Api/topCv.Api.csproj
```

Swagger mặc định:
- `http://localhost:5000/swagger` (profile `http`)
- hoặc `https://localhost:7294/swagger` (profile `https`)

## Migrate database (EF Core)

### 1) Cài `dotnet-ef` (nếu chưa có)

```powershell
dotnet tool install --global dotnet-ef
```

Kiểm tra:

```powershell
dotnet ef --version
```

### 2) Apply migration hiện có vào DB

```powershell
dotnet ef database update --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api
```

### 3) Tạo migration mới

```powershell
dotnet ef migrations add <MigrationName> --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api --output-dir Migrations
dotnet ef database update --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api
```

Ví dụ:

```powershell
dotnet ef migrations add AddUserProfileFields --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api --output-dir Migrations
```

## Chạy bằng Docker (tuỳ chọn)

Repo đã có:
- `docker-compose.yml`
- `.env.example`
- `run-migrations.sh`

Bạn có thể tham khảo thêm tài liệu chi tiết ở `README.Docker.md`.
