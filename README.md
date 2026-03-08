# 🏢 ElevatorCaseFlow API

A production-grade REST API built with **ASP.NET Core 8** and **Clean Architecture** 
for managing elevator design case requests and workflow processing.

Inspired by real-world enterprise systems built for **KONE** — one of the world's 
largest elevator manufacturers.

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles — keeping business logic 
completely independent of frameworks, databases, and external services.
```
┌─────────────────────────────────┐
│           API Layer             │  Controllers, Swagger, JWT Auth
├─────────────────────────────────┤
│       Application Layer         │  Business Logic, Services, DTOs
├─────────────────────────────────┤
│         Domain Layer            │  Entities, Enums, Core Rules
├─────────────────────────────────┤
│      Infrastructure Layer       │  SQL Server, EF Core, Repositories
└─────────────────────────────────┘
```

---

## ✨ Features

- ✅ Full **CRUD** operations for elevator case requests
- ✅ **Workflow engine** — Submitted → Validated → Processing → Completed / Rejected
- ✅ **Business rule validation** — floor count limits, required fields, XML payload checks
- ✅ **JWT Authentication** — all endpoints secured with Bearer tokens
- ✅ **Swagger UI** — interactive API documentation and testing
- ✅ **EF Core Migrations** — database auto-created on any machine
- ✅ **Repository Pattern** — clean separation between business logic and data access
- ✅ **Async/Await** throughout — non-blocking database operations

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Language | C# .NET 8 |
| Database | SQL Server + Entity Framework Core 8 |
| Auth | JWT Bearer Authentication |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Architecture | Clean Architecture + Repository Pattern |
| ORM | Entity Framework Core |

---

## 📋 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/cases` | Get all elevator cases |
| `POST` | `/api/cases` | Submit a new case request |
| `GET` | `/api/cases/{id}` | Get a specific case |
| `PUT` | `/api/cases/{id}/status` | Update case workflow status |
| `POST` | `/api/cases/{id}/validate` | Run validation rules on a case |
| `DELETE` | `/api/cases/{id}` | Delete a case |

---

## 🔄 Case Workflow
```
SUBMITTED → VALIDATED → PROCESSING → COMPLETED
               ↓
            REJECTED  (if validation fails)
```

**Validation Rules:**
- Client name must be provided
- Floor count must be between 2 and 200
- XML payload must be present

---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (any edition)
- Visual Studio 2022

### Setup

**1. Clone the repository**
```bash
git clone https://github.com/rahullakkawar/ElevatorCaseFlow.git
cd ElevatorCaseFlow
```

**2. Update the connection string**

Open `ElevatorCaseFlow.API/appsettings.json` and update:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ElevatorCaseFlowDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**3. Run migrations — database auto-created!**
```bash
cd ElevatorCaseFlow.API
dotnet ef database update
```

**4. Run the API**
```bash
dotnet run
```

**5. Open Swagger UI**
```
https://localhost:{port}/
```

---

## 📁 Project Structure
```
ElevatorCaseFlow/
│
├── ElevatorCaseFlow.API/              ← Entry point, Controllers, Middleware
│   ├── Controllers/
│   │   └── CasesController.cs
│   └── Program.cs
│
├── ElevatorCaseFlow.Application/      ← Business logic, DTOs, Interfaces
│   ├── DTOs/
│   │   ├── CreateCaseRequest.cs
│   │   └── CaseResponse.cs
│   ├── Interfaces/
│   │   ├── ICaseRepository.cs
│   │   └── ICaseService.cs
│   └── Services/
│       └── CaseService.cs
│
├── ElevatorCaseFlow.Domain/           ← Core entities and business rules
│   ├── Entities/
│   │   └── ElevatorCase.cs
│   └── Enums/
│       └── CaseStatus.cs
│
└── ElevatorCaseFlow.Infrastructure/   ← Database, EF Core, Repositories
    ├── Data/
    │   └── AppDbContext.cs
    ├── Migrations/
    └── Repositories/
        └── CaseRepository.cs
```

---

## 💡 Background

This project mirrors the architecture of real enterprise systems I built at 
**Seligent Consulting** for **KONE** — a global elevator manufacturer.

Key concepts demonstrated:
- **MailSave** inspired the XML validation and case routing logic
- **FLCAD workflow** inspired the multi-stage case processing pipeline
- **AWS migration experience** informed the clean, decoupled architecture design

---

## 👨‍💻 Author

**Rahul Lakkawar**  
Full Stack Developer · Backend .NET Engineer · Cloud/AWS Architect  
[LinkedIn](https://www.linkedin.com/in/rahul-lakkawar) · rahullakkawar@gmail.com
