CREATE TABLE [dbo].[AccountsTypes] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (50) NOT NULL,
    [UserId] INT           NOT NULL,
    [Orden]  INT           NOT NULL,
    CONSTRAINT [PK_AccountsTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccountsTypes_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

