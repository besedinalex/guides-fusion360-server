using System.Collections.Generic;
using System.Threading.Tasks;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data.Repositories
{
    public interface IModelAnnotationsRepository
    {
        /// <summary>Gets annotations from db.</summary>
        /// <param name="annotationId">Id of the annotation.</param>
        /// <returns>Returns all model annotations.</returns>
        public Task<ModelAnnotation> GetAnnotation(int annotationId);

        /// <summary>Gets annotations from db.</summary>
        /// <param name="guideId">Id of the guide.</param>
        /// <returns>Returns all model annotations.</returns>
        public Task<List<ModelAnnotation>> GetAnnotations(int guideId);

        /// <summary>Adds annotation to db.</summary>
        /// <param name="annotation">Annotation data.</param>
        /// <returns>Id of the new annotation.</returns>
        public Task<int> AddAnnotation(ModelAnnotation annotation);

        /// <summary>Removes annotation from db.</summary>
        /// <param name="annotation">Annotation data.</param>
        public Task DeleteAnnotation(ModelAnnotation annotation);

        /// <summary>Removes annotations from db.</summary>
        /// <param name="annotations">Annotations data.</param>
        public Task DeleteAnnotations(List<ModelAnnotation> annotations);
    }
}