using JRC_Abogados.Server.DataBaseContext;
using JRC_Abogados.Server.Models;
using JRC_Abogados.Server.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JRC_Abogados.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpedienteController : ControllerBase
    {
        private readonly DBaseContext _context;

        public ExpedienteController(DBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expediente>>> GetExpedientes()
        {
            var expedientes = await _context.Expediente.ToListAsync();

            foreach (var expediente in expedientes)
            {
                if (expediente.CasoId != null)
                {
                    expediente.Caso = await _context.Caso.FindAsync(expediente.CasoId);
                    expediente.Caso.TipoCaso = await _context.TipoCaso.FindAsync(expediente.Caso.TipoCasoId);
                    expediente.Caso.Juzgado = await _context.Juzgado.FindAsync(expediente.Caso.JuzgadoId);
                    expediente.Caso.Ubicacion = await _context.Ubicacion.FindAsync(expediente.Caso.UbicacionId);
                    expediente.Caso.Cliente = await _context.Cliente.FindAsync(expediente.ClienteId);
                }

                expediente.Cliente = await _context.Cliente.FindAsync(expediente.ClienteId);
            }

            return expedientes;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expediente>> GetExpediente(int id)
        {
            var expediente = await _context.Expediente.FindAsync(id);

            if (expediente == null)
            {
                return NotFound();
            }

            if (expediente.CasoId != null)
            {
                expediente.Caso = await _context.Caso.FindAsync(expediente.CasoId);
            }

            expediente.Cliente = await _context.Cliente.FindAsync(expediente.ClienteId);

            return expediente;
        }

        [HttpPost]
        public async Task<ActionResult<Expediente>> PostExpediente(ExpedienteDTO expedienteDTO)
        {
            var existingExpediente = await _context.Expediente.FirstOrDefaultAsync(c => c.CasoId == expedienteDTO.CasoId);

            if (existingExpediente != null)
            {
                return Conflict(new { message = "El expediente ya está registrado." });
            }

            var expediente = new Expediente();
            expediente.Nombre = expedienteDTO.Nombre;
            expediente.TipoExpedienteId = expedienteDTO.TipoExpedienteId;
            expediente.ClienteId = expedienteDTO.ClienteId;
            expediente.CasoId = expedienteDTO.CasoId;
            expediente.FechaInicio = DateTime.Now;

            _context.Expediente.Add(expediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpediente", new { id = expediente.Id }, expediente);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpediente(int id, ExpedienteDTO expedienteDTO)
        {
            if (id != expedienteDTO.Id)
            {
                return BadRequest();
            }

            var expediente = await _context.Expediente.FindAsync(id);
            expediente.Nombre = expedienteDTO.Nombre;
            expediente.TipoExpedienteId = expedienteDTO.TipoExpedienteId;
            expediente.ClienteId = expedienteDTO.ClienteId;
            expediente.CasoId = expedienteDTO.CasoId;


            _context.Entry(expediente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpedienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpediente(int id)
        {
            var expediente = await _context.Expediente.FindAsync(id);
            if (expediente == null)
            {
                return NotFound();
            }

            _context.Expediente.Remove(expediente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpedienteExists(int id)
        {
            return _context.Expediente.Any(e => e.Id == id);
        }
    }
}
