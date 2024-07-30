using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public partial class FAQCategoryService : IFAQCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public FAQCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateFAQCategory(CreateFAQCategoryDto request)
        {
            // Step 1: Validate that the request payload contains the necessary parameter ("Name").
            if (string.IsNullOrEmpty(request.Name))
            {
                // Step 2: If "Name" is null, return response with response.exception = new DP-422 exception.
                throw new BusinessException("DP-422", "Client Error");
            }

            // Step 3: Create a new FAQCategory type object (FAQCategories) with the provided details.
            var faqCategory = new FAQCategory
            {
                Id = Guid.NewGuid(), // Step 4: Generate a new unique identifier for the FAQCategory entry.
                Name = request.Name, // Step 5: FAQCategory.Name : request.Name
                Description = request.Description // Step 6: FAQCategory.Description : request.Description, otherwise leave it null
            };

            // Step 7: Save the newly created FAQCategory object to the database.
            const string sql = "INSERT INTO FAQCategories (Id, Name, Description) VALUES (@Id, @Name, @Description)";
            try
            {
                await _dbConnection.ExecuteAsync(sql, faqCategory);

                // Step 8: If the transaction is successful, return response.payload.id = FAQCategory.id
                return faqCategory.Id.ToString();
            }
            catch (Exception)
            {
                // Step 10: If the transaction fails, return response with response.exception = new DP-500 exception
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }
}
