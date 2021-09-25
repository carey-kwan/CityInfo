using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {   //note: point of interest is a child resource.  that is why it is only accessible through cities

        private readonly ILogger<PointsOfInterestController> _logger;
        //private readonly LocalMailService _mailService;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            //LocalMailService mailService)
            IMailService mailService, 
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {   //constructor injection is the preferred way for requesting dependencies 

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));    //?? is a check to make sure logger is not null
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception("Logfile cannot be read-only");

                //var city = CitiesDataStore.Current.Cities
                //    .FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");   //by default logs in console. added .UseNlog in Program.cs to override for writing to log file instead
                //    return NotFound();
                //}

                //return Ok(city.PointsOfInterest);

                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when " +
                        $"accessing points of interest.");
                    return NotFound();
                }
                
                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                //var pointsOfInterestForCityResults = new List<PointOfInterestDto>();
                //foreach (var poi in pointsOfInterestForCity)
                //{
                //    pointsOfInterestForCityResults.Add(new PointOfInterestDto()
                //    { 
                //        Id = poi.Id,
                //        Name = poi.Name,
                //        Description = poi.Description
                //    });

                //}
                //return Ok(pointsOfInterestForCityResults);

                //the below line of code replace the commented, mapping code above
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exeception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }


        }

        [HttpGet("{id}", Name ="GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            //var city = CitiesDataStore.Current.Cities
            //    .FirstOrDefault(c => c.Id == cityId);

            //if (city == null)
            //{
            //    return NotFound();
            //}

            ////find point of interest
            //var pointOfInterest = city.PointsOfInterest
            //    .FirstOrDefault(c => c.Id == id);

            //if (pointOfInterest == null)
            //{
            //    return NotFound();
            //}

            //return Ok(pointOfInterest);

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            //var pointOfInterestResult = new PointOfInterestDto()
            //{
            //    Id = pointOfInterest.Id,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};
            //return Ok(pointOfInterestResult);
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }


        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            //Modelstate checks to make sure that the constraints defined in the data annotations 
            //of the PointOfInterestForCreation are adhered to through model binding validation.

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",  //having the name of the property helps with UI validation, so the property of the control bound to this field will have an error set
                    "The provided description should be different from the name.");            
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);      
            }


            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }


            // demo purposes - to be improved
            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(     //no longer needed as column is now an identity column
            //                            c => c.PointsOfInterest).Max(p => p.Id);

            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxPointOfInterestId,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            //city.PointsOfInterest.Add(finalPointOfInterest);

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfinterestForCity(cityId, finalPointOfInterest);
            _cityInfoRepository.Save();

            var createdPointOfInterestToReturn = _mapper
                .Map<Models.PointOfInterestDto>(finalPointOfInterest);  //this is needed because need to return the DTO 

            //return CreatedAtRoute(
            //"GetPointOfInterest",
            //new { cityId = cityId, id = finalPointOfInterest},    when variable name matches parameter name, do not need the left aide of the equal
            //new { cityId, id = finalPointOfInterest.Id },
            //finalPointOfInterest);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId = cityId, id = createdPointOfInterestToReturn.Id },    //when variable name matches parameter name, do not need the left aide of the equal
                createdPointOfInterestToReturn);
        }


        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",  //having the name of the property helps with UI validation, so the property of the control bound to this field will have an error set
                    "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestFromStore = city.PointsOfInterest
            //    .FirstOrDefault(p => p.Id == id);
            //if (pointOfInterest == null)  //check to see if PoI exists
            //{
            //    return NotFound();
            //}

            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;

            //return NoContent();

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            _cityInfoRepository.UpdatePointOfinterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            //var city = CitiesDataStore.Current.Cities
            //    .FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestFromStore = city.PointsOfInterest
            //    .FirstOrDefault(c => c.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestToPatch =
            //    new PointOfInterestForUpdateDto()
            //    {
            //        Name = pointOfInterestFromStore.Name,
            //        Description = pointOfInterestFromStore.Description
            //    };

            //patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            //{
            //    ModelState.AddModelError(
            //        "Description",
            //        "The provided description should be different from the name");
            //}

            //if (!TryValidateModel(pointOfInterestToPatch))
            //{
            //    return BadRequest(ModelState); 
            //}

            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            //return NoContent();

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository
                .GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper
                .Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);


            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            _cityInfoRepository.UpdatePointOfinterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            //var city = CitiesDataStore.Current.Cities
            //    .FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterestFromStore = city.PointsOfInterest
            //    .FirstOrDefault(c => c.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //city.PointsOfInterest.Remove(pointOfInterestFromStore);

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            //return NoContent();

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository
                .GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }


            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            _cityInfoRepository.Save();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
