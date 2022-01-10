namespace DIONYSOS.API.ViewModels
{
    public class ReadOrderLineViewModels
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int QuantityServed { get; set; }

        //Info jointure
        public int ProductId { get; set; }
        public int OrderHeaderId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
    }

    public class WriteOrderLineViewModels
    {
        public int Quantity { get; set; }
        public int QuantityServed { get; set; }

        //Clé étrangère
        public int ProductId { get; set; }
        public int OrderHeaderId { get; set; }
    }
}
