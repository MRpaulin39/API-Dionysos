using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Data.Models
{
    public class APIUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Username { get; set; } //Nom d'utilisateur

        [MaxLength(255)]
        public string Email { get; set; } //Email

        [MaxLength(60)]
        public string Password { get; set; } //Mot de passe


    }
}
