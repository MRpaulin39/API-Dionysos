using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Models
{
    public class OrderSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime DateOrder { get; set; }

        [Required]
        public bool Receive { get; set; }

        //Clé étrangère
        public virtual Product Product { get; set; }

        public OrderSupplier()
        {
            Receive = false; //Par défaut, on n'a pas reçu le colis
        }
    }
}
