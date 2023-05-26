-- 01
SELECT [FirstName], [LastName]
  FROM [Employees]
 WHERE [FirstName] LIKE 'Sa%'

 -- 02
SELECT [FirstName], [LastName]
  FROM [Employees]
 WHERE [LastName] LIKE '%ei%'

  -- 03
SELECT [FirstName]
  FROM [Employees]
 WHERE [DepartmentID] = 3 OR [DepartmentID] = 10
   AND [HireDate] BETWEEN '1995' AND '2005'

-- 04
SELECT [FirstName], [LastName]
  FROM [Employees]
 WHERE [JobTitle] NOT LIKE '%engineer%'

-- 05
  SELECT [Name]
    FROM [Towns]
   WHERE LEN([Name]) IN (5,6)
ORDER BY [Name]

-- 06
  SELECT [TownID], [Name]
    FROM [Towns]
   WHERE [Name] LIKE 'M%'
      OR [Name] LIKE 'K%'
      OR [Name] LIKE 'B%'
      OR [Name] LIKE 'E%'
ORDER BY [Name]

-- 07
  SELECT [TownID], [Name]
    FROM [Towns]
   WHERE [Name] NOT LIKE 'R%'
     AND [Name] NOT LIKE 'B%'
     AND [Name] NOT LIKE 'D%'
ORDER BY [Name]

-- 08
GO
CREATE VIEW [V_EmployeesHiredAfter2000]
AS
(
  SELECT [FirstName], [LastName]
    FROM [Employees]
   WHERE [HireDate] > '2001'
)
GO

-- 09
SELECT [FirstName], [LastName]
  FROM [Employees]
 WHERE LEN([LastName]) = 5

-- 10
  SELECT [EmployeeID], [FirstName], [LastName], [Salary], DENSE_RANK() OVER   
                          (PARTITION BY Salary ORDER BY EmployeeID) AS Rank  
    FROM [Employees]
   WHERE [Salary] BETWEEN 10000 AND 50000
ORDER BY [Salary] DESC

-- 11
SELECT M.EmployeeID, M.FirstName, M.LastName, M.Salary, M.Rank
  FROM
  (
      SELECT
            [EmployeeID], [FirstName], [LastName], [Salary],
            DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS Rank
       FROM Employees
  ) AS M
   WHERE m.Rank = 2
     AND [Salary] BETWEEN 10000 AND 50000
ORDER BY [Salary] DESC

-- 12
SELECT [CountryName] AS [Country Name], [IsoCode] AS [Iso Code]
  FROM [Countries]
 WHERE LOWER([CountryName]) LIKE '%a%a%a%'
ORDER BY [Iso Code]

-- 13
  SELECT [p].[PeakName], 
         [r].[RiverName],
         LOWER(CONCAT(SUBSTRING([p].[PeakName], 1, LEN([p].[PeakName]) - 1), [r].[RiverName]))
      AS [Mix] 
    FROM [Peaks]
      AS [p],
         [Rivers]
      AS [r]
   WHERE RIGHT(LOWER([p].[PeakName]), 1) = LEFT(LOWER([r].[RiverName]), 1)
ORDER BY [Mix]

-- 14
  SELECT TOP(50)
         [Name], CONVERT(char, [Start], 23)
      AS [Start]
    FROM [Games]
   WHERE DATEPART(YEAR, [Start]) IN (2011, 2012)
ORDER BY [Start], [Name]

-- 15
  SELECT [Username],
          SUBSTRING([Email], CHARINDEX('@', [Email]) + 1, LEN([Email]) - CHARINDEX('@', [Email]))
      AS [Email Provider]
    FROM [Users]
ORDER BY [Email Provider], [Username]

-- 16
  SELECT [Username], [IpAddress] AS [Ip Address]
    FROM [Users]
   WHERE [IpAddress] LIKE '___.1%.%.___'
ORDER BY [Username]

-- 17
  SELECT [Name] AS [Game],
         CASE
             WHEN DATEPART(HOUR, [Start]) >= 0 AND DATEPART(HOUR, [Start]) < 12 THEN 'Morning'
             WHEN DATEPART(HOUR, [Start]) >= 12 AND DATEPART(HOUR, [Start]) < 18 THEN 'Afternoon'
             ELSE 'Evening'
          END
      AS [Part of the Day],
         CASE
             WHEN [Duration] <= 3 THEN 'Extra Short'
             WHEN [Duration] BETWEEN 4 AND 6 THEN 'Short'
             WHEN [Duration] > 6 THEN 'Long'
             ELSE 'Extra Long'
          END
      AS [Duration]
    FROM [Games]
ORDER BY [Game], [Duration]

-- 18
SELECT [ProductName], [OrderDate],
       DATEADD(DAY, 3, [OrderDate])
    AS [Pay Due],
       DATEADD(MONTH, 1, [OrderDate])
    AS [Deliver Due]
  FROM [Orders]