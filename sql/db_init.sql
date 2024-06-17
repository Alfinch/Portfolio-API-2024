IF OBJECT_ID('FK_Update_ProjectId_Project_Id', 'F') IS NOT NULL
ALTER TABLE [Update]
DROP CONSTRAINT [FK_Update_ProjectId_Project_Id];
GO

IF OBJECT_ID('Project', 'U') IS NOT NULL
DROP TABLE [Project];
GO

CREATE TABLE [Project] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(50) NOT NULL,
    [Image] NVARCHAR(50) NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
);
GO

IF OBJECT_ID('Update', 'U') IS NOT NULL
DROP TABLE [Update];
GO

CREATE TABLE [Update] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProjectId] INT NOT NULL,
    [Title] NVARCHAR(50) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [Date] DATETIME2 NOT NULL,
    CONSTRAINT [FK_Update_ProjectId_Project_Id] FOREIGN KEY ([ProjectId]) REFERENCES [Project] ([Id])
);
GO
