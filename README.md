# SoldierTrack

A fitness and athlete management web application built with ASP.NET Core 8.0. Manage workouts, track nutrition, monitor memberships, and earn achievements — all in one place.

## Features

- **Workouts** — Browse, schedule, and enroll in workout sessions with category filtering and participant limits
- **Exercises** — Catalog of exercises organized by category
- **Nutrition Tracking** — Food catalog with daily food diary (MyDiary) and meal planning
- **Memberships** — Membership request and archive management for athletes
- **Achievements** — Badge and achievement system for athletes
- **Admin Area** — Full CRUD management for athletes, workouts, exercises, and food items
- **Monitoring** — Built-in Prometheus metrics with Grafana dashboards

## Tech Stack

| Layer      | Technology                           |
| ---------- | ------------------------------------ |
| Framework  | ASP.NET Core 8.0 (MVC / Razor Pages) |
| ORM        | Entity Framework Core 8.0            |
| Database   | Microsoft SQL Server 2022            |
| Auth       | ASP.NET Core Identity                |
| Email      | SendGrid                             |
| Metrics    | Prometheus + Grafana                 |
| Containers | Docker / Docker Compose              |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or Docker — see below)

### Local Development

1. Clone the repository:

   ```bash
   git clone https://github.com/aleksandarMilev/soldier-track.git
   cd soldier-track
   ```

2. Configure your connection string and SMTP settings in `SoldierTrack.Web/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=SoldierTrack;Trusted_Connection=True;"
     },
     "SmtpSettings": {
       "Host": "smtp.example.com",
       "Port": 587,
       "Username": "your@email.com",
       "Password": "your-password"
     }
   }
   ```

3. Restore, build, and run:

   ```bash
   dotnet restore SoldierTrack.sln
   dotnet run --project SoldierTrack.Web/SoldierTrack.Web.csproj
   ```

4. Open your browser at `http://localhost:5079` (HTTP) or `https://localhost:7102` (HTTPS).

The database is automatically migrated on startup in the development environment.

### Docker (Recommended)

1. Copy the environment file and fill in the required values:

   ```bash
   cp .env-sample .env
   ```

   | Variable                     | Description                    |
   | ---------------------------- | ------------------------------ |
   | `ASPNETCORE_ENVIRONMENT`     | `Development` or `Production`  |
   | `CONNECTION_STRING_PASSWORD` | SQL Server `SA` password       |
   | `SMTP_HOST`                  | SMTP server hostname           |
   | `SMTP_PORT`                  | SMTP server port               |
   | `SMTP_USERNAME`              | SMTP username / sender address |
   | `SMTP_PASSWORD`              | SMTP password                  |
   | `ADMIN_EMAIL`                | Seed admin account email       |
   | `ADMIN_PASSWORD`             | Seed admin account password    |

2. Start all services:
   ```bash
   docker-compose up --build
   ```

| Service    | URL                                   |
| ---------- | ------------------------------------- |
| Web app    | http://localhost:8080                 |
| Prometheus | http://localhost:9090                 |
| Grafana    | http://localhost:3000 (admin / admin) |

## Project Structure

```
SoldierTrack/
├── SoldierTrack.Web/        # ASP.NET Core MVC app (controllers, views, areas)
│   ├── Areas/
│   │   ├── Administrator/   # Admin-only controllers and views
│   │   └── Identity/        # Login, register, logout
│   └── ...
├── SoldierTrack.Services/   # Business logic layer
├── SoldierTrack.Data/       # EF Core models, DbContext, migrations
├── SoldierTrack.Common/     # Shared constants and utilities
├── prometheus/              # Prometheus configuration
├── grafana/                 # Grafana dashboard provisioning
├── docker-compose.yml
└── Dockerfile
```

## Running Tests

```bash
dotnet test SoldierTrack.Tests
```

The test suite (`SoldierTrack.Tests`) uses xUnit with an EF Core in-memory provider and Moq to verify service-layer business logic in isolation. 60 tests across four suites:

| Suite | Coverage |
| --- | --- |
| `AchievementServiceTests` | 1RM formula (both branches), CRUD, duplicate guard, ownership auth, pagination, rankings |
| `MembershipServiceTests` | Pending/approved/expired states, workouts-left counter (join/leave/workout deleted), expiry cleanup, approval flow |
| `WorkoutServiceTests` | Participant cap, date-slot conflict, UTC conversion on create/edit, cascade delete with email notifications |
| `ExerciseServiceTests` | Soft-delete visibility, custom exercise limit (>50), global vs. custom ownership auth, CRUD |

## Roles

| Role      | Access                                                   |
| --------- | -------------------------------------------------------- |
| Anonymous | Browse workouts and exercises                            |
| Athlete   | Enroll in workouts, food diary, membership, achievements |
| Admin     | Full management via the Administrator area               |

## License

This project is for educational and personal use.
