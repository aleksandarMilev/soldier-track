# SoldierTrack

## Project Description

**SoldierTrack** is a platform designed to assist athletes in managing their daily routines and fitness goals. The platform offers several key features:

- **Membership Management**: Users can create and manage memberships to join workouts organized in the dedicated workout hall.
- **1RM Calculator & Achievements Tracking**: Users can calculate their **1-rep max (1RM)** and track their progress in the **Achievements** section. The calculator supports all major exercises from **weightlifting** and **powerlifting**.
- **Food Diary & Nutrition Tracking**: Users can track their calorie and macronutrient intake through the **Food Diary** section. The platform allows users to create daily diaries where they log meals (breakfast, lunch, dinner, snacks) by choosing from a large food database. Each food entry includes detailed nutritional information, such as total calories, protein, carbohydrates, and fats per 100 grams.

## Technologies Used

This project is built using the following technologies and tools:

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server**
- **Bootstrap**
- **jQuery**
- **AutoMapper**
- **XUnit**
- **Moq**

---

## Table of Contents
- [Login](#login)
- [Memberships](#memberships)
## Features

## Login

SoldierTrack requires users to log in to access most features. If a user is not logged in, they can only view the home page, with no access to workouts, memberships, or other features.

- **Registration**: Users can register with just an email and password. Once registered, they can view available workouts and their details, as well as see the achievements ranks. However, at this stage, they can only perform **read** operations (viewing workouts and ranks) and cannot log achievements, track food, or request memberships.

  ![Register Example](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/register.png.png)

- **Becoming an Athlete**: After logging in, users will see a "Become Athlete" button in the navigation bar at the top of the page. By clicking it, they are prompted to fill out a form providing their name and phone number. Once the form is submitted, the user gains **Athlete** status, unlocking full access to all features, including logging achievements, tracking food, and requesting memberships for workouts.

  ![Become Athlete](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/become-athlete.png)

## Administrator

Users with the role of **Administrator** have the ability to perform **CRUD (Create, Read, Update, Delete)** operations on athletes, workouts, and memberships. Their responsibilities include:

- Approving or rejecting membership requests.
- Managing workout schedules and details.
- Adding athletes to workouts when necessary, instead of the athletes doing it themselves.
- Modifying athlete and membership data as needed.

It’s important to note that administrators do not have access to athletes' achievements or food diaries.

- **Log in as Admin**:
  - **Email**: admin@mail.com
  - **Password**: admin1234

## Memberships

### Athletes

The **Membership** feature allows athletes to request and use their memberships:

- **Request Membership**: Athletes can request a membership through the homepage's carousel section labeled **"Memberships"** or by clicking the **"Request Membership"** button in the navigation bar.

- **Membership Options**: There are two membership options available:
  - **Monthly Membership**: Automatically expires after one month.
  - **Fixed Count Membership**: An indefinite membership that expires only when all workout participations have been used. Users can choose a participation count ranging from **1 to 30**.

- **Approval Process**: The admin team will review membership requests and either approve or reject them within the current working day. Requests made outside of working hours will be addressed on the next working day.

- **Start Date**: The monthly membership starts from the day it is approved. If the admin approves the membership the following day, the start date will be updated accordingly. Athletes cannot modify the start date; it will always be set to the current day and cannot be in the past or future.

- **Pricing**:
  - **Fixed Count Membership**: **8 leva** per workout.
  - **Monthly Membership**: A total of **200 leva**.

All these details are provided above the **Request Membership** form.

![Request Membership](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/request-membership.png)

After the athlete makes the request, they will be redirected to their profile, where they will see that they have a (non-approved) membership. The **"Request Membership"** button in the navigation will be removed, and if the athlete clicks on the **"Request Membership"** button in the memberships carousel section, they will be redirected back to their profile. Here is how the profile looks now:

![Athlete Profile After Request](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/athlete-profile-afterreq.png)

Once the athlete submits their membership request, they will need to wait for it to be approved. While the membership is pending, if they navigate to the details of any workout, they will not see the **"Join"** button, as it is only available when the membership is approved. 

Once the membership is approved, the athlete will receive an email notification. Additionally, their profile will update to reflect the membership status as **Active**, along with relevant information about the membership.

![Athlete Profile After Approved Membership](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/athlete-profile-approved-req.png)

Now, the athlete is free to choose a workout and join it:

![Athlete Join](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/join-workout-bfr.png)

They can also leave the workout later:

![Athlete Leave Workout](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/join-workout-aft.png)

An important note here is that if the athlete decides to leave a workout and their membership is a **Fixed Count Membership**, the participation will be restored. However, if the membership is a **Monthly Membership**, there will be no compensation for leaving the workout.

This concludes the most important aspects of the memberships from the athlete's perspective.

### Administrators

Administrators cannot create memberships themselves. They can only approve or reject membership requests and edit them if necessary. When an athlete requests a membership, the admin will see a **Pending Memberships** button in the navigation, which redirects them to a dedicated page for managing these requests:

![Pending Memberships](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/pending2.png)

Additionally, when they open the **Athletes Get All** page, they will see a **Membership Pending** button next to each athlete. By clicking this button, they will be presented with options to approve or reject the membership:

![Athlete Membership Pending](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/pending1.png)

#### Adding Athletes to Workouts

Once an athlete's membership is approved, the administrator will see an **"Add to Workout"** button located on the right side of the athlete's entry on the **Get All Athletes** page. 

When the admin clicks this button, they will be presented with a form where they need to specify the date and time for the workout. It is crucial to ensure that a workout exists at the specified date and time; otherwise, the operation will not be executed successfully.

![Add Athlete Fail](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/add-fail.png)

If a valid workout is found, the athlete will be added successfully, and the admin will be redirected to the specific workout page.

![Add Athlete Success](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/add-scs.png)

Once an athlete has been added to a workout, the administrator has the option to remove them from the list below the workout information. 

![Remove Athlete Success](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/admin-remove.png)

## Workouts

Workouts are created by admins, who can perform all CRUD operations on them later. Athletes can only view workouts and join or leave them. Admin users will see a **Create Workout** button in the navigation menu. A workout will not be created if:

- The selected date and time are not within the range of today to one month from today.
- A workout is already scheduled for the selected date and time.
- The **Brief**, **Full Description**, and **ImageUrl** fields are optional; however, **Categories** and **Times** are predefined and must be selected from the dropdown menu.

If the workout is created successfully, the admin will be redirected to the Workout Details page

![Workout create success](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/wrk-cr-s.png)

Otherwise, they will receive an error message

![Workout create fail](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/wrk-cr-f.png)

When admins navigate to the **Get All Workouts** page, they will see **Edit** and **Delete** buttons for managing workouts.

![Workout edit delete](https://github.com/aleksandarMilev/SoldierTrack/blob/master/%2C%20screenshots/wrk-ed-del.png)

Admins can also delete workouts that have athletes already joined if necessary. In this case, the athletes will receive an email notification regarding the deletion.
