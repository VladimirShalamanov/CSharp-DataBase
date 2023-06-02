-- CREATE DATABASE Airport

-- GO
-- USE Airport
-- GO

-- -- 01

-- CREATE TABLE Passengers
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      FullName VARCHAR(100) UNIQUE NOT NULL,
--      Email VARCHAR(50) UNIQUE NOT NULL
-- )

-- CREATE TABLE Pilots
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      FirstName VARCHAR(30) UNIQUE NOT NULL,
--      LastName VARCHAR(30) UNIQUE NOT NULL,
--      Age TINYINT CHECK(Age BETWEEN 21 AND 62) NOT NULL,
--      Rating FLOAT CHECK(Rating BETWEEN 0.0 AND 10.0)
-- )

-- CREATE TABLE AircraftTypes
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      TypeName VARCHAR(30) UNIQUE NOT NULL
-- )

-- CREATE TABLE Aircraft
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      Manufacturer VARCHAR(25) NOT NULL,
--      Model VARCHAR(30) NOT NULL,
--      [Year] INT NOT NULL,
--      FlightHours INT,
--      Condition CHAR(1) NOT NULL,
--      TypeId INT REFERENCES AircraftTypes(Id) NOT NULL
-- )

-- CREATE TABLE PilotsAircraft
-- (
--      AircraftId INT REFERENCES Aircraft(Id) NOT NULL,
--      PilotId INT REFERENCES Pilots(Id) NOT NULL,
--      PRIMARY KEY(AircraftId, PilotId)
-- )

-- CREATE TABLE Airports
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      AirportName VARCHAR(70) UNIQUE NOT NULL,
--      Country VARCHAR(100) UNIQUE NOT NULL
-- )

-- CREATE TABLE FlightDestinations
-- (
--      Id INT PRIMARY KEY IDENTITY,
--      AirportId INT REFERENCES Airports(Id) NOT NULL,
--      [Start] DATETIME NOT NULL,
--      AircraftId INT REFERENCES Aircraft(Id) NOT NULL,
--      PassengerId INT REFERENCES Passengers(Id) NOT NULL,
--      TicketPrice DECIMAL(18,2) DEFAULT 15 NOT NULL
-- )


-- -- 02

-- INSERT INTO Passengers(FullName, Email)
--      SELECT CONCAT(FirstName, ' ', LastName) AS FullName,
--             CONCAT(FirstName, LastName, '@gmail.com') AS Email
--        FROM Pilots
--       WHERE Id BETWEEN 5 AND 15

-- -- 03

-- UPDATE Aircraft
--    SET Condition = 'A'
--  WHERE (Condition = 'C' OR Condition = 'B') AND 
--        (FlightHours IS NULL OR FlightHours <= 100) AND
--        [Year] >= 2013

-- -- 04

-- DELETE FROM Passengers
--  WHERE LEN(FullName) <= 10

-- 05

SELECT Manufacturer,
       Model,
       FlightHours,
       Condition
  FROM Aircraft
ORDER BY FlightHours DESC

-- 06

    SELECT p.FirstName,
           p.LastName,
           a.Manufacturer,
           a.Model,
           a.FlightHours
      FROM Aircraft AS a
INNER JOIN PilotsAircraft AS pa
        ON a.Id = pa.AircraftId
INNER JOIN Pilots AS p
        ON pa.PilotId = p.Id
     WHERE a.FlightHours <= 304
  ORDER BY FlightHours DESC, FirstName

-- 07

    SELECT TOP(20)
           f.Id AS DestinationId,
           f.[Start],
           p.FullName,
           a.AirportName,
           f.TicketPrice
      FROM FlightDestinations AS f
INNER JOIN Passengers AS p
        ON f.PassengerId = p.Id
INNER JOIN Airports AS a
        ON a.Id = f.AirportId
     WHERE DAY([Start]) % 2 = 0
  ORDER BY TicketPrice DESC, AirportName

-- 08

    SELECT a.Id AS AircraftId,
           a.Manufacturer,
           a.FlightHours,
           COUNT(f.AircraftId) AS FlightDestinationsCount,
           ROUND(AVG(f.TicketPrice), 2) AS AvgPrice
      FROM Aircraft AS a
INNER JOIN FlightDestinations AS f
        ON f.AircraftId = a.Id
  GROUP BY a.Id, a.Manufacturer, a.FlightHours
    HAVING COUNT(AircraftId) >= 2
  ORDER BY FlightDestinationsCount DESC, AircraftId
     
-- 09

    SELECT p.FullName,
           COUNT(fd.AircraftId) AS CountOfAircraft,
           SUM(fd.TicketPrice) AS TotalPayed
      FROM Passengers AS p
INNER JOIN FlightDestinations AS fd ON fd.PassengerId = p.Id
     WHERE SUBSTRING(p.FullName, 2, 1) = 'a'
  GROUP BY p.Id, p.FullName
    HAVING COUNT(fd.AircraftId) > 1
  ORDER BY FullName
       
-- 10

    SELECT ai.AirportName,
           f.[Start] AS DayTime,
           f.TicketPrice,
           p.FullName,
           a.Manufacturer,
           a.Model
      FROM FlightDestinations AS f
INNER JOIN Airports AS ai ON ai.Id = f.AirportId
INNER JOIN Passengers AS p ON p.Id = f.PassengerId
INNER JOIN Aircraft AS a ON a.Id = f.AircraftId
     WHERE (DATEPART(HOUR, f.[Start]) BETWEEN 6 AND 20) AND
           f.TicketPrice > 2500
  ORDER BY Model

-- 11
GO
CREATE OR ALTER FUNCTION  udf_FlightDestinationsByEmail(@email VARCHAR(50))
RETURNS INT
AS 
BEGIN
        DECLARE @pId INT;
        DECLARE @countDest INT;

        SET @pId = (
                      SELECT Id
                        FROM Passengers
                       WHERE Email = @email
                   );

        SET @countDest = (
                            SELECT COUNT(AircraftId)
                              FROM FlightDestinations
                             WHERE PassengerId = @pId
                         );
        RETURN @countDest;
END
GO
SELECT dbo.udf_FlightDestinationsByEmail('Montacute@gmail.com')
SELECT dbo.udf_FlightDestinationsByEmail ('PierretteDunmuir@gmail.com')
GO

-- 12

CREATE PROCEDURE usp_SearchByAirportName(@airportName VARCHAR(70))
AS
BEGIN
        SELECT a.AirportName,
               p.FullName,
               CASE WHEN f.TicketPrice < 400 THEN 'Low'
                    WHEN f.TicketPrice BETWEEN 401 AND 1500 THEN 'Medium'
                    WHEN f.TicketPrice > 1501 THEN 'High'
               END AS LevelOfTickerPrice,
               ac.Manufacturer,
               ac.Condition,
               [at].TypeName
          FROM Airports AS a
    INNER JOIN FlightDestinations AS f ON f.AirportId = a.Id
    INNER JOIN Passengers AS p ON p.Id = f.PassengerId
    INNER JOIN Aircraft AS ac ON ac.Id = f.AircraftId
    INNER JOIN AircraftTypes AS [at] ON [at].Id = ac.TypeId
         WHERE AirportName =  @airportName
      ORDER BY Manufacturer, FullName
END

GO

EXEC usp_SearchByAirportName 'Sir Seretse Khama International Airport'