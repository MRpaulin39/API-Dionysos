namespace DIONYSOS.API.ViewModels
{
    public class ReadSupplierViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }

    public class WriteSupplierViewModels
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }
}
