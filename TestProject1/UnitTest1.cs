using System.Collections.Generic;
using System.Linq;
using RESTBottleProject.Models;
using Xunit;

namespace TestProject1
{
    public class BottlesRepositoryTests
    {
        [Fact]
        public void GetAllBottles_ReturnsFourInitialBottles()
        {
            BottlesRepository repo = new BottlesRepository();

            List<Bottle> all = repo.GetAllBottles().ToList();

            Assert.Equal(4, all.Count);
        }

        [Fact]
        public void AddBottle_AssignsIdAndAdds()
        {
            BottlesRepository repo = new BottlesRepository();
            Bottle bottle = new Bottle { Volume = 250, Description = "Test add" };

            Bottle added = repo.AddBottle(bottle);

            Assert.NotEqual(0, added.Id);
            Assert.Same(added, repo.GetBottleById(added.Id));
        }

        [Fact]
        public void GetBottleById_ReturnsNullWhenNotFound()
        {
            BottlesRepository repo = new BottlesRepository();

            Bottle? result = repo.GetBottleById(9999);

            Assert.Null(result);
        }

        [Fact]
        public void RemoveBottle_RemovesAndReturnsBottle()
        {
            BottlesRepository repo = new BottlesRepository();
            Bottle toRemove = repo.AddBottle(new Bottle { Volume = 123, Description = "ToRemove" });
            int countBefore = repo.GetAllBottles().Count();

            Bottle? removed = repo.RemoveBottle(toRemove.Id);

            Assert.NotNull(removed);
            Assert.Equal(toRemove.Id, removed.Id);
            Assert.Null(repo.GetBottleById(toRemove.Id));
            Assert.Equal(countBefore - 1, repo.GetAllBottles().Count());
        }

        [Fact]
        public void UpdateBottle_UpdatesPropertiesAndReturnsBottle()
        {
            BottlesRepository repo = new BottlesRepository();
            Bottle original = repo.AddBottle(new Bottle { Volume = 111, Description = "Old" });

            Bottle? updated = repo.UpdateBottle(original.Id, new Bottle { Volume = 222, Description = "New" });

            Assert.NotNull(updated);
            Assert.Equal(222, updated.Volume);
            Assert.Equal("New", updated.Description);

            Bottle? fetched = repo.GetBottleById(original.Id);
            Assert.NotNull(fetched);
            Assert.Equal(222, fetched.Volume);
            Assert.Equal("New", fetched.Description);
        }

        [Fact]
        // sort test 
        public void Get_SortByDescription_ReturnsFour()
        {
            BottlesRepository repo = new BottlesRepository();

            // sortOrder er bare en string, fx "DescriptionAsc"
            List<Bottle> all = repo.Get(null, null, "DescriptionAsc").ToList();

            Assert.Equal(4, all.Count);
        }


        // test for thread 


    }
}
