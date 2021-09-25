using CityInfo.API.Contexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }  

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefault();
            }

            return _context.Cities
                    .Where(c => c.Id == cityId).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest
                        .Where(p => p.CityId == cityId).ToList();
        }


        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        
        }

        public void AddPointOfinterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void UpdatePointOfinterestForCity(int cityId, PointOfInterest pointOfInterest)
        {   //this is an empty routine because entity framework tracks the context,
            //so performing a mapping and saving the context is sufficient.  this means no
            //explicit update code is needed.  however, not all frameworks track context, so
            //video author made this blank routine to make the code cleaner for other frameworks
            
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >=0);
        }
    }
}
