# Contributing to Orange Template

Thank you for your interest in contributing to this project! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Coding Standards](#coding-standards)
- [Adding New Features](#adding-new-features)
- [Database Migrations](#database-migrations)
- [Testing](#testing)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/your-username/orange-template.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`
4. Make your changes
5. Commit with conventional commits
6. Push to your fork
7. Open a Pull Request

## Development Setup

### Prerequisites

- .NET 10.0 SDK
- Docker and Docker Compose
- PostgreSQL 16+ (via Docker)
- Redis (via Docker)
- RabbitMQ (via Docker)
- Your favorite IDE (Visual Studio, Rider, or VS Code)

### Initial Setup

1. **Start infrastructure services:**
   ```bash
   docker-compose up -d
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update --project src/Modules/Users/Project.Modules.Users.Infrastructure --context UsersDbContext
   dotnet ef database update --project src/Modules/Notifications/Project.Modules.Notifications.Infrastructure --context NotificationsDbContext
   ```

4. **Run the application:**
   ```bash
   dotnet run --project src/API/Project.Api
   ```

## Project Structure

This project follows a **Modular Monolith** architecture with **Clean Architecture** principles:

```
src/
├── API/                          # Entry point
│   └── Project.Api/
├── Common/                       # Shared functionality
│   ├── Project.Common.Application/
│   ├── Project.Common.Domain/
│   ├── Project.Common.Infrastructure/
│   └── Project.Common.Presentation/
└── Modules/                      # Business modules
    ├── Users/
    │   ├── Project.Modules.Users.Application/
    │   ├── Project.Modules.Users.Domain/
    │   ├── Project.Modules.Users.Infrastructure/
    │   ├── Project.Modules.Users.IntegrationEvents/
    │   ├── Project.Modules.Users.Presentation/
    │   └── Project.Modules.Users.PublicApi/
    └── Notifications/
        └── ... (same structure)
```

### Layer Responsibilities

- **Domain**: Entities, value objects, domain events, business rules
- **Application**: Use cases, commands, queries, interfaces
- **Infrastructure**: Data access, external services, implementations
- **IntegrationEvents**: Events published to other modules
- **Presentation**: API endpoints, integration event handlers
- **PublicApi**: Public contracts exposed to other modules

## Coding Standards

### General Guidelines

- Use **C# 14** features appropriately
- Enable **nullable reference types**
- Follow **SOLID** principles
- Use **dependency injection** for all dependencies
- Prefer **immutability** where possible
- Use **records** for DTOs and value objects
- Use **primary constructors** for dependency injection (C# 12+ feature)

### Naming Conventions

- **Classes**: PascalCase (e.g., `UserService`)
- **Methods**: PascalCase (e.g., `GetUserById`)
- **Variables**: camelCase (e.g., `userId`)
- **Constants**: PascalCase (e.g., `MaxRetryAttempts`)
- **Private fields**: camelCase with underscore prefix (e.g., `_userRepository`)
- **Database objects**: snake_case (e.g., `user_id`, `created_at`)

### Code Organization

- One class per file
- Files named after the class they contain
- Organize files by feature (vertical slicing)
- Use folders to group related features

### CQRS Pattern

Commands and queries should follow these conventions:

```csharp
// Command (modifies state)
public sealed record CreateUserCommand(string Email, string FirstName) : ICommand;

// Command Handler - using primary constructor
internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Use injected dependencies directly
        var user = User.Create(command.Email, command.FirstName);
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}

// Query (reads state)
public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;

// Query Handler - using primary constructor
internal sealed class GetUserQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
        return user is not null 
            ? Result.Ok(new UserResponse(user.Id, user.Email)) 
            : Result.Fail<UserResponse>("User not found");
    }
}
```

### Result Pattern

Use `FluentResults` for error handling:

```csharp
public async Task<Result<User>> Handle(CreateUserCommand command)
{
    if (string.IsNullOrEmpty(command.Email))
        return Result.Fail<User>("Email is required");
    
    // Success
    return Result.Ok(user);
}
```

## Adding New Features

### Adding a New Module

1. Create the module structure (6 projects)
2. Add module to `Template.slnx`
3. Create `{Module}Module.cs` for DI registration
4. Register module in `Program.cs`
5. Add database context if needed
6. Create initial migration

Example module registration:

```csharp
public static class YourModule
{
    public static IServiceCollection AddYourModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDomainEventHandlers(typeof(IdempotentDomainEventHandler<>), Domain.AssemblyReference.Assembly)
            .AddIntegrationEventHandlers(typeof(IdempotentIntegrationEventHandler<>), Presentation.AssemblyReference.Assembly)
            .AddModuleEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }
}
```

### Adding a New Endpoint

1. Create endpoint class in `Presentation` layer
2. Implement `IEndpoint` interface
3. Endpoints are auto-registered via assembly scanning

```csharp
internal sealed class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
        })
        .WithTags("Users");
    }
}
```

### Adding Email Templates

See the [Email Template Documentation](src/Modules/Notifications/Project.Modules.Notifications.Application/Emails/README.md) for detailed instructions.

Quick steps:
1. Add template path to `EmailTemplatePaths.cs`
2. Create strongly-typed template class inheriting `EmailTemplate`
3. Create Razor `.cshtml` file
4. Create command and handler to send the email

## Database Migrations

### Creating a Migration

```bash
# Users module
dotnet ef migrations add MigrationName --project src/Modules/Users/Project.Modules.Users.Infrastructure --context UsersDbContext

# Notifications module
dotnet ef migrations add MigrationName --project src/Modules/Notifications/Project.Modules.Notifications.Infrastructure --context NotificationsDbContext
```

### Applying Migrations

```bash
dotnet ef database update --project src/Modules/Users/Project.Modules.Users.Infrastructure --context UsersDbContext
```

### Migration Guidelines

- Use descriptive names (e.g., `AddUserEmailIndex`, `CreateOrdersTable`)
- Review generated migrations before applying
- Never modify applied migrations
- Include both `Up` and `Down` methods
- Test migrations on a local database first

## Testing

### Test Structure

```
tests/
├── UnitTests/
├── IntegrationTests/
└── ArchitectureTests/
```

### Writing Tests

- Use **xUnit** for test framework
- Use **FluentAssertions** for assertions
- Follow **AAA pattern** (Arrange, Act, Assert)
- One assertion per test when possible
- Test names should describe the scenario

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateUserCommand("test@example.com", "John", "Doe");
        
        // Act
        var result = await _handler.Handle(command);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
```

## Commit Guidelines

We follow **Conventional Commits** specification:

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, semicolons, etc.)
- **refactor**: Code refactoring
- **perf**: Performance improvements
- **test**: Adding or updating tests
- **chore**: Maintenance tasks (dependencies, config, etc.)
- **build**: Build system changes
- **ci**: CI/CD changes

### Examples

```bash
feat(users): add user profile endpoints

fix(notifications): resolve email template rendering issue

docs: update API documentation for authentication

refactor(common): extract module registration to extensions

chore(deps): update Entity Framework to 10.0.2
```

### Commit Rules

- Use present tense ("add feature" not "added feature")
- Use imperative mood ("move cursor to..." not "moves cursor to...")
- Keep subject line under 50 characters
- Separate subject from body with a blank line
- Wrap body at 72 characters
- Reference issues/PRs in footer when applicable

## Pull Request Process

### Before Submitting

1. **Update your branch** with latest main:
   ```bash
   git fetch origin
   git rebase origin/main
   ```

2. **Ensure code quality:**
   - Run `dotnet build` - no warnings
   - Run all tests
   - Update documentation if needed

3. **Self-review** your changes:
   - Remove debug code
   - Check for commented code
   - Verify formatting
   - Review each file in the diff

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
How was this tested?

## Checklist
- [ ] Code follows project conventions
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] No new warnings
- [ ] Migrations created (if needed)
```

### Review Process

1. At least one approval required
2. All CI checks must pass
3. No merge conflicts
4. Up to date with main branch

### After Approval

- **Squash and merge** for feature branches
- **Rebase and merge** for hotfixes
- Delete branch after merge

## Code Review Guidelines

### For Authors

- Keep PRs small and focused
- Provide context in description
- Respond to feedback constructively
- Update PR based on feedback

### For Reviewers

- Be respectful and constructive
- Focus on code, not the person
- Explain the "why" behind suggestions
- Approve when concerns are addressed

## Questions or Issues?

- Open an issue for bugs or feature requests
- Start a discussion for questions
- Review existing issues before creating new ones

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing! 🎉
