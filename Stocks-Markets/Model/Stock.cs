using System.ComponentModel.DataAnnotations;

namespace Stocks_Markets.Models
{
    public class Stock
    {
        [Key]
        public int SId { get; set; }
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ. \-]{2,20}$")]
        public string SelskapNavn { get; set; }
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ. \-]{2,20}$")]

        public string Tegn { get; set; }
        [RegularExpression(@"-?\d+(?:\.\d+)?")]
        public int AntallStock { get; set; }
       [RegularExpression(@"-?\d+(?:\.\d+)?")]
        public double SistePrise { get; set; }
        [RegularExpression(@"-?\d+(?:\.\d+)?")]
        public double Endring { get; set; }
        [RegularExpression(@"-?\d+(?:\.\d+)?")]
        public double volume { get; set; }
    }
}
