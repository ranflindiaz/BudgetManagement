CREATE TABLE [dbo].[OperationsTypes] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_OperationsTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

