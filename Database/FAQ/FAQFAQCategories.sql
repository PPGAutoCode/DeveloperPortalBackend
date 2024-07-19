CREATE TABLE FAQFAQCategories (
    Id uniqueidentifier PRIMARY KEY,
    FAQId uniqueidentifier NOT NULL,
    FAQCategoryId uniqueidentifier NOT NULL,
    FOREIGN KEY (FAQId) REFERENCES FAQs(Id),
    FOREIGN KEY (FAQCategoryId) REFERENCES FAQCategories(Id)
);