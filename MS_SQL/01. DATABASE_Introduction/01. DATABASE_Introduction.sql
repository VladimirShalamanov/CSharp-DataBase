CREATE DATABASE [Minions]

USE Minions

CREATE TABLE [Minions]
(
    Id INT PRIMARY KEY,
    [Name] VARCHAR(100),
    Age INT
)

CREATE TABLE [Towns]
(
    Id INT PRIMARY KEY,
    [Name] VARCHAR(100)
)

ALTER TABLE Minions
  ADD [TownId] INT FOREIGN KEY REFERENCES Towns(Id)

-- 4 
INSERT INTO Towns
VALUES
    (1, 'Sofia'),
    (2, 'Plovdiv'),
    (3, 'Varna')

INSERT INTO Minions
VALUES
    (1, 'Kevin', 22, 1),
    (2, 'Bob', 15, 3),
    (3, 'Steward', NULL, 2)

TRUNCATE TABLE [Minions]

DROP TABLE Minions
DROP TABLE Towns

-- 7
CREATE TABLE People
(
    Id BIGINT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(200) NOT NULL,
    Picture VARBINARY(MAX),
    Height FLOAT(2),
    [Weight] FLOAT(2),
    Gender VARCHAR(1) CHECK(Gender in ('f', 'm')) NOT NULL,
    Birthdate DATETIME2,
    Biography VARCHAR(MAX)
)

INSERT INTO People
VALUES
    ('Peter1', null, 1.78, 78.34, 'f', '10-20-2022', null),
    ('Peter2', null, 1.77, 73.24, 'm', '11-20-2022', null),
    ('Peter3', null, 1.76, 74.44, 'f', '12-20-2022', null),
    ('Peter4', null, 1.75, 73.77, 'm', '09-20-2022', null),
    ('Peter5', null, 1.74, 79.05, 'f', '08-20-2022', null)

8
CREATE TABLE Users
(
    Id BIGINT PRIMARY KEY IDENTITY,
    Username NVARCHAR(30) NOT NULL,
    [Password] NVARCHAR(26) NOT NULL,
    ProfilePicture VARBINARY(MAX),
    LastLoginTime DATETIME2,
    IsDelete BIT
);

INSERT INTO Users
VALUES
    ('Peter', '1234567', null, '10-20-2022', 0),
    ('Peter2', '11234567', null, '11-20-2022', 1),
    ('Peter3', '12344567', null, '12-20-2022', 0),
    ('Peter4', '13323456667', null, '09-20-2022', 1),
    ('Peter5', '12334567', null, '08-20-2022', 0);

ALTER TABLE [Users] DROP CONSTRAINT PK__Users__3214EC07703D9D77
ALTER TABLE [Users] ADD CONSTRAINT PK_IdUsername PRIMARY KEY (Id, Username);

ALTER TABLE [Users] ADD CONSTRAINT CHK_PasswordMinLen CHECK(LEN([Password]) >= 5);

ALTER TABLE [Users] ADD CONSTRAINT DF_LastLogingTime DEFAULT GETDATE() FOR [LastLoginTime]

ALTER TABLE [Users] DROP CONSTRAINT PK_IdUsername

ALTER TABLE [Users] ADD CONSTRAINT PK_Id PRIMARY KEY (Id)

ALTER TABLE [Users] ADD CONSTRAINT UC_Username UNIQUE (Username);

15
CREATE DATABASE [Hotel]
USE Hotel

CREATE TABLE Employees
(
    Id INT PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Title NVARCHAR(50),
    Notes NVARCHAR(MAX)
)
INSERT INTO Employees
    (Id, FirstName, LastName)
VALUES
    (1, 'A', 'B'),
    (2, 'A', 'B'),
    (3, 'A', 'B')

