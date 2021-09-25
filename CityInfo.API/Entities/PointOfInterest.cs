using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("CityId")]  //can do this is you want to go against convention and not have the FK be the name of the class and "Id"
        public City City { get; set; }  //relationship between City and PointOfInterest tables
                                        //will be created when property is not a scalar type
                                        //note: this is a 1 to 1 relationship as it is just one object instance
        public int CityId { get; set; } //it is not required to define the foreign key on the dependent class
                                        //but it is recommended
        
    }
}
