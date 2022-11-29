using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Stocks_Markets.Controllers;
using Stocks_Markets.DAL;
using Stocks_Markets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Test_Stock_project
{
    public class StocksMarketsTest
    {
        private const string _loggetInn = "loggetInn";
        private const string _AdminLoggetInn = "AdminLoggetInn";
        private const string _LagreBId = "BId";
        private const string _ikkeLoggetInn = "";

        private readonly Mock<IstockRepository> mockrepo = new Mock<IstockRepository>();
        private readonly Mock<ILogger<StockController>> mockController = new Mock<ILogger<StockController>>();

        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();

        [Fact]
        public async Task HentAlleLoggetInnOK()
        {
            // Arrange
            var Stock1 = new Stock
            {
                SId = 1,
                Tegn = "SVA",
                AntallStock = 15,
                SistePrise = 2.3,
                Endring = 1.1,
                volume = 2.3

            };
            var Stock2 = new Stock
            {
                SId = 2,
                Tegn = "DNB",
                AntallStock = 15,
                SistePrise = 2.3,
                Endring = 1.1,
                volume = 2.3

            };
            var Stock3 = new Stock
            {
                SId = 1231,
                Tegn = "DNB",
                AntallStock = 15,
                SistePrise = 2.3,
                Endring = 1.1,
                volume = 2.3

            };

            var stocklist = new List<Stock>();
            stocklist.Add(Stock1);
            stocklist.Add(Stock2);
            stocklist.Add(Stock3);

            mockrepo.Setup(k => k.HentAlleStock()).ReturnsAsync(stocklist);

            var stoccontoller = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stoccontoller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stoccontoller.hentAlleStocks() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<List<Stock>>((List<Stock>)resultat.Value, stocklist);
        }

        [Fact]
        public async Task HentAlleIkkeLoggetInn()
        {
            // Arrange

            

            mockrepo.Setup(k => k.HentAlleStock()).ReturnsAsync(It.IsAny<List<Stock>>());

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentAlleStocks() as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleIkkeFunnetIkke()
        {
            // Arrange

            mockrepo.Setup(k => k.HentAlleStock()).ReturnsAsync(It.IsAny<List<Stock>>());

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentAlleStocks() as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Feil på server", resultat.Value);
        }


        [Fact]
        public async Task LagreLoggetInnOK()
        {

            // Kan unngå denne med It.IsAny<Kunde>
            // var kunde1 = new Kunde {Id = 1,Fornavn = "Per",Etternavn = "Hansen",Adresse = "Askerveien 82",
            //                      Postnr = "1370",Poststed = "Asker"};


            // Arrange

            mockrepo.Setup(k => k.LagreStock(It.IsAny<Stock>())).ReturnsAsync(true);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.lagreStock(It.IsAny<Stock>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("", resultat.Value);
        }


        [Fact]
        public async Task LagreLoggetInnIkkeOK()
        {

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "Ikke Admin";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.lagreStock(It.IsAny<Stock>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task LagreStockLoggetinnIkkeOk()
        {
            mockrepo.Setup(k => k.LagreStock(It.IsAny<Stock>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.lagreStock(It.IsAny<Stock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Stock ikke lagret!", result.Value);
        }

        [Fact]
        public async Task LagreStockLoggetinnFeilModell()
        {
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("SelskapNavn", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.lagreStock(It.IsAny<Stock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Vadering Feil på server!", result.Value);

        }
        [Fact]
        public async Task SlettStockLoggetInnok()
        {
            mockrepo.Setup(k => k.SlettStock(It.IsAny<int>())).ReturnsAsync(true);
            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.slettStock(It.IsAny<int>()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("", result.Value);

        }
        [Fact]
        public async Task SlettLoggetOKIkkeSlettet()
        {
            // Arrange

            mockrepo.Setup(k => k.SlettStock(It.IsAny<int>())).ReturnsAsync(false);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.slettStock(It.IsAny<int>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Stock ikke Slettet!", resultat.Value);
        }
        [Fact]
        public async Task SlettLoggetIkkeOK()
        {
            // Arrange

            mockrepo.Setup(k => k.SlettStock(It.IsAny<int>())).ReturnsAsync(true);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "Ikke Admin";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.slettStock(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentenStockIkkeLoggetinn()
        {
            mockrepo.Setup(k => k.HentEnStock(It.IsAny<int>())).ReturnsAsync(() => null);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.hentEnStock(It.IsAny<int>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value); //ikke logget inn 
        }

        [Fact]
        public async Task HentEnstockLoggetinnOk()
        {
            var stock = new Stock
            {
                SId = 1,
                SelskapNavn = "DS",
                Tegn = "DNB",
                AntallStock = 15,
                SistePrise = 2.3,
                Endring = 1.1,
                volume = 2.3
            };

            mockrepo.Setup(k => k.HentEnStock(1)).ReturnsAsync(stock);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentEnStock(1) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Stock>(stock, (Stock)resultat.Value);
        }

        [Fact]
        public async Task HentEnstockLoggetIkkefin()
        {

            mockrepo.Setup(k => k.HentEnStock(1)).ReturnsAsync(It.IsAny<Stock>());

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentEnStock(1) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("har ikke funnet stock", resultat.Value);
        }

        [Fact]
        public async Task HentEnstockLoggetFeilInput()
        {
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("AAAAAAA", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentEnStock(1) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task HentEnstockikkeLogget()
        {
            mockrepo.Setup(k => k.HentEnStock(1)).ReturnsAsync(It.IsAny<Stock>());

            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await stockController.hentEnStock(1) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("ikke logget inn", resultat.Value);
        }


        [Fact]
        public async Task HentEnBrukerLoggetinnOK()
        {
            var bruker = new Bruker { BId = 1, brukernavn = "Admin", password = "Admin11" };

            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(bruker);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.hentEnBruker() as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<Bruker>(bruker, (Bruker)result.Value);
        }

        [Fact]
        public async Task HentEnBrukerIkkelogget()
        {

            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(It.IsAny<Bruker>);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.hentEnBruker() as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value);
        }


        [Fact]
        public async Task HentEnBrukerIkkefinnes()
        {
            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(() => null);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.hentEnBruker() as NotFoundObjectResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("har ikke funnet bruker", result.Value);
        }

        [Fact]
        public async Task HentEnBrukerValidFeil()
        {
            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(() => null);
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("AAAAAAA", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _loggetInn;
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.hentEnBruker() as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", result.Value);
        }
        [Fact]
        public async Task HentBrukrtStockIkkeLoggetInn()
        {
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockSession.SetInt32(_LagreBId, 2);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.HentBrukerStocks() as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value);
        }
        [Fact]
        public async Task HentBrukrtStockOK()
        {
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            mockSession[_loggetInn] = _loggetInn;
            mockSession.SetInt32(_LagreBId, 2);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;
            
            var result = await stockController.HentBrukerStocks() as OkObjectResult;
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            
        }

        [Fact]
        public async Task kjopeKundeLoggetInnIkkeok()
        {
            mockrepo.Setup(k => k.KjopeStocks(It.IsAny<BrukerStock>(), It.IsAny<int>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockSession.SetInt32(_LagreBId, 2);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.kjopestocks(It.IsAny<BrukerStock>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value);
        }

        [Fact]
        public async Task kjopeKundeLoggetInnIkkeKjøpt()
        {
            mockrepo.Setup(k => k.KjopeStocks(It.IsAny<BrukerStock>(), It.IsAny<int>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.kjopestocks(It.IsAny<BrukerStock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("stocks ble ikke kjøpt!", result.Value);
        }

        [Fact]
        public async Task kjopeKundeLoggetInnFeilPåServer()
        {
            // mockrepo.Setup(k => k.KjopeStocks(It.IsAny<BrukerStock>(), It.IsAny<int>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("AAAAAAA", "Vadering Feil på server!");

            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.kjopestocks(It.IsAny<BrukerStock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Vadering Feil på server!", result.Value);
        }

        [Fact]
        public async Task kjopeloggetinnOk()
        {
            mockrepo.Setup(k => k.KjopeStocks(It.IsAny<BrukerStock>(), It.IsAny<int>())).ReturnsAsync(true);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockSession.SetInt32(_LagreBId, 1);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.kjopestocks(It.IsAny<BrukerStock>()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("", result.Value);
        }

        [Fact]
        public async Task SelgeKundeLoggetInnIkkeok()
        {
            //mockrepo.Setup(k => k.SelgeStocks(It.IsAny<BrukerStock>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "AAA";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.slegeStocks(It.IsAny<BrukerStock>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value);
        }

        [Fact]
        public async Task SelgeKundeLoggetInnok()
        {
            mockrepo.Setup(k => k.SelgeStocks(It.IsAny<BrukerStock>())).ReturnsAsync(true);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.slegeStocks(It.IsAny<BrukerStock>()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("", result.Value);
        }

        [Fact]
        public async Task SelgeKundeLoggetInnIKkeSolgt()
        {
            mockrepo.Setup(k => k.SelgeStocks(It.IsAny<BrukerStock>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.slegeStocks(It.IsAny<BrukerStock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Stocks har ikke solgt !", result.Value);
        }

        [Fact]
        public async Task SelgeKundeLoggetInnFeilValid()
        {
            //mockrepo.Setup(k => k.SelgeStocks(It.IsAny<BrukerStock>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("AAAAAAA", "Vadering Feil på server!");


            mockSession[_loggetInn] = "BrukerLoggetInn";
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.slegeStocks(It.IsAny<BrukerStock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Vadering Feil på server!", result.Value);
        }



        [Fact]
        public async Task EndreStockIkkelagret()

        {
            mockrepo.Setup(k => k.EndreStock(It.IsAny<Stock>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.endreStock(It.IsAny<Stock>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Stock ikke Edret!", result.Value);
        }

        [Fact]
        public async Task EndreStockIkkeLogget()

        {
            //mockrepo.Setup(k => k.EndreStock(It.IsAny<Stock>())).ReturnsAsync(false);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = "Ikke Admin";

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;
            stockController.ModelState.AddModelError("AAAAAAA", "ikke logget inn");

            var result = await stockController.endreStock(It.IsAny<Stock>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("ikke logget inn", result.Value);
        }

        [Fact]
        public async Task EndreStocLogginFeilValidering()
        {

            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("ddfvdfvfdb vdfgfvd", "Vadering Feil på server!");

            mockSession[_loggetInn] = _AdminLoggetInn
                ;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.endreStock(It.IsAny<Stock>()) as BadRequestObjectResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Vadering Feil på server!", result.Value);
        }

        [Fact]
        public async Task EndreStocLoggetinnOK()
        {

            mockrepo.Setup(k => k.EndreStock(It.IsAny<Stock>())).ReturnsAsync(true);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.endreStock(It.IsAny<Stock>()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("", result.Value);
        }



        [Fact]
        public async Task Resgistrereok()
        {
            mockrepo.Setup(k => k.RegistrereNyBruker(It.IsAny<Bruker>())).ReturnsAsync(true);
            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.registereBruker(It.IsAny<Bruker>()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("", result.Value);
        }
        [Fact]
        public async Task NyRegistrereFeil()
        {
            mockrepo.Setup(k => k.RegistrereNyBruker(It.IsAny<Bruker>())).ReturnsAsync(false);
            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.registereBruker(It.IsAny<Bruker>()) as BadRequestObjectResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Bruker har ikke registert!", result.Value);
        }

        [Fact]
        public async Task RegistrerInpytFeil()
        {
            var stockController = new StockController(mockrepo.Object, mockController.Object);
            stockController.ModelState.AddModelError("SelskapNavn", "Feil i inputvalidering på server");


            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.registereBruker(It.IsAny<Bruker>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Vadering Feil på server!", result.Value);
        }



        [Fact]
        public async Task LogginnExseptionFeil()
        {

            mockrepo.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(-2);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.logginn(It.IsAny<Bruker>()) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("feil på server! ", result.Value);
        }

        [Fact]
        public async Task LogginnTilIkkeRegistrertBruker()
        {

            mockrepo.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(-3);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.logginn(It.IsAny<Bruker>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("Bruker er ikke registert", result.Value);
        }

        [Fact]
        public async Task LogginnFeilPass()
        {

            mockrepo.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(-1);
            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.logginn(It.IsAny<Bruker>()) as UnauthorizedObjectResult;

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("feil passowrd", result.Value);
        }

        [Fact]
        public void Loggut()
        {

            var stockController = new StockController(mockrepo.Object, mockController.Object);


            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            stockController.LoggUt();

            Assert.Equal(_ikkeLoggetInn, mockSession[_loggetInn]);

        }

        [Fact]
        public async Task LogginnBrukerOk()
        {
            var bruker = new Bruker();
            bruker.brukernavn = "Ciwan";
            bruker.password = "Kurd111@Kn";
            bruker.BType = false; // dette er bruker
            bruker.BId = 2;


            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(bruker);

            mockrepo.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(bruker.BId);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockSession.SetInt32(_LagreBId, bruker.BId);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.logginn(bruker) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, result.Value);



        }


        [Fact]
        public async Task LogginnAdminOk()
        {
            var admin = new Bruker();
            admin.brukernavn = "Admin";
            admin.password = "Admin11@!";
            admin.BType = true; // dette er bruker
            admin.BId = 1;


            mockrepo.Setup(k => k.HentEnBruker(It.IsAny<int>())).ReturnsAsync(admin);

            mockrepo.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(admin.BId);

            var stockController = new StockController(mockrepo.Object, mockController.Object);

            mockSession[_loggetInn] = _AdminLoggetInn;
            mockSession.SetInt32(_LagreBId, admin.BId);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await stockController.logginn(admin) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Value);



        }
    }
}


