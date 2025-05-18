# 📄 DocumentManagement Project

## 🔍 Overview

The **DocumentManagement** project is a robust and scalable document management system built using **C# 12.0** and **.NET 8**. It offers a secure and modular **RESTful API** for managing documents, user authentication, and ingestion workflows. Designed for enterprise-level use cases, the system emphasizes **clean architecture**, **security**, and **extensibility**.

---

## ✨ Features

### 1. User Authentication and Authorization

- Secure registration and login with **BCrypt-hashed passwords**
- **Role-Based Access Control (RBAC)**: Supports `admin`, `editor`, and `viewer` roles
- **JWT authentication** for protected API access

### 2. Document Management

- Full **CRUD** operations for documents
- **Pagination support** for efficient document listing
- Role restrictions:
  - `admin` and `editor`: Create/Update
  - `admin`: Delete

### 3. Ingestion Workflow

- Trigger ingestion requests per document
- Track ingestion statuses: `Pending`, `InProgress`, `Complete`, `Cancelled`
- Ready for **external ingestion service integration**

### 4. Database Integration

- Powered by **Entity Framework Core**
- Key entities: `User`, `Document`, `IngestionRequest`

### 5. Logging & Error Handling

- **Centralized logging** using `ILogger`
- Graceful error responses for a better API consumer experience

---

## 🧱 Project Structure

### 🔹 Controllers

- `DocumentsController`: Handles all document-related APIs
- `IngestionController`, `AuthController`: For ingestion and auth flows
- Secured using `[Authorize]` attributes

### 🔹 Services

- `AuthService`: Handles registration, login, JWT generation
- `DocumentService`: Business logic for document management
- `IngestionService`: Manages ingestion requests and statuses

### 🔹 Models

#### Entities

- `User`: App users with roles and credentials
- `Document`: Document metadata like title, content, timestamps
- `IngestionRequest`: Tracks ingestion process

#### DTOs

- `RegisterDto`, `LoginDto`, `DocumentDto`, `IngestionRequestDto`, etc.

### 🔹 Data Layer

- `ApplicationDbContext`: Entity mappings and EF Core configurations

### 🔹 Utility

- `JwtSettings`: Configuration for JWT issuer, audience, and secret key

---

## 📌 API Endpoints

### 🔐 Authentication

- `POST /api/auth/register` – Register a new user
- `POST /api/auth/login` – Get JWT token after login
- `POST /api/auth/logout` – Logout - Invalidate token
- `POST /api/auth/set-role` – Update role for existing user

- ![AuthorizedSwagger](https://github.com/user-attachments/assets/599481ff-c4dc-4319-ab25-69ca22d7bd4a)
- ![AuthorizedUsingBearer](https://github.com/user-attachments/assets/e7491129-ffb3-45f9-ad36-cf81387cfe6a)
- ![Register](https://github.com/user-attachments/assets/48e77c0f-ae8a-482b-a0c5-31c27ae253fe)
- ![login](https://github.com/user-attachments/assets/18ea9490-6394-42f9-b676-15c867f2f2ea)
- ![PasswordValidation](https://github.com/user-attachments/assets/635d57b9-dd6a-45d2-8bd5-a05938ddadb6)
- ![UsersTable](https://github.com/user-attachments/assets/87aabb58-2a06-426e-b4a7-de94fa50857f)
- ![InvalidRole](https://github.com/user-attachments/assets/645f0dbe-fa69-4308-bdc3-50f4c92f25ae)
- ![SetRole](https://github.com/user-attachments/assets/af902c00-cb4a-4cd8-9744-c3a42189a8f8)







### 📁 Document Management

- `GET /api/document` – Get all documents (paginated)
- `GET /api/document/{id}` – Get document by ID
- `POST /api/document` – Create document (admin/editor only)
- `PUT /api/document/{id}` – Update document (admin/editor only)
- `DELETE /api/document/{id}` – Delete document (admin only)

- ![GetAllDocumentsWithPagination](https://github.com/user-attachments/assets/5c18b037-56b8-4e1b-b6e8-733144aa8aec)
- ![DocumentByIds](https://github.com/user-attachments/assets/c91eaacf-2fd8-4bf6-be6a-d844a93ecf30)
- ![DocumentsTable](https://github.com/user-attachments/assets/db0dd4dd-4809-4a7b-a186-fb67270660b2)




### 🔄 Ingestion Workflow

- `GET /api/ingestion` – Get all ingestion requests
- `POST /api/ingestion` – Create new ingestion request

- ![Ingestion](https://github.com/user-attachments/assets/cfbdc9d1-7af8-40fa-afe1-0d5c88cda5cf)
- ![IngestionStatusUpdate](https://github.com/user-attachments/assets/24c21ac1-b604-4159-8396-23876d172b8a)
- ![IngestionRequestTable](https://github.com/user-attachments/assets/54235ce6-956c-41e1-9b70-a6d9ca28def3)




---

## 🛠️ Technologies Used

- **Framework**: .NET 8
- **Language**: C# 12.0
- **Database**: Entity Framework Core (supports SQL Server, others)
- **Authentication**: JWT (JSON Web Tokens)
- **Logging**: Microsoft.Extensions.Logging
- **DI Container**: Built-in .NET Dependency Injection

### 🧪 Running Tests

This project uses **xUnit** for unit testing and **Moq** for mocking dependencies.

![image](https://github.com/user-attachments/assets/2fa53ccb-9c06-4710-96f0-7d13c8eba78a)



