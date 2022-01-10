using System;

namespace DIONYSOS.API.ViewModels
{
    public class ReadOrderHeaderViewModels
    {
        public int Id { get; set; }
        public bool Paid { get; set; }
        public DateTime DateOrder { get; set; }

        //Info jointure
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class WriteOrderHeaderViewModels
    {
        public bool Paid { get; set; }
        public DateTime DateOrder { get; set; }

        //Clé étrangère
        public int CustomerId { get; set; }
    }
}
