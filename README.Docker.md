# Docker Setup Guide - TopCV Backend

Hướng dẫn chi tiết để build và chạy dự án TopCV Backend sử dụng Docker và Docker Compose.

## 📋 Mục Lục

- [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
- [Cài Đặt Nhanh](#cài-đặt-nhanh)
- [Cấu Hình Chi Tiết](#cấu-hình-chi-tiết)
- [Database Setup](#database-setup)
- [Development Workflow](#development-workflow)
- [Production Deployment](#production-deployment)
- [Troubleshooting](#troubleshooting)
- [Các Lệnh Hữu Ích](#các-lệnh-hữu-ích)

## 🔧 Yêu Cầu Hệ Thống

### Phần Mềm Cần Thiết

- **Docker**: Version 20.10 trở lên
- **Docker Compose**: Version 2.0 trở lên
- **Disk Space**: Ít nhất 5GB trống
- **RAM**: Khuyến nghị 4GB trở lên

### Kiểm Tra Cài Đặt

```bash
# Kiểm tra Docker version
docker --version

# Kiểm tra Docker Compose version
docker-compose --version

# Kiểm tra Docker đang chạy
docker ps
```

### Kiểm Tra Port

Đảm bảo các ports sau chưa được sử dụng:

- **5000**: TopCV API
- **1433**: SQL Server

```bash
# Linux/Mac: Kiểm tra port đang sử dụng
sudo lsof -i :5000
sudo lsof -i :1433

# Hoặc với netstat
netstat -tuln | grep -E '5000|1433'
```

## 🚀 Cài Đặt Nhanh

### 1. Copy Environment File

```bash
cd /home/nghialvm/code/backend/dotnet/topcv-be
cp .env.example .env
```

### 2. Cấu Hình Environment Variables (Optional)

Mở file `.env` và điều chỉnh nếu cần:

```bash
nano .env
# hoặc
vim .env
```

**Lưu ý quan trọng:**
- `SA_PASSWORD` phải đủ mạnh (ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt)
- `JWT_KEY` nên được tạo mới cho mỗi môi trường
- Không commit file `.env` vào git

### 3. Build và Start Services

```bash
# Build và start tất cả services
docker-compose up -d

# Xem logs để kiểm tra
docker-compose logs -f
```

### 4. Chờ Services Khởi Động

SQL Server cần khoảng 30-60 giây để khởi động hoàn toàn. Bạn có thể theo dõi:

```bash
# Xem logs của SQL Server
docker-compose logs -f sqlserver

# Xem logs của API
docker-compose logs -f topcv-api
```

### 5. Chạy Database Migrations

Sau khi SQL Server đã sẵn sàng, chạy migrations:

```bash
# Chạy migrations
docker-compose exec topcv-api dotnet ef database update --no-build

# Hoặc nếu migrations chưa được tạo, tạo mới:
docker-compose exec topcv-api dotnet ef migrations add InitialCreate --no-build
docker-compose exec topcv-api dotnet ef database update --no-build
```

### 6. Kiểm Tra Ứng Dụng

Truy cập các URLs sau:

- **Swagger UI**: http://localhost:5000/swagger
- **API Health Check**: http://localhost:5000/swagger/index.html

```bash
# Test với curl
curl http://localhost:5000/swagger/index.html
```

## ⚙️ Cấu Hình Chi Tiết

### Environment Variables

#### ASPNETCORE_ENVIRONMENT

Môi trường runtime của ứng dụng:

- `Development`: Cho local development, hiển thị Swagger UI
- `Staging`: Cho staging environment
- `Production`: Cho production, tắt Swagger UI

#### SQL Server Configuration

```bash
# Password mặc định (thay đổi trong production!)
SA_PASSWORD=TopCv@2024!Strong
```

**Yêu cầu password:**
- Ít nhất 8 ký tự
- Bao gồm chữ hoa (A-Z)
- Bao gồm chữ thường (a-z)
- Bao gồm số (0-9)
- Bao gồm ký tự đặc biệt (@, #, $, %, etc.)

#### JWT Configuration

```bash
# Generate secure JWT key
openssl rand -base64 32
```

Các biến JWT:
- `JWT_KEY`: Secret key (tối thiểu 32 ký tự)
- `JWT_ISSUER`: Issuer name
- `JWT_AUDIENCE`: Audience name
- `JWT_ACCESS_TOKEN_MINUTES`: Access token expiration (minutes)
- `JWT_REFRESH_TOKEN_DAYS`: Refresh token expiration (days)

### Docker Compose Services

#### topcv-api

API service chính của ứng dụng:

- **Image**: Build từ Dockerfile
- **Port**: 5000 (host) → 8080 (container)
- **Volumes**: 
  - `uploads:/app/uploads` - Persistent storage cho uploaded files
- **Depends on**: `sqlserver` service

#### sqlserver

SQL Server 2022 Developer Edition:

- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Port**: 1433 (host & container)
- **Volumes**: 
  - `sqlserver-data:/var/opt/mssql` - Persistent database storage
- **Memory Limit**: Mặc định unlimited (có thể cấu hình)

### Volumes

#### sqlserver-data

Lưu trữ persistent data của SQL Server:

```bash
# Xem thông tin volume
docker volume inspect topcv-be_sqlserver-data

# Backup volume
docker run --rm -v topcv-be_sqlserver-data:/data -v $(pwd):/backup ubuntu tar czf /backup/sqlserver-backup.tar.gz /data
```

#### uploads

Lưu trữ uploaded files (CVs, company logos, etc.):

```bash
# Xem thông tin volume
docker volume inspect topcv-be_uploads

# Truy cập files
docker-compose exec topcv-api ls -la /app/uploads
```

## 💾 Database Setup

### Kết Nối Từ Host Machine

Bạn có thể kết nối vào SQL Server từ máy host bằng các công cụ như:

- **Azure Data Studio**
- **SQL Server Management Studio (SSMS)**
- **DBeaver**
- **sqlcmd**

**Connection String:**
```
Server=localhost,1433
Database=topCv
User Id=sa
Password=<SA_PASSWORD từ .env file>
```

### Sử Dụng sqlcmd Trong Container

```bash
# Truy cập SQL Server CLI
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TopCv@2024!Strong"

# Chạy query trực tiếp
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TopCv@2024!Strong" -Q "SELECT name FROM sys.databases"
```

### Entity Framework Migrations (Sử dụng Docker)

Vì môi trường host có thể không cài đặt .NET SDK, bạn có thể chạy migrations thông qua một Docker container tạm thời bằng script hỗ trợ:

```bash
# Cấp quyền thực thi cho script
chmod +x run-migrations.sh

# Chạy migrations
./run-migrations.sh
```

Hoặc chạy lệnh thủ công:

```bash
docker run --rm \
    --network topcv-be_topcv-network \
    -v $(pwd):/src \
    -w /src \
    -e "ConnectionStrings__DefaultConnection=Server=sqlserver;Database=topCv;User Id=sa;Password=TopCv@2024!Strong;TrustServerCertificate=True;" \
    mcr.microsoft.com/dotnet/sdk:8.0 \
    sh -c "dotnet tool install --global dotnet-ef && \
           export PATH=\"\$PATH:/root/.dotnet/tools\" && \
           dotnet ef database update --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api --no-build"
```

#### Tạo Migration Mới

Để tạo migration mới khi có thay đổi code:

```bash
docker run --rm \
    -v $(pwd):/src \
    -w /src \
    mcr.microsoft.com/dotnet/sdk:8.0 \
    sh -c "dotnet tool install --global dotnet-ef && \
           export PATH=\"\$PATH:/root/.dotnet/tools\" && \
           dotnet ef migrations add <MigrationName> --project topCv/topCv.Infrastructure --startup-project topCv/topCv.Api"
```


### Database Backup & Restore

#### Backup Database

```bash
# Backup database
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TopCv@2024!Strong" -Q "BACKUP DATABASE [topCv] TO DISK = N'/var/opt/mssql/backup/topCv.bak' WITH NOFORMAT, NOINIT, NAME = 'topCv-full', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

# Copy backup file ra host
docker cp topcv-sqlserver:/var/opt/mssql/backup/topCv.bak ./topCv-backup.bak
```

#### Restore Database

```bash
# Copy backup file vào container
docker cp ./topCv-backup.bak topcv-sqlserver:/var/opt/mssql/backup/

# Restore database
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TopCv@2024!Strong" -Q "RESTORE DATABASE [topCv] FROM DISK = N'/var/opt/mssql/backup/topCv.bak' WITH REPLACE"
```

## 🛠️ Development Workflow

### Rebuild Sau Khi Thay Đổi Code

```bash
# Rebuild và restart API service
docker-compose up -d --build topcv-api

# Xem logs khi restart
docker-compose logs -f topcv-api
```

### Debug Logs

```bash
# Xem logs realtime
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f topcv-api
docker-compose logs -f sqlserver

# Xem 100 dòng logs gần nhất
docker-compose logs --tail=100 topcv-api

# Xem logs từ thời điểm cụ thể
docker-compose logs --since 2024-02-09T10:00:00 topcv-api
```

### Truy Cập Container Shell

```bash
# Bash shell trong API container
docker-compose exec topcv-api bash

# SQL Server container (không có bash, dùng sh)
docker-compose exec sqlserver bash

# Chạy command một lần
docker-compose exec topcv-api ls -la /app
```

### File Upload Testing

```bash
# Kiểm tra upload directory
docker-compose exec topcv-api ls -la /app/uploads

# Tạo test file
docker-compose exec topcv-api touch /app/uploads/test.txt

# Xem volume từ host
docker volume inspect topcv-be_uploads
```

### Hot Reload (Optional)

Để enable hot reload trong development:

1. Modify `docker-compose.yml`:

```yaml
topcv-api:
  # ... existing config
  volumes:
    - uploads:/app/uploads
    - ./topCv/topCv.Api:/src/topCv.Api:ro  # Mount source code
  environment:
    - DOTNET_USE_POLLING_FILE_WATCHER=true
    - ASPNETCORE_ENVIRONMENT=Development
```

2. Rebuild: `docker-compose up -d --build topcv-api`

**Lưu ý**: Hot reload có thể gây performance issues, chỉ dùng khi cần thiết.

## 🚢 Production Deployment

### Environment Configuration

1. **Tạo .env file riêng cho production:**

```bash
cp .env.example .env.production
```

2. **Cập nhật production values:**

```bash
ASPNETCORE_ENVIRONMENT=Production
SA_PASSWORD=<generated-strong-password>
JWT_KEY=<generated-secure-key>
```

3. **Chạy với production config:**

```bash
docker-compose --env-file .env.production up -d
```

### Security Best Practices

#### 1. Secrets Management

**Không lưu secrets trong .env file cho production!**

Options tốt hơn:
- **Docker Secrets** (cho Swarm mode)
- **Kubernetes Secrets** (cho K8s deployment)
- **Azure Key Vault**
- **AWS Secrets Manager**
- **HashiCorp Vault**

#### 2. Database Security

```yaml
# docker-compose.production.yml
sqlserver:
  environment:
    - SA_PASSWORD_FILE=/run/secrets/sa_password  # Sử dụng Docker secrets
  secrets:
    - sa_password

secrets:
  sa_password:
    external: true
```

#### 3. Network Security

```yaml
# Expose only necessary ports
services:
  topcv-api:
    ports:
      - "5000:8080"  # Only API port
  
  sqlserver:
    # Không expose port ra ngoài, chỉ internal network
    # ports:
    #   - "1433:1433"  # Comment out trong production
```

#### 4. Resource Limits

```yaml
services:
  topcv-api:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
  
  sqlserver:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 4G
        reservations:
          cpus: '1.0'
          memory: 2G
```

### Monitoring & Logging

#### Health Checks

Services đã được cấu hình health checks. Kiểm tra status:

```bash
# Xem health status
docker-compose ps

# Test health endpoint manually
curl http://localhost:5000/swagger/index.html
```

#### Centralized Logging (Optional)

Integrate với logging solutions:

- **ELK Stack** (Elasticsearch, Logstash, Kibana)
- **Grafana Loki**
- **Azure Monitor**
- **AWS CloudWatch**

Example với Grafana Loki:

```yaml
# docker-compose.production.yml
services:
  topcv-api:
    logging:
      driver: "loki"
      options:
        loki-url: "http://loki:3100/loki/api/v1/push"
```

### Scaling

#### Horizontal Scaling

```bash
# Scale API service to 3 instances
docker-compose up -d --scale topcv-api=3
```

**Lưu ý**: Cần load balancer (nginx, traefik) để distribute traffic.

#### Orchestration

Cho production scale lớn, consider:

- **Docker Swarm**: Built-in orchestration
- **Kubernetes**: Enterprise-grade orchestration
- **Azure Container Instances**
- **AWS ECS/EKS**

## 🔍 Troubleshooting

### SQL Server Connection Failed

**Triệu chứng:**
```
SqlException: A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

**Giải pháp:**

1. **Kiểm tra SQL Server đã ready:**

```bash
# Xem logs
docker-compose logs sqlserver

# Kiểm tra health status
docker-compose ps
```

2. **Test connection thủ công:**

```bash
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TopCv@2024!Strong" -Q "SELECT 1"
```

3. **Restart services theo thứ tự:**

```bash
docker-compose down
docker-compose up -d sqlserver
# Đợi 30 giây
docker-compose up -d topcv-api
```

### Port Already in Use

**Triệu chứng:**
```
Error: bind: address already in use
```

**Giải pháp:**

1. **Tìm process đang dùng port:**

```bash
# Linux
sudo lsof -i :5000
sudo lsof -i :1433

# Kill process
sudo kill -9 <PID>
```

2. **Hoặc thay đổi port mapping:**

```yaml
# docker-compose.yml
topcv-api:
  ports:
    - "5001:8080"  # Thay vì 5000
```

### Permission Denied - Volumes

**Triệu chứng:**
```
Error: Permission denied when writing to /app/uploads
```

**Giải pháp:**

```bash
# Fix permissions
docker-compose exec topcv-api chown -R appuser:appgroup /app/uploads

# Hoặc recreate volume
docker-compose down
docker volume rm topcv-be_uploads
docker-compose up -d
```

### Migrations Failed

**Triệu chứng:**
```
Unable to create an object of type 'AppDbContext'
```

**Giải pháp:**

1. **Kiểm tra connection string:**

```bash
docker-compose exec topcv-api env | grep ConnectionStrings
```

2. **Chạy migrations từ migration project:**

```bash
docker-compose exec topcv-api dotnet ef database update --project /src/topCv.Infrastructure --startup-project /src/topCv.Api --no-build
```

### Out of Memory

**Triệu chứng:**
```
Container killed due to OOM (Out of Memory)
```

**Giải pháp:**

```yaml
# docker-compose.yml - Tăng memory limits
services:
  sqlserver:
    deploy:
      resources:
        limits:
          memory: 4G
```

### Container Không Start

**Giải pháp:**

```bash
# Xem logs chi tiết
docker-compose logs --tail=100 topcv-api

# Inspect container
docker inspect topcv-api

# Check container events
docker events --filter container=topcv-api
```

## 📝 Các Lệnh Hữu Ích

### Docker Compose Commands

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# Restart services
docker-compose restart

# Rebuild and restart
docker-compose up -d --build

# Stop and remove volumes (WARNING: Deletes data!)
docker-compose down -v

# View service status
docker-compose ps

# View logs
docker-compose logs -f [service-name]

# Execute command in service
docker-compose exec [service-name] [command]

# Scale service
docker-compose up -d --scale topcv-api=3
```

### Docker Commands

```bash
# List containers
docker ps
docker ps -a  # Include stopped

# List images
docker images

# Remove unused resources
docker system prune
docker system prune -a  # Remove all unused images

# View resource usage
docker stats

# Inspect container
docker inspect [container-name]

# View container logs
docker logs -f [container-name]
docker logs --tail=100 [container-name]
```

### Volume Management

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect topcv-be_sqlserver-data

# Remove volume (WARNING: Deletes data!)
docker volume rm topcv-be_sqlserver-data

# Backup volume
docker run --rm -v topcv-be_sqlserver-data:/data -v $(pwd):/backup ubuntu tar czf /backup/backup.tar.gz /data

# Restore volume
docker run --rm -v topcv-be_sqlserver-data:/data -v $(pwd):/backup ubuntu tar xzf /backup/backup.tar.gz -C /data --strip 1
```

### Network Management

```bash
# List networks
docker network ls

# Inspect network
docker network inspect topcv-be_topcv-network

# Connect container to network
docker network connect topcv-be_topcv-network [container-name]
```

### Cleanup Commands

```bash
# Remove stopped containers
docker container prune

# Remove unused images
docker image prune
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove unused networks
docker network prune

# Complete cleanup
docker system prune -a --volumes
```

## 📚 Additional Resources

### Official Documentation

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [.NET on Docker](https://learn.microsoft.com/en-us/dotnet/core/docker/introduction)

### Useful Tools

- **Portainer**: Docker management UI
- **Lazydocker**: Terminal UI for Docker
- **Azure Data Studio**: Cross-platform database tool
- **Postman**: API testing

### Security Resources

- [Docker Security Best Practices](https://docs.docker.com/engine/security/)
- [OWASP Docker Security](https://cheatsheetseries.owasp.org/cheatsheets/Docker_Security_Cheat_Sheet.html)

---

## 🎯 Quick Reference

### Start Development Environment

```bash
cp .env.example .env
docker-compose up -d
docker-compose logs -f
# Wait for SQL Server to be ready (~30s)
docker-compose exec topcv-api dotnet ef database update --no-build
# Access: http://localhost:5000/swagger
```

### Stop Everything

```bash
docker-compose down
# Or to remove volumes too:
docker-compose down -v
```

### View Service Status

```bash
docker-compose ps
docker-compose logs -f topcv-api
curl http://localhost:5000/swagger/index.html
```

---

**Version**: 1.0.0  
**Last Updated**: 2026-02-09  
**Maintained By**: TopCV Development Team
