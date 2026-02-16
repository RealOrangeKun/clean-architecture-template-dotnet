# Orange Template

A production-ready .NET 10 modular monolith template featuring clean architecture, CQRS, event-driven design, and modern best practices.

## 🚀 Features

- **Modular Monolith Architecture** - Well-organized modules with clear boundaries (Users, Notifications)
- **Clean Architecture** - Separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- **CQRS Pattern** - Command and Query Responsibility Segregation using MediatR
- **Event-Driven Architecture** - Integration events with RabbitMQ and MassTransit
- **Domain-Driven Design** - Rich domain models with proper encapsulation
- **Distributed Caching** - Redis for high-performance caching
- **Background Jobs** - Inbox/Outbox pattern for reliable message processing
- **Health Checks** - Built-in health monitoring for all dependencies
- **API Documentation** - OpenAPI/Swagger integration
- **Exception Handling** - Global exception handling with Problem Details
- **Docker Support** - Full containerization with Docker Compose

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started) and Docker Compose
- Your preferred IDE:
  - [Visual Studio 2025+](https://visualstudio.microsoft.com/)
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/)

## 🏗️ Project Structure

```
orange-template/
├── src/
│   ├── API/
│   │   └── Project.Api/              # API entry point
│   ├── Common/                        # Shared components
│   │   ├── Project.Common.Application/    # Application layer shared code
│   │   ├── Project.Common.Domain/         # Domain layer shared code
│   │   ├── Project.Common.Infrastructure/ # Infrastructure shared code
│   │   └── Project.Common.Presentation/   # Presentation layer shared code
│   └── Modules/                       # Feature modules
│       ├── Notifications/             # Notification module
│       │   ├── Application/           # Use cases and CQRS handlers
│       │   ├── Domain/                # Domain entities and logic
│       │   ├── Infrastructure/        # Data access and external services
│       │   ├── IntegrationEvents/     # Integration event definitions
│       │   ├── Presentation/          # API endpoints
│       │   └── PublicApi/             # Public contracts and interfaces
│       └── Users/                     # User module
│           ├── Application/
│           ├── Domain/
│           ├── Infrastructure/
│           ├── IntegrationEvents/
│           ├── Presentation/
│           └── PublicApi/
├── docker-compose.yml                 # Infrastructure services
├── Dockerfile                         # Application container
├── Template.slnx                      # Solution file
└── global.json                        # .NET SDK version
```

## 🚦 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/orange-template.git
cd orange-template
```

### 2. Rename the Project (Optional)

If you want to use a custom name instead of "Project":

```bash
chmod +x rename-project.sh
./rename-project.sh YourProjectName
```

This script will rename all namespaces, file names, and project references.

### 3. Start All Services with Docker Compose

Start the entire application stack (API + infrastructure):

```bash
docker-compose up -d
```

**Services Available:**
- **API Application**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- PostgreSQL: `localhost:5432` (user: `postgres`, password: `postgres`)
- Redis: `localhost:6379`
- RabbitMQ Management: `http://localhost:15672` (user: `admin`, password: `admin`)
- PgAdmin: `http://localhost:5050` (email: `admin@admin.com`, password: `admin`)
- Mailpit (Email Testing): `http://localhost:8080`

**Note:** The API will automatically connect to all infrastructure services within the Docker network.

### 3a. Start Infrastructure Only (for local development)

If you prefer to run the API locally and only use Docker for infrastructure:

```bash
docker-compose up -d postgres redis rabbitmq pgadmin mailpit
```

### 4. Access the Application

If using Docker Compose, the application is now running! Access:

- **API**: `http://localhost:5000`
- **Swagger**: `http://localhost:5000/swagger`

### 4a. Local Development Setup (Optional)

If you prefer to run the API outside of Docker:

**Configure Application Settings:**

Update connection strings in [appsettings.Development.json](src/API/Project.Api/appsettings.Development.json):

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=project_db;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  }
}
```

**Apply Database Migrations:**

```bash
# From the repository root
dotnet ef database update --project src/Modules/Users/Project.Modules.Users.Infrastructure --context UsersDbContext
dotnet ef database update --project src/Modules/Notifications/Project.Modules.Notifications.Infrastructure --context NotificationsDbContext
```

**Run the Application:**

```bash
dotnet run --project src/API/Project.Api/Project.Api.csproj
```

The API will be available at `https://localhost:7251` or `http://localhost:5001` (as specified in launchSettings.json).

## 🏃 Running with Docker

### Option 1: Using Docker Compose (Recommended)

The easiest way to run the entire stack:

```bash
# Start all services (API + infrastructure)
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build
```

### Option 2: Using Docker CLI Only

Build and run the application container separately:

