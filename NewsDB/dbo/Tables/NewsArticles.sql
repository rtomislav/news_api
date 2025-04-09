CREATE TABLE [dbo].[NewsArticles] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Title]     NVARCHAR (200) NOT NULL,
    [Content]   NVARCHAR (MAX) NOT NULL,
    [CreatedAt] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [UpdatedAt] DATETIME2 (7)  NULL,
    [AuthorId]  NVARCHAR (450) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NewsArticles_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Authors] ([IdAuthors])
);

