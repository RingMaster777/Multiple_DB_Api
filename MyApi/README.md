# Multi-Database Web API (.NET 9)

A production-ready .NET 9 Web API with **true database independence**, supporting both **Microsoft SQL Server** and **PostgreSQL** with proper migration handling.

## 🎯 Features

- ✅ Single codebase works with both MSSQL and PostgreSQL
- ✅ Database provider selection via configuration
- ✅ Independent migration management for each database
- ✅ Database-agnostic entity and repository patterns
- ✅ Clean Architecture with separation of concerns
- ✅ Async/await throughout
- ✅ Comprehensive error handling
- ✅ Swagger/OpenAPI documentation
- ✅ Health check endpoint
- ✅ Auto-migration on startup (development)

## 📁 Project Structure

```
MyApi/
├── Controllers/
│   └── ProductsController.cs      # REST API endpoints
├── Data/
│   ├── ApplicationDbContext.cs     # Database context
│   └── Migrations/
│       ├── MSSQL/                  # SQL Server migrations
│       └── PostgreSQL/             # PostgreSQL migrations
├── Models/
│   ├── Product.cs                  # Entity model
│   └── ProductDto.cs               # Data transfer objects
├── Services/
│   └── ProductService.cs           # Business logic layer
├── Program.cs                      # Application entry point
├── appsettings.json                # Production configuration
└── appsettings.Development.json    # Development configuration
```

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (for MSSQL) OR PostgreSQL
- Visual Studio 2022 / VS Code / Rider
- Entity Framework Core Tools

### Installation

1. **Install EF Core Tools globally** (if not already installed)
```bash
dotnet tool install --global dotnet-ef
# Or update existing
dotnet tool update --global dotnet-ef
```

2. **Restore packages**
```bash
cd MyApi
dotnet restore
```

3. **Configure your database** (edit `appsettings.json`)

Choose your database provider and update connection string:

**For SQL Server:**
```json
{
  "DatabaseProvider": "MSSQL",
  "ConnectionStrings": {
    "MSSQL": "Server=localhost;Database=MyApiDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**For PostgreSQL:**
```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=myapidb;Username=postgres;Password=yourpassword;Include Error Detail=true"
  }
}
```

4. **Run the application**
```bash
dotnet run
```

The API will:
- Automatically apply migrations in development mode
- Start on `https://localhost:5001` (or as configured)
- Open Swagger UI at the root URL

## 📊 Database Migrations

### Understanding the Multi-Database Migration Strategy

This project maintains **separate migrations** for each database provider in dedicated folders:
- `Data/Migrations/MSSQL/` - SQL Server specific migrations
- `Data/Migrations/PostgreSQL/` - PostgreSQL specific migrations

### Generating New Migrations

#### For PostgreSQL:

1. **Set PostgreSQL as active provider** in `appsettings.json`:
```json
"DatabaseProvider": "PostgreSQL"
```

2. **Generate migration:**
```bash
dotnet ef migrations add YourMigrationName --context ApplicationDbContext --output-dir Data/Migrations/PostgreSQL
```

#### For MSSQL:

1. **Set MSSQL as active provider** in `appsettings.json`:
```json
"DatabaseProvider": "MSSQL"
```

2. **Generate migration:**
```bash
dotnet ef migrations add YourMigrationName --context ApplicationDbContext --output-dir Data/Migrations/MSSQL
```

### Applying Migrations

#### Option 1: Automatic (Development)
Migrations are **automatically applied** when running in Development mode.

#### Option 2: Manual via CLI

**PostgreSQL:**
```bash
# Ensure appsettings.json has "DatabaseProvider": "PostgreSQL"
dotnet ef database update --context ApplicationDbContext
```

**MSSQL:**
```bash
# Ensure appsettings.json has "DatabaseProvider": "MSSQL"
dotnet ef database update --context ApplicationDbContext
```

#### Option 3: Manual via Code
Migrations are applied in `Program.cs` on startup (development only):
```csharp
dbContext.Database.Migrate();
```

### Removing Last Migration

