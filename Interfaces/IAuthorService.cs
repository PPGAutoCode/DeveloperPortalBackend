using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing author-related operations.
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Asynchronously creates a new author based on the provided data.
        /// </summary>
        /// <param name="createAuthorDto">The data transfer object containing the information for the new author.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a string indicating the result of the creation process.</returns>
        Task<string> CreateAuthor(CreateAuthorDto createAuthorDto);

        /// <summary>
        /// Asynchronously retrieves an author based on the provided request data.
        /// </summary>
        /// <param name="authorRequestDto">The data transfer object containing the request information for retrieving an author.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the author data transfer object.</returns>
        Task<AuthorDto> GetAuthor(AuthorRequestDto authorRequestDto);

        /// <summary>
        /// Asynchronously updates an existing author based on the provided data.
        /// </summary>
        /// <param name="updateAuthorDto">The data transfer object containing the updated information for the author.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a string indicating the result of the update process.</returns>
        Task<string> UpdateAuthor(UpdateAuthorDto updateAuthorDto);

        /// <summary>
        /// Asynchronously deletes an author based on the provided data.
        /// </summary>
        /// <param name="deleteAuthorDto">The data transfer object containing the information for deleting an author.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteAuthor(DeleteAuthorDto deleteAuthorDto);

        /// <summary>
        /// Asynchronously retrieves a list of authors based on the provided request data.
        /// </summary>
        /// <param name="listAuthorRequestDto">The data transfer object containing the request information for retrieving a list of authors.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of author data transfer objects.</returns>
        Task<List<AuthorDto>> GetListAuthor(ListAuthorRequestDto listAuthorRequestDto);
    }
}
