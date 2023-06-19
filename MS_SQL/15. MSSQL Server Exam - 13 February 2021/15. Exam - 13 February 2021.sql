CREATE DATABASE Bitbucket
GO
USE Bitbucket
GO

CREATE TABLE Users
(
    Id INT PRIMARY KEY IDENTITY,
    Username VARCHAR(30) NOT NULL,
    [Password] VARCHAR(30) NOT NULL,
    Email VARCHAR(50) NOT NULL
)

CREATE TABLE Repositories
(
    Id INT PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL
)

CREATE TABLE RepositoriesContributors
(
    RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
    ContributorId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL,
    PRIMARY KEY(RepositoryId, ContributorId)
)

CREATE TABLE Issues
(
    Id INT PRIMARY KEY IDENTITY,
    Title VARCHAR(255) NOT NULL,
    IssueStatus VARCHAR(6) NOT NULL,
    RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
    AssigneeId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
)

CREATE TABLE Commits
(
    Id INT PRIMARY KEY IDENTITY,
    [Message] VARCHAR(255) NOT NULL,
    IssueId INT FOREIGN KEY REFERENCES Issues(Id),
    RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
    ContributorId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
)

CREATE TABLE Files
(
    Id INT PRIMARY KEY IDENTITY,
    [Name]  VARCHAR(100) NOT NULL,
    Size DECIMAL(18,2) NOT NULL,
    ParentId INT FOREIGN KEY REFERENCES Files(Id),
    CommitId INT FOREIGN KEY REFERENCES Commits(Id) NOT NULL
)

-- 02

INSERT INTO Files
            ([Name],Size,ParentId,CommitId)
    VALUES
            ('Trade.idk',2598.0,1,1),
            ('menu.net',9238.31,2,2),
            ('Administrate.soshy',1246.93,3,3),
            ('Controller.php',7353.15,4,4),
            ('Find.java',9957.86,5,5),
            ('Controller.json',14034.87,3,6),
            ('Operate.xix',7662.92,7,7)

INSERT INTO Issues
             (Title,IssueStatus,RepositoryId,AssigneeId)
        VALUES
            ('Critical Problem with HomeController.cs file','open',1,4),
            ('Typo fix in Judge.html','open',4,3),
            ('Implement documentation for UsersService.cs','closed',8,2),
            ('Unreachable code in Index.cs','open',9,8)

-- 03

UPDATE Issues
   SET IssueStatus = 'closed'
 WHERE AssigneeId = 6

-- 04

DELETE FROM RepositoriesContributors WHERE RepositoryId = 3
DELETE FROM Issues WHERE RepositoryId = 3
DELETE FROM Files WHERE CommitId = 36
DELETE FROM Commits WHERE RepositoryId = 3
DELETE FROM Repositories WHERE [Name] = 'Softuni-Teamwork'

DELETE FROM RepositoriesContributors
WHERE RepositoryId = 3
 
DELETE FROM Issues
WHERE RepositoryId = 3

-- 05

SELECT Id, [Message], RepositoryId, ContributorId
FROM Commits
ORDER BY Id, [Message], RepositoryId, ContributorId

-- 06

SELECT Id,[Name],[Size]
FROM Files
WHERE [Size] > 1000 AND [Name] LIKE '%html'
ORDER BY [Size] DESC, Id, [Name]

-- 07

SELECT I.Id, CONCAT(U.Username,' : ',I.Title) AS  IssueAssignee
FROM Issues AS I
LEFT JOIN Users AS U ON I.AssigneeId = U.Id
ORDER BY I.Id DESC, I.AssigneeId

-- 08

SELECT F2.Id, F2.[Name], CONCAT(F2.Size, 'KB') AS Size
FROM Files AS F
RIGHT JOIN Files AS F2 ON F.ParentId = F2.Id
WHERE F.Id IS NULL
ORDER BY F2.Id, F2.[Name], F2.Size DESC

-- 09

SELECT TOP(5)
        R.Id, R.[Name], COUNT(C.RepositoryId) AS Commits
FROM Repositories AS R
LEFT JOIN Commits AS C ON C.RepositoryId = R.Id
LEFT JOIN RepositoriesContributors AS RC ON RC.RepositoryId = R.Id
GROUP BY R.Id,R.[Name]
ORDER BY Commits DESC, R.Id, R.Name

-- 10

SELECT U.Username, AVG(F.Size) AS Size
FROM Users AS U
LEFT JOIN Commits AS C ON C.ContributorId = U.Id
LEFT JOIN Files AS F ON F.CommitId = C.Id
WHERE F.CommitId IS NOT NULL
GROUP BY U.Username
ORDER BY Size DESC, Username

-- 11

GO

CREATE FUNCTION udf_AllUserCommits(@username VARCHAR(30))
RETURNS INT
AS
BEGIN
        RETURN (    SELECT COUNT(C.RepositoryId)
                      FROM Users AS U
                 LEFT JOIN Commits AS C ON C.ContributorId = U.Id
                     WHERE U.Username = @username
                )
END

GO

SELECT dbo.udf_AllUserCommits('UnderSinduxrein')

-- 12
GO

CREATE PROCEDURE usp_SearchForFiles(@fileExtension VARCHAR(100))
AS
BEGIN
        SELECT F.Id, F.Name, CONCAT(F.Size, 'KB') AS Size
        FROM Files AS F
        WHERE F.Name LIKE CONCAT('%', @fileExtension)
        ORDER BY F.Id, F.Name, F.Size DESC
END

GO

EXEC usp_SearchForFiles 'txt'