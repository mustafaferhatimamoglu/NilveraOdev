-- Create schema objects for the Customers module
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Customers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Customers
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        ContactInfoJson NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END;
GO

IF OBJECT_ID('dbo.usp_Customers_Create', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Customers_Create;
END;
GO

CREATE PROCEDURE dbo.usp_Customers_Create
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @ContactInfoJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Customers (FirstName, LastName, ContactInfoJson)
    VALUES (@FirstName, @LastName, @ContactInfoJson);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;
GO

IF OBJECT_ID('dbo.usp_Customers_Update', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Customers_Update;
END;
GO

CREATE PROCEDURE dbo.usp_Customers_Update
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @ContactInfoJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET FirstName = @FirstName,
        LastName = @LastName,
        ContactInfoJson = @ContactInfoJson,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.usp_Customers_Delete', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Customers_Delete;
END;
GO

CREATE PROCEDURE dbo.usp_Customers_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Customers
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.usp_Customers_GetById', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Customers_GetById;
END;
GO

CREATE PROCEDURE dbo.usp_Customers_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        Id,
        FirstName,
        LastName,
        ContactInfoJson,
        CreatedAt,
        UpdatedAt
    FROM dbo.Customers
    WHERE Id = @Id;
END;
GO

IF OBJECT_ID('dbo.usp_Customers_GetAll', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Customers_GetAll;
END;
GO

CREATE PROCEDURE dbo.usp_Customers_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FirstName,
        LastName,
        ContactInfoJson,
        CreatedAt,
        UpdatedAt
    FROM dbo.Customers
    ORDER BY CreatedAt DESC;
END;
GO
