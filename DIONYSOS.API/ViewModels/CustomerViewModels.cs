using System.ComponentModel.DataAnnotations;

namespace DIONYSOS.API.ViewModels
{
    public class ReadCustomerViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public string FirstName { get; set; }
        public string Adress { get; set; }
        public int FidCard { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Password { get; set; }
    }

    public class WriteCustomerViewModels
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public bool Gender { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(80)]
        public string Adress { get; set; }
        public int FidCard { get; set; }
        public string Phone { get; set; }
        [MaxLength(120)]
        public string Mail { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(255)]
        public string Password { get; set; }
    }
}
