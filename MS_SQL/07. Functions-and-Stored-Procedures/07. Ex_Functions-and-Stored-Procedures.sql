-- 01

CREATE PROCEDURE [usp_GetEmployeesSalaryAbove35000]
    AS
 BEGIN
          SELECT [FirstName],
                 [LastName]
            FROM [Employees]
           WHERE [Salary] > 35000
    END

EXEC [dbo].[usp_GetEmployeesSalaryAbove35000]

GO
-- 02

CREATE PROCEDURE [usp_GetEmployeesSalaryAboveNumber] @minSalary DECIMAL(18,4)
              AS
           BEGIN
                    SELECT [FirstName],
                           [LastName]
                      FROM [Employees]
                     WHERE [Salary] >= @minSalary
             END

EXEC [dbo].[usp_GetEmployeesSalaryAboveNumber] 43000

GO

-- 03

CREATE PROCEDURE [usp_GetTownsStartingWith] @string VARCHAR(50)
              AS
           BEGIN
                   DECLARE @countOfString INT = LEN(@string)

                    SELECT [Name]
                      FROM [Towns]
                     WHERE LEFT([Name], @countOfString) = @string
             END

EXEC [dbo].[usp_GetTownsStartingWith] 'b'

GO

-- 04

CREATE PROCEDURE [usp_GetEmployeesFromTown] @townName VARCHAR(50)
              AS
           BEGIN
                    SELECT [e].[FirstName],
                           [e].[LastName]
                      FROM [Employees] AS [e]
                 LEFT JOIN [Addresses] AS [a]
                        ON [a].[AddressID] = [e].[AddressID]
                 LEFT JOIN [Towns] AS [t]
                        ON [t].[TownID] = [a].[TownID]
                     WHERE [t].[Name] = @townName
             END

EXEC [dbo].[usp_GetEmployeesFromTown] Sofia

GO

-- 05

CREATE FUNCTION [ufn_GetSalaryLevel] (@salary DECIMAL(18,4))
RETURNS VARCHAR(10)
             AS
          BEGIN
                    DECLARE @salaryLevel VARCHAR(10)

                    IF @salary < 30000
                       SET @salaryLevel = 'Low'

                    ELSE IF @salary <= 50000
                       SET @salaryLevel = 'Average'

                    ELSE
                       SET @salaryLevel = 'High'

                    RETURN @salaryLevel
            END

GO

-- 06

CREATE PROCEDURE [usp_EmployeesBySalaryLevel] (@salaryOfLevel VARCHAR(8))
             AS
          BEGIN
                    SELECT [FirstName],
                           [LastName]
                      FROM [Employees]
                     WHERE [dbo].[ufn_GetSalaryLevel]([Salary]) = @salaryOfLevel
            END

EXEC [dbo].[usp_EmployeesBySalaryLevel] 'High'

GO

-- 07

CREATE FUNCTION [ufn_IsWordComprised] (@setOfLetters VARCHAR(50), @word VARCHAR(50))
     RETURNS BIT
              AS
           BEGIN
                    DECLARE @i INT = 1

                    WHILE(@i <= LEN(@word))
                    BEGIN
                            DECLARE @currCh CHAR = SUBSTRING(@word, @i, 1)

                            IF CHARINDEX(@currCh, @setOfLetters) = 0
                            BEGIN
                                RETURN 0;
                            END

                            SET @i += 1;

                      END

                      RETURN 1;
             END

GO

SELECT [dbo].[ufn_IsWordComprised]('oistmiahf', 'halves') AS [Result]

GO

-- 08

