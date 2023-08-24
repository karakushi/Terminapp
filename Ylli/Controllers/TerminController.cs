namespace Ylli.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Ylli.Data;
    using Ylli.Models;

    [ApiController]
    [Route("api/Termin")]
    public class TerminController : Controller
    {
        private readonly TerminAPI dbContext;

        public TerminController(TerminAPI dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTermine()
        {
            return Ok(await dbContext.Termine.ToListAsync());
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddTermin(AddTermin addTermin)
        {
            if (string.IsNullOrEmpty(addTermin.Beschreibung) || addTermin.Beschreibung.Length < 5)
            {
                return BadRequest("Beschreibung zu kurz");
            }

            var termin = new Termin()
            {
                Id = Guid.NewGuid(),
                Datum = addTermin.Datum,
                Beschreibung = addTermin.Beschreibung
            };
            if (termin.Datum.Day < DateTime.Now.Day)
            {
                return BadRequest("Falsches Datum");
            }

            await dbContext.Termine.AddAsync(termin);
            await dbContext.SaveChangesAsync();

            return Ok(termin);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTermin([FromRoute] Guid id, UpdateTermin updateTerminRequest)
        {
            var termin = await dbContext.Termine.FindAsync(id);
            if (termin != null)
            {
                termin.Datum = updateTerminRequest.Datum;

                await dbContext.SaveChangesAsync();

                return Ok(termin);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTermin([FromRoute] Guid id)
        {
            var termin = await dbContext.Termine.FindAsync(id);
            if (termin != null)
            {
                dbContext.Remove(termin);
                await dbContext.SaveChangesAsync();

                return Ok(termin);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetTermin([FromRoute] Guid id)
        {
            var termin = await dbContext.Termine.FindAsync(id);
            if (termin == null)
            {
                return NotFound();
            }

            return Ok(termin);
        }
    }
}
