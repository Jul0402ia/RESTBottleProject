using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTBottleProject.Models
{
    public class BottlesRepository : IBottlesRepository
    {
        private List<Bottle> bottles = new List<Bottle>(); // "database"
        private int nextId = 1; // bruges til Id

        public BottlesRepository()
        {
            AddBottle(new Bottle() { Volume = 500, Description = "Water Bottle" });
            AddBottle(new Bottle() { Volume = 600, Description = "Juice Bottle" });
            AddBottle(new Bottle() { Volume = 700, Description = "Soda Bottle" });
            AddBottle(new Bottle() { Volume = 300, Description = "Sparkling water Bottle" });
        }

        public IEnumerable<Bottle> Get(int? volumeAtLeast = null, string? descriptionStartsWith = null, string? sortOrder = null)
        {
            // laver kopi så vi ikke sender privat liste direkte
            IEnumerable<Bottle> result = new List<Bottle>(bottles);

            // filter på volume
            if (volumeAtLeast != null)
            {
                result = result.Where(b => b.Volume >= volumeAtLeast);
            }

            // filter på description
            if (descriptionStartsWith != null)
            {
                result = result.Where(b =>
                    b.Description != null &&
                    b.Description.StartsWith(descriptionStartsWith));
            }

            // sortering
            if (sortOrder != null)
            {
                switch (sortOrder)
                {
                    case "Volume":
                    case "VolumeAsc":
                        result = result.OrderBy(b => b.Volume);
                        break;

                    case "VolumeDesc":
                        result = result.OrderByDescending(b => b.Volume);
                        break;

                    case "Description":
                    case "DescriptionAsc":
                        result = result.OrderBy(b => b.Description);
                        break;

                    case "DescriptionDesc":
                        result = result.OrderByDescending(b => b.Description);
                        break;
                }
            }

            return result;
        }

        public IEnumerable<Bottle> GetAllBottles()
        {
            return bottles; // returnerer alle
        }

        public Bottle AddBottle(Bottle bottle)
        {
            if (bottle == null)
            {
                throw new ArgumentNullException(nameof(bottle));
            }

            bottle.Id = nextId++; // giver nyt id
            bottles.Add(bottle);  // tilføjer til liste
            return bottle;
        }

        public Bottle? GetBottleById(int id)
        {
            return bottles.FirstOrDefault(b => b.Id == id); // finder på id
        }

        public Bottle? RemoveBottle(int id)
        {
            Bottle? bottle = GetBottleById(id);

            if (bottle != null)
            {
                bottles.Remove(bottle); // fjerner fra liste
            }

            return bottle;
        }

        public Bottle? UpdateBottle(int id, Bottle updatedBottle)
        {
            Bottle? bottle = GetBottleById(id);

            if (bottle != null)
            {
                bottle.Volume = updatedBottle.Volume; // opdater volume
                bottle.Description = updatedBottle.Description; // opdater description
            }

            return bottle;
        }
    }
}
