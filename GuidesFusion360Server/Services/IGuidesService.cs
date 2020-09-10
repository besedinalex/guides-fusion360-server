using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos.Guides;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to process guides requests.</summary>
    public interface IGuidesService
    {
        /// <summary>Requests all public guides.</summary>
        /// <returns>Returns all public guides.</returns>
        Task<ServiceResponse<List<GetGuideDto>>> GetAllPublicGuides();

        /// <summary>Requests all private guides.</summary>
        /// <returns>Returns all available private guides.</returns>
        Task<ServiceResponse<List<GetGuideDto>>> GetAllHiddenGuides();

        /// <summary>Requests part guides of the guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns part guides.</returns>
        Task<ServiceResponse<List<GetPartGuideDto>>> GetPartGuides(int guideId);

        /// <summary>Requests file of the guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="filename">Name of the requested file.</param>
        /// <returns>Returns file.</returns>
        Task<ServiceResponse<FileContentResult>> GetGuideFile(int guideId, string filename);

        /// <summary>Requests guide owner data.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns guide owner data.</returns>
        Task<ServiceResponse<GetGuideOwnerDto>> GetGuideOwner(int guideId);

        /// <summary>Request to create guide.</summary>
        /// <param name="newGuide">Guide data.</param>
        /// <returns>Returns id of the guide.</returns>
        Task<ServiceResponse<int>> CreateGuide(AddGuideDto newGuide);

        /// <summary>Request to create new part guide.</summary>
        /// <param name="newPartGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide.</returns>
        Task<ServiceResponse<int>> CreatePartGuide(AddPartGuideDto newPartGuide);

        /// <summary>Request to upload model.</summary>
        /// <param name="model">Model data.</param>
        /// <returns>Returns id of the guide.</returns>
        Task<ServiceResponse<int>> UploadModel(AddGuideModelDto model);

        /// <summary>Request to make guide hidden or not.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="hidden">Parameter that chooses guide visibility. Accepts 'true' or 'false'.</param>
        /// <returns>Returns id of the guide.</returns>
        Task<ServiceResponse<int>> ChangeGuideVisibility(int guideId, string hidden);

        /// <summary>Request to update part guide.</summary>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <param name="updatedGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide.</returns>
        Task<ServiceResponse<int>> UpdatePartGuide(int partGuideId, UpdatePartGuideDto updatedGuide);

        /// <summary>Request to switch part guide's sort keys.</summary>
        /// <param name="partGuideId1">Id of the part guide to be switched.</param>
        /// <param name="partGuideId2">Id of the part guide to be switched.</param>
        Task<ServiceResponse<int>> SwitchPartGuides(int partGuideId1, int partGuideId2);

        /// <summary>Request to remove guide.</summary>
        /// <param name="guideId">Id of the guide to be removed.</param>
        Task<ServiceResponse<int>> RemoveGuide(int guideId);

        /// <summary>Request to remove part guide.</summary>
        /// <param name="partGuideId">Id of the part guide to be removed.</param>
        Task<ServiceResponse<int>> RemovePartGuide(int partGuideId);

        /// <summary>Checks if guide is available for edit and if it exists at all.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns 'true' if guide is public and service response.</returns>
        Task<Tuple<bool, ServiceResponse<T>, Guide>> GuideIsPublic<T>(int guideId);

        /// <summary>Checks if guide is editable with current access level.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="requiresAdminAccess">Checks if user should be editor or admin. False (editor) by default.</param>
        /// <typeparam name="T">Type of service response.</typeparam>
        /// <returns>Returns 'true' if guide is editable, service response and guide itself.</returns>
        Task<Tuple<bool, ServiceResponse<T>, Guide>> GuideIsEditable<T>(int guideId, bool requiresAdminAccess = false);
    }
}