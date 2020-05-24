using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using(OracleContext _context = new OracleContext())
            {
                var _items = await _context.PRODUCTS
                                            .Take(2)
                                            .ToListAsync();

                var _json = JsonConvert.SerializeObject(_items, Formatting.Indented);

                Console.WriteLine(_json);
            }

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }
    }

    public class OracleContext : DbContext
    {
        const string connectionString = "Data Source=localhost;User Id=system;Password=oracle";

        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<PRODUCT> PRODUCTS { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseOracle(connectionString, option => {
                    //specify Oracle database version
                    option.UseOracleSQLCompatibility("11");
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PRODUCT>(entity => {

                entity.ToTable("PRODUCT");

                entity.HasKey(x => x.ID);

                entity.Property(x => x.CODE)
                        .HasMaxLength(20);

                entity.Property(x => x.NAME)
                        .HasMaxLength(50)
                        .IsUnicode();
            });
        }
    }


    public class PRODUCT
    {
        public int ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
    }

}
