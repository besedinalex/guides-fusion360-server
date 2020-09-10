using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos.ModelAnnotations
{
    public class AddModelAnnotationDto
    {
        [Required] public int GuideId { get; set; }

        [Required] public double X { get; set; }

        [Required] public double Y { get; set; }

        [Required] public double Z { get; set; }

        [Required] public string Name { get; set; }

        public string Text { get; set; }
    }
}