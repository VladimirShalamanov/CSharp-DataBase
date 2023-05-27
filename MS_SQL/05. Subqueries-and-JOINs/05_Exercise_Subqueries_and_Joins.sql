-- 01
   SELECT
      TOP (5) [e].[EmployeeID],
          [e].[JobTitle],
          [e].[AddressID],
          [a].[AddressText]
     FROM [Employees]
       AS [e]
LEFT JOIN [Addresses]
       AS [a]
       ON [e].[AddressID] = [a].[AddressID]
 ORDER BY [e].[AddressID]

-- 02

   SELECT
 TOP (50) [e].[FirstName],
          [e].[LastName],
          [t].[Name],
          [a].[AddressText]
     FROM [Employees] AS [e]
LEFT JOIN [Addresses] AS [a]
       ON [e].[AddressID] = [a].[AddressID]
LEFT JOIN [Towns] AS [t]
       ON [a].[TownID] = [t].[TownID]
 ORDER BY [e].[FirstName],
          [e].[LastName]

-- 03

   SELECT [e].[EmployeeID],
          [e].[FirstName],
          [e].[LastName],
          [d].[Name]
     FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d]
       ON [e].[DepartmentID] = [d].[DepartmentID]
    WHERE [d].[Name] = 'Sales'
 ORDER BY [e].[EmployeeID]

 -- 04

   SELECT
  TOP (5) [e].[EmployeeID],
          [e].[FirstName],
          [e].[Salary],
          [d].[Name] AS [DepartmentName]
     FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d]
       ON [e].[DepartmentID] = [d].[DepartmentID]
    WHERE [e].[Salary] > 15000
 ORDER BY [e].[DepartmentID]

  -- 05

   SELECT
  TOP (3) [e].[EmployeeID],
          [e].[FirstName]
     FROM [Employees] AS [e]
LEFT JOIN [EmployeesProjects] AS [ep]
       ON [e].[EmployeeID] = [ep].[EmployeeID]
    WHERE [ep].[ProjectID] IS NULL
 ORDER BY [e].[EmployeeID]
 
  -- 06

   SELECT [e].[FirstName],
          [e].[LastName],
          [e].[HireDate],
          [d].[Name] AS [DeptName]
     FROM [Employees] AS [e]
LEFT JOIN [Departments] AS [d]
       ON [e].[DepartmentID] = [d].[DepartmentID]
    WHERE [e].[HireDate] > '1.1.1999' AND [d].[Name] IN ('Sales', 'Finance')
 ORDER BY [e].[HireDate]
 
  -- 07

    SELECT
   TOP (5) [e].[EmployeeID],
           [e].[FirstName],
           [p].[Name] AS [ProjectName]
      FROM [EmployeesProjects] AS [ep]
INNER JOIN [Employees] AS [e]
        ON [ep].[EmployeeID] = [e].[EmployeeID]
INNER JOIN [Projects] AS [p]
        ON [ep].[ProjectID] = [p].[ProjectID]
     WHERE [p].[StartDate] > '08.13.2002' AND [p].[EndDate] IS NULL
  ORDER BY [e].[EmployeeID]

-- 08

    SELECT [e].[EmployeeID],
           [e].[FirstName],
           CASE
               WHEN [p].[StartDate] > '01.01.2005' THEN NULL
               ELSE [p].[Name]
             END AS [ProjectName]
      FROM [EmployeesProjects] AS [ep]
INNER JOIN [Employees] AS [e]
        ON [ep].[EmployeeID] = [e].[EmployeeID]
INNER JOIN [Projects] AS [p]
        ON [ep].[ProjectID] = [p].[ProjectID]
     WHERE [e].[EmployeeID] = 24

-- 09

    SELECT [e].[EmployeeID],
           [e].[FirstName],
           [e].[ManagerID],
           [m].[FirstName] AS [ManagerName]
      FROM [Employees] AS [e]
INNER JOIN [Employees] AS [m]
        ON [e].[ManagerID] = [m].[EmployeeID]
     WHERE [e].[ManagerID] IN (3, 7)
  ORDER BY [e].[EmployeeID]

-- 10

    SELECT
   TOP(50) [e].[EmployeeID],
           [e].[FirstName] + ' ' + [e].[LastName] AS [EmployeeName],
           [m].[FirstName] + ' ' + [m].[LastName] AS [ManagerName],
           [d].[Name] AS [DepartmentName]
      FROM [Employees] AS [e]
INNER JOIN [Employees] AS [m]
        ON [e].[ManagerID] = [m].[EmployeeID]
 LEFT JOIN [Departments] AS [d]
        ON [e].[DepartmentID] = [d].[DepartmentID]
  ORDER BY [e].[EmployeeID]
  
-- 11

