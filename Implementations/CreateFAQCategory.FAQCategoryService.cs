using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public partial class FAQCategoryService : IFAQCategoryService
    {
        private readonly IDbConnection _dbConnection;

        public FAQCategoryService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public async Task<string> CreateFAQCategory(CreateFAQCategoryDto createFAQCategoryDto)
        {
            // Validation logic
            if (createFAQCategoryDto == null)
                throw new BusinessException("DP-422", "CreateFAQCategoryDto cannot be null.");
            if (string.IsNullOrWhiteSpace(createFAQCategoryDto.Name))
                throw new BusinessException("DP-422", "Name cannot be null or empty.");

            try
            {
                var sql = "INSERT INTO FAQCategory (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as varchar(50));";
                var id = await _dbConnection.QuerySingleAsync<string>(sql, createFAQCategoryDto);
                return id;
            }
            catch (Exception ex)
            {
                throw new TechnicalException("DP-500", "A technical error occurred while creating FAQ category.");
            }
        }

        // Other methods (GetFAQCategory, UpdateFAQCategory, DeleteFAQCategory, GetListFAQCategory) would follow a similar pattern.
    }
}