CREATE PROCEDURE usp_DeleteEmployeesFromDepartment (@departmentId INT)
              AS
           BEGIN
                    DECLARE @employeesToDelete TABLE ([Id] INT);
                    INSERT INTO @employeesToDelete
                         SELECT [EmployeeID]
                           FROM [Employees]
                          WHERE [DepartmentID] = @departmentId


                    DELETE
                      FROM [EmployeesProjects]
                     WHERE [EmployeeID] IN (SELECT * FROM @employeesToDelete)

                     ALTER TABLE [Departments]
                    ALTER COLUMN [ManagerID] INT

                    UPDATE [Departments]
                       SET [ManagerID] = NULL
                     WHERE [ManagerID] IN (SELECT * FROM @employeesToDelete)

                     UPDATE [Employees]
                        SET [ManagerID] = NULL
                      WHERE [ManagerID] IN (SELECT * FROM @employeesToDelete)


                    DELETE
                      FROM [Employees]
                     WHERE [DepartmentID] = @departmentId

                    DELETE
                      FROM [Departments]
                     WHERE [DepartmentID] = @departmentId


                    SELECT COUNT(*)
                      FROM [Employees]
                     WHERE [DepartmentID] = @departmentId
             END

EXEC [dbo].[usp_DeleteEmployeesFromDepartment] 7
GO

-- 09

CREATE PROCEDURE [usp_GetHoldersFullName]
              AS
           BEGIN
                  SELECT CONCAT([FirstName], ' ', [LastName]) AS [Full Name]
                    FROM [AccountHolders]
             END

GO

-- 10

CREATE PROCEDURE [usp_GetHoldersWithBalanceHigherThan] (@number DECIMAL(10,2))
              AS
           BEGIN
                     SELECT [ah].[FirstName] AS [First Name],
                            [ah].[LastName] AS [Last Name]
                       FROM [AccountHolders] AS [ah]
                  LEFT JOIN [Accounts] AS [a]
                         ON [a].[AccountHolderId] = [ah].[Id]
                   GROUP BY [ah].[FirstName], [ah].[LastName], [a].[AccountHolderId]
                     HAVING SUM([a].[Balance]) > @number
                   ORDER BY [First Name], [Last Name]
             END

EXEC [dbo].[usp_GetHoldersWithBalanceHigherThan] 5000000

GO

-- 11

CREATE FUNCTION [ufn_CalculateFutureValue]
  (@sum DECIMAL(10,2), @yearlyInterestRate FLOAT, @years INT)
RETURNS DECIMAL(18,4)
                  AS
               BEGIN
                     DECLARE @result DECIMAL(18,4);
                     SET @result = @sum * POWER(1 + @yearlyInterestRate, @years)

                     RETURN @result;
               END
GO

SELECT [dbo].[ufn_CalculateFutureValue] (1000, 0.1, 5)

GO

-- 12

CREATE PROCEDURE [usp_CalculateFutureValueForAccount] (@AccountId INT, @InterestRate FLOAT)
              AS
           BEGIN
                     SELECT a.Id AS [Account Id],
                           ah.FirstName AS [First Name],
                           ah.LastName AS [Last Name],
                           a.Balance AS [Current Balance],
                           dbo.ufn_CalculateFutureValue(a.Balance, @InterestRate, 5) AS [Balance in 5 years]
                     FROM AccountHolders AS ah
                     JOIN Accounts AS a
                        ON ah.Id = a.Id
                     WHERE a.Id = @AccountId
             END
GO

EXEC [dbo].[usp_CalculateFutureValueForAccount] 1, 0.1

GO

-- 13

CREATE FUNCTION ufn_CashInUsersGames(@gameName VARCHAR(50))
  RETURNS TABLE
             AS
         RETURN (
                     SELECT SUM(Cash) AS SumCash
                       FROM (
                                   SELECT g.Name,
                                          ug.Cash,
                                          ROW_NUMBER() OVER(ORDER BY ug.Cash DESC) AS RowNumber
                                     FROM UsersGames AS ug
                               INNER JOIN Games AS g
                                       ON ug.GameId = g.Id
                                    WHERE g.Name = @gameName
                            )
                         AS RankSubQuery
                      WHERE RowNumber % 2 <> 0   
                )

GO

SELECT * FROM dbo.ufn_CashInUsersGames('Love in a mist')