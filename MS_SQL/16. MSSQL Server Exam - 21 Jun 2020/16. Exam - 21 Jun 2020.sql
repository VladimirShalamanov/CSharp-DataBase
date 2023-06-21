CREATE DATABASE TripService
GO
USE TripService
GO
-- USE master
-- DROP DATABASE TripService
CREATE TABLE Cities
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(20) NOT NULL,
    CountryCode VARCHAR(2) NOT NULL
)

CREATE TABLE Hotels
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(30) NOT NULL,
    CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
    EmployeeCount INT NOT NULL,
    BaseRate DECIMAL(18,2)
)

CREATE TABLE Rooms
(
    Id INT PRIMARY KEY IDENTITY,
    Price DECIMAL(18,2) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    Beds INT NOT NULL,
    HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
)

CREATE TABLE Trips
(
    Id INT PRIMARY KEY IDENTITY,
    RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
    BookDate DATE NOT NULL,
    ArrivalDate DATE NOT NULL,
                CHECK(BookDate < ArrivalDate),
    ReturnDate DATE NOT NULL,
                CHECK(ArrivalDate < ReturnDate),
    CancelDate DATE
)

CREATE TABLE Accounts
(
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(20),
    LastName NVARCHAR(50) NOT NULL,
    CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
    BirthDate DATE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL 
)

CREATE TABLE AccountsTrips
(
    AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
    TripId INT FOREIGN KEY REFERENCES Trips(Id) NOT NULL,
    Luggage INT CHECK(Luggage >= 0) NOT NULL,
    PRIMARY KEY (AccountId, TripId)
)

-- 02

INSERT INTO Accounts
    (FirstName,	MiddleName,	LastName,	CityId,	BirthDate,	Email)
    VALUES
    ('John',	'Smith',	'Smith',	34,	'1975-07-21',	'j_smith@gmail.com'),
    ('Gosho',	NULL,	'Petrov',	11,	'1978-05-16',	'g_petrov@gmail.com'),
    ('Ivan',	'Petrovich',	'Pavlov',	59,	'1849-09-26',	'i_pavlov@softuni.bg'),
    ('Friedrich',	'Wilhelm',	'Nietzsche',	2,	'1844-10-15',	'f_nietzsche@softuni.bg')

INSERT INTO Trips
    (RoomId,	BookDate,	ArrivalDate,	ReturnDate,	CancelDate)
VALUES
        (101,	'2015-04-12',	'2015-04-14',	'2015-04-20',	'2015-02-02'),
        (102,	'2015-07-07',	'2015-07-15',	'2015-07-22',	'2015-04-29'),
        (103,	'2013-07-17',	'2013-07-23',	'2013-07-24',	NULL),
        (104,	'2012-03-17',	'2012-03-31',	'2012-04-01',	'2012-01-10'),
        (109,	'2017-08-07',	'2017-08-28',	'2017-08-29',	NULL)

-- 03

UPDATE Rooms
   SET Price *= 1.14
 WHERE HotelId IN (5,7,9)

-- 04

DELETE FROM AccountsTrips WHERE AccountId = 47
DELETE FROM Accounts WHERE Id = 47

-- 05

SELECT A.FirstName,
       A.LastName,
       FORMAT(A.BirthDate, 'MM-dd-yyyy') AS BirthDate,
       C.Name AS HomeTown,
       A.Email
  FROM Accounts AS A
LEFT JOIN Cities AS C ON A.CityId = C.Id
    WHERE LEFT(Email, 1) = 'e'
 ORDER BY HomeTown 

-- 06

   SELECT C.Name AS City,
          COUNT(H.Id) AS Hotels
     FROM Cities AS C
INNER JOIN Hotels AS H ON H.CityId = C.Id
 GROUP BY C.Name
 ORDER BY Hotels DESC, City

-- 07

    SELECT A.Id AS AccountId,
           CONCAT(A.FirstName, ' ', A.LastName) AS FullName,
           MAX(DATEDIFF(DAY, T.ArrivalDate, T.ReturnDate)) AS LongestTrip,
           MIN(DATEDIFF(DAY, T.ArrivalDate, T.ReturnDate)) AS ShortestTrip
      FROM Accounts AS A
INNER JOIN AccountsTrips AS AR ON AR.AccountId = A.Id
INNER JOIN Trips AS T ON AR.TripId = T.Id
     WHERE A.MiddleName IS NULL AND T.CancelDate IS NULL
  GROUP BY A.Id, CONCAT(A.FirstName, ' ', A.LastName)
  ORDER BY LongestTrip DESC, ShortestTrip
  
-- 08

    SELECT TOP(10)
           C.Id,
           C.Name AS City,
           C.CountryCode AS Country,
           COUNT(A.Id) AS Accounts
      FROM Cities AS C
INNER JOIN Accounts AS A ON A.CityId = C.Id
  GROUP BY C.Id, C.Name, C.CountryCode
  ORDER BY Accounts DESC

-- 09

    SELECT A.Id,
           A.Email,
           C.Name AS City,
           COUNT(AR.TripId) AS Trips
      FROM Accounts AS A
INNER JOIN AccountsTrips AS AR ON A.Id = AR.AccountId
INNER JOIN Trips AS T ON T.Id = AR.TripId
INNER JOIN Rooms AS R ON R.Id = T.RoomId
INNER JOIN Hotels AS H ON H.Id = R.HotelId
INNER JOIN Cities AS C ON C.Id = H.CityId
     WHERE A.CityId = H.CityId
  GROUP BY A.Id,A.Email,C.Name
  ORDER BY Trips DESC, A.Id

