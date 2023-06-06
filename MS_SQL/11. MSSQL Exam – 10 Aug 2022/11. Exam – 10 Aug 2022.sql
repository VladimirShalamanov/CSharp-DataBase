-- CREATE DATABASE NationalTouristSitesOfBulgaria
-- GO
-- USE NationalTouristSitesOfBulgaria
-- GO

 CREATE TABLE Categories
 (
     Id INT PRIMARY KEY IDENTITY,
     [Name] VARCHAR(50) NOT NULL
 )

 CREATE TABLE Locations
 (
     Id INT PRIMARY KEY IDENTITY,
     [Name] VARCHAR(50) NOT NULL,
     Municipality VARCHAR(50),
     Province VARCHAR(50)
 )

 CREATE TABLE Sites
 (
     Id INT PRIMARY KEY IDENTITY,
     [Name] VARCHAR(100) NOT NULL,
     LocationId INT REFERENCES Locations(Id) NOT NULL,
     CategoryId INT REFERENCES Categories(Id) NOT NULL,
     Establishment VARCHAR(15)
 )

 CREATE TABLE Tourists
 (
     Id INT PRIMARY KEY IDENTITY,
     [Name] VARCHAR(50) NOT NULL,
     Age INT CHECK(Age BETWEEN 0 AND 120) NOT NULL,
     PhoneNumber VARCHAR(20) NOT NULL,
     Nationality VARCHAR(30) NOT NULL,
     Reward VARCHAR(20)
 )

 CREATE TABLE SitesTourists
 (
     TouristId INT REFERENCES Tourists(Id) NOT NULL,
     SiteId INT REFERENCES Sites(Id) NOT NULL,
     PRIMARY KEY(TouristId, SiteId)
 )

 CREATE TABLE BonusPrizes
 (
     Id INT PRIMARY KEY IDENTITY,
     [Name] VARCHAR(50) NOT NULL
 )

 CREATE TABLE TouristsBonusPrizes
 (
     TouristId INT REFERENCES Tourists(Id) NOT NULL,
     BonusPrizeId INT REFERENCES BonusPrizes(Id) NOT NULL,
     PRIMARY KEY(TouristId, BonusPrizeId)
 )

-- -- 02

INSERT INTO Tourists(Name, Age, PhoneNumber, Nationality, Reward)
     VALUES
            ('Borislava Kazakova', 52, '+359896354244', 'Bulgaria', NULL),
            ('Peter Bosh', 48, '+447911844141', 'UK', NULL),
            ('Martin Smith', 29, '+353863818592', 'Ireland', 'Bronze badge'),
            ('Svilen Dobrev', 49, '+359986584786', 'Bulgaria', 'Silver badge'),
            ('Kremena Popova', 38, '+359893298604', 'Bulgaria', NULL)

INSERT INTO Sites(Name, LocationId, CategoryId, Establishment)
     VALUES
                ('Ustra fortress', 90, 7, 'X'),
                ('Karlanovo Pyramids', 65, 7, NULL),
                ('The Tomb of Tsar Sevt', 63, 8, 'V BC'),
                ('Sinite Kamani Natural Park', 17, 1, NULL),
                ('St. Petka of Bulgaria – Rupite', 92, 6, '1994')

-- 03

UPDATE Sites
   SET Establishment = '(not defined)'
 WHERE Establishment IS NULL

 -- 04

DELETE FROM TouristsBonusPrizes
 WHERE BonusPrizeId = 5

DELETE FROM BonusPrizes
 WHERE Id = 5

-- 05

SELECT [Name], Age, PhoneNumber, Nationality
  FROM Tourists
ORDER BY Nationality, Age DESC, [Name]

-- 06

   SELECT s.Name AS [Site],
          l.Name AS [Location],
          s.Establishment,
          c.Name AS [Category]
     FROM Sites AS s
LEFT JOIN Locations AS l ON l.Id = s.LocationId
LEFT JOIN Categories AS c ON c.Id = s.CategoryId
 ORDER BY c.Name DESC, l.Name, s.Name
 
-- 07

   SELECT l.Province,
          l.Municipality,
          l.Name AS [Location],
          COUNT(s.Id) AS CountOfSites
     FROM Locations AS l
     JOIN Sites AS s ON l.Id = s.LocationId
    WHERE l.Province = 'Sofia'
 GROUP BY l.Id, l.Province, l.Municipality, l.Name
 ORDER BY CountOfSites DESC, [Location]
  
-- 08

   SELECT s.Name AS [Site],
          l.Name AS [Location],
          l.Municipality,
          l.Province,
          s.Establishment
     FROM Locations AS l
LEFT JOIN Sites AS s ON l.Id = s.LocationId
    WHERE LEFT(l.Name, 1) NOT IN ('B', 'M', 'D') AND s.Establishment LIKE '%BC'
 ORDER BY [Site]
   
-- 09

   SELECT t.Name,
          t.Age,
          t.PhoneNumber,
          t.Nationality,
          ISNULL(b.Name, '(no bonus prize)') AS [Reward]
     FROM Tourists AS t
LEFT JOIN TouristsBonusPrizes AS tb ON tb.TouristId = t.Id
LEFT JOIN BonusPrizes AS b ON b.Id = tb.BonusPrizeId
 ORDER BY t.Name

-- 10

   SELECT SUBSTRING(t.Name, CHARINDEX(' ', t.Name), LEN(t.Name)) AS LastName,
          t.Nationality,
          t.Age,
          t.PhoneNumber
     FROM Tourists AS t
LEFT JOIN SitesTourists AS st ON st.TouristId = t.Id
LEFT JOIN Sites AS s ON s.Id = st.SiteId
    WHERE s.CategoryId = (
                              SELECT Id FROM Categories
                              WHERE [Name] = 'History and archaeology'
                         )
 GROUP BY t.Name,
          t.Nationality,
          t.Age,
          t.PhoneNumber
 ORDER BY LastName

-- 11
GO
CREATE FUNCTION udf_GetTouristsCountOnATouristSite (@Site VARCHAR(100))
RETURNS INT
AS
BEGIN
          RETURN (
                    SELECT COUNT(TouristId)
                      FROM SitesTourists
                     WHERE SiteId = (
                                        SELECT Id FROM Sites
                                         WHERE [Name] = @Site
                                    )
                 )
END
GO

SELECT dbo.udf_GetTouristsCountOnATouristSite ('Regional History Museum – Vratsa')
GO

-- 12

CREATE OR ALTER PROCEDURE usp_AnnualRewardLottery(@TouristName VARCHAR(50))
AS
BEGIN
		DECLARE @countOfVisitedSites INT;
		SET @countOfVisitedSites = ( SELECT COUNT(s.Id)
									   FROM Tourists AS t
								  LEFT JOIN SitesTourists AS st ON st.TouristId = t.Id
								  LEFT JOIN Sites AS s ON s.Id = st.SiteId
									  WHERE t.Name = @TouristName )

	UPDATE Tourists
			SET	Reward = 
			CASE
				WHEN @countOfVisitedSites >= 100 THEN 'Gold badge'
				WHEN @countOfVisitedSites >= 50 THEN 'Silver badge'
				WHEN @countOfVisitedSites >= 25 THEN 'Bronze badge'
			 END
	 WHERE Name = @TouristName

                    
          SELECT Name, Reward FROM Tourists
		   WHERE Name = @TouristName

END
GO

EXEC usp_AnnualRewardLottery 'Gerhild Lutgard'