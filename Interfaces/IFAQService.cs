
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing FAQ (Frequently Asked Questions) entries.
    /// </summary>
    public interface IFAQService
    {
        /// <summary>
        /// Creates a new FAQ entry.
        /// </summary>
        /// <param name="createFAQDto">The data transfer object containing the information for the new FAQ entry.</param>
        /// <returns>A string representing the unique identifier of the newly created FAQ entry.</returns>
        Task<string> CreateFAQ(CreateFAQDto createFAQDto);

        /// <summary>
        /// Retrieves a detailed FAQ entry based on the provided request.
        /// </summary>
        /// <param name="faqRequestDto">The data transfer object containing the request parameters to fetch the FAQ entry.</param>
        /// <returns>An FAQ object containing the detailed information of the requested FAQ entry.</returns>
        Task<FAQ> GetFAQSuper(FAQRequestDto faqRequestDto);

        /// <summary>
        /// Updates an existing FAQ entry.
        /// </summary>
        /// <param name="updateFAQDto">The data transfer object containing the updated information for the FAQ entry.</param>
        /// <returns>A string representing the unique identifier of the updated FAQ entry.</returns>
        Task<string> UpdateFAQ(UpdateFAQDto updateFAQDto);

        /// <summary>
        /// Deletes an FAQ entry.
        /// </summary>
        /// <param name="deleteFAQDto">The data transfer object containing the information necessary to delete the FAQ entry.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteFAQ(DeleteFAQDto deleteFAQDto);

        /// <summary>
        /// Retrieves a list of FAQ entries based on the provided request.
        /// </summary>
        /// <param name="listFAQRequestDto">The data transfer object containing the request parameters to fetch the list of FAQ entries.</param>
        /// <returns>A list of FAQ objects containing the information of the requested FAQ entries.</returns>
        Task<List<FAQ>> GetListFAQ(ListFAQRequestDto listFAQRequestDto);
    }
}
