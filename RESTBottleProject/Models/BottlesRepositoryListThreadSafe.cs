using System;
using System.Collections.Generic;
using System.Threading;

namespace RESTBottleProject.Models
{
    public class BottlesRepositoryListThreadSafe : IBottlesRepository
    {
        // privat liste (vores "database")
        private readonly List<Bottle> bottles = new();

        // bruges til id
        private int nextId = 0;

        // lock objekt (synkronisering)
        private readonly object sync = new();

        public BottlesRepositoryListThreadSafe(bool includeData = false)
        {
            if (includeData)
            {
                AddBottle(new Bottle { Volume = 400, Description = "Juice Bottle" });
                AddBottle(new Bottle { Volume = 200, Description = "Soda" });
                AddBottle(new Bottle { Volume = 330, Description = "Water Bottle" });
            }
        }

        // returnerer alle bottles (snapshot)
        public IEnumerable<Bottle> GetAllBottles()
        {
            lock (sync)
            {
                List<Bottle> snapshot = new List<Bottle>(bottles);
                return snapshot.AsReadOnly();
            }
        }

        // filtrering + sortering
        public IEnumerable<Bottle> Get(int? volumeAtLeast = null,
                                       string? descriptionStartsWith = null,
                                       string? sortOrder = null)
        {
            lock (sync)
            {
                List<Bottle> result = new List<Bottle>();

                foreach (Bottle bottle in bottles)
                {
                    bool include = true;

                    if (volumeAtLeast != null && bottle.Volume < volumeAtLeast)
                    {
                        include = false;
                    }

                    if (descriptionStartsWith != null &&
                        (bottle.Description == null ||
                         !bottle.Description.StartsWith(descriptionStartsWith)))
                    {
                        include = false;
                    }

                    if (include)
                    {
                        result.Add(bottle);
                    }
                }

                // simpel sortering
                if (sortOrder == "VolumeAsc")
                {
                    result.Sort((a, b) => a.Volume.CompareTo(b.Volume));
                }
                else if (sortOrder == "VolumeDesc")
                {
                    result.Sort((a, b) => b.Volume.CompareTo(a.Volume));
                }
                else if (sortOrder == "DescriptionAsc")
                {
                    result.Sort((a, b) => string.Compare(a.Description, b.Description));
                }
                else if (sortOrder == "DescriptionDesc")
                {
                    result.Sort((a, b) => string.Compare(b.Description, a.Description));
                }

                return result.AsReadOnly();
            }
        }

        // finder bottle via id
        public Bottle? GetBottleById(int id)
        {
            lock (sync)
            {
                foreach (Bottle bottle in bottles)
                {
                    if (bottle.Id == id)
                    {
                        return bottle;
                    }
                }
                return null;
            }
        }

        // tilføjer bottle (thread safe id)
        public Bottle AddBottle(Bottle bottle)
        {
            if (bottle == null)
            {
                throw new ArgumentNullException(nameof(bottle));
            }

            // thread safe id increment
            int assignedId = Interlocked.Increment(ref nextId);
            bottle.Id = assignedId;

            lock (sync)
            {
                bottles.Add(bottle);
            }

            return bottle;
        }

        // fjerner bottle
        public Bottle? RemoveBottle(int id)
        {
            lock (sync)
            {
                foreach (Bottle bottle in bottles)
                {
                    if (bottle.Id == id)
                    {
                        bottles.Remove(bottle);
                        return bottle;
                    }
                }

                return null;
            }
        }

        // opdaterer bottle
        public Bottle? UpdateBottle(int id, Bottle updatedBottle)
        {
            if (updatedBottle == null)
            {
                throw new ArgumentNullException(nameof(updatedBottle));
            }

            lock (sync)
            {
                foreach (Bottle bottle in bottles)
                {
                    if (bottle.Id == id)
                    {
                        bottle.Volume = updatedBottle.Volume;
                        bottle.Description = updatedBottle.Description;
                        return bottle;
                    }
                }

                return null;
            }
        }
    }
}

