# niceapi-clients

## Overview

This project provides a set of Lambda functions to manage client data using AWS Lambda and MySQL. The solution is implemented in C# and includes the following key components:

- **Service Layer**: Handles database operations for creating, updating, and retrieving client data.
- **Lambda Functions**: Exposes HTTP endpoints for interacting with the service layer.
- **Stored Procedures**: MySQL stored procedures for efficient database operations.

## Key Components

### Service Layer

The `Service` class implements the `IService` interface and provides methods to interact with the database:

- `GetClientsAsync(int clientId, int userId)`: Retrieves a list of clients based on the provided `clientId` and `userId`.
- `CreateClientAsync(ClientModel client)`: Creates a new client in the database.
- `UpdateClientAsync(ClientModel client)`: Updates an existing client in the database.

The service uses MySQL stored procedures for database operations and validates input data before executing queries.

### Lambda Functions

The `Functions` class provides HTTP endpoints for client operations:

- `GET /clients`: Retrieves clients for a specific user.
- `POST /clients`: Creates a new client.
- `PUT /clients`: Updates an existing client.

Each function uses the `IService` implementation to perform the required operations and returns appropriate HTTP responses.

### Stored Procedures

The solution relies on the following MySQL stored procedures:

- `sp_ClientsGet`: Retrieves clients based on `clientId` and `userId`.
- `sp_ClientsCreate`: Creates a new client.
- `sp_ClientsUpdate`: Updates an existing client.

## How to Use

1. **Setup Database**: Ensure the required MySQL stored procedures are created in your database.
2. **Configure Environment**: Set the `DATABASE_CONNECTION_STRING` environment variable or update the `DefaultConnection` in the configuration file.
3. **Deploy**: Use the provided `deploy.sh` script to deploy the Lambda functions.

## Dependencies

- **MySQL.Data**: For database connectivity.
- **Amazon.Lambda.Annotations**: For defining Lambda functions and HTTP endpoints.
- **Amazon.Lambda.APIGatewayEvents**: For handling API Gateway requests and responses.

## Error Handling

The service and Lambda functions include error handling to return meaningful error messages in case of failures. For example:

- Database connection issues.
- Validation errors for client data.

## Example Usage

### Create a Client

```json
POST /clients
{
  "Name": "John Doe",
  "Phone": "123-456-7890",
  "Comments": "VIP client"
}
```

### Update a Client

```json
PUT /clients
{
  "ClientId": 1,
  "Name": "John Doe",
  "Phone": "987-654-3210",
  "Comments": "Updated comments"
}
```

### Get Clients

```json
GET /clients
{
  "ClientId": 1,
  "UserId": 42
}
```
