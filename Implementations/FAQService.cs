
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;
using System.Data.SqlClient;

namespace ProjectName.Services
{
    public partial class FAQService : IFAQService
    {
        private readonly string _connectionString;

        public FAQService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> CreateFAQCategory(CreateFAQCategoryDto request)
        {
            // Step 1: Validate the request payload
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Name is required.");
            }

            // Step 3: Create a new FaqCategory type object
            var faqCategory = new FaqCategory
            {
                Id = Guid.NewGuid(), // Step 4: Generate a new unique identifier
                Name = request.Name, // Step 5: Assign the Name
                Description = request.Description // Step 5: Assign the Description, otherwise leave it null
            };

            // Step 7: Save the newly created FaqCategory object to the database
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT INTO FAQCategories (Id, Name, Description) VALUES (@Id, @Name, @Description)";

                int rowsAffected = await db.ExecuteAsync(sqlQuery, faqCategory);

                // Step 8: If the transaction is successful
                if (rowsAffected > 0)
                {
                    return faqCategory.Id.ToString();
                }
                else
                {
                    // Step 10: If the transaction fails
                    throw new TechnicalException("DP-500", "Failed to create FAQ category.");
                }
            }
        }
    }
}
