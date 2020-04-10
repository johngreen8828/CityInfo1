using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Contollers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _mailService = mailService ?? 
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception("Exception Example.");
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found.");
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

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest city.", ex);
                return StatusCode(500, "A problem happened handling your request.");
            }

        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
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

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Name and Description must be different.");
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

            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
            //    c => c.PointsOfInterest).Max(p => p.Id);

            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxPointOfInterestId,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            _cityInfoRepository.Save();

            var createdPointOfInterestToReturn = _mapper
                .Map<Models.PointOfInterestDto>(finalPointOfInterest);

            
            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = createdPointOfInterestToReturn.Id },
                createdPointOfInterestToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Name and Description must be different.");
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


            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}

            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;

            var pointOfInterestEntity = _cityInfoRepository
                .GetPointOfInterestForCity(cityId, id);
            
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            _cityInfoRepository.Save();

            return NoContent();

        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}
            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = pointOfInterestFromStore.Name,
            //    Description = pointOfInterestFromStore.Description
            //};

            //patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
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

            //if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            //{
            //    ModelState.AddModelError("Description", "Name and Description must be different.");
            //}

            

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError(
                    "Description", "Name and description must be different.");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);
            _cityInfoRepository.Save();

            //pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            //pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            //if (pointOfInterestFromStore == null)
            //{
            //    return NotFound();
            //}
            var pointOfInterestEntity = _cityInfoRepository
                .GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            _cityInfoRepository.Save();

            //city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} " +
                $"with id {pointOfInterestEntity.Id} was deleted.");
            return NoContent();
        }

    }
}
