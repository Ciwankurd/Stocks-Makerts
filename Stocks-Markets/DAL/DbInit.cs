using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Stocks_Markets.DAL;
using Stocks_Markets.Models;

namespace Stocks_Markets.DAL
{
    public class DbInit
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScop = app.ApplicationServices.CreateScope())
            {
                var context = serviceScop.ServiceProvider.GetService<StockContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                // addere noen Stocks til table Stocks
                var stock1 = new Stock { AntallStock = 50, Endring = 0.5, SelskapNavn = "OSLO AS", SistePrise = 12.4, Tegn = "OSA", volume = 2.8 };
                var stock2 = new Stock { AntallStock = 80, Endring = 0.7, SelskapNavn = "ASKO AS", SistePrise = 6.8, Tegn = "AKA", volume = 4.5 };
                var stock3 = new Stock { AntallStock = 10, Endring = 0.6, SelskapNavn = "DNB AS", SistePrise = 7.8, Tegn = "AKA", volume = 9.3 };
                var stock4 = new Stock { AntallStock = 100, Endring = 0.3, SelskapNavn = "Spare AS", SistePrise = 8.8, Tegn = "SPA", volume = 7.8 };
                var stock5 = new Stock { AntallStock = 20, Endring = 0.0, SelskapNavn = "Equinur AS", SistePrise = 2.8, Tegn = "EQ", volume = 2.5 };
                var stock6 = new Stock { AntallStock = 12, Endring = 0.1, SelskapNavn = "AKER Soultion AS", SistePrise = 3.8, Tegn = "AKR", volume = 1.6 };


                context.stocks.Add(stock1);
                context.stocks.Add(stock2);
                context.stocks.Add(stock3);
                context.stocks.Add(stock4);
                context.stocks.Add(stock5);
                context.stocks.Add(stock6);

                var bruker = new Brukere();
                var bruker1 = new Brukere();

                //Admin
                bruker.brukernavn = "Admin";
                string password = "Admin11@!";
                bruker.BType = true;  // antar at Bruker Type er Admin og resten er bruker Typer er Bruker by defulat se på  RegistrereNyBruker() in Repo.
                byte[] salt = StockRepository.SaltingPass();
                byte[] hash = StockRepository.HashingPass(password, salt);
                bruker.password = hash;
                bruker.salt = salt;

                // Bruker
                bruker1.brukernavn = "Ciwan";
                string password1 = "Kurd111@Kn";
                bruker1.BType = false; // dette er bruker
                byte[] salt1 = StockRepository.SaltingPass();
                byte[] hash1 = StockRepository.HashingPass(password1, salt1);
                bruker1.password = hash1;
                bruker1.salt = salt1;

                context.brukere.Add(bruker);
                context.brukere.Add(bruker1);
                context.SaveChangesAsync();
            }
        }
    }
}
