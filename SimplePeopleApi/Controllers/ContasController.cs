using System;
using System.Threading.Tasks;
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
    public class ContasController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ContasController(AppDbContext db)
        {
            _db = db;
        }

        // ===== Contas a Pagar =====
        [HttpGet("apagar")]
        public async Task<IActionResult> GetApagar()
        {
            var list = await _db.ContasAPagar.Include(c => c.Fornecedor).ToListAsync();
            return Ok(list);
        }

        [HttpGet("apagar/{numero}")]
        public async Task<IActionResult> GetApagar(long numero)
        {
            var c = await _db.ContasAPagar.FindAsync(numero);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost("apagar")]
        public async Task<IActionResult> CreateApagar([FromBody] ContaAPagar model)
        {
            if (model == null) return BadRequest(new { message = "Payload vazio" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // validate fornecedor exists
                var existsFornecedor = await _db.Pessoas.AnyAsync(p => p.Codigo == model.CodigoFornecedor);
                if (!existsFornecedor) return BadRequest(new { message = "Fornecedor não encontrado" });
                if (model.Valor < 0) return BadRequest(new { message = "Valor inválido" });

                // ensure unique Numero
                var exists = await _db.ContasAPagar.AnyAsync(x => x.Numero == model.Numero);
                if (exists) return Conflict(new { message = "Número já existe" });

                // set InseridoPor from token (username claim)
                var username = User?.Identity?.Name;
                model.InseridoPor = username;

                _db.ContasAPagar.Add(model);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetApagar), new { numero = model.Numero }, model);
            }
            catch (Exception ex)
            {
                // log to server console
                Console.WriteLine("Erro CreateApagar: " + ex);
                // return diagnostic info temporarily to help debugging (remove in production)
                return StatusCode(500, new { message = "Erro criar conta a pagar", details = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPut("apagar/{numero}")]
        public async Task<IActionResult> UpdateApagar(long numero, [FromBody] ContaAPagar model)
        {
            var item = await _db.ContasAPagar.FindAsync(numero);
            if (item == null) return NotFound();
            if (model == null) return BadRequest();
            var existsFornecedor = await _db.Pessoas.AnyAsync(p => p.Codigo == model.CodigoFornecedor);
            if (!existsFornecedor) return BadRequest(new { message = "Fornecedor não encontrado" });

            item.CodigoFornecedor = model.CodigoFornecedor;
            item.DataVencimento = model.DataVencimento;
            item.DataProrrogacao = model.DataProrrogacao;
            item.Valor = model.Valor;
            item.Acrescimo = model.Acrescimo;
            item.Desconto = model.Desconto;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("apagar/{numero}")]
        public async Task<IActionResult> DeleteApagar(long numero)
        {
            var item = await _db.ContasAPagar.FindAsync(numero);
            if (item == null) return NotFound();
            _db.ContasAPagar.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ===== Contas Pagas =====
        [HttpGet("pagas")]
        public async Task<IActionResult> GetPagas()
        {
            var list = await _db.ContasPagas.Include(c => c.Fornecedor).ToListAsync();
            return Ok(list);
        }

        [HttpGet("pagas/{numero}")]
        public async Task<IActionResult> GetPagas(long numero)
        {
            var c = await _db.ContasPagas.FindAsync(numero);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost("pagas")]
        public async Task<IActionResult> CreatePagas([FromBody] ContaPaga model)
        {
            if (model == null) return BadRequest(new { message = "Payload vazio" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existsFornecedor = await _db.Pessoas.AnyAsync(p => p.Codigo == model.CodigoFornecedor);
                if (!existsFornecedor) return BadRequest(new { message = "Fornecedor não encontrado" });
                if (model.Valor < 0) return BadRequest(new { message = "Valor inválido" });

                var exists = await _db.ContasPagas.AnyAsync(x => x.Numero == model.Numero);
                if (exists) return Conflict(new { message = "Número já existe" });

                var username = User?.Identity?.Name;
                model.InseridoPor = username;

                _db.ContasPagas.Add(model);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPagas), new { numero = model.Numero }, model);
            }
catch (Exception ex)
{
    // monta mensagem com todas inner exceptions
    string GetInnerMessages(Exception e)
    {
        if (e == null) return string.Empty;
        var sb = new System.Text.StringBuilder();
        var cur = e;
        int i = 0;
        while (cur != null)
        {
            sb.AppendLine($"[{i}] {cur.GetType().FullName}: {cur.Message}");
            cur = cur.InnerException;
            i++;
        }
        return sb.ToString();
    }

    var full = GetInnerMessages(ex);
    Console.WriteLine("Erro CreateApagar (full): " + full);
    // retorno detalhado TEMPORÁRIO para ajudar debug — remova em produção
    return StatusCode(500, new
    {
        message = "Erro criar conta a pagar",
        error = ex.Message,
        inner = full,
        stack = ex.StackTrace
    });
}
        }

        [HttpPut("pagas/{numero}")]
        public async Task<IActionResult> UpdatePagas(long numero, [FromBody] ContaPaga model)
        {
            var item = await _db.ContasPagas.FindAsync(numero);
            if (item == null) return NotFound();
            if (model == null) return BadRequest();
            var existsFornecedor = await _db.Pessoas.AnyAsync(p => p.Codigo == model.CodigoFornecedor);
            if (!existsFornecedor) return BadRequest(new { message = "Fornecedor não encontrado" });

            item.CodigoFornecedor = model.CodigoFornecedor;
            item.DataVencimento = model.DataVencimento;
            item.DataPagamento = model.DataPagamento;
            item.Valor = model.Valor;
            item.Acrescimo = model.Acrescimo;
            item.Desconto = model.Desconto;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("pagas/{numero}")]
        public async Task<IActionResult> DeletePagas(long numero)
        {
            var item = await _db.ContasPagas.FindAsync(numero);
            if (item == null) return NotFound();
            _db.ContasPagas.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
