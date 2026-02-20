using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTBottleProject.Models
{
    public class BottlesRepositoryDatabase : IBottlesRepository
    {
        private readonly BottlesDBContext _context; // db context

        public BottlesRepositoryDatabase(BottlesDBContext context)
        {
            _context = context;
        }

        public Bottle AddBottle(Bottle bottle)
        {
            if (bottle == null)
            {
                throw new ArgumentNullException(nameof(bottle));
            }

            _context.Bottles.Add(bottle); // tilføjer til db
            _context.SaveChanges();       // gemmer ændring
            return bottle;
        }

        public IEnumerable<Bottle> GetAllBottles()
        {
            return _context.Bottles; // returnerer alle
        }

        public Bottle? GetBottleById(int id)
        {
            return _context.Bottles.Find(id); // finder på id
        }

        public Bottle? RemoveBottle(int id)
        {
            Bottle? bottle = GetBottleById(id);

            if (bottle != null)
            {
                _context.Bottles.Remove(bottle); // fjerner
                _context.SaveChanges();          // gemmer
            }

            return bottle;
        }

        public Bottle? UpdateBottle(int id, Bottle updatedBottle)
        {
            Bottle? existingBottle = GetBottleById(id);

            if (existingBottle != null)
            {
                existingBottle.Volume = updatedBottle.Volume;       // opdater volume
                existingBottle.Description = updatedBottle.Description; // opdater description
                _context.SaveChanges(); // gemmer ændring
            }

            return existingBottle;
        }

        // filtrering + sortering
        public IEnumerable<Bottle> Get(int? volumeAtLeast = null,
                                       string? descriptionStartsWith = null,
                                       string? sortOrder = null)
        {
            IEnumerable<Bottle> result = _context.Bottles; // starter med alle

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

            return result; // returnerer resultat
        }
    }
}
