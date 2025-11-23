# Nice API Token Library

## Overview

This library provides utilities for authentication and authorization in AWS Lambda-based applications. It includes JWT token generation and validation, user management, and middleware to secure Lambda functions.

## Main Components

### 1. `JwtSettings`

Configuration class for JWT. It includes the following parameters:

- `SecretKey`: Secret key used to sign the tokens.
- `ExpiryMinutes`: Token expiration time in minutes.
- `Issuer`: Token issuer.
- `Audience`: Token audience.

### 2. `Tokenizer`

Static class that provides methods for working with JWT tokens and passwords.

- **`GenerateJwtToken`**: Generates a JWT token with the claims `UserId` and `Role`.
- **`HashPassword`**: Generates a SHA-256 hash for a password.
- **`VerifyPassword`**: Verifies if a password matches a stored hash.
- **`GetHashedValue`**: Returns the hash of a given value.

### 3. `UserModel`

Model representing a user in the system. It includes the following fields:

- `UserId`: Unique identifier for the user.
- `Phone`: User's phone number.
- `Name`: User's name.
- `Password`: User's password.
- `Type`: User type (`user` or `admin`).
- `Team`: Team the user belongs to.
- `Selfie`: URL of the user's image.
- `Status`: User status (`0`: Inactive, `1`: Active, `2`: Suspended).

### 4. `AuthMiddleware`

Class that validates JWT tokens to secure Lambda functions.

- **`ValidateToken`**: Validates a JWT token extracted from the `Authorization` header. It checks the signature, issuer, audience, and token expiration.

### 5. `AuthorizedFunctionBase`

Abstract base class for protected Lambda functions.

- **`ValidateRequest`**: Validates the `Authorization` header of a request and verifies the token.
- **`CreateSuccessResponse`**: Creates a successful HTTP response with serialized JSON data.
- **`CreateErrorResponse`**: Creates an HTTP error response with a message.
- **`GetUserIdFromRequest`**: Extracts the `UserId` from the JWT token included in the request.

## Usage Example

### JWT Configuration

```csharp
var jwtSettings = new JwtSettings
{
    SecretKey = "YourSecretKey",
    ExpiryMinutes = 60,
    Issuer = "NiceAppApi",
    Audience = "NiceAppMobile"
};
```

### Generate a JWT Token

```csharp
var user = new UserModel
{
    UserId = 1,
    Type = "admin"
};

string token = Tokenizer.GenerateJwtToken(user, jwtSettings.SecretKey, jwtSettings.ExpiryMinutes);
```

### Validate a Token in a Lambda Function

```csharp
public class MyProtectedFunction : AuthorizedFunctionBase
{
    public MyProtectedFunction() : base(new JwtSettings
    {
        SecretKey = "YourSecretKey",
        Issuer = "NiceAppApi",
        Audience = "NiceAppMobile"
    }) { }

    public APIGatewayHttpApiV2ProxyResponse Handler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try
        {
            int userId = GetUserIdFromRequest(request, context);
            return CreateSuccessResponse(new { Message = "Access granted", UserId = userId });
        }
        catch (UnauthorizedAccessException ex)
        {
            return CreateErrorResponse(401, ex.Message);
        }
    }
}
```

## Requirements

- .NET 8.0 or higher
- AWS Lambda

## Installation

Clone this repository and build it using the .NET SDK:

```bash
dotnet build
```

## License

This project is licensed under the MIT License.
