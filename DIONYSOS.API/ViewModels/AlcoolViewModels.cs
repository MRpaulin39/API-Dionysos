using System.ComponentModel.DataAnnotations;

namespace DIONYSOS.API.ViewModels
{
    public class ReadAlcoholViewModels
    {
        public int Id { get; set; }
        public string GrapeVariety { get; set; } //Cépages = Variété
        public string Vintage { get; set; } //Millésime
        public bool Organic { get; set; } //Bio
        public string Place { get; set; } //Région de production
        public int Keeping { get; set; } //Garde d'un vin
        public string Color { get; set; } //Couleur du vin
        public string Pairing { get; set; } //Accord Met-vin

        //Info Jointure
        public int ProductId { get; set; } //Id du produits
        public string ProductName { get; set; }
        public string ProductBarCode { get; set; }
        public double ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductState { get; set; }
        public int ProductQuantity { get; set; }
        public int ProductQuantityMax { get; set; }
        public string ProductSite { get; set; }
    }

    public class WriteAlcoholViewModels
    {
        [MaxLength(60)]
        public string GrapeVariety { get; set; } //Cépages = Variété
        [MaxLength(5)]
        public string Vintage { get; set; } //Millésime
        public bool Organic { get; set; } //Bio
        [MaxLength(50)]
        public string Place { get; set; } //Région de production
        public int Keeping { get; set; } //Garde d'un vin
        [MaxLength(15)]
        public string Color { get; set; } //Couleur du vin
        [MaxLength(70)]
        public string Pairing { get; set; } //Accord Met-vin

        //Clé étrangère
        public int ProductId { get; set; } //Id du produits
    }
}
