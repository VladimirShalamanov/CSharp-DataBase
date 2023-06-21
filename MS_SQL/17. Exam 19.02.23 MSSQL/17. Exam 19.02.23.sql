CREATE DATABASE Boardgames
GO
USE master
GO

CREATE TABLE Categories
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL
)

CREATE TABLE Addresses
(
    Id INT PRIMARY KEY IDENTITY,
    StreetName NVARCHAR(100) NOT NULL,
    StreetNumber INT NOT NULL,
    Town VARCHAR(30) NOT NULL,
    Country VARCHAR(50) NOT NULL,
    ZIP INT NOT NULL
)

CREATE TABLE Publishers
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(30) UNIQUE NOT NULL,
    AddressId INT FOREIGN KEY REFERENCES Addresses(Id) NOT NULL,
    Website NVARCHAR(40),
    Phone NVARCHAR(20)
)

CREATE TABLE PlayersRanges
(
    Id INT PRIMARY KEY IDENTITY,
    PlayersMin INT NOT NULL,
    PlayersMax INT NOT NULL
)

CREATE TABLE Boardgames
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(30) NOT NULL,
    YearPublished INT NOT NULL,
    Rating DECIMAL(15,2) NOT NULL,
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
    PublisherId INT FOREIGN KEY REFERENCES Publishers(Id) NOT NULL,
    PlayersRangeId INT FOREIGN KEY REFERENCES PlayersRanges(Id) NOT NULL
)

CREATE TABLE Creators
(
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(30) NOT NULL,
    LastName NVARCHAR(30) NOT NULL,
    Email NVARCHAR(30) NOT NULL
)

CREATE TABLE CreatorsBoardgames
(
    CreatorId INT FOREIGN KEY REFERENCES Creators(Id) NOT NULL,
    BoardgameId INT FOREIGN KEY REFERENCES Boardgames(Id) NOT NULL,
    PRIMARY KEY(CreatorId, BoardgameId)
)

-- 02

INSERT INTO Publishers
    ([Name],AddressId,Website,Phone)
    VALUES
        ('Agman Games',5,'www.agmangames.com','+16546135542'),
        ('Amethyst Games',7,'www.amethystgames.com','+15558889992'),
        ('BattleBooks',13,'www.battlebooks.com','+12345678907')

INSERT INTO Boardgames
    ([Name],YearPublished,Rating,CategoryId,PublisherId,PlayersRangeId)
    VALUES
        ('Deep Blue','2019',5.67,1,15,7),
        ('Paris','2016',9.78,7,1,5),
        ('Catan: Starfarers','2021',9.87,7,13,6),
        ('Bleeding Kansas','2020',3.25,3,7,4),
        ('One Small Step','2019',5.75,5,9,2)

-- 03

UPDATE PlayersRanges
   SET PlayersMax += 1
 WHERE PlayersMin = 2 AND PlayersMax = 2

UPDATE Boardgames
   SET [Name] = CONCAT([Name], 'V2')
 WHERE YearPublished >= 2020

-- 04


DELETE FROM Creators WHERE Id IN ( 
    SELECT CreatorId FROM CreatorsBoardgames WHERE BoardgameId IN (
                        SELECT Id FROM Boardgames WHERE PublisherId = 1))
------------------------------------------------------------------


DELETE FROM CreatorsBoardgames WHERE BoardgameId IN (
                SELECT Id FROM Boardgames WHERE PublisherId = 1)

DELETE FROM Boardgames
WHERE PublisherId = 1

DELETE FROM Publishers
WHERE AddressId = 5

DELETE FROM Addresses
WHERE Id = 5

-- 05

SELECT [Name], Rating
FROM Boardgames
ORDER BY YearPublished, [Name] DESC

-- 06

SELECT B.Id, B.Name, B.YearPublished, C.Name AS CategoryName
FROM Boardgames AS B
LEFT JOIN  Categories AS C ON B.CategoryId = C.Id
WHERE C.Name IN ('Strategy Games', 'Wargames')
ORDER BY B.YearPublished DESC

-- 07

SELECT C.Id,
       CONCAT(C.FirstName, ' ', C.LastName) AS CreatorName,
       C.Email
FROM Creators AS C
LEFT JOIN  CreatorsBoardgames AS CB ON CB.CreatorId = C.Id
WHERE CB.BoardgameId IS NULL
ORDER BY CreatorName

-- 08

SELECT TOP(5)
       B.Name,
       B.Rating,
       C.Name AS CategoryName
FROM Boardgames AS B
LEFT JOIN Categories AS C ON B.CategoryId = C.Id
LEFT JOIN PlayersRanges AS PR ON B.PlayersRangeId = PR.Id
WHERE (B.Rating > 7.00 AND (B.Name LIKE 'a%' OR B.Name LIKE '%a%' OR B.Name LIKE '%a')) OR
      (B.Rating > 7.50 AND (PR.PlayersMin = 2 AND PR.PlayersMax = 5))
ORDER BY B.Name, B.Rating DESC

-- 09

SELECT Sq.FullName,
       Sq.Email,
       Sq.Rating
  FROM (
                SELECT CONCAT(C.FirstName, ' ', C.LastName) AS FullName,
                       C.Email,
                       B.Rating,
                DENSE_RANK() OVER(PARTITION BY C.FirstName ORDER BY B.Rating DESC) AS RankRg
                FROM Boardgames AS B
                LEFT JOIN CreatorsBoardgames AS CB ON B.Id = CB.BoardgameId
                LEFT JOIN Creators AS C ON CB.CreatorId = C.Id
                WHERE C.Email LIKE '%.com' ) AS Sq
  WHERE Sq.RankRg = 1
ORDER BY FullName

-- 10

SELECT C.LastName,
        CEILING(AVG(B.Rating)) AS AverageRating,
        P.Name AS PublisherName
FROM Creators AS C
LEFT JOIN CreatorsBoardgames AS CB ON CB.CreatorId = C.Id
LEFT JOIN Boardgames AS B ON B.Id = CB.BoardgameId
LEFT JOIN Publishers AS P ON B.PublisherId = P.Id
WHERE CB.BoardgameId IS NOT NULL AND P.Name = 'Stonemaier Games'
GROUP BY C.LastName, P.Name
ORDER BY AVG(B.Rating) DESC

-- 11

GO

CREATE FUNCTION udf_CreatorWithBoardgames(@name NVARCHAR(30))
RETURNS INT
AS
BEGIN
        RETURN (
            SELECT COUNT(CB.BoardgameId) 
            FROM Creators AS C
            LEFT JOIN CreatorsBoardgames AS CB ON C.Id = CB.CreatorId
            WHERE C.FirstName = @name
        )
END

GO

SELECT dbo.udf_CreatorWithBoardgames('Bruno')

-- 12
GO

CREATE PROCEDURE usp_SearchByCategory(@category VARCHAR(50))
AS
BEGIN
        SELECT B.Name,
               B.YearPublished,
               B.Rating,
               C.Name AS CategoryName,
               P.Name AS PublisherName,
               CONCAT(PR.PlayersMin, ' people') AS MinPlayers,
               CONCAT(PR.PlayersMax , ' people') AS MaxPlayers
          FROM Categories AS C
           JOIN Boardgames AS B ON B.CategoryId = C.Id
           JOIN PlayersRanges AS PR ON B.PlayersRangeId = PR.Id
           JOIN Publishers AS P ON B.PublisherId = P.Id
           WHERE C.Name = @category
           ORDER BY PublisherName, YearPublished DESC
END

GO

EXEC usp_SearchByCategory 'Wargames'