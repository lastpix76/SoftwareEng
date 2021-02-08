using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiDriverServiceWPF.DataTypes
{
    [Table("Clients")]
    public class TaxiClient
    {
        [Key]
        public int ClientId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(20)]
        [Required]
        public string PhoneNumber { get; set; }

        public TaxiClient()
        {
        }
        public TaxiClient(string _name, string _phoneNumber)
        {
            Name = _name;
            PhoneNumber = _phoneNumber;
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", ClientId, Name, PhoneNumber);
        }
    }
}
