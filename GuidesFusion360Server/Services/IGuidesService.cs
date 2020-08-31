using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to process guides requests.</summary>
    public interface IGuidesService
    {
        /// <summary>Requests all public guides.</summary>
        /// <returns>Returns all public guides.</returns>
        Task<ServiceResponseModel<List<GetGuidesDto>>> GetAllPublicGuides();

        /// <summary>Requests all private guides.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <returns>Returns all available private guides.</returns>
        Task<ServiceResponseModel<List<GetGuidesDto>>> GetAllHiddenGuides(int userId);

        /// <summary>Requests part guides of the guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="userId">Id of the user.</param>
        /// <returns>Returns part guides and http code.</returns>
        Task<Tuple<ServiceResponseModel<List<GetPartGuidesDto>>, int>> GetPartGuides(int guideId, int userId);

        /// <summary>Requests file of the guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="filename">Name of the requested file.</param>
        /// <param name="userId">Id of the user.</param>
        /// <returns>Returns file and http code.</returns>
        Task<Tuple<ServiceResponseModel<FileContentResult>, int>> GetGuideFile(int guideId, string filename,
            int userId);

        /// <summary>Request to create guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="newGuide">Guide data.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> CreateGuide(int userId, AddGuideDto newGuide);

        /// <summary>Request to create new part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="newPartGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide and http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> CreatePartGuide(int userId, AddPartGuideDto newPartGuide);

        /// <summary>Request to upload model.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="model">Model data.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> UploadModel(int userId, AddGuideModelDto model);

        /// <summary>Request to make guide hidden or not.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="hidden">Parameter that chooses guide visibility. Accepts 'true' or 'false'.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> ChangeGuideVisibility(int userId, int guideId, string hidden);

        /// <summary>Request to update part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <param name="updatedGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide and http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> UpdatePartGuide(int userId, int partGuideId,
            UpdatePartGuideDto updatedGuide);

        /// <summary>Request to switch part guide's sort keys.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId1">Id of the part guide to be switched.</param>
        /// <param name="partGuideId2">Id of the part guide to be switched.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> SwitchPartGuides(int userId, int partGuideId1, int partGuideId2);

        /// <summary>Request to remove guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="guideId">Id of the guide to be removed.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> RemoveGuide(int userId, int guideId);

        /// <summary>Request to remove part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId">Id of the part guide to be removed.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponseModel<int>, int>> RemovePartGuide(int userId, int partGuideId);
    }
}