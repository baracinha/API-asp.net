using hotel.Data;
using hotel.Services;
using hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using hotel.DTOs.PropertyDTOs.Requests;

namespace hotel.Controllers
{
    [Route("/")]
    [ApiController]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly SalesServices _salesServices;

        public SalesController(AppDbContext context, IDistributedCache cache, SalesServices salesServices)
        {
            _context = context;
            _cache = cache;
            _salesServices = salesServices;
        }

        [HttpPost("InsertProperty")]
        public async Task<IActionResult> InsertProperty([FromBody] InsertPropertyDTO propertyDTO)
        {
            await _salesServices.InsertProperty(propertyDTO);
            return Ok(propertyDTO);
        }

        [HttpGet("GetProperties")]
        public async Task<IActionResult> GetProperties([FromQuery] GetPropertiesDTO getPropertiesDTO)
        {
            var properties = await _salesServices.GetProperties(getPropertiesDTO);
            return Ok(properties);
        }

        [HttpGet("GetMyProperties")]
        public async Task<IActionResult> GetMyProperties([FromQuery] GetPropertiesDTO getPropertiesDTO)
        {
            var properties = await _salesServices.GetMyProperties(getPropertiesDTO);
            return Ok(properties);
        }

        [HttpGet("GetPropertyById")]
        public async Task<IActionResult> GetPropertyById([FromQuery] GetPropertyByIdDTO getPropertyByIdDTO)
        {
            var property = await _salesServices.GetPropertyById(getPropertyByIdDTO);
            return Ok(property);
        }

        [HttpDelete("DeleteProperty")]
        public async Task<IActionResult> DeleteProperty([FromQuery] DeletePopertieDTO deletePopertieDTO)
        {
            await _salesServices.DeleteProperty(deletePopertieDTO);
            return Ok(deletePopertieDTO);
        }

        [HttpPost("UpdateProperty")]
        public async Task<IActionResult> UpdateProperty([FromBody] UpdatePropertyDTO updatePropertyDTO)
        {
            await _salesServices.UpdateProperty(updatePropertyDTO);
            return Ok(updatePropertyDTO);
        }

        [HttpGet("GetFilteredProperties")]
        public async Task<IActionResult> GetFilteredProperties([FromQuery] GetFilteredPropertiesDTO getFilteredPropertiesDTO)
        {
            var properties = await _salesServices.GetFilteredProperties(getFilteredPropertiesDTO);
            return Ok(properties);
        }
    }
}
