# User Service API Documentation

## Overview

- **Base URL:** `/api`
- **Authentication:** JWT Bearer token
- **Common Response Structure**

  All responses follow this structure:

  ```json
  {
    "status": "string",
    "code": integer,
    "data": object or array of returned data,
    "message": "string"
  }
  ```

---

## Endpoints

### 1. Account

#### **POST** `/api/Account/register`
Register a new user account.

- **Request Body:**
  ```json
  {
    "email": "string",
    "phone_number": "string",
    "password": "string",
    "roles": ["string"]
  }
  ```
- **Responses:**
  - `201 Created`: User account created successfully.
    ```json
    {
      "status": "string",
      "code": 201,
      "data": {
        "id": "uuid",
        "email": "string",
        "phone_number": "string",
        "email_confirmed": boolean,
        "phone_number_confirmed": boolean,
        "roles": ["string"],
        "created_at": "2025-04-07T00:00:00Z"
      },
      "message": "string"
    }
    ```

#### **POST** `/api/Account/login`
Login to an existing account.

- **Request Body:**
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- **Responses:**
  - `200 OK`: Login successful.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": {
        "token": "string",
        "user": {
          "id": "uuid",
          "email": "string",
          "phone_number": "string",
          "email_confirmed": boolean,
          "phone_number_confirmed": boolean,
          "roles": ["string"],
          "created_at": "2025-04-07T00:00:00Z"
        }
      },
      "message": "string"
    }
    ```

#### **GET** `/api/Account/me`
Retrieve the current authenticated user's profile.

- **Authorization:** Bearer token required
- **Responses:**
  - `200 OK`: User profile retrieved successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": {
        "id": "uuid",
        "email": "string",
        "phone_number": "string",
        "email_confirmed": boolean,
        "phone_number_confirmed": boolean,
        "roles": ["string"],
        "created_at": "2025-04-07T00:00:00Z"
      },
      "message": "string"
    }
    ```

#### **POST** `/api/Account/confirm-email`
Confirm a user's email address.

- **Query Parameters:**
  - `user_id` (uuid, required): The ID of the user.
  - `token` (string, required): The confirmation token.
- **Responses:**
  - `200 OK`: Email confirmed successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": boolean,
      "message": "string"
    }
    ```

#### **POST** `/api/Account/confirm-phone`
Confirm a user's phone number.

- **Query Parameters:**
  - `user_id` (uuid, required): The ID of the user.
  - `token` (string, required): The confirmation token.
- **Responses:**
  - `200 OK`: Phone number confirmed successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": boolean,
      "message": "string"
    }
    ```

#### **POST** `/api/Account/add-to-role`
Add a user to a role.

- **Authorization:** Bearer token required
- **Request Body:**
  ```json
  {
    "user_id": "uuid",
    "role_name": "string"
  }
  ```
- **Responses:**
  - `200 OK`: User added to role successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": boolean,
      "message": "string"
    }
    ```

#### **POST** `/api/Account/remove-from-role`
Remove a user from a role.

- **Authorization:** Bearer token required
- **Request Body:**
  ```json
  {
    "user_id": "uuid",
    "role_name": "string"
  }
  ```
- **Responses:**
  - `200 OK`: User removed from role successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": boolean,
      "message": "string"
    }
    ```

#### **GET** `/api/Account/roles`
Get roles for the current authenticated user.

- **Authorization:** Bearer token required
- **Responses:**
  - `200 OK`: Roles retrieved successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": ["string"],
      "message": "string"
    }
    ```

---

### 2. Roles

#### **GET** `/api/Roles`
Retrieve all available roles.

- **Authorization:** Bearer token required
- **Responses:**
  - `200 OK`: Roles retrieved successfully.
    ```json
    {
      "status": "string",
      "code": 200,
      "data": [
        {
          "name": "string"
        }
      ],
      "message": "string"
    }
    ```

#### **POST** `/api/Roles`
Create a new role.

- **Authorization:** Bearer token required
- **Request Body:**
  ```json
  {
    "name": "string"
  }
  ```
- **Responses:**
  - `201 Created`: Role created successfully.
    ```json
    {
      "status": "string",
      "code": 201,
      "data": boolean,
      "message": "string"
    }
    ```

---

## Security

Authentication is handled via JWT Bearer tokens. Include the token in the Authorization header for protected endpoints:

```
Authorization: Bearer {your_token}
```

The token is obtained afte