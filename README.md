# RaceDay

RaceDay is an event management system designed for the South African road running, cycling, and walking community. Event Organisers can create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, and track their personal performance history.

This is an individual Portfolio of Evidence (POE) built progressively across three parts:

- **Part 1:** System planning — ERD, API endpoint plan, and SQL database script
- **Part 2:** RESTful API built in C#, connected to the database, with unit tests and GitHub CI/CD
- **Part 3:** MVC web application consuming the API, with Azure Blob Storage and Docker

## Roles

- **Organiser** — can create, edit, and delete events, manage event categories, capture participant results, view all event enrolments, and upload event banners.
- **Participant** — can create an account, browse events, enter an event by selecting a category, view their own enrolments, track their personal results, and manage their profile.

## Part 1 Contents (`/docs`)

- `RaceDay_ERD.png` — Entity Relationship Diagram
- `RaceDay_Database_Script.sql` — full SQL Server script (schema + seed data)
- `RaceDay_Part1_Submission_FINAL.docx` — full planning document (Sections A, B, C, and UI designs)

## Part 2: API Setup (RaceDay.Api)

The API is built using ASP.NET Core Web API and Entity Framework Core (SQL Server). It includes JWT Authentication for role-based access.

### How to run the API locally
1. Ensure SQL Server (LocalDB) is running.
2. Open the terminal in the `RaceDay.Api` folder.
3. Run `dotnet ef database update` to apply migrations.
4. Run `dotnet run` to start the API.
5. The Swagger interface will open at `http://localhost:5216`.

## Part 3: MVC Application (RaceDay.Mvc)

The front-end is an ASP.NET Core MVC application that consumes the Part 2 API. It uses session management to store the JWT token and provide role-based navigation.

### How to run the MVC App locally
1. Ensure the API is running in the background (`http://localhost:5216`).
2. Start the **Azurite** Storage Emulator (required for image uploads).
3. Set `RaceDay.Mvc` as the Startup Project in Visual Studio and press **F5**.
4. The app will open at `https://localhost:7118`.

### Azure Blob Storage Integration
Event banners are uploaded directly from the MVC app to the API, which then stores them in Azure Blob Storage (using Azurite for local development). The MVC app never communicates with Azure directly.

### Docker Setup
The application is fully containerised. The repository includes individual `Dockerfile`s for both the API and MVC projects, as well as a `docker-compose.yml` to orchestrate them together.

To run the full application using Docker:
1. Ensure Docker Desktop is running.
2. Open a terminal in the root directory (where `docker-compose.yml` is located).
3. Run the following command:
   ```bash
   docker-compose up --build
## Test Accounts

To test the application, use the following credentials (or register a new account using the Register page):

**Organizer Account:**
- Email: jimmybuhle@gmail.com
- Password: Jimmy10

**Participant Account:**
- Email: andy.jacobs@gmail.com
- Password: password123
