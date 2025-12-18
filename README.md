# Stargate ACTS - Astronaut Career Tracking System

A full-stack application for managing astronaut personnel and tracking their duty assignments throughout their careers. Built with .NET 8 Web API backend and Angular 18 frontend.

**Project Completion:** December 2024

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Testing](#testing)
  - [Test Suite](#test-suite)
  - [Running Tests](#running-tests)
  - [Key Achievements](#key-achievements)
- [API Endpoints](#api-endpoints)
  - [Person Management](#person-management)
  - [Astronaut Duty Management](#astronaut-duty-management)
- [Business Rules](#business-rules)
- [Future Enhancements](#future-enhancements)

## Features

### Person Management
- Create new personnel records
- View all personnel with career details
- Update person names with duplicate prevention
- Real-time UI updates

### Astronaut Duty Management
- Create duty assignments with validation
- Track rank and duty title changes
- View complete duty history per astronaut
- Automatic career timeline management
- Retirement handling

### Quality & Security
- 87% unit test coverage (business logic)
- 75% total test coverage (including controllers)
- SQL injection protection (parameterized queries)
- Comprehensive input validation
- Database-persisted error logging
- CORS-enabled API


## Technology Stack

### Backend
- **.NET 8.0** - Web API framework
- **Entity Framework Core 9.0** - ORM
- **SQLite** - Database
- **MediatR 12.4** - CQRS pattern implementation
- **Dapper 2.1** - High-performance queries
- **XUnit** - Testing framework
- **FluentAssertions** - Test assertions
- **Coverlet** - Code coverage

### Frontend
- **Angular 18** - SPA framework
- **TypeScript** - Type-safe JavaScript
- **Bootstrap 5** - UI framework
- **RxJS** - Reactive programming
- **Angular Router** - Navigation

### Development Tools
- **Swagger/OpenAPI** - API documentation
- **ReportGenerator** - Coverage reports
- **GitHub Actions** - CI/CD pipeline


## Getting Started

### Prerequisites

**Required Software:**
- .NET 8.0 SDK or later
- Node.js 20.x or later
- npm 10.x or later

**Optional:**
- Visual Studio 2022 / VS Code
- Angular CLI (or use npx)

### Installation

**1. Clone the repository**
```bash
git clone [repo-url]
cd bamtech-acts
```

**2. Install backend dependencies**
```bash
cd api
dotnet restore
```

**3. Install frontend dependencies**
```bash
cd ../ui
npm install
```

### Running the Application

**Terminal 1 - Start the API:**
```bash
cd api
dotnet run
# API runs on http://localhost:5204
```

**Terminal 2 - Start Angular:**
```bash
cd ui
npm start
# UI runs on http://localhost:4200
```

**Access the application:**
- Frontend: http://localhost:4200
- API: http://localhost:5204
- Swagger: http://localhost:5204/swagger

---

## Testing

### Test Suite
- **41 tests total** (29 unit tests + 8 integration tests + 4 service tests)
- **87% unit test coverage** (business logic)
- **75% total coverage** (including controllers)
- Framework: XUnit with FluentAssertions
- Database: SQLite in-memory for test isolation

### Running All Tests
```bash
dotnet test
```

### Linux/Mac - Running Tests with Coverage
```bash
./generate-coverage.sh
```

### Windows - Running Tests with Coverage
```powershell
.\generate-coverage.ps1
```

## Key Achievements
1. All 7 business rules verified with tests
2. Critical business logic at >85% coverage
3. Proper HTTP status code handling tested
4. Validation logic comprehensively tested
5. Both unit and integration test layers
6. Test isolation with in-memory database
7. CI/CD ready (GitHub Actions compatible)

---

### Person Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Person` | Get all people with career details |
| GET | `/Person/{name}` | Get person by name |
| POST | `/Person` | Create new person |
| PUT | `/Person/{currentName}` | Update person's name |

### Astronaut Duty Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/AstronautDuty` | Create duty assignment |
| GET | `/AstronautDuty/{name}` | Get astronaut's duty history |


## Business Rules

The application enforces these business rules for astronaut duty assignments:

1. **Person must exist** before creating astronaut duty
2. **First duty creates AstronautDetail** with career start date
3. **New duty updates AstronautDetail** with current rank and title
4. **Previous duty gets end date** when new duty starts
5. **Previous duty end date = new duty start date** (no gaps)
6. **RETIRED duty sets career end date**
7. **RETIRED duty end date = career end date**



## Future Enhancements

- [ ] User authentication and role-based access
- [ ] Duty assignment approval workflow
- [ ] Career progression analytics
- [ ] Export to PDF/Excel
- [ ] Real-time notifications
- [ ] Audit trail for all changes
- [ ] Advanced search and filtering
- [ ] PostgreSQL/SQL Server support