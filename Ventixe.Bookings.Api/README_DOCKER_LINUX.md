
# Ventixe.Bookings.Grupp5.Api (Docker + Linux)

A lightweight REST API for managing event bookings, built with ASP.NET Core and designed to run in Docker on Linux.

## 🐳 Run with Docker (Linux)

### Requirements
- Docker installed on your machine (Linux/Mac/WSL/Windows)
- .NET 9.0 SDK installed for building (if not building inside container)

### 🏗️ Build Docker Image

```bash
docker build -t ventixe-bookings-api .
```

### 🚀 Run Docker Container

```bash
docker run -d -p 5000:80 --name bookings-api ventixe-bookings-api
```

Swagger UI will be available at:  
[http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 🔧 Configuration

Ensure you have a connection string for SQL Server configured via environment variable:

```bash
-e "ConnectionStrings__SqlConnection=Your-SQL-Connection-String"
```

Or modify `appsettings.json` before building.

---

## 🛡️ JWT Authentication

This API expects a valid JWT bearer token.

- Set the authority and audience in `appsettings.json`:

```json
"Auth": {
  "Authority": "https://your-auth-provider",
  "ApiName": "ventixe.bookings.api"
}
```

---

## 🔍 API Endpoints

### 📄 GET `/api/bookings`
Returns a list of all bookings.

### 📄 GET `/api/bookings/{id}`
Returns a booking by ID.

### 🆕 POST `/api/bookings`
Creates a new booking.

### ✏️ PUT `/api/bookings/{id}`
Updates a booking.

### ❌ DELETE `/api/bookings/{id}`
Deletes a booking.

All endpoints require valid JWT tokens in the `Authorization: Bearer {token}` header.

---

## 🔧 Example C# Client Usage

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000");

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-jwt-token");

var result = await client.GetFromJsonAsync<List<BookingDto>>("/api/bookings");
foreach (var booking in result)
{
    Console.WriteLine($"{booking.Id}: {booking.Title}");
}
```

---

## 🧪 Swagger UI

Swagger will be available by default at:

```
http://localhost:5000/swagger
```

JWT authentication is supported via "Authorize" button in Swagger UI.

---

## 📦 NuGet Packages Used

- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

---

## 📁 DTOs

- `CreateBookingDto`
- `UpdateBookingDto`
- `BookingFilterDto`
- `BookingStatisticsDto`

Each DTO is used for strongly typed communication between client and server.