```bash
# Build the Docker image
docker build -t orange-template .

# Run with a local database
docker run -p 5000:8080 \
  -e ConnectionStrings__Database="Host=host.docker.internal;Database=project_db;Username=postgres;Password=postgres" \
  -e ConnectionStrings__Redis="host.docker.internal:6379" \
  orange-template
```

### Option 3: Infrastructure Only

Run only infrastructure services and develop the API locally:

```bash
docker-compose up -d postgres redis rabbitmq pgadmin mailpit
```

## 🧪 Testing

> **Note:** This template doesn't include test projects yet. You'll need to add your own test projects for unit, integration, and end-to-end testing.

## 📦 Technology Stack

**Backend:**
- .NET 10.0
- ASP.NET Core
- Entity Framework Core
- MediatR (CQRS)
- FluentValidation
- MassTransit (Message Bus)
- Dapper (for queries)

**Infrastructure:**
- PostgreSQL 18 (Database)
- Redis 8 (Caching)
- RabbitMQ 4 (Message Broker)
- Mailpit (Email Testing)

**Patterns & Practices:**
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS
- Repository Pattern
- Unit of Work Pattern
- Inbox/Outbox Pattern
- Vertical Slice Architecture (within modules)

## 📚 Module Architecture

Each module follows a consistent layered architecture:

### Domain Layer
- Entities and Value Objects
- Domain Events
- Domain Services
- Repository Interfaces

### Application Layer
- Commands and Queries (CQRS)
- Command/Query Handlers
- Validation (FluentValidation)
- Application Services
- Integration Event Handlers

### Infrastructure Layer
- EF Core DbContext and Configurations
- Repository Implementations
- External Service Integrations
- Background Jobs (Inbox/Outbox processors)

### Presentation Layer
- API Endpoints (Minimal APIs)
- Endpoint Filters
- DTOs and Mapping

### PublicApi Layer
- Public contracts for cross-module communication
- Shared interfaces and DTOs

### IntegrationEvents Layer
- Integration event definitions for cross-module communication
- Event messages published/consumed by other modules

## 🔧 Configuration

Key configuration files:

- [appsettings.json](src/API/Project.Api/appsettings.json) - Production settings
- [appsettings.Development.json](src/API/Project.Api/appsettings.Development.json) - Development settings
- [docker-compose.yml](docker-compose.yml) - Infrastructure services
- [global.json](global.json) - .NET SDK version
- [mise.toml](mise.toml) - Development environment version management

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines on:

- Development setup
- Coding standards
- Adding new features
- Database migrations
- Testing requirements
- Commit conventions
- Pull request process

## 📝 Adding a New Module

To add a new module (e.g., "Products"):

1. Create the module structure:
   ```bash
   mkdir -p src/Modules/Products/{Application,Domain,Infrastructure,IntegrationEvents,Presentation,PublicApi}
   ```

2. Create projects for each layer:
   ```bash
   dotnet new classlib -n Project.Modules.Products.Domain -o src/Modules/Products/Domain
   dotnet new classlib -n Project.Modules.Products.Application -o src/Modules/Products/Application
   # ... repeat for other layers
   ```

3. Add project references following the dependency flow:
   - Domain → (no dependencies)
   - Application → Domain, Common.Application
   - Infrastructure → Application, Common.Infrastructure
   - Presentation → Application, Common.Presentation
   - IntegrationEvents → Domain (for event data)
   - PublicApi → (minimal dependencies, shared contracts only)

4. Register the module in [Program.cs](src/API/Project.Api/Program.cs)

## 🔒 Security

- Environment-based configuration
- Secure connection strings
- CORS policies
- Authentication and Authorization ready (implement based on your needs)

## 📈 Health Checks

Health check endpoint:

- `/health` - Health status with detailed information (checks database and Redis)

The endpoint is configured in [Program.cs](src/API/Project.Api/Program.cs#L44-L91) and uses the `HealthChecks.UI.Client` for detailed JSON responses.

## 🐛 Troubleshooting

**Database connection issues:**
```bash
# Verify PostgreSQL is running
docker ps | grep postgres

# Check connection
psql -h localhost -U postgres -d project_db
```

**Redis connection issues:**
```bash
# Test Redis connection
redis-cli -h localhost -p 6379 ping
```

**RabbitMQ issues:**
```bash
# Check RabbitMQ status
docker logs rabbitmq_project
```

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with inspiration from modern .NET architecture patterns
- Follows guidelines from Microsoft's eShopOnContainers and Clean Architecture examples

## 📞 Support

For questions and support:
- Open an issue in the GitHub repository
- Check existing issues for similar problems
- Review the [CONTRIBUTING.md](CONTRIBUTING.md) guide

---

**Happy Coding! 🎉**
