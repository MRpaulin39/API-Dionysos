using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required(ErrorMessage = "BarCode is required")]
        [MaxLength(30)]
        public string BarCode { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; } //Add : Prix / unité HT

        [MaxLength(600)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string ImagePathFile { get; set; } //AAAA/MM/000000.JPG

        [MaxLength(20)]
        public string State { get; set; } //Etat d'un produit

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "QuantityMax is required")]
        public int QuantityMax { get; set; }

        public bool OrderAuto { get; set; }
        public int DayOrderAuto { get; set; }

        [MaxLength(6)]
        public string Site { get; set; }

        //Clé étrangère
        public virtual Supplier Supplier { get; set; } //Id du fournisseur

        public Product()
        {
            OrderAuto = false; //Par défaut, l'order Auto est à false
        }
    }
}
