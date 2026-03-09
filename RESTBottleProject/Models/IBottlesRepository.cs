
namespace RESTBottleProject.Models
{
    public interface IBottlesRepository
    {
        Bottle AddBottle(Bottle bottle);

        IEnumerable<Bottle> Get(
            int? volumeAtLeast = null,
            string? descriptionStartsWith = null,
            string? sortOrder = null);

        IEnumerable<Bottle> GetAllBottles(string substring);
        Bottle? GetBottleById(int id);
        Bottle? RemoveBottle(int id);
        Bottle? UpdateBottle(int id, Bottle updatedBottle);
    }
}
