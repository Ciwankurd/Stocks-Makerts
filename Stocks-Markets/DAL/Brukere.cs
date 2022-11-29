
using System.ComponentModel.DataAnnotations;
namespace Stocks_Markets.DAL
{
    public class Brukere
    {
        [Key]
        public int BId { get; set; }
        public string brukernavn { get; set; }
        public bool BType { get; set; }
        public byte[] password { get; set; }
        public byte[] salt { get; set; }
    }
}
