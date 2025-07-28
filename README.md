# SoldierTrack

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

## Database Diagram

![Database Diagram](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/db.png)

## Accounts

### Administrator Account:

- Email: admin@mail.com
- Password: admin1234

### User Account:

- Email: MyUser@mail.com
- Password: 123456

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

Create a file similar to the .env-sample. It should be something like this:

```bash
ASPNETCORE_ENVIRONMENT=Development

CONNECTION_STRING=Server=sqlserver;Database=SoldierTrackDb;User Id=sa;Password=!Passw0rd;TrustServerCertificate=True;

SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=mail@gmail.com
SMTP_PASSWORD=smtp_password

SA_PASSWORD=!Passw0rd
```

### 3. Run the app

Use Docker Compose to start the application and the SQL Server database:

```bash
docker-compose up --build -d
```

The application should be available at **http://localhost:8080**

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

### Workouts List

![Workout List](https://github.com/aleksandarMilev/soldier-track/blob/master/screenshots/workouts.png)

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

1. Fork this repository
2. Create a new branch (`git switch -c feature-name`)
3. Make your changes
4. Commit your changes (`git commit -m "Add some feature"`)
5. Push to your branch (`git push origin feature-name`)
6. Open a pull request

I'll be extremely happy to review and merge your PR!
