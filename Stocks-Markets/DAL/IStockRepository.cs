using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks_Markets.Models;

namespace Stocks_Markets.DAL
{
    public interface IstockRepository
    {
        Task<bool> LagreStock(Stock nystock);
        Task<List<Stock>> HentAlleStock();
        Task<bool> SlettStock(int id);
        Task<Stock> HentEnStock(int id);
        Task<bool> EndreStock(Stock endreStock);
        Task<bool> RegistrereNyBruker(Bruker nybruker);
        Task<int> LoggInn(Bruker bruker);
        Task<Bruker> HentEnBruker(int id);
        Task<List<BrukerStocks>> HentAlleBrukerStocks(int BId);
        Task<bool> KjopeStocks(BrukerStock kjopstocks,int BId);
        Task<bool> SelgeStocks(BrukerStock selgeEnstocks);
        
    }
}