```bash
# Make sure DatabaseProvider is set correctly
dotnet ef migrations remove --context ApplicationDbContext
```

### Viewing Migration History

```bash
dotnet ef migrations list --context ApplicationDbContext
```

### Creating Database from Scratch

**PostgreSQL:**
```bash
# 1. Set DatabaseProvider to PostgreSQL
# 2. Create database manually in PostgreSQL or let EF create it
dotnet ef database update --context ApplicationDbContext

# Alternative: Generate SQL script
dotnet ef migrations script --context ApplicationDbContext --output migrate-postgres.sql
```

**MSSQL:**
```bash
# 1. Set DatabaseProvider to MSSQL
# 2. Create database or let EF create it
dotnet ef database update --context ApplicationDbContext

# Alternative: Generate SQL script
dotnet ef migrations script --context ApplicationDbContext --output migrate-mssql.sql
```

## 🔌 API Endpoints

### Health Check
```
GET /health
```

### Products API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### Example Requests

**Create Product:**
```bash
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "description": "Product description",
    "price": 99.99,
    "stock": 50
  }'
```

**Get All Products:**
```bash
curl https://localhost:5001/api/products
```

**Update Product:**
```bash
curl -X PUT https://localhost:5001/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Product",
    "price": 149.99
  }'
```

## 🏗️ Architecture

### Database-Agnostic Design

The application is designed to work seamlessly with either database:

1. **Single DbContext**: `ApplicationDbContext` works with both providers
2. **Provider Selection**: Configured in `Program.cs` based on `appsettings.json`
3. **Standard LINQ**: All queries use EF Core LINQ (no raw SQL)
4. **Compatible Data Types**: Entity properties use types supported by both databases

### Key Components

**Program.cs**: Dynamic database provider configuration
```csharp
var databaseProvider = builder.Configuration["DatabaseProvider"];
switch (databaseProvider.ToUpper())
{
    case "POSTGRESQL":
        options.UseNpgsql(connectionString);
        break;
    case "MSSQL":
        options.UseSqlServer(connectionString);
        break;
}
```

**ApplicationDbContext.cs**: Database-agnostic context
- No provider-specific code
- Standard EF Core configurations
- Works identically with both databases

**Services**: Pure business logic
- No database-specific dependencies
- Uses standard EF Core queries
- Repository pattern (optional)

## 🔄 Switching Databases

To switch from one database to another:

1. **Update** `appsettings.json`:
```json
"DatabaseProvider": "PostgreSQL"  // or "MSSQL"
```

2. **Ensure** connection string is correct

3. **Restart** the application

That's it! No code changes required.

## 🧪 Testing Different Databases

### Test with PostgreSQL:
```bash
# 1. Start PostgreSQL (Docker example)
docker run --name postgres-test -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres

# 2. Update appsettings.json to use PostgreSQL
# 3. Run the application
dotnet run
```

### Test with SQL Server:
```bash
# 1. Start SQL Server (Docker example)
docker run --name mssql-test -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Update appsettings.json to use MSSQL
# 3. Run the application
dotnet run
```

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

## 🎓 Best Practices Implemented

1. **Separation of Concerns**: Controllers → Services → Data Access
2. **Dependency Injection**: All services registered in DI container
3. **Async/Await**: All database operations are asynchronous
4. **Error Handling**: Comprehensive try-catch with logging
5. **DTOs**: Separate data transfer objects from entities
6. **Logging**: Structured logging with ILogger
7. **Configuration**: Environment-based settings
8. **API Documentation**: Swagger/OpenAPI integration

## 🐛 Troubleshooting

### Migration Issues

**Problem**: "The name 'MigrationName' is used by an existing migration"
- **Solution**: The migration already exists in one of the folders. Use a different name or remove the existing one.

**Problem**: Migrations applied to wrong database
- **Solution**: Double-check `DatabaseProvider` in `appsettings.json` matches your intended database.

### Connection Issues

**PostgreSQL**:
- Ensure PostgreSQL is running: `pg_isready`
- Check connection string username/password
- Verify port 5432 is accessible

