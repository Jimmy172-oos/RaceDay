# RaceDay

RaceDay is an event management system designed for the South African road running, cycling, and walking community. Event Organisers can create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, and track their personal performance history.

This is an individual Portfolio of Evidence (POE) built progressively across three parts:

- **Part 1:** System planning — ERD, API endpoint plan, and SQL database script (this submission)
- **Part 2:** RESTful API built in C#, connected to the database, with unit tests and GitHub CI/CD
- **Part 3:** MVC web application consuming the API, with Azure Blob Storage and Docker

## Roles

- **Organiser** — can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
- **Participant** — can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.

## Part 1 Contents (`/docs`)

- `RaceDay_ERD.png` — Entity Relationship Diagram
- `RaceDay_Database_Script.sql` — full SQL Server script (schema + seed data)
- `RaceDay_Part1_Submission_FINAL.docx` — full planning document (Sections A, B, C, and UI designs)

## CI/CD

A GitHub Actions workflow (`.github/workflows/validate.yml`) validates that the repository structure is correct — checking that `/docs` exists and contains the ERD, SQL script, and planning document.

**Build status screenshot:**

<img width="1295" height="476" alt="Workflow" src="https://github.com/user-attachments/assets/c573d656-1fa0-43a5-bc3a-6ac6720b502e" />


## Video Walkthrough

**YouTube (unlisted): https://youtu.be/_5ZlNYL9wQE

The video walks through the ERD design decisions, the API endpoint plan, and a live run of the SQL script in SQL Server Management Studio.
