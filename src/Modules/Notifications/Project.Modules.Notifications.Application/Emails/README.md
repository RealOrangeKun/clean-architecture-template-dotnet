# Email Template System

## Overview

This module provides a scalable, strongly-typed email template system using Razor templates. The architecture ensures type safety, easy maintenance, and follows best practices.

## Architecture

### Core Components

1. **IEmailTemplate** - Interface that all email templates must implement
2. **EmailTemplate** - Abstract base class providing common functionality
3. **IEmailService** - Service for sending emails with templates
4. **EmailTemplatePaths** - Centralized constants for template paths

### Directory Structure

```
Emails/
├── Templates/
│   ├── EmailTemplatePaths.cs          # Template path constants
│   ├── Welcome/
│   │   ├── WelcomeEmailTemplate.cs    # Strongly-typed model
│   │   └── WelcomeEmail.cshtml        # Razor template
│   └── [Other Templates]/
└── SendWelcomeEmail/
    └── SendWelcomeEmailCommand.cs     # Command to send email
```

## How to Add a New Email Template

### Step 1: Add Template Path Constant

Edit `EmailTemplatePaths.cs` and add your template path:

```csharp
public static class PasswordReset
{
    public const string TemplatePath = $"{TemplateBasePath}/PasswordReset/PasswordResetEmail.cshtml";
}
```

### Step 2: Create Strongly-Typed Template Model

Create a new file `Emails/Templates/PasswordReset/PasswordResetEmailTemplate.cs`:

```csharp
using Project.Modules.Notifications.Application.Abstractions.Email;

namespace Project.Modules.Notifications.Application.Emails.Templates.PasswordReset;

public sealed class PasswordResetEmailTemplate : EmailTemplate
{
    public PasswordResetEmailTemplate(
        string recipientEmail,
        string userName,
        string resetUrl,
        DateTime expiresAt)
        : base(recipientEmail)
    {
        UserName = userName;
        ResetUrl = resetUrl;
        ExpiresAt = expiresAt;
    }

    public override string TemplatePath => EmailTemplatePaths.PasswordReset.TemplatePath;
    
    public override string Subject => "Reset Your Password";

    public string UserName { get; }
    public string ResetUrl { get; }
    public DateTime ExpiresAt { get; }
    
    public string ExpiryTimeRemaining => 
        $"{(ExpiresAt - DateTime.UtcNow).TotalHours:F0} hours";
}
```

### Step 3: Create Razor Template

Create `Emails/Templates/PasswordReset/PasswordResetEmail.cshtml`:

```html
@using Project.Modules.Notifications.Application.Emails.Templates.PasswordReset
@model PasswordResetEmailTemplate

<!DOCTYPE html>
<html>
<head>
    <title>@Model.Subject</title>
</head>
<body>
    <h1>Password Reset Request</h1>
    <p>Hi @Model.UserName,</p>
    <p>Click the link below to reset your password:</p>
    <a href="@Model.ResetUrl">Reset Password</a>
    <p>This link expires in @Model.ExpiryTimeRemaining.</p>
</body>
</html>
```

### Step 4: Create Command to Send the Email

Create `Emails/SendPasswordResetEmail/SendPasswordResetEmailCommand.cs`:

```csharp
using FluentResults;
using Project.Common.Application.Messaging;
using Project.Modules.Notifications.Application.Abstractions.Email;
using Project.Modules.Notifications.Application.Emails.Templates.PasswordReset;

namespace Project.Modules.Notifications.Application.Emails.SendPasswordResetEmail;

public sealed record SendPasswordResetEmailCommand(
    string Email,
    string UserName,
    string ResetUrl,
    DateTime ExpiresAt)
    : ICommand;

internal sealed class SendPasswordResetEmailCommandHandler(
    IEmailService emailService)
    : ICommandHandler<SendPasswordResetEmailCommand>
{
    public async Task<Result> Handle(
        SendPasswordResetEmailCommand request, 
        CancellationToken cancellationToken)
    {
        var template = new PasswordResetEmailTemplate(
            recipientEmail: request.Email,
            userName: request.UserName,
            resetUrl: request.ResetUrl,
            expiresAt: request.ExpiresAt
        );

        bool success = await emailService.SendTemplateAsync(
            template, 
            cancellationToken);

        return success 
            ? Result.Ok() 
            : Result.Fail("Failed to send password reset email");
    }
}
```

### Step 5: Use the Command

```csharp
// In your application code
var command = new SendPasswordResetEmailCommand(
    Email: "user@example.com",
    UserName: "John Doe",
    ResetUrl: "https://app.com/reset?token=xyz",
    ExpiresAt: DateTime.UtcNow.AddHours(24)
);

await mediator.Send(command);
```

## Best Practices

### 1. Type Safety
- Always use strongly-typed models inheriting from `EmailTemplate`
- Define all template properties in the model class
- Use descriptive property names

### 2. Template Organization
- One template per folder
- Keep related files together (model + cshtml)
- Use namespaces that match folder structure

### 3. Path Management
- Always use `EmailTemplatePaths` constants
- Never hardcode template paths in commands or handlers
- Keep paths consistent with folder structure

### 4. Reusability
- Create shared CSS styles as inline styles or separate style classes
- Extract common template sections into partial views if needed
- Consider creating base templates for consistent branding

### 5. Testing
- Test template rendering with various data
- Verify all properties are displayed correctly
- Test email sending in different environments

## Configuration

Ensure your `appsettings.json` contains email configuration:

```json
{
  "Email": {
    "From": "noreply@yourapp.com",
    "Host": "smtp.yourprovider.com",
    "Port": 587
  },
  "FluentEmail": {
    "From": "noreply@yourapp.com",
    "Host": "smtp.yourprovider.com",
    "Port": 587,
    "Username": "your-username",
    "Password": "your-password"
  }
}
```

## Common Patterns

### Conditional Content
```csharp
@if (Model.SomeCondition)
{
    <p>Conditional content here</p>
}
```

### Lists
```csharp
@if (Model.Items.Any())
{
    <ul>
        @foreach (var item in Model.Items)
        {
            <li>@item</li>
        }
    </ul>
}
```

### Formatting
```csharp
<p>Date: @Model.Date.ToString("MMMM dd, yyyy")</p>
<p>Price: @Model.Price.ToString("C")</p>
```

## Troubleshooting

### Template Not Found
- Verify the template path in `EmailTemplatePaths.cs`
- Ensure the `.cshtml` file has `CopyToOutputDirectory` set to `Always`
- Check the file exists in the correct folder

### Compilation Errors
- Verify the `@model` directive matches your template class
- Check all property names are correct
- Ensure proper `@using` directives

### Styling Issues
- Use inline styles for better email client compatibility
- Test in multiple email clients
- Avoid complex CSS features

## Examples

See the `Welcome` template for a complete, production-ready example.
