using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Data.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool Gender { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string Adress { get; set; }

        [MaxLength(12)]
        public int FidCard { get; set; }

        [MaxLength(12)]
        public string Phone { get; set; }

        [MaxLength(120)]
        public string Mail { get; set; }
        [MaxLength(50)]
        public string City { get; set; } //Add : Ville
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(255)]
        public string Password { get; set; }


    }
}
