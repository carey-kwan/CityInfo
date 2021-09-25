using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{

    //Data annotations are for validating input for models.  This manual manual checks 
    //with code does not have to be done like checking to see if something is null
    public class PointOfInterestForCreationDto
    {
        
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

    }
}
