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

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `POST` | `/api/auth/login` | Get JWT token | ❌ Public |
| `GET` | `/api/cases` | Get all elevator cases | ✅ Required |
| `POST` | `/api/cases` | Submit a new case request | ✅ Required |
| `GET` | `/api/cases/{id}` | Get a specific case | ✅ Required |
| `PUT` | `/api/cases/{id}/status` | Update case workflow status | ✅ Required |
| `POST` | `/api/cases/{id}/validate` | Run validation rules on a case | ✅ Required |
| `DELETE` | `/api/cases/{id}` | Delete a case | ✅ Required |

---

## 🔄 Case Workflow
```
SUBMITTED → VALIDATED → PROCESSING → COMPLETED
               ↓
            REJECTED  (if validation fails)
```

**Validation Rules:**
- Client name must be provided
- Floor count must be between **2 and 200**
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

## 🧪 Testing Guide

All endpoints can be tested directly through **Swagger UI** at `https://localhost:{port}/`

---

### Step 1 — Get Your JWT Token

All case endpoints are secured. First get a token:

**Endpoint:** `POST /api/auth/login`

**Request body:**
```json
{
  "username": "rahul",
  "password": "Admin@123"
}
```

**Expected response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": "60 minutes",
  "message": "Login successful! Copy the token and use it in Swagger Authorize."
}
```

---

### Step 2 — Authorize Swagger

1. Click the **`Authorize 🔒`** button at the top right of Swagger
2. Enter: `Bearer {your token}`
3. Click **Authorize** → **Close**

All 🔒 endpoints are now unlocked for testing.

---

### Step 3 — Test Scenarios

#### ✅ Scenario 1: Happy Path — Valid Case

**Create a case** `POST /api/cases`
```json
{
  "clientName": "KONE North America",
  "country": "United States",
  "buildingType": "Commercial",
  "floorCount": 25,
  "xmlPayload": "<case><design><floors>25</floors><type>commercial</type></design></case>"
}
```

**Expected:** `201 Created`
```json
{
  "id": 1,
  "caseNumber": "CASE-20260308091401",
  "clientName": "KONE North America",
  "status": "Submitted"
}
```

**Validate it** `POST /api/cases/1/validate`

**Expected:** `200 OK`
```json
{
  "isValid": true,
  "message": "Case validated successfully. Ready for processing."
}
```

---

#### ❌ Scenario 2: Rejection — Floor Count Too Low

**Create a case with floorCount: 1** `POST /api/cases`
```json
{
  "clientName": "KONE Europe",
  "country": "Finland",
  "buildingType": "Residential",
  "floorCount": 1,
  "xmlPayload": "<case></case>"
}
```

**Validate it** `POST /api/cases/{id}/validate`

**Expected:** `200 OK` — but auto-rejected!
```json
{
  "isValid": false,
  "message": "Validation failed: Floor count must be at least 2."
}
```

**Get the case** `GET /api/cases/{id}` — status is now `Rejected` with reason recorded ✅

---

#### ❌ Scenario 3: Rejection — Missing XML Payload

**Create a case with empty xmlPayload** `POST /api/cases`
```json
{
  "clientName": "KONE Asia",
  "country": "Japan",
  "buildingType": "Office",
  "floorCount": 10,
  "xmlPayload": ""
}
```

**Validate it** `POST /api/cases/{id}/validate`

**Expected:** Auto-rejected
```json
{
  "isValid": false,
  "message": "Validation failed: XML payload is required."
}
```

---

#### 🔄 Scenario 4: Full Workflow
```
1. POST /api/cases              → status: Submitted
2. POST /api/cases/{id}/validate → status: Validated
3. PUT  /api/cases/{id}/status  → status: Processing
4. PUT  /api/cases/{id}/status  → status: Completed
```

**Update to Processing** `PUT /api/cases/{id}/status`
```json
{
  "status": "Processing",
  "rejectionReason": null
}
```

**Update to Completed** `PUT /api/cases/{id}/status`
```json
{
  "status": "Completed",
  "rejectionReason": null
}
```

---

#### 🔒 Scenario 5: Auth — Invalid Credentials

**Login with wrong password** `POST /api/auth/login`
```json
{
  "username": "rahul",
  "password": "wrongpassword"
}
```

**Expected:** `401 Unauthorized`
```json
{
  "message": "Invalid username or password."
}
```

---

#### 🔍 Scenario 6: Not Found

**Get a case that doesn't exist** `GET /api/cases/9999`

**Expected:** `404 Not Found`
```json
{
  "message": "Case with ID 9999 not found."
}
```

---

#### 🗑️ Scenario 7: Delete a Case

**Delete** `DELETE /api/cases/{id}`

**Expected:** `204 No Content` ✅

**Verify it's gone** `GET /api/cases/{id}`

**Expected:** `404 Not Found` ✅

---

## 📁 Project Structure
```
ElevatorCaseFlow/
│
├── ElevatorCaseFlow.API/              ← Entry point, Controllers, Swagger, JWT
│   ├── Controllers/
│   │   ├── AuthController.cs          ← Login + token generation
│   │   └── CasesController.cs         ← All case endpoints
│   └── Program.cs                     ← DI, middleware, EF config
│
├── ElevatorCaseFlow.Application/      ← Business logic, DTOs, Interfaces
│   ├── DTOs/
│   │   ├── CreateCaseRequest.cs       ← What comes IN
│   │   └── CaseResponse.cs            ← What goes OUT
│   ├── Interfaces/
│   │   ├── ICaseRepository.cs         ← DB contract
│   │   └── ICaseService.cs            ← Service contract
│   └── Services/
│       └── CaseService.cs             ← All business rules live here
│
├── ElevatorCaseFlow.Domain/           ← Core entities and business rules
│   ├── Entities/
│   │   └── ElevatorCase.cs            ← Core domain model
│   └── Enums/
│       └── CaseStatus.cs              ← Workflow stages
│
└── ElevatorCaseFlow.Infrastructure/   ← Database, EF Core, Repositories
    ├── Data/
    │   └── AppDbContext.cs             ← EF Core DB context
    ├── Migrations/                     ← Auto-generated DB migrations
    └── Repositories/
        └── CaseRepository.cs          ← All SQL operations
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


