using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Models
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Adress is required")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } //Add : Ville

        [Required(ErrorMessage = "ZipCode is required")]
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }
}