SELECT
      MIN(a.AverageSalary) AS MinAverageSalary
      FROM
      (
          SELECT [e].[DepartmentID],
                 AVG(e.Salary) AS [AverageSalary]
            FROM [Employees] AS [e]
        GROUP BY [e].[DepartmentID]
      ) AS a


-- 12

    SELECT [c].[CountryCode],
           [m].[MountainRange],
           [p].[PeakName],
           [p].[Elevation]
      FROM [MountainsCountries] AS [mc]
INNER JOIN [Countries] AS [c]
        ON [mc].[CountryCode] = [c].[CountryCode]
INNER JOIN [Mountains] AS [m]
        ON [mc].[MountainId] = [m].[Id]
INNER JOIN [Peaks] AS [p]
        ON [m].[Id] = [p].[MountainId]
     WHERE [c].[CountryName] = 'Bulgaria'
       AND [p].[Elevation] > 2835
  ORDER BY [p].[Elevation] DESC

-- 13

  SELECT [CountryCode],
         COUNT([MountainId]) AS [MountainRanges]
    FROM [MountainsCountries]
   WHERE [CountryCode] IN (
                            SELECT [CountryCode]
                              FROM [Countries]
                             WHERE [CountryName] IN ('United States', 'Russia', 'Bulgaria')
                          )
GROUP BY [CountryCode]

-- 14

    SELECT TOP(5)
           [c].[CountryName],
           [r].[RiverName]
      FROM [CountriesRivers] AS [cr]
      INNER JOIN [Rivers] AS [r]
        ON [r].[Id] = [cr].[RiverId]
FULL JOIN [Countries] AS [c]
        ON [c].[CountryCode] = [cr].[CountryCode]
FULL JOIN [Continents] AS [co]
        ON [co].[ContinentCode] = [c].[ContinentCode]
     WHERE [co].[ContinentName] = 'Africa'
  ORDER BY [c].[CountryName]

-- 15

SELECT rc.ContinentCode,
       rc.CurrencyCode,
       rc.Count 
  FROM
      (
         SELECT c.ContinentCode,
                c.CurrencyCode,
                COUNT(c.CurrencyCode) AS Count,
                DENSE_RANK() OVER (PARTITION BY c.ContinentCode ORDER BY COUNT(c.CurrencyCode) DESC) AS Rank
           FROM Countries AS c
       GROUP BY c.ContinentCode, c.CurrencyCode
      ) AS rc
 WHERE rc.Rank = 1 AND rc.Count > 1

-- 16

   SELECT COUNT([ContinentCode]) AS [Count]
     FROM [MountainsCountries] AS [mc]
FULL JOIN [Countries] AS [c]
       ON [c].[CountryCode] = [mc].[CountryCode]
    WHERE [mc].[MountainId] IS NULL

-- 17

   SELECT TOP(5)
          [c].[CountryName],
          MAX([p].[Elevation]) AS [HighestPeakElevation],
          MAX([r].[Length]) AS [LongestRiverLength]
     FROM [Countries] AS [c]
LEFT JOIN [CountriesRivers] AS [cr]
       ON [cr].[CountryCode] = [c].[CountryCode]
LEFT JOIN [Rivers] AS [r]
       ON [r].[Id] = [cr].[RiverId]
LEFT JOIN [MountainsCountries] AS [mc]
       ON [mc].[CountryCode] = [c].[CountryCode]
LEFT JOIN [Mountains] AS [m]
       ON [m].[Id] = [mc].[MountainId]
LEFT JOIN [Peaks] AS [p]
       ON [p].[MountainId] = [m].[Id]
 GROUP BY [c].[CountryName]
 ORDER BY [HighestPeakElevation] DESC,
          [LongestRiverLength] DESC,
          [CountryName]

-- 18

 SELECT TOP (5)
        [CountryName] AS [Country],
        ISNULL([PeakName], '(no highest peak)') AS [Highest Peak Name],
        ISNULL([Elevation], 0) AS 'Highest Peak Elevation',
        ISNULL([MountainRange], '(no mountain)') AS [Mountain]
   FROM 
         (
                   SELECT [c].[CountryName],
                          [p].[PeakName],
                          [p].[Elevation],
                          [m].[MountainRange],
                          DENSE_RANK() OVER(PARTITION BY [c].[CountryName] ORDER BY [p].[Elevation]) AS [PeakRank]
                     FROM [Countries] AS [c]
                LEFT JOIN [MountainsCountries] AS [mc]
                       ON [mc].[CountryCode] = [c].[CountryCode]
                LEFT JOIN [Mountains] AS [m]
                       ON [m].[Id] = [mc].[MountainId]
                LEFT JOIN [Peaks] AS [p]
                       ON [p].[MountainId] = [m].[Id]
         ) AS [PeaksSubQuery]
   WHERE [PeakRank] = 1
ORDER BY [Country],
         [Highest Peak Name]