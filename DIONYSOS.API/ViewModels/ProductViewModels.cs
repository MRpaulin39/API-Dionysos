using System.ComponentModel.DataAnnotations;

namespace DIONYSOS.API.ViewModels
{
    public class ReadProductViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public double Price { get; set; } //Add : Prix / unité HT
        public string Description { get; set; }
        public string ImagePathFile { get; set; } //AAAA/MM/000000.JPG
        public string State { get; set; } //Etat d'un produit
        public int Quantity { get; set; }
        public int QuantityMax { get; set; }
        public bool OrderAuto { get; set; }
        public int DayOrderAuto { get; set; }
        public string Site { get; set; }

        //Info jointure
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
    }

    public class WriteProductViewModels
    {
        [MaxLength(80)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string BarCode { get; set; }
        public double Price { get; set; } //Add : Prix / unité HT
        [MaxLength(600)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string ImagePathFile { get; set; } //AAAA/MM/000000.JPG
        [MaxLength(20)]
        public string State { get; set; } //Etat d'un produit
        public int Quantity { get; set; }
        public int QuantityMax { get; set; }
        public bool OrderAuto { get; set; }
        public int DayOrderAuto { get; set; }
        [MaxLength(6)]
        public string Site { get; set; }
        public int SupplierId { get; set; } //ID du fournisseur
    }
}
