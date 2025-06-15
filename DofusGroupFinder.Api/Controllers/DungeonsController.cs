using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DofusGroupFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DungeonsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DungeonsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dungeons = await _dbContext.Dungeons
                .OrderBy(d => d.Name)
                .Select(d => new
                {
                    d.Id,
                    d.ExternalId,
                    d.Name,
                    d.MinLevel,
                    d.MaxLevel
                })
                .ToListAsync();

            return Ok(dungeons);
        }
    }
}
