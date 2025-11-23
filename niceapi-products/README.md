# niceapi-products

## Overview

The `niceapi-products` project is an AWS Lambda-based API for managing products. It provides endpoints for retrieving, creating, and updating product information. The solution is implemented in C# and uses MySQL as the database.

## Features

- **Get Products**: Retrieve a list of products based on search criteria.
- **Create Product**: Add a new product to the database.
- **Update Product**: Modify an existing product's details.

## Project Structure

- **Functions.cs**: Contains the Lambda functions for handling API requests.
- **Service.cs**: Implements the business logic and database interactions.
- **IService.cs**: Defines the service interface for product operations.

## Endpoints

### GET /products

Retrieves products based on the provided search criteria.

- **Request Body**:
  ```json
  {
    "IdProduct": 1,
    "Name": "Product Name",
    "Description": "Product Description",
    "Type": "Product Type"
  }
  ```
- **Response**:
  - Success: Returns a list of products.
  - Error: Returns an error message with the appropriate HTTP status code.

### POST /products

Creates a new product.

- **Request Body**:
  ```json
  {
    "Name": "Product Name",
    "Description": "Product Description",
    "Image": "Image URL",
    "Price": 100.0,
    "Type": "Product Type"
  }
  ```
- **Response**:
  - Success: Returns the created product.
  - Error: Returns an error message with the appropriate HTTP status code.

### PUT /products

Updates an existing product.

- **Request Body**:
  ```json
  {
    "IdProduct": 1,
    "Name": "Updated Name",
    "Description": "Updated Description",
    "Image": "Updated Image URL",
    "Price": 150.0,
    "Type": "Updated Type"
  }
  ```
- **Response**:
  - Success: Returns the updated product.
  - Error: Returns an error message with the appropriate HTTP status code.

## Database

The solution uses MySQL stored procedures for database operations:

- `sp_ProductsGet`: Retrieves products based on search criteria.
- `sp_ProductsCreate`: Creates a new product.
- `sp_ProductsUpdate`: Updates an existing product.

## Error Handling

- **401 Unauthorized**: Returned when the user is not authorized.
- **500 Internal Server Error**: Returned for unexpected errors.

## Prerequisites

- .NET 8.0 SDK
- MySQL database
- AWS Lambda setup

## How to Run

1. Configure the database connection string in the environment variables or `appsettings.json`.
2. Deploy the solution to AWS Lambda using the `deploy.sh` script.
3. Test the API using tools like Postman or curl.
