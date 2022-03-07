using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Data.Models
{
    public class OrderLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int QuantityServed { get; set; }

        //Clé étrangère
        public virtual Product Product { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }

    }
}
