using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController] //this improves developer experience for developing APIs
    //[Route("api/[controller]")] //disadvantage of using [controller] is if the code changes the interface is broken.  specifically listing "citiies" would make sure you do not break an interface
    [Route("api/cities")]
    public class CitiesController : ControllerBase  //controller base class contains basic functions that controllers need like access to model state and common methods for determining responses
    {
        private readonly ICityInfoRepository _cityinfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _cityinfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }



        //[HttpGet("api/cities")]   //can move the specific route to class level
        //[HttpGet]
        //public JsonResult GetCities()
        //{
        //    return new JsonResult(CitiesDataStore.Current.Cities);  
        //}
        [HttpGet]
        public IActionResult GetCities()
        {
            //return Ok(CitiesDataStore.Current.Cities);
            var cityEntities = _cityinfoRepository.GetCities();

            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto    //manual field mappings to an object  
            //    {                                                 //that does not have all fields
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}
            //return Ok(results);

            //this one line of code replaces the above code that manually mapped fields from one object to another
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        //[HttpGet("api/cities/{id}")]  
        //[HttpGet("{id}")]   //api/cities is defined in the class definition, so not needed on method
        //public JsonResult GetCity(int id) //note: if id does not exist it returns null
        //{
        //    return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}
        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)    //note: JsonResult inherits from IActionResult; however, not informative
        {
            //find city 
            //var cityToReturn = CitiesDataStore.Current.Cities
            //    .FirstOrDefault(c => c.Id == id);

            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}
            //return Ok(cityToReturn);

            var city = _cityinfoRepository.GetCity(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                //var cityResult = new CityDto()
                //{
                //    Id = city.Id,
                //    Name= city.Name,
                //    Description = city.Description
                //};

                //foreach (var poi in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(
                //        new PointOfInterestDto()
                //        { 
                //            Id = poi.Id,
                //            Name = poi.Name,
                //            Description = poi.Description
                //        });
                //}
                //return Ok(cityResult);

                //var cityResult = _mapper.Map<CityDto>(city);
                //return Ok(cityResult);
                //-- Or --
                return Ok(_mapper.Map<CityDto>(city));      //this errored out without PointOfInterestProfile defined
            }

            //var cityWithoutPointsOfInterestResult =
            //    new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = city.Id,
            //        Description = city.Description,
            //        Name = city.Name
            //    };

            //return Ok(cityWithoutPointsOfInterestResult);

            //the one line of code below replaces the above code
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }


    }

}
