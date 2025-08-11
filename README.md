# SoldierTrack

## Table of Contents
- [Overview](#overview)
- [Feature Limits](#feature-limits)
- [Technologies Used](#technologies-used)
- [Deployment & Hosting](#deployment--hosting)
- [Database Diagram](#database-diagram)
- [Accounts](#accounts)
  - [Administrator Account](#administrator-account)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [1. Clone the Repository](#1-clone-the-repository)
  - [2. Set Up Environment Variables](#2-set-up-environment-variables)
  - [3. Run the App](#3-run-the-app)
  - [4. (Optional) Run the Tests](#4-optional-run-the-tests)
- [Screenshots](#screenshots)
  - [Guest Home Page](#guest-home-page)
  - [Authenticated User Home Page](#authenticated-user-home-page)
  - [Register](#register)
  - [Request Membership](#request-membership)
  - [Workouts List](#workouts-list)
  - [Workout Details](#workout-details)
  - [User Profile](#user-profile)
  - [Exercises List](#exercises-list)
  - [Achievements](#achievements)
  - [Food List](#food-list)
  - [Diary](#diary)
  - [Diary Details](#diary-details)
  - [Athlete List (Admin)](#athlete-list-admin)
- [License](#license)
- [Contributing](#contributing)

## Overview

**SoldierTrack** is a fitness management platform tailored for athletes to track progress, optimize training, and manage nutrition. It offers:

- **Membership Management**: Join workout halls with fixed-start memberships.
- **1RM Calculator & Achievements**: Calculate your one-rep max for major lifts and track progress.
- **Food Diary**: Log meals using a detailed food database or custom entries with full macro breakdowns.

## Feature Limits

- Memberships must start on the current day and cannot be modified.
- Users can create up to **50 custom exercises**.
- Users can create up to **50 custom food entries**.
- Diary entries cannot be:
  - Older than 1 month ago
  - More than 1 month in the future

## Technologies Used

This project is built using the following technologies and tools:

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap
- jQuery
- AutoMapper
- XUnit
- Moq
- Azure App Service
- Docker
- GitHub Actions

## Deployment & Hosting

**SoldierTrack** is deployed to **Microsoft Azure** using **Docker** containers and an automated **GitHub Actions** CI/CD pipeline.  
Every push to the master branch triggers:

1. Automated Docker build
2. Docker image creation and push to Azure Container Registry
3. Deployment to Azure App Service

Live application: [https://soldiertrack-app.azurewebsites.net](https://soldiertrack-app.azurewebsites.net)

## Database Diagram

![Database Diagram](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/db.png)

## Accounts

### Administrator Account:

The application will start with a built-in account that has admin rights. The email and password will be whatever you specified in the `.env` file (more about `.env` in the next section).

- Email: admin@mail.com
- Password: admin1234

## Getting Started

To run the application locally using Docker:

### Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) installed and running

### 1. Clone the Repository

```bash
git clone https://github.com/aleksandarMilev/soldier-track
cd soldier-track
```

### 2. Set Up Environment Variables

Create a .env file in the root directory (you can use the .env-sample as skeleton). It should be something like this:

```bash
ASPNETCORE_ENVIRONMENT=Development

CONNECTION_STRING_PASSWORD=!Passw0rd

SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=foo@mail.com
SMTP_PASSWORD=foo

ADMIN_EMAIL=foo@boo.com
ADMIN_PASSWORD=bar

```

### 3. Run the app

Use Docker Compose to start the application and the SQL Server database:

```bash
docker-compose up --build -d
```

The application should be available at **http://localhost:8080**

### 4. (_Optional_) Run the tests

Simply run `dotnet test` from the project root.

## Screenshots

### Guest Home Page

![Guest Home Page](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/home.png)

### Authenticated User Home Page

![Authenticated User Home Page](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/home-aut.png)

### Register

![Register](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/register.png)

### Request Membership

![Request Membership](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/request-m-form.png)

### Workouts List

![Workouts List](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/workouts.png)

### Workout Details

![Workout Details](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/w-details.png)

### User Profile

![User Profile](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/profile.png)

### Exercises List

![Exercises List](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/exercises.png)

### Achievements

![Achievements](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/achv.png)

### Food List

![Food List](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/food.png)

### Diary

![Diary](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/diary.png)

### Diary Details

![Diary Details](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/diary-d-1.png)
![Diary Details](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/diary-d-2.png)

### Athlete List (Admin)

![Athlete List (Admin)](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/admin-athlete.png)

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).  
You are free to use, modify, and distribute it as you like.

---

## Contributing

Contributions are welcome and appreciated! 🙌  
If you find a bug, want to suggest a feature, or improve existing functionality, feel free to open an issue or submit a pull request (PR).

Steps to contribute:

1. Create a new branch from develop (`git switch -c feature/foo develop`)
2. Make your changes
3. Commit them (`git commit -m "Add some cool feature"`)
4. Push (`git push origin feature/foo`)
5. Open a pull request from `foo` to `develop`

I'll be extremely happy to review and merge your PR!

Known issues:

- Grafana does not visualize HTTP Requests Rate
- All images are saved as URLs at the database (except the static ones, they are saved as files at `/www.root`). This is not ideal. It will be better to keep them as files too.
- There are a few unit tests and not integration tests at all.
- The code itself is not commented at all (add XML comments, especially in the service interfaces and the extension methods for the web project).
- The application does not have any logging, which makes troubleshooting a nightmare.
