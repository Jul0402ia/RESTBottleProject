namespace RESTBottleProject.Models
{
    public class Bottle : Object
    {
        public int Id { get; set; }
        public int Volume { get; set; }

        public string? Description { get; set; }

        //usynelig constroctor
        // override fordi man override ToString metoden fra Object klassen

        public override string ToString()
        {
            return $"Bottle Id: {Id}, Volume: {Volume}, Description: {Description}";
        }
    }
}
