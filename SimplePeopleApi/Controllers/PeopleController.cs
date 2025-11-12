using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplePeopleApi.Data;
using SimplePeopleApi.Models;

namespace SimplePeopleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PeopleController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Pessoas.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _db.Pessoas.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Models.Pessoa person)
        {
            // server-side validation
            if (person == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(person.Nome) || string.IsNullOrWhiteSpace(person.CPF) || person.DataDeNascimento == default || string.IsNullOrWhiteSpace(person.UF))
                return BadRequest(new { message = "Nome, CPF, UF and DataDeNascimento are required" });

            // check duplicate CPF
            var existsCpf = await _db.Pessoas.AnyAsync(p => p.CPF == person.CPF);
            if (existsCpf) return Conflict(new { message = "CPF already exists" });

            person.DataDeCriacao = DateTime.Now;
            // normalize UF to uppercase 2 letters
            person.UF = person.UF?.Trim().ToUpper();
            _db.Pessoas.Add(person);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = person.Codigo }, person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Models.Pessoa person)
        {
            if (person == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(person.Nome) || string.IsNullOrWhiteSpace(person.CPF) || person.DataDeNascimento == default || string.IsNullOrWhiteSpace(person.UF))
                return BadRequest(new { message = "Nome, CPF, UF and DataDeNascimento are required" });

            var p = await _db.Pessoas.FindAsync(id);
            if (p == null) return NotFound();

            // if CPF changed, ensure uniqueness
            if (!string.Equals(p.CPF, person.CPF, StringComparison.OrdinalIgnoreCase))
            {
                var existsCpf = await _db.Pessoas.AnyAsync(x => x.CPF == person.CPF && x.Codigo != id);
                if (existsCpf) return Conflict(new { message = "CPF already exists" });
            }

            p.Nome = person.Nome;
            p.CPF = person.CPF;
            p.DataDeNascimento = person.DataDeNascimento;
            p.UF = person.UF?.Trim().ToUpper();
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Pessoas.FindAsync(id);
            if (p == null) return NotFound();
            _db.Pessoas.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