**MSSQL**:
- Ensure SQL Server is running
- Check Windows Authentication or SQL Authentication settings
- Verify `TrustServerCertificate=True` if using self-signed certificates

### EF Core Tools

**Problem**: "No executable found matching command 'dotnet-ef'"
- **Solution**: `dotnet tool install --global dotnet-ef`

**Problem**: "The Entity Framework tools version is older than the runtime"
- **Solution**: `dotnet tool update --global dotnet-ef`

## 📝 License

This project is provided as-is for educational and commercial use.

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

---

**Happy coding! 🚀**
```bash
dotnet restore
```

3. **Configure your database** in `appsettings.json`:

```json
{
  "DatabaseProvider": "MSSQL",  // or "PostgreSQL"
  "ConnectionStrings": {
    "MSSQL": "Server=localhost;Database=MyApiDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQL": "Host=localhost;Port=5432;Database=myapidb;Username=postgres;Password=yourpassword;"
  }
}
```

## 📊 Database Migrations

### Generate Migrations

The project uses **separate migration folders** for each database provider. This allows you to maintain database-specific optimizations while keeping the core code database-agnostic.

#### For SQL Server (MSSQL)

1. **Update configuration** to use MSSQL:
```json
"DatabaseProvider": "MSSQL"
```

2. **Generate migration**:
```bash
dotnet ef migrations add InitialCreate --context ApplicationDbContext --output-dir Data/Migrations/MSSQL
```

3. **Add more migrations** (example):
```bash
dotnet ef migrations add AddProductIndex --context ApplicationDbContext --output-dir Data/Migrations/MSSQL
```

#### For PostgreSQL

1. **Update configuration** to use PostgreSQL:
```json
"DatabaseProvider": "PostgreSQL"
```

2. **Generate migration**:
```bash
dotnet ef migrations add InitialCreate --context ApplicationDbContext --output-dir Data/Migrations/PostgreSQL
```

3. **Add more migrations** (example):
```bash
dotnet ef migrations add AddProductIndex --context ApplicationDbContext --output-dir Data/Migrations/PostgreSQL
```

### Apply Migrations

#### Automatic (Development)
Migrations are automatically applied on startup in development mode. Just run:
```bash
dotnet run
```

#### Manual Application

**For SQL Server:**
```bash
# Update appsettings.json: "DatabaseProvider": "MSSQL"
dotnet ef database update --context ApplicationDbContext
```

**For PostgreSQL:**
```bash
# Update appsettings.json: "DatabaseProvider": "PostgreSQL"
dotnet ef database update --context ApplicationDbContext
```

### Remove Last Migration

```bash
# Make sure DatabaseProvider matches the migration you want to remove
dotnet ef migrations remove --context ApplicationDbContext
```

### View Migration History

```bash
dotnet ef migrations list --context ApplicationDbContext
```

## 🏃 Running the Application

### Development Mode

```bash
dotnet run
```

The API will be available at:
- **HTTPS**: https://localhost:7xxx
- **HTTP**: http://localhost:5xxx
- **Swagger UI**: https://localhost:7xxx (root)

### Production Mode

```bash
dotnet run --environment Production
```

## 🔌 API Endpoints

### Health Check
```http
GET /health
```
Returns database connection status and provider information.

### Products API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### Example Requests

**Create Product:**
```json
POST /api/products
Content-Type: application/json

{
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 1299.99,
  "stock": 50
}
```

**Update Product:**
```json
PUT /api/products/1
Content-Type: application/json

{
  "name": "Gaming Laptop",
  "price": 1499.99,
  "stock": 45
}
```

## 🔄 Switching Databases

To switch between databases, simply update `appsettings.json`:

**Switch to SQL Server:**
```json
{
  "DatabaseProvider": "MSSQL",
  "ConnectionStrings": {
    "MSSQL": "your-mssql-connection-string"
  }
}
```