CREATE TABLE Customers
(
    AccountNumber INT PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber CHAR(10) NOT NULL,
    EmergencyName NVARCHAR(100) NOT NULL,
    EmergencyNumber CHAR(10) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Customers
    (AccountNumber, FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber)
VALUES
    (1, 'A', 'B', '3', 'C', '9'),
    (2, 'A', 'B', '3', 'C', '9'),
    (3, 'A', 'B', '3', 'C', '9')



CREATE TABLE RoomStatus
(
    RoomStatus NVARCHAR(10) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO RoomStatus
    (RoomStatus)
VALUES
    ('A'),
    ('A'),
    ('A')


CREATE TABLE RoomTypes
(
    RoomType NVARCHAR(10) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO RoomTypes
    (RoomType)
VALUES
    ('A'),
    ('A'),
    ('A')


CREATE TABLE BedTypes
(
    BedType NVARCHAR(10) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO BedTypes
    (BedType)
VALUES
    ('A'),
    ('A'),
    ('A')


CREATE TABLE Rooms
(
    RoomNumber INT PRIMARY KEY,
    RoomType NVARCHAR(10) NOT NULL,
    BedType NVARCHAR(10) NOT NULL,
    Rate TINYINT,
    RoomStatus NVARCHAR(10) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Rooms
    (RoomNumber, RoomType, BedType, Rate, RoomStatus)
VALUES
    (1, 'A', 'B', 4, 'C'),
    (2, 'A', 'B', 4, 'C'),
    (3, 'A', 'B', 4, 'C')


CREATE TABLE Payments
(
    Id INT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    PaymentDate DATETIME2,
    AccountNumber INT,
    FirstDateOccupied DATETIME2,
    LastDateOccupied DATETIME2,
    TotalDays TINYINT,
    AmountCharged DECIMAL(15,2),
    TaxRate DECIMAL(15,2),
    TaxAmount DECIMAL(15,2),
    PaymentTotal DECIMAL(15,2),
    Notes NVARCHAR(MAX)
)

INSERT INTO Payments
    (Id, EmployeeId, TaxRate)
VALUES
    (1, 3, 10.00),
    (2, 4, 20.00),
    (3, 5, 30.00)

UPDATE Payments
SET TaxRate = TaxRate * 0.97
WHERE TaxRate IS NOT NULL

SELECT TaxRate FROM Payments

DROP TABLE Payments

CREATE TABLE Occupancies
(
    Id INT PRIMARY KEY,
    EmployeeId INT,
    DateOccupied DATETIME2,
    AccountNumber INT,
    RoomNumber INT,
    RateApplied INT,
    PhoneCharge DECIMAL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Occupancies
    (Id)
VALUES
    (1),
    (2),
    (3)
TRUNCATE TABLE Occupancies

13
CREATE DATABASE Movies
USE Movies

CREATE TABLE Directors
(
    Id INT PRIMARY KEY,
    DirectorName NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Directors
    (Id, DirectorName)
VALUES
    (1, 'A'),
    (2, 'A'),
    (3, 'A'),
    (4, 'A'),
    (5, 'A')

CREATE TABLE Genres
(
    Id INT PRIMARY KEY,
    GenreName NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Genres
    (Id, GenreName)
VALUES
    (1, 'A'),
    (2, 'A'),
    (3, 'A'),
    (4, 'A'),
    (5, 'A')

CREATE TABLE Categories
(
    Id INT PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Categories
    (Id, CategoryName)
VALUES
    (1, 'A'),
    (2, 'A'),
    (3, 'A'),
    (4, 'A'),
    (5, 'A')


CREATE TABLE Movies
(
    Id INT PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    DirectorId INT NOT NULL,
    CopyrightYear DATETIME2,
    [Length] TINYINT,
    GenreId INT,
    CategoryId INT,
    Rating DECIMAL,
    Notes NVARCHAR(MAX)
)
INSERT INTO Movies
    (Id, Title, DirectorId)
VALUES
    (1, 'A', 4),
    (2, 'A', 5),
    (3, 'A', 6),
    (4, 'A', 7),
    (5, 'A', 8)


14
CREATE DATABASE CarRental
USE CarRental

CREATE TABLE Categories
(
    Id INT PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    DailyRate DECIMAL,
    WeeklyRate DECIMAL,
    MonthlyRate DECIMAL,
    WeekendRate DECIMAL
)
INSERT INTO Categories
    (Id ,CategoryName)
VALUES
    (1, 'A'),
    (2, 'A'),
    (3, 'A')

CREATE TABLE Cars
(
    Id INT PRIMARY KEY,
    PlateNumber CHAR(4) NOT NULL,
    Manufacturer NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    CarYear DATETIME2 NOT NULL,
    CategoryId INT NOT NULL,
    Doors TINYINT,
    Picture VARBINARY(MAX),
    Condition NVARCHAR(100),
    Available NVARCHAR(10)
)
INSERT INTO Cars
    (Id, PlateNumber, Manufacturer, Model, CarYear, CategoryId)
VALUES
    (1, 1234, 'A', 'B', '06-21-2020', 4),
    (2, 1235, 'A', 'B', '06-22-2020', 5),
    (3, 1236, 'A', 'B', '06-23-2020', 6)

CREATE TABLE Employees
(
    Id INT PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Title NVARCHAR(100),
    Notes NVARCHAR(MAX)
)
INSERT INTO Employees
    (Id, FirstName, LastName)
VALUES
    (1, 'A', 'B'),
    (2, 'A', 'B'),
    (3, 'A', 'B')


CREATE TABLE Customers
(
    Id INT PRIMARY KEY,
    DriverLicenceNumber INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(100),
    City NVARCHAR(20),
    ZIPCode INT,
    Notes NVARCHAR(MAX)
)
INSERT INTO Customers
    (Id, DriverLicenceNumber, FullName)
VALUES
    (1, 22, 'B'),
    (2, 33, 'B'),
    (3, 44, 'B')

CREATE TABLE RentalOrders
(
    Id INT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    CustomerId INT NOT NULL,
    CarId INT NOT NULL,
    TankLevel TINYINT,
    KilometrageStart INT,
    KilometrageEnd INT,
    TotalKilometrage INT,
    StartDate DATETIME2,
    EndDate DATETIME2,
    TotalDays TINYINT,
    RateApplied NVARCHAR(50),
    TaxRate DECIMAL,
    OrderStatus NVARCHAR(20),
    Notes NVARCHAR(MAX)
)
INSERT INTO RentalOrders
    (Id, EmployeeId, CustomerId, CarId)
VALUES
    (1, 22, 33, 44),
    (2, 22, 33, 44),
    (3, 22, 33, 44)

16
CREATE DATABASE SoftUni
USE SoftUni

CREATE TABLE Towns
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL
)
CREATE TABLE Addresses
(
    Id INT PRIMARY KEY IDENTITY,
    AddressText NVARCHAR(MAX),
    TownId INT
)
CREATE TABLE Departments
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL
)
CREATE TABLE Employees
(
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    JobTitle NVARCHAR(100) NOT NULL,
    DepartmentId NVARCHAR(100) NOT NULL,
    HireDate DATETIME2,
    Salary DECIMAL(15,2) NOT NULL,
    AddressId INT
)


INSERT INTO Towns
    ([Name])
VALUES
    ('Sofia'),
    ('Plovdiv'),
    ('Varna'),
    ('Burgas')

INSERT INTO Departments
    ([Name])
VALUES
    ('Engineering'),
    ('Sales'),
    ('Marketing'),
    ('Software Development'),
    ('Quality Assurance')

INSERT INTO Employees
    (FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary)
VALUES
    ('Ivan', 'Ivanov', 'Ivanov', '.NET Developer', 'Software Development', CONVERT(date, '01/02/2013', 104), '3500.00'),
    ('Petar', 'Petrov', 'Petrov', 'Senior Engineer', 'Engineering', CONVERT(date, '02/03/2004', 104), '4000.00'),
    ('Maria', 'Petrova', 'Ivanova', 'Intern', 'Quality Assurance', CONVERT(date, '28/08/2016', 104), '525.25'),
    ('Georgi', 'Teziev', 'Ivanov', 'CEO', 'Sales', CONVERT(date, '09/12/2007', 104), '3000.00'),
    ('Peter', 'Pan', 'Pan', 'Intern', 'Marketing', CONVERT(date, '28/08/2016', 104), '599.88')

SELECT [Name] FROM Towns ORDER BY [Name]
SELECT [Name] FROM Departments ORDER BY [Name]
SELECT FirstName, LastName, JobTitle, Salary FROM Employees ORDER BY Salary DESC

UPDATE Employees
SET Salary = Salary * 1.10
WHERE Salary IS NOT NULL
SELECT Salary FROM Employees

