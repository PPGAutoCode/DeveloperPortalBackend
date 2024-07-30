
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
import ProjectName.Interfaces;
import ProjectName.ControllersExceptions;

public class APIEndpointService : IAPIEndpointService
{
    private readonly IDbConnection _dbConnection;
    private readonly IAttachmentService _attachmentService;
    private readonly IAppEnvironmentService _appEnvironmentService;
    private readonly IApiTagService _apiTagService;

    public APIEndpointService(IDbConnection dbConnection, IAttachmentService attachmentService, IAppEnvironmentService appEnvironmentService, IApiTagService apiTagService)
    {
        _dbConnection = dbConnection;
        _attachmentService = attachmentService;
        _appEnvironmentService = appEnvironmentService;
        _apiTagService = apiTagService;
    }

    public async Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto request)
    {
        // Step 1: Validate the UpdateAPIEndpointDto
        if (request == null || request.Id == Guid.Empty || string.IsNullOrEmpty(request.ApiName) || string.IsNullOrEmpty(request.Langcode) || request.AppEnvironment == Guid.Empty || request.Deprecated == null || request.Sticky == null || request.Promote == null || request.Published == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch the existing APIEndpoint object from the database
        var apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
        if (apiEndpoint == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Update Attachments
        async Task UpdateAttachmentAsync(Guid? existingAttachmentId, UpdateAttachmentDto updateAttachmentDto)
        {
            if (existingAttachmentId.HasValue && updateAttachmentDto != null)
            {
                updateAttachmentDto.Id = existingAttachmentId.Value;
                var attachmentResult = await _attachmentService.UpdateAttachment(updateAttachmentDto);
                if (Guid.TryParse(attachmentResult, out var attachmentGuid))
                {
                    return attachmentGuid;
                }
            }
            return null;
        }

        apiEndpoint.Documentation = await UpdateAttachmentAsync(apiEndpoint.Documentation, request.Documentation);
        apiEndpoint.Swagger = await UpdateAttachmentAsync(apiEndpoint.Swagger, request.Swagger);
        apiEndpoint.Tour = await UpdateAttachmentAsync(apiEndpoint.Tour, request.Tour);

        // Step 4: Fetch and Validate Related Entities
        var appEnvironment = await _appEnvironmentService.GetAppEnvironment(new AppEnvironmentRequestDto { Id = request.AppEnvironment });
        if (appEnvironment == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 5: Handle ApiTags
        List<ApiTag> temporaryApiTags = null;
        if (request.ApiTags != null && request.ApiTags.Any())
        {
            temporaryApiTags = new List<ApiTag>();
            foreach (var tagName in request.ApiTags)
            {
                var apiTag = await _apiTagService.GetApiTag(new ApiTagRequestDto { Name = tagName });
                if (apiTag == null)
                {
                    apiTag = await _apiTagService.CreateApiTag(new CreateApiTagDto { Name = tagName });
                }
                temporaryApiTags.Add(apiTag);
            }
        }

        // Step 6: ApiTags Removal and Addition
        var existingApiTags = (await _dbConnection.QueryAsync<Guid>("SELECT ApiTagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = request.Id })).ToList();
        var newApiTags = temporaryApiTags?.Select(t => t.Id).ToList() ?? new List<Guid>();

        var tagsToRemove = existingApiTags.Except(newApiTags).ToList();
        var tagsToAdd = newApiTags.Except(existingApiTags).ToList();

        // Step 7: Update the APIEndpoint object
        apiEndpoint.ApiName = request.ApiName;
        apiEndpoint.ApiScope = request.ApiScope;
        apiEndpoint.ApiScopeProduction = request.ApiScopeProduction;
        apiEndpoint.Deprecated = request.Deprecated.Value;
        apiEndpoint.Description = request.Description;
        apiEndpoint.EndpointUrls = request.EndpointUrls;
        apiEndpoint.AppEnvironment = request.AppEnvironment;
        apiEndpoint.ApiVersion = request.ApiVersion;
        apiEndpoint.Langcode = request.Langcode;
        apiEndpoint.Sticky = request.Sticky.Value;
        apiEndpoint.Promote = request.Promote.Value;
        apiEndpoint.UrlAlias = request.UrlAlias;
        apiEndpoint.Published = request.Published.Value;

        // Step 8: Perform Database Updates in a Single Transaction
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                // Remove Old Tags
                foreach (var tagId in tagsToRemove)
                {
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE ApiTagId = @ApiTagId AND APIEndpointId = @APIEndpointId", new { ApiTagId = tagId, APIEndpointId = request.Id }, transaction);
                }

                // Add New Tags
                foreach (var tagId in tagsToAdd)
                {
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, ApiTagId, APIEndpointId) VALUES (@Id, @ApiTagId, @APIEndpointId)", new { Id = Guid.NewGuid(), ApiTagId = tagId, APIEndpointId = request.Id }, transaction);
                }

                // Update APIEndpoint
                await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET ApiName = @ApiName, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, Deprecated = @Deprecated, Description = @Description, EndpointUrls = @EndpointUrls, AppEnvironment = @AppEnvironment, ApiVersion = @ApiVersion, Langcode = @Langcode, Sticky = @Sticky, Promote = @Promote, UrlAlias = @UrlAlias, Published = @Published WHERE Id = @Id", apiEndpoint, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        return apiEndpoint.Id.ToString();
    }
}
