using Microsoft.EntityFrameworkCore;
using RESTBottleProject.Models;
using Xunit;
using System;
using System.Linq;


namespace TestProject1
{
    public class IRepoBottlesTests
    {
        // true = tester database repo, false = tester list repo
        private bool useDatabase = true;

        // vi tester via interfacet
        private IBottlesRepository repo;

        // din lokale database
        private const string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=BottleDb;Trusted_Connection=True;TrustServerCertificate=True;";


        public IRepoBottlesTests()
        {
            if (useDatabase)
            {
                var optionsBuilder = new DbContextOptionsBuilder<BottlesDBContext>();
                optionsBuilder.UseSqlServer(connectionString);

                BottlesDBContext dbContext = new(optionsBuilder.Options);

                // ryd tabellen før hver test (ingen DBCC)
                dbContext.Database.ExecuteSqlRaw("DELETE FROM dbo.Bottles");

                // database repo
                repo = new BottlesRepositoryDatabase(dbContext);
            }
            else
            {
                // list repo
                repo = new BottlesRepositoryList(includeData: false);
            }
        }

        [Fact]
        public void AddBottle_AssignsIdAndStores()
        {
            Bottle newBottle = new Bottle { Volume = 250, Description = "TestBottle" };

            Bottle added = repo.AddBottle(newBottle);

            Assert.True(added.Id > 0);
            Assert.Equal(250, added.Volume);
            Assert.Equal("TestBottle", added.Description);

            int countAfterAdd = repo.GetAllBottles().Count();
            Assert.Equal(1, countAfterAdd);

            Bottle secondBottle = new Bottle { Volume = 300, Description = "Second" };
            Bottle addedSecond = repo.AddBottle(secondBottle);

            Assert.True(addedSecond.Id > added.Id);

            // objectet du sendte ind får også id
            Assert.True(newBottle.Id > 0);
            Assert.True(secondBottle.Id > newBottle.Id);
        }

        [Fact]
        public void AddBottle_Null_ThrowsArgumentNullException()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => repo.AddBottle(null!));
            Assert.Equal("bottle", ex.ParamName);
        }

        [Fact]
        public void GetBottleById_ReturnsCorrectBottleOrNull()
        {
            Bottle added = repo.AddBottle(new Bottle { Volume = 100, Description = "B1" });

            Bottle? first = repo.GetBottleById(added.Id);
            Assert.NotNull(first);
            Assert.Equal(added.Id, first!.Id);

            Bottle? notFound = repo.GetBottleById(999999);
            Assert.Null(notFound);
        }

        [Fact]
        public void RemoveBottle_RemovesAndReturns()
        {
            Bottle bottle = new Bottle { Volume = 123, Description = "ToRemove" };
            Bottle added = repo.AddBottle(bottle);

            Bottle? removed = repo.RemoveBottle(added.Id);

            Assert.NotNull(removed);
            Assert.Equal(added.Id, removed!.Id);

            Bottle? postLookup = repo.GetBottleById(added.Id);
            Assert.Null(postLookup);

            int remaining = repo.GetAllBottles().Count();
            Assert.Equal(0, remaining);
        }

        [Fact]
        public void RemoveBottle_NonExistent_ReturnsNull()
        {
            Bottle? removed = repo.RemoveBottle(12345);
            Assert.Null(removed);
        }

        [Fact]
        public void UpdateBottle_UpdatesExisting_ReturnsUpdated()
        {
            Bottle original = new Bottle { Volume = 100, Description = "Old" };
            Bottle added = repo.AddBottle(original);

            Bottle payload = new Bottle { Volume = 200, Description = "New" };
            Bottle? updated = repo.UpdateBottle(added.Id, payload);

            Assert.NotNull(updated);
            Assert.Equal(added.Id, updated!.Id);
            Assert.Equal(200, updated.Volume);
            Assert.Equal("New", updated.Description);

            Bottle? stored = repo.GetBottleById(added.Id);
            Assert.NotNull(stored);
            Assert.Equal("New", stored!.Description);
            Assert.Equal(200, stored.Volume);
        }

        [Fact]
        public void UpdateBottle_NonExistent_ReturnsNull()
        {
            Bottle payload = new Bottle { Volume = 1, Description = "Whatever" };

            Bottle? updated = repo.UpdateBottle(42, payload);

            Assert.Null(updated);
        }

        [Fact]
        public void Get_SortByDescription_ReturnsCorrectCount()
        {
            repo.AddBottle(new Bottle { Volume = 400, Description = "Juice Bottle" });
            repo.AddBottle(new Bottle { Volume = 200, Description = "Soda" });
            repo.AddBottle(new Bottle { Volume = 330, Description = "Water Bottle" });

            var sorted = repo.Get(null, null, "DescriptionAsc").ToList();

            Assert.Equal(3, sorted.Count);
        }
    }
}
