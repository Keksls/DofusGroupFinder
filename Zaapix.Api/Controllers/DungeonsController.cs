using Zaapix.Domain.DTO;
using Zaapix.Domain.DTO.Responses;
using Zaapix.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Zaapix.Api.Controllers
{
    [ApiController]
    [Route("api/dungeons")]
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
                .Select(d => new DungeonResponse()
                {
                    Id = d.Id,
                    ExtId = d.ExtId,
                    Name = d.Name,
                    Level = d.Level,
                    Succes = d.Succes,
                    BossId = d.BossId,
                    BossName = d.BossName,
                    BossGfxId = d.BossGfxId
                })
                .ToListAsync();

            return Ok(dungeons);
        }
    }
}
