-- ============================================
-- RaceDay Database Creation Script
-- Part 1: System Planning and Database
-- ============================================

-- DROP THE DATABASE IF IT ALREADY EXISTS
-- This removes the old database so we can start fresh
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'RaceDayDB')
BEGIN
    DROP DATABASE RaceDayDB;
END
GO

-- Create the database
CREATE DATABASE RaceDayDB;
GO

-- Use the new database
USE RaceDayDB;
GO

-- ============================================
-- TABLE CREATION
-- ============================================

-- 1. USERS Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 2. EVENTS Table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Distance NVARCHAR(50) NOT NULL,
    EventType NVARCHAR(50) NOT NULL CHECK (EventType IN ('Running', 'Walking', 'Cycling')),
    OrganiserId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);
GO

-- 3. CATEGORIES Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    EventId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);
GO

-- 4. ENROLMENTS Table
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Confirmed', 'Pending', 'Cancelled')),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT UQ_Enrolment_Unique UNIQUE (ParticipantId, CategoryId)
);
GO

-- 5. RESULTS Table
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE,
    FinishTime TIME NULL,
    Position INT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Finished', 'DNF', 'DNS', 'Pending')),
    Notes NVARCHAR(200) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId)
);
GO

-- 6. PROFILES Table
CREATE TABLE Profiles (
    ProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    Bio NVARCHAR(500) NULL,
    CONSTRAINT FK_Profile_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- ============================================
-- INSERT SAMPLE DATA (SEED DATA)
-- ============================================

-- Insert Organisers (2 organisers)
INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
VALUES 
('Thabo Mokoena', 'thabo@racerun.co.za', 'hashed_password_1', 'Organiser', GETDATE()),
('Sarah Naidoo', 'sarah@capetowncycle.co.za', 'hashed_password_2', 'Organiser', GETDATE());
GO

-- Insert Participants (2 participants)
INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
VALUES 
('Andy Jacobs', 'andy.jacobs@gmail.com', 'hashed_password_3', 'Participant', GETDATE()),
('Pamela Jim', 'pamela.jim@yahoo.com', 'hashed_password_4', 'Participant', GETDATE());
GO

-- Insert Events (4 events)
INSERT INTO Events (EventName, Description, EventDate, Location, Distance, EventType, OrganiserId)
VALUES 
('Johannesburg City Marathon', 'Annual marathon through the streets of Johannesburg', '2026-09-15', 'Johannesburg, Gauteng', '42.2km', 'Running', 1),
('Pretoria Park Run', 'Community park run in Pretoria', '2026-10-01', 'Pretoria, Gauteng', '5km', 'Running', 1),
('Cape Town Cycle Classic', 'Premier cycling event in Cape Town', '2026-11-05', 'Cape Town, Western Cape', '55km', 'Cycling', 2),
('Bloemfontein City Marathon', 'Marathon event in the heart of Bloemfontein', '2026-12-10', 'Bloemfontein, Free State', '42.2km', 'Running', 2);
GO

-- Insert Categories for Johannesburg City Marathon (EventId = 1)
INSERT INTO Categories (CategoryName, EventId)
VALUES 
('Full Marathon (42km)', 1),
('Half Marathon (21km)', 1);
GO

-- Insert Categories for Pretoria Park Run (EventId = 2)
INSERT INTO Categories (CategoryName, EventId)
VALUES 
('5km Run', 2),
('5km Walk', 2);
GO

-- Insert Categories for Cape Town Cycle Classic (EventId = 3)
INSERT INTO Categories (CategoryName, EventId)
VALUES 
('55km Fun Ride', 3),
('25km Family Ride', 3);
GO

-- Insert Categories for Bloemfontein City Marathon (EventId = 4)
INSERT INTO Categories (CategoryName, EventId)
VALUES 
('Full Marathon (42km)', 4),
('Half Marathon (21km)', 4);
GO

-- Insert Enrolments
INSERT INTO Enrolments (ParticipantId, CategoryId, Status, EnrolmentDate)
VALUES 
-- Andy Jacobs (ParticipantId = 3) enrolments
(3, 2, 'Confirmed', GETDATE()),  -- Half Marathon (21km) - Confirmed
(3, 5, 'Pending', GETDATE()),    -- 55km Fun Ride - Pending
-- Pamela Jim (ParticipantId = 4) enrolments
(4, 4, 'Confirmed', GETDATE()),  -- 5km Walk - Confirmed
(4, 7, 'Confirmed', GETDATE());  -- Full Marathon (42km) - Confirmed
GO

-- Insert Results
INSERT INTO Results (EnrolmentId, FinishTime, Position, Status, Notes)
VALUES 
-- Andy Jacobs Half Marathon result (EnrolmentId = 1)
(1, '01:45:32', 45, 'Finished', 'Good performance'),
-- Pamela Jim 5km Walk result (EnrolmentId = 3)
(3, '00:35:15', 12, 'Finished', 'Personal best');
GO

-- Insert Profiles
INSERT INTO Profiles (UserId, DateOfBirth, Bio)
VALUES 
(1, '1985-03-15', 'Experienced marathon organiser based in Johannesburg'),
(2, '1990-07-22', 'Cycling event organiser from Cape Town'),
(3, '1992-11-08', 'Avid runner and cycling enthusiast'),
(4, '1988-05-03', 'Walking and running participant');
GO

-- ============================================
-- VERIFICATION QUERIES
-- ============================================

-- Check all users
PRINT '=== All Users ===';
SELECT UserId, FullName, Email, Role, CreatedAt FROM Users;
GO

-- Check all events with organiser names
PRINT '=== All Events with Organisers ===';
SELECT 
    e.EventName, 
    e.Location, 
    e.EventDate, 
    e.EventType,
    u.FullName AS Organiser
FROM Events e
JOIN Users u ON e.OrganiserId = u.UserId
ORDER BY e.EventDate;
GO

-- Check all categories with event names
PRINT '=== All Categories with Events ===';
SELECT 
    c.CategoryName,
    e.EventName,
    e.EventDate,
    e.Location
FROM Categories c
JOIN Events e ON c.EventId = e.EventId
ORDER BY e.EventDate, c.CategoryName;
GO

-- Check all enrolments
PRINT '=== All Enrolments ===';
SELECT 
    u.FullName AS Participant,
    c.CategoryName,
    e.EventName,
    en.Status,
    en.EnrolmentDate
FROM Enrolments en
JOIN Users u ON en.ParticipantId = u.UserId
JOIN Categories c ON en.CategoryId = c.CategoryId
JOIN Events e ON c.EventId = e.EventId
ORDER BY en.EnrolmentDate;
GO

-- Check results with participant names
PRINT '=== All Results ===';
SELECT 
    u.FullName AS Participant,
    c.CategoryName,
    e.EventName,
    r.FinishTime,
    r.Position,
    r.Status AS ResultStatus,
    r.Notes
FROM Results r
JOIN Enrolments en ON r.EnrolmentId = en.EnrolmentId
JOIN Users u ON en.ParticipantId = u.UserId
JOIN Categories c ON en.CategoryId = c.CategoryId
JOIN Events e ON c.EventId = e.EventId
ORDER BY r.Position;
GO

-- Summary statistics
PRINT '=== Database Summary ===';
SELECT 
    (SELECT COUNT(*) FROM Users) AS TotalUsers,
    (SELECT COUNT(*) FROM Users WHERE Role = 'Organiser') AS Organisers,
    (SELECT COUNT(*) FROM Users WHERE Role = 'Participant') AS Participants,
    (SELECT COUNT(*) FROM Events) AS TotalEvents,
    (SELECT COUNT(*) FROM Categories) AS TotalCategories,
    (SELECT COUNT(*) FROM Enrolments) AS TotalEnrolments,
    (SELECT COUNT(*) FROM Results) AS TotalResults;
GO