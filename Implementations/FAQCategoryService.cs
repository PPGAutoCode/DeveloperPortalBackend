using System;
using System.Collections.Generic;
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

        public async Task<string> CreateFAQCategory(CreateFAQCategoryDto dto)
        {
            ValidateCreateFAQCategoryDto(dto);

            const string sql = @"
                INSERT INTO FAQCategory (Id, Name, Description)
                VALUES (@Id, @Name, @Description);
            ";

            var id = Guid.NewGuid();
            await _dbConnection.ExecuteAsync(sql, new { Id = id, dto.Name, dto.Description });

            return id.ToString();
        }

        public async Task<FAQCategory> GetFAQCategory(FAQCategoryRequestDto dto)
        {
            const string sql = @"
                SELECT Id, Name, Description
                FROM FAQCategory
                WHERE Id = @Id;
            ";

            var result = await _dbConnection.QuerySingleOrDefaultAsync<FAQCategory>(sql, new { dto.Id });
            if (result == null)
            {
                throw new BusinessException("DP-404", "FAQ Category not found.");
            }

            return result;
        }

        public async Task<string> UpdateFAQCategory(UpdateFAQCategoryDto dto)
        {
            ValidateUpdateFAQCategoryDto(dto);

            const string sql = @"
                UPDATE FAQCategory
                SET Name = @Name, Description = @Description
                WHERE Id = @Id;
            ";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { dto.Id, dto.Name, dto.Description });
            if (rowsAffected == 0)
            {
                throw new BusinessException("DP-404", "FAQ Category not found.");
            }

            return "FAQ Category updated successfully.";
        }

        public async Task<bool> DeleteFAQCategory(DeleteFAQCategoryDto dto)
        {
            const string sql = @"
                DELETE FROM FAQCategory
                WHERE Id = @Id;
            ";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { dto.Id });
            return rowsAffected > 0;
        }

        public async Task<List<FAQCategory>> GetListFAQCategory(ListFAQCategoryRequestDto dto)
        {
            var sql = @"
                SELECT Id, Name, Description
                FROM FAQCategory
            ";

            if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
            {
                sql += $" ORDER BY {dto.SortField} {dto.SortOrder}";
            }

            sql += " OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";

            var result = await _dbConnection.QueryAsync<FAQCategory>(sql, new { dto.PageOffset, dto.PageLimit });
            return result.AsList();
        }

        private void ValidateCreateFAQCategoryDto(CreateFAQCategoryDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                throw new BusinessException("DP-422", "Name is required.");
            }
        }

        private void ValidateUpdateFAQCategoryDto(UpdateFAQCategoryDto dto)
        {
            if (dto.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Id is required.");
            }

            if (!string.IsNullOrEmpty(dto.Name) && dto.Name.Length > 255)
            {
                throw new BusinessException("DP-422", "Name cannot exceed 255 characters.");
            }

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
            {
                throw new BusinessException("DP-422", "Description cannot exceed 1000 characters.");
            }
        }
    }
}