**Switch to PostgreSQL:**
```json
{
  "DatabaseProvider": "PostgreSQL",
  "ConnectionStrings": {
    "PostgreSQL": "your-postgresql-connection-string"
  }
}
```

Then restart the application. No code changes required! 🎉

## 🏗️ Architecture Highlights

### Database Independence

The application achieves true database independence through:

1. **Provider-agnostic DbContext**: No database-specific code in `ApplicationDbContext`
2. **Standard LINQ queries**: All queries use EF Core LINQ (no raw SQL)
3. **Dynamic provider configuration**: Database provider selected at runtime
4. **Separate migrations**: Each database has its own migration history

### Key Design Patterns

- **Repository Pattern**: Service layer abstracts data access
- **Dependency Injection**: All dependencies injected via DI container
- **DTO Pattern**: Separate DTOs from entity models
- **Async/Await**: All I/O operations are asynchronous

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
```

## 🔧 Configuration Options

### Connection String Examples

**SQL Server - Windows Authentication:**
```
Server=localhost;Database=MyApiDb;Trusted_Connection=True;TrustServerCertificate=True;
```

**SQL Server - SQL Authentication:**
```
Server=localhost;Database=MyApiDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

**PostgreSQL - Local:**
```
Host=localhost;Port=5432;Database=myapidb;Username=postgres;Password=postgres;
```

**PostgreSQL - Remote:**
```
Host=myserver.com;Port=5432;Database=myapidb;Username=myuser;Password=mypass;SSL Mode=Require;
```

## 🧪 Testing the API

### Using Swagger UI
Navigate to the root URL (e.g., `https://localhost:7xxx`) to access Swagger UI for interactive testing.

### Using curl

```bash
# Health check
curl https://localhost:7xxx/health

# Get all products
curl https://localhost:7xxx/api/products

# Get product by ID
curl https://localhost:7xxx/api/products/1

# Create product
curl -X POST https://localhost:7xxx/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","description":"Test","price":99.99,"stock":10}'

# Update product
curl -X PUT https://localhost:7xxx/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Product","price":89.99}'

# Delete product
curl -X DELETE https://localhost:7xxx/api/products/1
```

## 🐛 Troubleshooting

### Migration Issues

**Problem**: Migration fails with "table already exists"
**Solution**: 
```bash
# Drop and recreate database
dotnet ef database drop --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

**Problem**: Wrong migration folder used
**Solution**: Ensure `DatabaseProvider` in `appsettings.json` matches the database you're targeting

### Connection Issues

**Problem**: Cannot connect to SQL Server
**Solution**: 
- Verify SQL Server is running
- Check connection string
- Ensure TCP/IP is enabled in SQL Server Configuration Manager
- Add `TrustServerCertificate=True` for development

**Problem**: Cannot connect to PostgreSQL
**Solution**:
- Verify PostgreSQL is running: `pg_isready`
- Check pg_hba.conf for connection permissions
- Ensure port 5432 is not blocked by firewall

## 📝 Best Practices

1. **Always use async/await** for database operations
2. **Never use database-specific SQL** in business logic
3. **Keep migrations in sync** with your target database
4. **Use separate databases** for development/staging/production
5. **Back up production data** before running migrations
6. **Test migrations** on a copy of production data first
7. **Use environment variables** for sensitive connection strings in production

## 🔐 Production Considerations

1. **Never commit sensitive connection strings** to source control
2. **Use Azure Key Vault, AWS Secrets Manager**, or similar for production secrets
3. **Disable automatic migrations** in production (remove auto-migrate code from Program.cs)
4. **Apply migrations manually** in production using deployment scripts
5. **Enable connection pooling** for better performance
6. **Monitor database performance** and add indexes as needed
7. **Implement retry logic** for transient database failures

## 📚 Additional Resources

- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [PostgreSQL with EF Core](https://www.npgsql.org/efcore/)
- [SQL Server with EF Core](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)

## 📄 License

This project is provided as-is for educational and commercial use.

## 🤝 Contributing

Feel free to submit issues, fork the repository, and create pull requests for any improvements.

---

**Happy Coding! 🚀**
