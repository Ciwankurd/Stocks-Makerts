using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stocks_Markets.Models
{
    public class BrukerStock
    {
        [Key]
        public int BSId { get; set; }
        public int antallstock { get; set; }
        public int SId { get; set; }
        public int BId { get; set; }
        
    }
}
