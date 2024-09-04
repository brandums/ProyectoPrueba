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
    public class DocumentoController : ControllerBase
    {
        private readonly DBaseContext _context;

        public DocumentoController(DBaseContext context)
        {
            _context = context;
        }

        [HttpGet("documentsByExpedient/{id}")]
        public async Task<ActionResult<IEnumerable<Documento>>> GetDocumentos(int id)
        {
            var documentos = await _context.Documento
                                          .Where(e => e.ExpedienteId == id)
                                          .ToListAsync();

            foreach (var documento in documentos)
            {
                documento.TipoDocumento = await _context.TipoDocumento.FindAsync(documento.TipoDocumentoId);
                documento.Cliente = await _context.Cliente.FindAsync(documento.ClienteId);
            }

            return documentos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Documento>> GetDocumento(int id)
        {
            var documento = await _context.Documento.FindAsync(id);

            if (documento == null)
            {
                return NotFound();
            }

            documento.TipoDocumento = await _context.TipoDocumento.FindAsync(documento.TipoDocumentoId);
            documento.Cliente = await _context.Cliente.FindAsync(documento.ClienteId);

            return documento;
        }

        //[HttpPost, DisableRequestSizeLimit]
        //public async Task<ActionResult<Expediente>> PostDocumento([FromForm] DocumentoDTO documentoDTO, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("El archivo es requerido.");
        //    }

        //    var documento = new Documento
        //    {
        //        Nombre = documentoDTO.Nombre,
        //        TipoDocumentoId = documentoDTO.TipoDocumentoId,
        //        ExpedienteId = documentoDTO.ExpedienteId,
        //        FechaInicio = DateTime.Now,
        //    };

        //    var expediente = await _context.Expediente.FindAsync(documentoDTO.ExpedienteId);
        //    documento.ClienteId = expediente.ClienteId;

        //    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        //    if (!Directory.Exists(uploadsPath))
        //    {
        //        Directory.CreateDirectory(uploadsPath);
        //    }

        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //    var extension = Path.GetExtension(file.FileName);
        //    var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
        //    var filePath = Path.Combine(uploadsPath, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    documento.Path = filePath;

        //    _context.Documento.Add(documento);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetDocumento", new { id = documento.Id }, documento);
        //}

        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult<Documento>> PostDocumento([FromForm] DocumentoDTO documentoDTO, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("El archivo es requerido.");
            }

            var documento = new Documento
            {
                Nombre = documentoDTO.Nombre,
                TipoDocumentoId = documentoDTO.TipoDocumentoId,
                ExpedienteId = documentoDTO.ExpedienteId,
                FechaInicio = DateTime.Now,
            };

            var expediente = await _context.Expediente.FindAsync(documentoDTO.ExpedienteId);
            documento.ClienteId = expediente.ClienteId;

            // Obtener la ruta a la carpeta 'wwwroot/uploads' del servidor
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Verificar si la carpeta existe, si no, crearla
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generar un nombre de archivo único
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Guardar el archivo en el servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Guardar la ruta del archivo en la base de datos
            documento.Path = $"/uploads/{uniqueFileName}"; // Utilizar una ruta relativa

            _context.Documento.Add(documento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocumento", new { id = documento.Id }, documento);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumento(int id, DocumentoDTO documentoDTO)
        {
            if (id != documentoDTO.Id)
            {
                return BadRequest();
            }

            var documento = await _context.Documento.FindAsync(id);
            documento.Nombre = documentoDTO.Nombre;

            _context.Entry(documento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentoExists(id))
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
        public async Task<IActionResult> DeleteDocumento(int id)
        {
            var documento = await _context.Documento.FindAsync(id);
            if (documento == null)
            {
                return NotFound();
            }

            _context.Documento.Remove(documento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentoExists(int id)
        {
            return _context.Documento.Any(e => e.Id == id);
        }
    }
}
