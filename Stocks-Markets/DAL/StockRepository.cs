using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Stocks_Markets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Stocks_Markets.DAL
{
    public class StockRepository : IstockRepository
    {
        private readonly StockContext _db;
        private ILogger<StockRepository> _log;

        public StockRepository(StockContext db, ILogger<StockRepository> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<bool> LagreStock(Stock enstock)
        {
            try
            {               
                _db.stocks.Add(enstock);
                await _db.SaveChangesAsync();

                return true;
            }
            catch( Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<List<Stock>> HentAlleStock()
        {
            try
            {
                List<Stock> alleStocks = await _db.stocks.Select(s => new Stock
                {
                    SId = s.SId,
                    SelskapNavn = s.SelskapNavn,
                    SistePrise = s.SistePrise,
                    Tegn = s.Tegn,
                    AntallStock = s.AntallStock,
                    Endring = s.Endring,
                    volume = s.volume
                }).ToListAsync();
                return alleStocks;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }


        }

        public async Task<bool> SlettStock(int id)
        {
            try
            {
                Stock slettStock = await _db.stocks.FindAsync(id);
                _db.stocks.Remove(slettStock);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }

        }


        public async Task<Stock> HentEnStock(int Sid)
        {
            try
            {
                Stock s = await _db.stocks.FindAsync(Sid);
                return s;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

        public async Task<bool> EndreStock(Stock endreStock)
        {
            try
            {
                Stock enstock = await _db.stocks.FindAsync(endreStock.SId);

                enstock.AntallStock = endreStock.AntallStock;
                enstock.Endring = endreStock.Endring;
                enstock.SistePrise = endreStock.SistePrise;
                enstock.volume = endreStock.volume;
                enstock.SelskapNavn = endreStock.SelskapNavn;
                enstock.Tegn = endreStock.Tegn;
                _db.stocks.Update(enstock);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }
       
       public async Task<List<BrukerStocks>> HentAlleBrukerStocks(int BId)
        {
            try
            {
            List<BrukerStocks> bs = await _db.brukerstock.Where(S => S.BId==BId).ToListAsync();
               

                return bs;
                

            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                 return null;
            }
        }


        public async Task<Bruker> HentEnBruker(int id)
        {
            try
            {
                Brukere b = await _db.brukere.FindAsync(id);
                var Enbruker = new Bruker();
                Enbruker.brukernavn = b.brukernavn;
                Enbruker.BId = b.BId;
                Enbruker.BType = b.BType;
                return Enbruker;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return null;
            }
        }

       

        public async Task<bool> KjopeStocks(BrukerStock BS, int BId)
        {
            try
            {
                
                var NyRadKjoptSB = new BrukerStocks();
                NyRadKjoptSB.antallstock = BS.antallstock;
                NyRadKjoptSB.BId = BId;
                NyRadKjoptSB.SId = BS.SId;
                NyRadKjoptSB.DateAndTime = DateAndTime.Now.ToString();

                var funnetstock = await _db.stocks.FindAsync(BS.SId);
                if (BS.antallstock < funnetstock.AntallStock)           // Bruker skal ikke kjøpe alle stocks slik at det blir ikke tom eller negativ tall for stocks (dvs. retunere stocks til Stocks-tabelen)
                {
                    funnetstock.AntallStock = funnetstock.AntallStock - BS.antallstock;
                    _db.stocks.Update(funnetstock);                     // Updatere på antall stocks i Stock tablen
                    await _db.SaveChangesAsync();

                    _db.brukerstock.Add(NyRadKjoptSB);                  // legge kjopt stock i tabelen BrukerStocks med ID til Bruker som har kjøpt.
                    await _db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }

        public async Task<bool> SelgeStocks(BrukerStock BS)
        {
            try
            {
                BrukerStocks bs = await _db.brukerstock.FindAsync(BS.BSId);

                if (bs.antallstock == BS.antallstock)                                       // Hvis Bruker skal selge alle sine stocks
                {
                    var funnetstock = await _db.stocks.FindAsync(bs.SId);  
                    funnetstock.AntallStock = funnetstock.AntallStock + BS.antallstock;
                    _db.stocks.Update(funnetstock);                                        // oppdatere antall Stocks i Stock-tablen. dvs. retunere stocks til Stocks-tabelen
                    _db.brukerstock.Remove(bs);                                            // slette Stock fra BrukerStock tabelen
                    await _db.SaveChangesAsync();
                    return true;
                }
                else if (bs.antallstock > BS.antallstock) {                               // Hvis Bruker skal Selge en del av sine Stocks

                    bs.antallstock = bs.antallstock - BS.antallstock;               
                    var funnetstock = await _db.stocks.FindAsync(bs.SId);                
                    funnetstock.AntallStock = funnetstock.AntallStock + BS.antallstock;
                    _db.stocks.Update(funnetstock);                                      //oppdatere på antal Stocks i Stocks-tabelen
                    _db.brukerstock.Update(bs);                                         // oppdatere på antall Stocks til brukeren i BrukerStocks tabelen
                    await _db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;                                                     // Bruker skal ikke selge mer enn antall sine stocks
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }
        }
        public async Task<bool> RegistrereNyBruker(Bruker nybruker)
        {
            try
            {
                // Hvis ny bruker skal registere skal passord 'HASHES' og 'SALTES'
                Brukere b = new Brukere();
                b.BType = false;
                b.brukernavn = nybruker.brukernavn;                 
                string password = nybruker.password;
                byte[] salt = SaltingPass();
                byte[] hash = HashingPass(password, salt);
                b.password = hash;
                b.salt = salt;
                _db.brukere.Add(b);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return false;
            }

        }

        // Hashing av Pass
        public static byte[] HashingPass(string passord, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                                password: passord,
                                salt: salt,
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 1000,
            numBytesRequested: 32);
        }

        // Generat Salt
        public static byte[] SaltingPass()
        {
            var csp = new RNGCryptoServiceProvider();
            var salt = new byte[24];
            csp.GetBytes(salt);
            return salt;
        }

        public async Task<int> LoggInn(Bruker bruker)
        {
            try
            {
                Brukere funnetBruker = await _db.brukere.FirstOrDefaultAsync(b => b.brukernavn == bruker.brukernavn);
                // hash passordet til Bruker
                if (funnetBruker != null)
                {
                    byte[] hash = HashingPass(bruker.password, funnetBruker.salt);
                    // compare passord
                    bool ok = hash.SequenceEqual(funnetBruker.password);
                    if (ok)
                    {
                        return funnetBruker.BId;    // loggetinn
                    }
                    return -1;                      // bruker finnes men det er feil password
                }
                else
                {
                    return -3;                      // bruker er finnes ikke i DB må regsiteres
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation(ex.Message);
                return -2;
            }
        }
        

    }
}
