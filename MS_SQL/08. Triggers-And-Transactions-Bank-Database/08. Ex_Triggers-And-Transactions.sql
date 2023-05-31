-- 01

CREATE TABLE Logs
(
    [LogId] INT NOT NULL IDENTITY, 
    [AccountId] INT FOREIGN KEY REFERENCES Accounts(Id), 
    [OldSum] MONEY, 
    [NewSum] MONEY
)

GO

CREATE TRIGGER tr_ChngeBalance ON Accounts
AFTER UPDATE
AS
BEGIN
INSERT INTO Logs(AccountId, OldSum, NewSum)
SELECT i.Id, d.Balance, i.Balance
FROM inserted AS i
INNER JOIN deleted AS d
ON i.Id = d.Id
END

GO

-- 02

CREATE TABLE NotificationEmails
(
    [Id] INT IDENTITY PRIMARY KEY,
    [Recipient] VARCHAR(100),
    [Subject] NVARCHAR(100),
    [Body] NVARCHAR(MAX)
)
GO

CREATE TRIGGER tr_EmailNotification
ON Logs
AFTER INSERT
AS
BEGIN
	INSERT NotificationEmails(Recipient, Subject, Body)
	SELECT i.AccountId, 
			CONCAT('Balance change for account: ', i.AccountId), 
			CONCAT('On ', GETDATE(), ' your balance was changed from ', i.OldSum, ' to ', i.NewSum)
	FROM inserted AS i
END

GO

-- 03

CREATE PROCEDURE usp_DepositMoney(@AccountId INT, @MoneyAmount DECIMAL(18,4))
AS
BEGIN TRANSACTION 
            UPDATE Accounts SET Balance += @MoneyAmount
            WHERE Id = @AccountId
            IF @@ROWCOUNT <> 1 AND @AccountId IS NOT NULL AND @MoneyAmount < 1
                BEGIN
                    ROLLBACK
                END
            COMMIT

GO

-- 04

GO

CREATE PROCEDURE usp_WithdrawMoney(@AccountId INT, @MoneyAmount DECIMAL(18,4))
AS
BEGIN TRANSACTION 

            UPDATE Accounts
            SET Balance -= @MoneyAmount
            WHERE Id = @AccountId

            IF @@ROWCOUNT <> 1 AND @AccountId IS NULL AND @MoneyAmount > (
                                                                           SELECT a.Balance
                                                                              FROM Accounts AS a
                                                                             WHERE a.id = @AccountId
                                                                         )
            BEGIN ROLLBACK
            END
                
        COMMIT

GO

EXEC dbo.usp_WithdrawMoney 5,25

SELECT * FROM Accounts WHERE [Id] = 5

GO

-- 05

CREATE PROCEDURE usp_TransferMoney(@SenderId INT, @ReceiverId INT, @Amount DECIMAL(18,4))
AS
BEGIN
        EXEC dbo.usp_WithdrawMoney @SenderId, @Amount
        EXEC dbo.usp_DepositMoney @ReceiverId, @Amount
END

GO

EXEC dbo.usp_TransferMoney 5, 1, 5000

SELECT [Balance] FROM [Accounts] WHERE [Id] IN (1,5)

GO

-- Employees with Three Projects ==>> Queries for SoftUni Database

CREATE PROCEDURE usp_AssignProject(@emloyeeId INT, @projectID INT)
AS 
BEGIN
    BEGIN TRANSACTION
        INSERT INTO [EmployeesProjects]([EmployeeID], [ProjectID])
            VALUES(@emloyeeId, @projectID);

        IF ((
                    SELECT COUNT([ProjectID]) 
                    FROM [EmployeesProjects]
                    WHERE [EmployeeID] = @emloyeeId
            ) > 3)
            BEGIN
                ROLLBACK
                RAISERROR ('The employee has too many projects!', 16, 1);
                RETURN
            END
        COMMIT
END

GO