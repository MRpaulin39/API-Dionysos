using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIONYSOS.API.Models
{
    public class OrderHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool Paid { get; set; }

        [Required]
        public DateTime DateOrder { get; set; }

        //Clé étrangère
        public virtual Customer Customer { get; set; }

        public OrderHeader()
        {
            //Met la valeur Payé à False par défaut
            Paid = false;
        }
    }
}
