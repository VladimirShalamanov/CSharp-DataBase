CREATE DATABASE Service

GO

USE Service

GO

CREATE TABLE Users
(
    Id INT PRIMARY KEY IDENTITY,
    Username VARCHAR(30) UNIQUE NOT NULL,
    [Password] VARCHAR(50) NOT NULL,
    [Name] VARCHAR(50),
    Birthdate DATETIME,
    Age INT CHECK(Age BETWEEN 14 AND 110) NOT NULL,
    Email VARCHAR(50) NOT NULL
)

CREATE TABLE Departments
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL
)

CREATE TABLE Employees
(
    Id INT PRIMARY KEY IDENTITY,
    FirstName VARCHAR(25),
    LastName VARCHAR(25),
    Birthdate DATETIME,
    Age INT CHECK(Age BETWEEN 18 AND 110),
    DepartmentId INT REFERENCES Departments(Id) NOT NULL
)

CREATE TABLE Categories
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL,
    DepartmentId INT REFERENCES Departments(Id) NOT NULL
)

CREATE TABLE Status
(
    Id INT PRIMARY KEY IDENTITY,
    Label VARCHAR(20) NOT NULL
)

CREATE TABLE Reports
(
    Id INT PRIMARY KEY IDENTITY,
    CategoryId INT REFERENCES Categories(Id) NOT NULL,
    StatusId INT REFERENCES [Status](Id) NOT NULL,
    OpenDate DATETIME NOT NULL,
    CloseDate DATETIME,
    [Description] VARCHAR(200) NOT NULL,
    UserId INT REFERENCES Users(Id) NOT NULL,
    EmployeeId INT REFERENCES Employees(Id)
)

-- 02

INSERT INTO Employees(FirstName,LastName,Birthdate,DepartmentId)
    VALUES
        ('Marlo','O`Malley','1958-9-21',1),
        ('Niki','Stanaghan','1969-11-26',4),
        ('Ayrton','Senna','1960-03-21',9),
        ('Ronnie','Peterson','1944-02-14',9),
        ('Giovanna','Amati','1959-07-20',5)

INSERT INTO Reports(CategoryId,StatusId,OpenDate,CloseDate,[Description],UserId,EmployeeId)
    VALUES
        (1,1,'2017-04-13',NULL,'Stuck Road on Str.133',6,2),
        (6,3,'2015-09-05','2015-12-06','Charity trail running',3,5),
        (14,2,'2015-09-07',NULL,'Falling bricks on Str.58',5,2),
        (4,3,'2017-07-03','2017-07-06','Cut off streetlight on Str.11',1,1)

-- 03

UPDATE Reports
   SET CloseDate = GETDATE()
 WHERE CloseDate IS NULL

-- 04

DELETE FROM Reports WHERE StatusId = 4

-- 05

   SELECT r.Description,
          FORMAT(r.OpenDate, 'dd-MM-yyyy') AS OpenDate
     FROM Reports AS r
    WHERE r.EmployeeId IS NULL
 ORDER BY r.OpenDate, r.Description

-- 06

   SELECT r.Description,
          c.Name AS CategoryName
     FROM Reports AS r
LEFT JOIN Categories AS c ON c.Id = r.CategoryId
 ORDER BY r.Description, CategoryName
 
-- 07

   SELECT TOP(5)
          c.Name AS CategoryName,
          COUNT(r.CategoryId) AS ReportsNumber
     FROM Reports AS r
LEFT JOIN Categories AS c ON c.Id = r.CategoryId
 GROUP BY c.Name
 ORDER BY ReportsNumber DESC, CategoryName
  
-- 08

   SELECT u.Username,
          c.Name AS CategoryName
     FROM Reports AS r
LEFT JOIN Categories AS c ON c.Id = r.CategoryId
LEFT JOIN Users AS u ON u.Id = r.UserId
    WHERE DAY(u.Birthdate) = DAY(r.OpenDate) AND MONTH(u.Birthdate) = MONTH(r.OpenDate)
 ORDER BY u.Username, CategoryName
  
-- 09

   SELECT CONCAT(e.FirstName, ' ', e.LastName) AS FullName,
          COUNT(u.Id) AS UsersCount
     FROM Employees AS e
LEFT JOIN Reports AS r ON e.Id = r.EmployeeId
LEFT JOIN Users AS u ON r.UserId = u.Id
 GROUP BY e.FirstName, e.LastName
 ORDER BY UsersCount DESC, FullName
   
-- 10

   SELECT ISNULL(e.FirstName + ' ' + e.LastName, 'None') AS Employee,
          ISNULL(d.Name, 'None') AS Department,
          ISNULL(c.Name, 'None') AS Category,
          ISNULL(r.[Description], 'None') AS [Description],
          ISNULL(FORMAT(r.OpenDate, 'dd.MM.yyyy'), 'None') AS OpenDate,
          ISNULL(s.Label, 'None') AS [Status],
          ISNULL(u.Name, 'None') AS [User]
     FROM Reports AS r
LEFT JOIN Employees AS e ON e.Id = r.EmployeeId
LEFT JOIN Categories AS c ON c.Id = r.CategoryId
LEFT JOIN Departments AS d ON d.Id = e.DepartmentId
LEFT JOIN Users AS u ON u.Id = r.UserId
LEFT JOIN [Status] AS s ON s.Id = r.StatusId
 ORDER BY e.FirstName DESC, e.LastName DESC, Department, Category, r.[Description], r.OpenDate, s.Label, [User]

-- 11
GO

CREATE FUNCTION udf_HoursToComplete(@StartDate DATETIME, @EndDate DATETIME)
RETURNS INT
AS 
BEGIN
        DECLARE @TotalHours INT;
        IF (@StartDate IS NULL OR @EndDate IS NULL)
            BEGIN
                SET @TotalHours = 0;
            END
        ELSE
            BEGIN
                SET @TotalHours = DATEDIFF(HOUR, @StartDate, @EndDate);
            END
        RETURN @TotalHours;
END

GO

SELECT dbo.udf_HoursToComplete(OpenDate, CloseDate) AS TotalHours
   FROM Reports

-- 12
GO

CREATE PROCEDURE usp_AssignEmployeeToReport(@EmployeeId INT, @ReportId INT)
AS
BEGIN
        DECLARE @Employee INT = (
                                    SELECT DepartmentId FROM Employees
                                     WHERE Id = @EmployeeId                       
                                )

        DECLARE @Report INT =   (
                                    SELECT c.DepartmentId
                                      FROM Reports AS r
                                 LEFT JOIN Categories AS c ON r.CategoryId = c.Id
                                     WHERE r.Id = @ReportId                       
                                )

        IF (@Employee <> @Report)
            THROW 100000, 'Employee doesn''t belong to the appropriate department!', 1
            
        UPDATE Reports
           SET EmployeeId = @EmployeeId
         WHERE Id = @ReportId
END