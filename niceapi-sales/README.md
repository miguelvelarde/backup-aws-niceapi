# niceapi-sales

## Overview
`niceapi-sales` is a serverless microservice built on AWS Lambda that provides a REST API for managing sales operations. It is part of the Nice API ecosystem and handles CRUD operations for sales records.

## Architecture
The project follows a clean architecture pattern with the following components:

- **AWS Lambda Functions**: Handles HTTP requests and responses.
- **Service Layer**: Implements business logic and database operations.
- **Data Access**: Uses MySQL stored procedures for database operations.

## API Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET    | `/sales/{records_to_return}` | Retrieves sales for the authenticated user. | `records_to_return`: Maximum number of records to return. |
| GET    | `/sales/client/{clientId}/{records_to_return}` | Retrieves sales for a specific client. | `clientId`: Client ID, `records_to_return`: Maximum number of records. |
| GET    | `/sales/{saleId}` | Retrieves a specific sale. | `saleId`: Sale ID. |
| POST   | `/sales` | Creates a new sale. | Sale details in the request body. |
| PUT    | `/sales` | Updates an existing sale. | Sale details with ID in the request body. |

## Authentication
All endpoints require JWT authentication. The token must be included in the `Authorization` header with the format: `Bearer {token}`.

## Data Models

### SaleModel
```json
{
  "saleId": int,
  "saleDate": datetime,
  "clientId": int?,
  "userId": int,
  "productId": int,
  "quantity": int,
  "price": decimal,
  "total": decimal,
  "status": string,
  "comments": string?,
  "noTicket": int,
  "saleType": string
}
```

## Database Operations
The service connects to MySQL and uses stored procedures for database operations:

- `sp_SalesGetByUser`: Retrieves sales for a specific user.
- `sp_SalesGetByClient`: Retrieves sales for a specific client.
- `sp_SalesGetBySaleId`: Retrieves a specific sale.
- `sp_SalesCreate`: Creates a new sale record.
- `sp_SalesUpdate`: Updates an existing sale record.

## Validation
Sale records are validated before processing:

- User ID must be greater than zero.
- Product ID must be greater than zero.
- Quantity must be greater than zero.
- Price must be greater than zero.
- Ticket number must be greater than zero.
- Sale type must be either "Contado" or "Credito".
- Status must be one of: "completed", "canceled", or "returned".

## Setup and Deployment

### Prerequisites
- .NET Core 6.0 or higher.
- AWS CLI configured.
- MySQL database with appropriate stored procedures.

### Configuration
The application requires the following configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

These can be provided through an `appsettings.json` file or environment variables.

### Deployment
Deploy using the AWS Serverless Application Model (SAM):

```bash
sam build
sam deploy --guided
```

## Error Handling
The API returns appropriate HTTP status codes:

- 200: Success.
- 400: Invalid request data.
- 401: Unauthorized access.
- 404: Resource not found.
- 500: Server error.

## Dependencies

- Amazon.Lambda.Core
- Amazon.Lambda.Annotations
- Amazon.Lambda.APIGatewayEvents
- MySql.Data
- Microsoft.Extensions.Configuration