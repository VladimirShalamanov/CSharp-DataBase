CREATE DATABASE WMS
GO
USE WMS
GO

CREATE TABLE Clients
(
    ClientId INT PRIMARY KEY IDENTITY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Phone VARCHAR(12) CHECK(LEN(Phone) = 12) NOT NULL
)

CREATE TABLE Mechanics
(
    MechanicId INT PRIMARY KEY IDENTITY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    [Address] VARCHAR(255) NOT NULL
)

CREATE TABLE Models
(
    ModelId INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Jobs  
(
    JobId INT PRIMARY KEY IDENTITY,
    ModelId INT FOREIGN KEY REFERENCES Models(ModelId) NOT NULL,
    [Status] VARCHAR(11) DEFAULT('Pending')
            CHECK([Status] IN ('Pending','In Progress','Finished')) NOT NULL,
    ClientId INT FOREIGN KEY REFERENCES Clients(ClientId) NOT NULL,
    MechanicId INT FOREIGN KEY REFERENCES Mechanics(MechanicId),
    IssueDate DATE NOT NULL,
    FinishDate DATE
)

CREATE TABLE Orders
(
    OrderId INT PRIMARY KEY IDENTITY,
    JobId INT FOREIGN KEY REFERENCES Jobs(JobId),
    IssueDate DATE,
    Delivered BIT DEFAULT(0) NOT NULL
)

CREATE TABLE Vendors
(
    VendorId INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) UNIQUE
)

CREATE TABLE Parts
(
    PartId INT PRIMARY KEY IDENTITY,
    SerialNumber VARCHAR(50) UNIQUE,
    [Description] VARCHAR(255),
    Price MONEY CHECK(Price > 0 AND Price <= 999.99) NOT NULL,
    VendorId INT FOREIGN KEY REFERENCES Vendors(VendorId),
    StockQty INT CHECK(StockQty >= 0) DEFAULT 0
)

CREATE TABLE OrderParts
(
    OrderId INT REFERENCES Orders(OrderId) NOT NULL,
    PartId INT REFERENCES Parts(PartId) NOT NULL,
    PRIMARY KEY (OrderId,PartId),
    Quantity INT CHECK(Quantity > 0) DEFAULT(1)
)

CREATE TABLE PartsNeeded
(
    JobId INT REFERENCES Jobs(JobId) NOT NULL,
    PartId INT REFERENCES Parts(PartId) NOT NULL,
    PRIMARY KEY (JobId,PartId),
    Quantity INT CHECK(Quantity > 0) DEFAULT(1)
)

-- 02

INSERT INTO Clients
(FirstName,LastName,Phone)
VALUES
            ('Teri','Ennaco','570-889-5187'),
            ('Merlyn','Lawler','201-588-7810'),
            ('Georgene','Montezuma','925-615-5185'),
            ('Jettie','Mconnell','908-802-3564'),
            ('Lemuel','Latzke','631-748-6479'),
            ('Melodie','Knipp','805-690-1682'),
            ('Candida','Corbley','908-275-8357')

INSERT INTO Parts
(SerialNumber,Description,Price,VendorId)
VALUES
        ('WP8182119','Door Boot Seal',117.86,2),
        ('W10780048','Suspension Rod',42.81,1),
        ('W10841140','Silicone Adhesive',6.77,4),
        ('WPY055980','High Temperature Adhesive',13.94,3)

-- 03

UPDATE Jobs
   SET [Status] = 'In Progress', MechanicId = 3
 WHERE [Status] = 'Pending' AND MechanicId IS NULL

-- 04

DELETE FROM OrderParts WHERE OrderId = 19

DELETE FROM Orders WHERE OrderId = 19

-- 05

SELECT CONCAT(M.FirstName, ' ', M.LastName) AS Mechanic,
       J.[Status],
       J.IssueDate
FROM Mechanics AS M
LEFT JOIN Jobs AS J ON J.MechanicId = M.MechanicId
ORDER BY M.MechanicId, J.IssueDate, J.JobId

-- 06

SELECT CONCAT(C.FirstName, ' ', C.LastName) AS Client,
       DATEDIFF(DAY, J.IssueDate, '2017-04-24') AS [Days going],
       J.[Status]
FROM Clients AS C
LEFT JOIN Jobs AS J ON J.ClientId = C.ClientId
WHERE J.[Status] <> 'Finished'
ORDER BY [Days going] DESC, C.ClientId

-- 07

SELECT CONCAT(M.FirstName, ' ', M.LastName) AS Mechanic,
       AVG(DATEDIFF(DAY, J.IssueDate, J.FinishDate)) AS [Average Days]
FROM Mechanics AS M
LEFT JOIN Jobs AS J ON J.MechanicId = M.MechanicId
WHERE J.[Status] <> 'In Progress'
GROUP BY M.MechanicId, M.FirstName, M.LastName
ORDER BY M.MechanicId

-- 08

SELECT CONCAT(M.FirstName, ' ', M.LastName) AS Available
FROM Mechanics AS M
LEFT JOIN Jobs AS J ON J.MechanicId = M.MechanicId
WHERE J.MechanicId NOT IN (SELECT MechanicId FROM Jobs WHERE [Status] = 'In Progress')
GROUP BY M.MechanicId,M.FirstName,M.LastName
ORDER BY M.MechanicId

-- 09

SELECT J.JobId,
       ISNULL(SUM(P.Price * OP.Quantity), 0) AS Total
FROM Jobs AS J
LEFT JOIN Orders AS O ON J.JobId = O.JobId
LEFT JOIN OrderParts AS OP ON OP.OrderId = O.OrderId
LEFT JOIN Parts AS P ON P.PartId = OP.PartId
WHERE J.[Status] = 'Finished'
GROUP BY J.JobId
ORDER BY Total DESC, J.JobId 

-- 12 
GO

CREATE FUNCTION udf_GetCost(@JobId INT)
RETURNS DECIMAL(15,2)
AS
BEGIN
        DECLARE @TotalSum DECIMAL(15,2);

        SET @TotalSum = ( SELECT SUM(P.Price * OP.Quantity) AS Total
                            FROM Jobs AS J
                       LEFT JOIN Orders AS O ON J.JobId = O.JobId
                       LEFT JOIN OrderParts AS OP ON OP.OrderId = O.OrderId
                       LEFT JOIN Parts AS P ON P.PartId = OP.PartId
                           WHERE J.JobId = @JobId
                        GROUP BY J.JobId )

        IF (@TotalSum IS NULL)
            SET @TotalSum = 0;

        RETURN @TotalSum;
            
END