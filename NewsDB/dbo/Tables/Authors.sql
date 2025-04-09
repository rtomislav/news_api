CREATE TABLE [dbo].[Authors] (
    [IdAuthors]   NVARCHAR (450) NOT NULL,
    [DisplayName] NVARCHAR (100) NOT NULL,
    [Bio]         NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([IdAuthors] ASC),
    CONSTRAINT [FK_Authors_AspNetUsers] FOREIGN KEY ([IdAuthors]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

