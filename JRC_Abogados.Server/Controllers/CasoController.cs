﻿using JRC_Abogados.Server.DataBaseContext;
using JRC_Abogados.Server.Models;
using JRC_Abogados.Server.Models.EmailHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JRC_Abogados.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CasoController : ControllerBase
    {
        private readonly DBaseContext _context;
        private readonly IEmailSender _emailSender;

        public CasoController(DBaseContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Caso>>> GetCasos()
        {
            var casos = await _context.Caso.ToListAsync();

            foreach (var caso in casos)
            {
                caso.TipoCaso = await _context.TipoCaso.FindAsync(caso.TipoCasoId);
                caso.Juzgado = await _context.Juzgado.FindAsync(caso.JuzgadoId);
                caso.Ubicacion = await _context.Ubicacion.FindAsync(caso.UbicacionId);
                caso.Estado = await _context.Estado.FindAsync(caso.EstadoId);
                caso.Cliente = await _context.Cliente.FindAsync(caso.ClienteId);
            }

            return casos;
        }

        [HttpGet("expedientesByClient/{id}")]
        public async Task<ActionResult<IEnumerable<Caso>>> GetCasesByClient(int id)
        {
            var casos = await _context.Caso.Where(c => c.ClienteId == id).ToListAsync();

            foreach (var caso in casos)
            {
                caso.TipoCaso = await _context.TipoCaso.FindAsync(caso.TipoCasoId);
                caso.Juzgado = await _context.Juzgado.FindAsync(caso.JuzgadoId);
                caso.Ubicacion = await _context.Ubicacion.FindAsync(caso.UbicacionId);
                caso.Estado = await _context.Estado.FindAsync(caso.EstadoId);
                caso.Cliente = await _context.Cliente.FindAsync(caso.ClienteId);
            }

            return casos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Caso>> GetCaso(int id)
        {
            var caso = await _context.Caso.FindAsync(id);
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            caso.TipoCaso = await _context.TipoCaso.FindAsync(caso.UbicacionId);
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            caso.Juzgado = await _context.Juzgado.FindAsync(caso.JuzgadoId);
            caso.Ubicacion = await _context.Ubicacion.FindAsync(caso.UbicacionId);
            caso.Estado = await _context.Estado.FindAsync(caso.EstadoId);
            caso.Cliente = await _context.Cliente.FindAsync(caso.ClienteId);

            if (caso == null)
            {
                return NotFound();
            }

            return caso;
        }

        [HttpPost]
        public async Task<ActionResult<Caso>> PostCaso(Caso caso)
        {
            var existingCaso = await _context.Caso.FirstOrDefaultAsync(c => c.Juzgado.NumeroExpediente == caso.Juzgado.NumeroExpediente);

            if (existingCaso != null)
            {
                return Conflict(new { message = "El Caso ya esta registrado." });
            }

            caso.Cliente = await _context.Cliente.FindAsync(caso.ClienteId);
            caso.TipoCaso = await _context.TipoCaso.FindAsync(caso.TipoCasoId);
            caso.Estado = await _context.Estado.FindAsync(caso.EstadoId);
            SendCreateEmail(caso);

            caso.TipoCaso = null;
            caso.Juzgado = null;
            caso.Ubicacion = null;
            caso.Estado = null;
            caso.Cliente = null;

            _context.Caso.Add(caso);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCaso", new { id = caso.Id }, caso);
        }

        [HttpGet("sendCreationEmail")]
        private async void SendCreateEmail(Caso caso)
        {
            string emailSubject = "Nuevo Caso!";
            string emailBody = $"<p> Estimado/a {caso.Cliente.Nombre},</p>" +
                    $"<p>Nos ponemos en contacto para informarle sobre el estado actual de su caso en JRC Abogados.A continuación, encontrará los detalles de su expediente:</p>" +
                    $"<p><strong>Rol:</strong> {caso.TipoCaso.Nombre}</p>" +
                    $"<p><strong>Juzgado:</strong> {caso.Juzgado.Nombre}</p>" +
                    $"<p><strong>Número de Expediente:</strong> {caso.Juzgado.NumeroExpediente}</p>" +
                    $"<p><strong>Estado Actual del Caso:</strong> {caso.Estado.Nombre}</p>" +
                    $"<p>Nos comprometemos a mantenerlo/a informado/a sobre cualquier novedad o cambio en su caso. Si tiene alguna pregunta o necesita más información, no dude en ponerse en contacto con nosotros.</p>" +
                    $"<p>Gracias por confiar en JRC Abogados.</p>" +
                    $"<p>Atentamente,<br> Equipo de JRC Abogados. </p>";

            await _emailSender.SendEmailAsync(caso.Cliente.CorreoElectronico, emailSubject, emailBody);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCaso(int id, Caso caso)
        {
            if (id != caso.Id)
            {
                return BadRequest();
            }

            var caseExisting = await _context.Caso.FindAsync(id);

            if (caseExisting.EstadoId != caso.EstadoId)
            {
                caso.Cliente = await _context.Cliente.FindAsync(caso.ClienteId);
                caso.Estado = await _context.Estado.FindAsync(caso.EstadoId);
                SendStatusEmail(caso);
            }

            caso.Cliente = null;
            caso.Estado = null;
            caso.Juzgado = null;
            caso.TipoCaso = null;
            caso.Ubicacion = null;

            _context.Entry(caseExisting).CurrentValues.SetValues(caso);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("sendStatusEmail")]
        private async void SendStatusEmail(Caso caso)
        {
            string emailSubject = " Actualización de Estado de su Caso";
            string emailBody = $"<p> Estimado/a {caso.Cliente.Nombre},</p>" +
                $"<p>Le informamos que el estado de su caso ha sido actualizado a {caso.Estado.Nombre}</p>" +
                $"<p>Si tiene alguna pregunta, estamos a su disposición.</p>" +
                $"<p>Atentamente,<br> Equipo de JRC Abogados. </p>";

            await _emailSender.SendEmailAsync(caso.Cliente.CorreoElectronico, emailSubject, emailBody);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaso(int id)
        {
            var caso = await _context.Caso.FindAsync(id);
            if (caso == null)
            {
                return NotFound();
            }

            _context.Caso.Remove(caso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CasoExists(int id)
        {
            return _context.Caso.Any(e => e.Id == id);
        }
    }
}
