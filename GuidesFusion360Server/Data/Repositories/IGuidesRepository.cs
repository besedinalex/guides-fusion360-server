using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data.Repositories
{
    /// <summary>Repository to work with guides db data.</summary>
    public interface IGuidesRepository
    {
        /// <summary>Gets guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns guide.</returns>
        Task<Guide> GetGuide(int guideId);

        /// <summary>Gets all guides.</summary>
        /// <returns>Returns all guides.</returns>
        Task<List<Guide>> GetAllGuides();

        /// <summary>Gets all public guides.</summary>
        /// <returns>Returns all public guides.</returns>
        Task<List<Guide>> GetAllPublicGuides();

        /// <summary>Gets all private guides.</summary>
        /// <returns>Returns all private guides.</returns>
        Task<List<Guide>> GetAllHiddenGuides();

        /// <summary>Gets part guide.</summary>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <returns>Returns part guide.</returns>
        Task<PartGuide> GetPartGuide(int partGuideId);

        /// <summary>Gets part guides of the guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns part guides of the guide.</returns>
        Task<List<PartGuide>> GetPartGuides(int guideId);

        /// <summary>Adds new guide.</summary>
        /// <param name="guide">Guide data.</param>
        /// <returns>Returns id of the new guide.</returns>
        Task<int> CreateGuide(Guide guide);

        /// <summary>Adds new part guide.</summary>
        /// <param name="partGuide">Part guide data.</param>
        /// <returns>Returns id of the new part guide.</returns>
        Task<int> CreatePartGuide(PartGuide partGuide);

        /// <summary>Updates guide.</summary>
        /// <param name="guide">Guide data.</param>
        Task UpdateGuide(Guide guide);

        /// <summary>Updates guides.</summary>
        /// <param name="guides">Guides data.</param>
        Task UpdateGuides(List<Guide> guides);

        /// <summary>Updates part guide.</summary>
        /// <param name="partGuide">Part guide data.</param>
        Task UpdatePartGuide(PartGuide partGuide);

        /// <summary>Updates part guides.</summary>
        /// <param name="partGuides">Part guides data.</param>
        Task UpdatePartGuides(List<PartGuide> partGuides);

        /// <summary>Removes guide and all its part guides.</summary>
        /// <param name="guide">Guide data.</param>
        Task RemoveGuide(Guide guide);

        /// <summary>Removes part guide.</summary>
        /// <param name="partGuide">Part guide data.</param>
        Task RemovePartGuide(PartGuide partGuide);
    }
}