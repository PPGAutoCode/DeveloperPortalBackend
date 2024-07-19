CREATE TABLE FAQs (
    Id uniqueidentifier PRIMARY KEY,
    Question nvarchar(500) NOT NULL,
    Answer nvarchar(max) NOT NULL,
    Langcode nvarchar(4) NOT NULL CHECK (Langcode IN ('el', 'en')),
    Status bit NOT NULL,
    Created datetime2(7) NOT NULL,
    Changed datetime2(7) NOT NULL,
    FaqOrder int NOT NULL
);