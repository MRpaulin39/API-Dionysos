using System;

namespace DIONYSOS.API.ViewModels
{
    public class ReadOrderSupplierViewModels
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime DateOrder { get; set; }
        public bool Receive { get; set; }

        //Info jointure
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class WriteOrderSupplierViewModels
    {
        public int Quantity { get; set; }
        public DateTime DateOrder { get; set; }
        public bool Receive { get; set; }
        public int ProductId { get; set; }
    }
}
