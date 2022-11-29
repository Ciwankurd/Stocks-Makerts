using Microsoft.VisualBasic;
using Stocks_Markets.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stocks_Markets.Models
{
    public class BrukerStocks
    {
        [Key]
        public int BSId { get; set; }
        public int antallstock { get; set; }
        public string DateAndTime { get; set; }
        public int BId { get; set; }
        public int SId { get; set; }
        [ForeignKey("BId")]
        virtual public Brukere bruker { get; set; }
        [ForeignKey("SId")]
        virtual public Stock stock { get; set; }
    }
}
