# niceapi-users

## Description

This project implements a set of Lambda functions to manage users in an API. The functions are designed to run on AWS Lambda and use MySQL as the database. Additionally, a service is included to handle the business logic related to users.

## Lambda Functions

### GetUser
- **HTTP Route**: `GET /users`
- **Description**: Retrieves user information by their ID.
- **Parameters**:
  - `APIGatewayHttpApiV2ProxyRequest`: Incoming HTTP request.
  - `ILambdaContext`: Lambda execution context.
- **Responses**:
  - `200`: User information.
  - `401`: Unauthorized.
  - `500`: Internal server error.

### CreateUser
- **HTTP Route**: `POST /users`
- **Description**: Creates a new user.
- **Parameters**:
  - `UserModel`: Model of the user to be created.
  - `APIGatewayHttpApiV2ProxyRequest`: Incoming HTTP request.
  - `ILambdaContext`: Lambda execution context.
- **Responses**:
  - `200`: User successfully created.
  - `400`: User already exists.
  - `401`: Unauthorized.
  - `500`: Internal server error.

### UpdateUser
- **HTTP Route**: `PUT /users`
- **Description**: Updates the information of an existing user.
- **Parameters**:
  - `UserModel`: Model of the user to be updated.
  - `APIGatewayHttpApiV2ProxyRequest`: Incoming HTTP request.
  - `ILambdaContext`: Lambda execution context.
- **Responses**:
  - `200`: User successfully updated.
  - `400`: User ID does not match the ID in the request.
  - `401`: Unauthorized.
  - `500`: Internal server error.

## Service (`Service`)

The service implements the business logic to manage users. It provides the following methods:

### Methods

#### GetAllUsersAsync
- **Description**: Retrieves all users.
- **Return**: A list of users.

#### GetUser
- **Description**: Retrieves a user by their ID.
- **Parameters**:
  - `userId`: User ID.
- **Return**: User information or `null` if not found.

#### CreateUserAsync
- **Description**: Creates a new user.
- **Parameters**:
  - `UserModel`: Model of the user to be created.
- **Return**: Information of the created user.

#### UpdateUserAsync
- **Description**: Updates an existing user.
- **Parameters**:
  - `UserModel`: Model of the user to be updated.
- **Return**: Information of the updated user.

#### UpdateStatusAsync
- **Description**: Updates the status of a user.
- **Parameters**:
  - `userId`: User ID.
  - `status`: New status.
- **Return**: `true` if the update was successful, otherwise `false`.

## Models

### UserModel
The `UserModel` represents the structure of a user in the system. It includes the following fields:
- `UserId`: User ID.
- `Phone`: User's phone number.
- `Name`: User's name.
- `Password`: User's password (encrypted).
- `Type`: User type (`user` or `admin`).
- `Team`: Team to which the user belongs.
- `Selfie`: URL of the user's selfie.
- `Status`: User's status.