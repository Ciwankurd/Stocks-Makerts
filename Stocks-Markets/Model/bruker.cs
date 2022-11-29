
using System.ComponentModel.DataAnnotations;

namespace Stocks_Markets.Models
{
    public class Bruker
    {
        [Key]
        public int BId { get; set; }

        [RegularExpression(@"^[a-zA-ZæøåÆØÅ. \-]{2,20}$")]
        public string brukernavn { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}$")]
        public string password { get; set; }
        public bool BType { get; set; }

    }
}
