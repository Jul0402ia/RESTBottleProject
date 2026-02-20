using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RESTBottleProject.Models;

namespace RESTBottleProject.Models
{
    public class BottlesRepositoryList : IBottlesRepository
    {
        private readonly List<Bottle> bottles = new(); // "database"
        private int nextId = 1; // id

        public BottlesRepositoryList(bool includeData = false)
        {
            if (includeData)
            {
                AddBottle(new Bottle { Volume = 400, Description = "Juice Bottle" });
                AddBottle(new Bottle { Volume = 200, Description = "Soda" });
                AddBottle(new Bottle { Volume = 330, Description = "Water Bottle" });
            }
        }

        public IEnumerable<Bottle> GetAllBottles()
        {
            return bottles.AsReadOnly(); // returnerer alle
        }

        public Bottle? GetBottleById(int id)
        {
            return bottles.FirstOrDefault(b => b.Id == id); // finder på id
        }

        public Bottle AddBottle(Bottle bottle)
        {
            if (bottle == null)
            {
                throw new ArgumentNullException(nameof(bottle));
            }

            bottle.Id = nextId++; // giver nyt id
            bottles.Add(bottle);  // tilføjer
            return bottle;
        }

        public Bottle? RemoveBottle(int id)
        {
            Bottle? bottle = GetBottleById(id);

            if (bottle != null)
            {
                bottles.Remove(bottle); // fjerner
            }

            return bottle;
        }

        public Bottle? UpdateBottle(int id, Bottle updatedBottle)
        {
            Bottle? existingBottle = GetBottleById(id);

            if (existingBottle != null)
            {
                existingBottle.Volume = updatedBottle.Volume; // opdater volume
                existingBottle.Description = updatedBottle.Description; // opdater description
            }

            return existingBottle;
        }

        public IEnumerable<Bottle> Get(int? volumeAtLeast = null, string? descriptionStartsWith = null, string? sortOrder = null)
        {
            IEnumerable<Bottle> result = new List<Bottle>(bottles); // kopi

            // filter volume
            if (volumeAtLeast != null)
            {
                result = result.Where(b => b.Volume >= volumeAtLeast);
            }

            // filter description
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
    }
}