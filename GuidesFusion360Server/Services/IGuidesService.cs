using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Dtos;
using GuidesFusion360Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuidesFusion360Server.Services
{
    /// <summary>Service to work with guides data.</summary>
    public interface IGuidesService
    {
        /// <summary>Requests all public guides.</summary>
        /// <returns>Returns all public guides.</returns>
        Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllGuides();

        /// <summary>Requests all private guides.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <returns>Returns all available private guides.</returns>
        Task<ServiceResponse<List<GetAllGuidesDto>>> GetAllHiddenGuides(int userId);

        /// <summary>Requests part guides of public guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns part guides and http code.</returns>
        Task<Tuple<ServiceResponse<List<GetAllPartGuidesDto>>, int>> GetAllPublicPartGuides(int guideId);

        /// <summary>Requests part guides of private guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <returns>Returns part guides.</returns>
        Task<ServiceResponse<List<GetAllPartGuidesDto>>> GetAllPrivatePartGuides(int guideId, int userId);
        
        /// <summary>Requests file of public guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="filename">Name of the requested file.</param>
        /// <returns>Returns file and http code.</returns>
        Task<Tuple<ServiceResponse<FileContentResult>, int>> GetPublicGuideFile(int guideId, string filename);
        
        /// <summary>Requests file of public guide.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="filename">Name of the requested file.</param>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <returns>Returns file.</returns>
        Task<ServiceResponse<FileContentResult>> GetPrivateGuideFile(int guideId, string filename, int userId);

        /// <summary>Request to create guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="newGuide">Guide data.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> CreateNewGuide(int userId, AddNewGuideDto newGuide);

        /// <summary>Request to create new part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="newPartGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide and http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> CreateNewPartGuide(int userId, AddNewPartGuideDto newPartGuide);

        /// <summary>Request to upload model.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="newModel">Model data.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> UploadModel(int userId, AddNewGuideModelDto newModel);

        /// <summary>Request to make guide hidden or not.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="guideId">Id of the guide.</param>
        /// <param name="hidden">Parameter that chooses guide visibility. Accepts 'true' or 'false'.</param>
        /// <returns>Returns id of the guide and http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> ChangeGuideVisibility(int userId, int guideId, string hidden);

        /// <summary>Request to update part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId">Id of the part guide.</param>
        /// <param name="updatedGuide">Part guide data.</param>
        /// <returns>Returns id of the part guide and http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> UpdatePartGuide(int userId, int partGuideId,
            UpdatePartGuideDto updatedGuide);

        /// <summary>Request to switch part guide's sort keys.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId1">Id of the part guide to be switched.</param>
        /// <param name="partGuideId2">Id of the part guide to be switched.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> SwitchPartGuides(int userId, int partGuideId1, int partGuideId2);

        /// <summary>Request to remove guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="guideId">Id of the guide to be removed.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> RemoveGuide(int userId, int guideId);
        
        /// <summary>Request to remove part guide.</summary>
        /// <param name="userId">Id of the user who makes the request.</param>
        /// <param name="partGuideId">Id of the part guide to be removed.</param>
        /// <returns>Returns http code.</returns>
        Task<Tuple<ServiceResponse<int>, int>> RemovePartGuide(int userId, int partGuideId);
    }
}
