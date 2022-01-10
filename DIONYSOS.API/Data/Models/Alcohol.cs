using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Models
{
    public class Alcohol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(60)]
        public string GrapeVariety { get; set; } //Cépages = Variété

        [Required(ErrorMessage = "Vintage of production is required")]
        [MaxLength(5)]
        public string Vintage { get; set; } //Millésime
        public bool Organic { get; set; } //Bio

        [MaxLength(50)]
        public string Place { get; set; } //Région de production
        public int Keeping { get; set; } //Garde d'un vin

        [Required(ErrorMessage = "Color is required")]
        [MaxLength(15)]
        public string Color { get; set; } //Couleur du vin
        [MaxLength(70)]
        public string Pairing { get; set; } //Accord Met-vin

        //Clé étrangère
        public virtual Product Product { get; set; } //Id du produits

        public Alcohol()
        {
            Organic = false; //Par défaut, un vin n'est pas bio
        }
    }
}
