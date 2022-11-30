using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stocks_Markets.DAL;
using Stocks_Markets.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks_Markets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        public readonly IstockRepository _db;
        private ILogger<StockController> _log;
        private const string _sjekkLogginn = "loggetInn";       // Key for sission
        private const string _LagreBId = "BId";                 // Key for lagre Bruker ID
        

        public StockController(IstockRepository db, ILogger<StockController> log)
        {
            _db = db;
            _log = log;
        }

        //------------------Lagre Stock--------------------------

        [HttpPost("lagreStock")]
        public async Task<ActionResult> lagreStock(Stock nystock)
        {
            if (!string.Equals(HttpContext.Session.GetString(_sjekkLogginn), "AdminLoggetInn"))         //sjekk om session er tom eller Null
            {
                return Unauthorized("ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool retOK = await _db.LagreStock(nystock);
                if (!retOK)
                {
                    _log.LogInformation("Stock ikke lagret!");
                    return BadRequest("Stock ikke lagret!");
                }
                return Ok("");
            }
            _log.LogInformation("Valdering Feil!");
            return BadRequest("Vadering Feil på server!");
        }

        // ------------------Endre Stock-----------------------------

        [HttpPut("endreStock")]
        public async Task<ActionResult> endreStock(Stock endrestock)
        {
    
            if (!string.Equals(HttpContext.Session.GetString(_sjekkLogginn), "AdminLoggetInn")) // hivs denne logget inn ikke Admin returen Unauthorized.
            {
                return Unauthorized("ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool retOK = await _db.EndreStock(endrestock);
                if (!retOK)
                {
                    _log.LogInformation("Stock ikke Endret!");
                    return BadRequest("Stock ikke Edret!");
                }
              
                return Ok("");
            }
            _log.LogInformation("Valdering Feil!");
            return BadRequest("Vadering Feil på server!");

        }

        //------------------Slette En Stock---------------

        [HttpDelete("slettStock{SId}")]
        public async Task<ActionResult> slettStock(int SId)
        {
            
            if (!string.Equals(HttpContext.Session.GetString(_sjekkLogginn), "AdminLoggetInn"))
            {
                return Unauthorized("ikke logget inn");
            }
                bool retOK = await _db.SlettStock(SId);
                if (!retOK)
                {
                    _log.LogInformation("Stock ikke Slettet!");
                    return BadRequest("Stock ikke Slettet!");
                }
                return Ok("");
        }

        //-------------Hente Alle Stocks----------------------

        [HttpGet("hentAlleStocks")]
        public async Task<ActionResult> hentAlleStocks()
        {
            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sjekkLogginn)))
            {
                return Unauthorized("ikke logget inn");
            }
            List<Stock> alleStocks = await _db.HentAlleStock();
            if (alleStocks == null)
            {
                return NotFound("Feil på server");
            }
            return Ok(alleStocks);
        }

        // --------------Hente En Stock BY ID----------------

        [HttpGet("hentEnStock{Sid}")]
        public async Task<ActionResult> hentEnStock(int Sid)
        {
           
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sjekkLogginn)))
            {
                return Unauthorized("ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                Stock stock = await _db.HentEnStock(Sid);
                if (stock == null)
                {
                    _log.LogInformation("har ikke funnet stock");
                    return NotFound("har ikke funnet stock");
                }
                return Ok(stock);
            }
            _log.LogInformation("Feil i inputvalidering");
            return BadRequest("Feil i inputvalidering på server");
        }

        // -------------- Registere En Ny Bruker-----------------

        [HttpPost("registereBruker")]
        public async Task<ActionResult> registereBruker(Bruker nybruker)
        {
            
            if (ModelState.IsValid)
            {
                bool retOK = await _db.RegistrereNyBruker(nybruker);
            if (!retOK)
                {
                    _log.LogInformation("Bruker har ikke registert!");
                    return BadRequest("Bruker har ikke registert!");
                }
                return Ok("");
            }
                _log.LogInformation("Valdering Feil!");
                return BadRequest("Vadering Feil på server!");
        }

        // -------------Logginn SOM Admin/Bruker ----------------

        [HttpPost("loggin")]
        public async Task<ActionResult> logginn(Bruker b)
        {
            // sjekk validtion
            if (ModelState.IsValid)
            {
                int BId = await _db.LoggInn(b);
                if (BId >= 0)
                {
                    Bruker funnetBruker = await _db.HentEnBruker(BId);

                    if (funnetBruker.BType)
                    {
                        HttpContext.Session.SetString(_sjekkLogginn, "AdminLoggetInn");     //set sisson for Admin logget inn for key
                        HttpContext.Session.SetInt32(_LagreBId, BId);
                        return Ok(BId);
                    }

                        HttpContext.Session.SetString(_sjekkLogginn, "BrukerLoggetInn");    //set sisson for Bruker logget inn for key
                        HttpContext.Session.SetInt32(_LagreBId, BId);
                        return Ok(BId);
                    
                }
                else if (BId == -1)                                                        // antatt at -1 indeker feil password
                {
                    _log.LogInformation("password er feil");
                    HttpContext.Session.SetString(_sjekkLogginn, "");                     // Tom value for Key
                    return Unauthorized("feil passowrd");
                }
                else if (BId == -2)                                                       // -2 Exeption feil.
                {
                    _log.LogInformation("feil på server!");
                    HttpContext.Session.SetString(_sjekkLogginn, ""); 
                    return BadRequest("feil på server! ");
                }
                _log.LogInformation("Bruker er ikke registert");                         // -3 antat at Bruker finnes ikke DB dvs. ikke registeret.
                        HttpContext.Session.SetString(_sjekkLogginn, ""); 
                        return Unauthorized("Bruker er ikke registert");

            }
            _log.LogInformation("ValideringsFeil for Brukernavn eller Passord");        // Validtion feil fra server side
            return BadRequest("Feil inputValidering fra Server!");
        }

        // ---------------Loggut -------------
        [HttpGet("LoggUt")]
        public void LoggUt()
        {
            HttpContext.Session.SetString(_sjekkLogginn, ""); 
        }

        // ------------ Hent En Bruker -----------

        [HttpGet("hentEnBruker")]
        public async Task<ActionResult> hentEnBruker()
        {
            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sjekkLogginn)))
            {
                return Unauthorized("ikke logget inn");
            }
            
            int BId = (int)HttpContext.Session.GetInt32(_LagreBId);                 // hent BId til bruker/Admin fra Session

            if (ModelState.IsValid)
            {
                Bruker bruker = await _db.HentEnBruker(BId);
                if (bruker == null)
                {
                    _log.LogInformation("har ikke funnet bruker");
                    return NotFound("har ikke funnet bruker");
                }
                return Ok(bruker);
            }
            _log.LogInformation("Feil i inputvalidering");
            return BadRequest("Feil i inputvalidering på server");
        }

        // ---------------- Kjøpe Ny Stock ------------------
        [HttpPost("kjopestocks")]
        public async Task<ActionResult> kjopestocks(BrukerStock kjopeNystocks)
        {
            
            if (!string.Equals(HttpContext.Session.GetString(_sjekkLogginn), "BrukerLoggetInn"))
            {
                return Unauthorized("ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                int BId = (int)HttpContext.Session.GetInt32(_LagreBId);
                bool retOK = await _db.KjopeStocks(kjopeNystocks,BId);
                if (!retOK)
                {
                    _log.LogInformation("stocks ble ikke Kjøpt!");
                    return BadRequest("stocks ble ikke kjøpt!");
                }
                return Ok("");
            }
            _log.LogInformation("Valdering Feil!");
            return BadRequest("Vadering Feil på server!");
        }

        // ------------------Selge Stocks----------------
        [HttpPut("slegeStocks")]
        public async Task<ActionResult> slegeStocks(BrukerStock SelgeEnStocks)
        {
            
            if (!string.Equals(HttpContext.Session.GetString(_sjekkLogginn), "BrukerLoggetInn"))
            {
                return Unauthorized("ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool retOK =  await _db.SelgeStocks(SelgeEnStocks);
                if (!retOK)
                {
                    _log.LogInformation("Stocks har ikke Solgt!");
                    return BadRequest("Stocks har ikke solgt !");
                }
                return Ok("");
            }
            _log.LogInformation("Valdering Feil!");
            return BadRequest("Vadering Feil på server!");
        }

        // ---------------Hent Stocks Til Bruker Som loggetinn ---------
        [HttpGet("HentBrukerStocks")]
        public async Task<ActionResult> HentBrukerStocks()
        {
            int BId = (int)HttpContext.Session.GetInt32(_LagreBId);

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sjekkLogginn)))
            {
                return Unauthorized("ikke logget inn");
            }
            List<BrukerStocks> alleBrukerStock = await _db.HentAlleBrukerStocks(BId);

            return Ok(alleBrukerStock);
        }
    }
}