-- 10

    SELECT T.Id,
           CONCAT(A.FirstName,' ',ISNULL(A.MiddleName + ' ', ''),A.LastName) AS [Full Name],
           C.Name AS [From],
           C2.Name AS [To],
           IIF(T.CancelDate IS NULL, 
                CONCAT(DATEDIFF(DAY, T.ArrivalDate, T.ReturnDate), ' days'),
                'Canceled') AS Duration
      FROM AccountsTrips AS AR
LEFT JOIN Accounts AS A ON A.Id = AR.AccountId
LEFT JOIN Cities AS C ON C.Id = A.CityId
LEFT JOIN Trips AS T ON T.Id = AR.TripId
LEFT JOIN Rooms AS R ON R.Id = T.RoomId
LEFT JOIN Hotels AS H ON H.Id = R.HotelId
LEFT JOIN Cities AS C2 ON C2.Id = H.CityId

 ORDER BY [Full Name], T.Id

-- 11
GO


CREATE OR ALTER FUNCTION udf_GetAvailableRoom (@hotelId INT, @date DATE, @people INT)
RETURNS NVARCHAR(400)
AS
BEGIN
	
	DECLARE @roomId INT = (SELECT TOP 1 r.[Id] FROM Rooms r JOIN [Hotels] h ON h.[Id] = r.[HotelId] WHERE h.[Id] = @hotelId ORDER BY r.[Price] DESC)

	DECLARE @roomPrice DECIMAL(15, 2) = (SELECT [Price] FROM [Rooms] WHERE [Id] = @roomId)
	
	DECLARE @hotelBaseRate DECIMAL(15, 2) = (SELECT [BaseRate] FROM [Hotels] WHERE [Id] = @hotelId)

	DECLARE @totalPrice DECIMAL (15, 2) = (@hotelBaseRate + @roomPrice) * @people

	DECLARE @tripArrivalDate DATE = 
								(SELECT TOP 1 t.[ArrivalDate]	
								FROM [Rooms] r 
								JOIN [Hotels] h ON h.Id = r.[HotelId]
								JOIN [Trips] t ON t.[RoomId] = r.[Id] 
								WHERE h.[Id] = @hotelId AND t.[CancelDate] IS NULL AND t.[RoomId] = @roomId)

	DECLARE @tripReturnDate DATE = 
								(SELECT TOP 1 t.[ReturnDate] 
								FROM [Rooms] r 
								JOIN [Hotels] h ON h.[Id] = r.[HotelId] 
								JOIN [Trips] t ON t.[RoomId] = r.[Id]
								WHERE h.[Id] = @hotelId AND t.[CancelDate] IS NULL AND t.[RoomId] = @roomId)

	DECLARE @result NVARCHAR(400) = 'No rooms available'

	IF (@date BETWEEN @tripArrivalDate AND @tripReturnDate)
		RETURN @result

	IF NOT EXISTS 
	(
		SELECT r.[Id] 
		FROM [Hotels] h 
		JOIN [Rooms] r ON r.[HotelId] = h.[Id] 
		WHERE h.[Id] = @hotelId AND @roomId = ANY 
											  (
											    SELECT r.[Id] 
											    FROM [Hotels] h JOIN [Rooms] r ON r.[HotelId] = h.[Id] 
											    WHERE h.[Id] = @hotelId AND r.[Id] = @roomId
											  )
	)
		RETURN @result

	DECLARE @roomBeds INT = (SELECT [Beds] FROM [Rooms] WHERE [Id] = @roomId)

	IF (@people > @roomBeds)
		RETURN @result

	DECLARE @roomType NVARCHAR(100) = (SELECT [Type] FROM [Rooms] WHERE [Id] = @roomId)

	SET @result = 
		'Room ' + CAST(@roomId AS NVARCHAR(50)) + ': ' + CAST(@roomType AS NVARCHAR(50)) + ' (' + CAST(@roomBeds AS NVARCHAR(50)) + ' beds) - $' + CAST(@totalPrice AS NVARCHAR(50))
			RETURN @result
END

GO

SELECT dbo.udf_GetAvailableRoom(112, '2011-12-17', 2)

-- 12
GO

CREATE PROCEDURE usp_SwitchRoom(@TripId INT, @TargetRoomId INT)
AS
BEGIN
        DECLARE @HotelTrip INT = ( SELECT R.HotelId FROM Trips AS T JOIN Rooms AS R ON R.Id = T.RoomId WHERE T.Id = @TripId )

        DECLARE @HotelRoom INT = ( SELECT HotelId FROM Rooms WHERE Id = @TargetRoomId )

        IF (@HotelTrip <> @HotelRoom)
            THROW 50001, 'Target room is in another hotel!', 1

        DECLARE @NeededBeds INT = ( SELECT COUNT(TripId) FROM AccountsTrips WHERE TripId = @TripId )

        DECLARE @Beds INT = ( SELECT Beds FROM Rooms WHERE Id = @TargetRoomId )

        IF (@NeededBeds > @Beds)
            THROW 50002, 'Not enough beds in target room!', 1

        UPDATE Trips
           SET RoomId = @TargetRoomId
         WHERE Id = @TripId
END

GO

EXEC usp_SwitchRoom 10, 11
SELECT RoomId FROM Trips WHERE Id = 10
