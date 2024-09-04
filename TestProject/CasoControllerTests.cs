using JRC_Abogados.Server.Controllers;
using JRC_Abogados.Server.DataBaseContext;
using JRC_Abogados.Server.Models;
using JRC_Abogados.Server.Models.EmailHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestProject
{
    public class CasoControllerTests
    {
        private DBaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DBaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DBaseContext(options);
        }

        [Fact]
        public async Task GetCasos_ReturnsCasosWithDetails()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion { Id = 1, Direccion = "123 Calle Falsa", Estado = "Chiapas", Ciudad = "CDMX", CodigoPostal = 20043 };
            var cliente = new Cliente { Id = 1, Nombre = "Juan", Apellido = "Perez", CorreoElectronico = "juan@example.com" };
            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Tipo 1" };
            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado 1", NumeroExpediente = 123454323 };
            var estado = new Estado { Id = 1, Nombre = "Estado 1" };

            context.Ubicacion.Add(ubicacion);
            context.Cliente.Add(cliente);
            context.TipoCaso.Add(tipoCaso);
            context.Juzgado.Add(juzgado);
            context.Estado.Add(estado);

            var caso = new Caso
            {
                Id = 1,
                TipoCasoId = 1,
                JuzgadoId = 1,
                UbicacionId = 1,
                EstadoId = 1,
                ClienteId = 1,
                Descripcion = "Esto es una prueba",
                FechaInicio = DateTime.Now,
                FechaTermino = DateTime.Now,
            };

            context.Caso.Add(caso);
            await context.SaveChangesAsync();

            var controller = new CasoController(context, mockEmailSender.Object);

            // Act
            var result = await controller.GetCasos();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Caso>>>(result);
            var casos = Assert.IsAssignableFrom<IEnumerable<Caso>>(actionResult.Value);
            var casoResult = casos.FirstOrDefault();
            Assert.NotNull(casoResult);
            Assert.Equal("Tipo 1", casoResult.TipoCaso.Nombre);
            Assert.Equal("Juzgado 1", casoResult.Juzgado.Nombre);
            Assert.Equal("123 Calle Falsa", casoResult.Ubicacion.Direccion);
            Assert.Equal("Estado 1", casoResult.Estado.Nombre);
            Assert.Equal("Juan", casoResult.Cliente.Nombre);
        }

        [Fact]
        public async Task GetCaso_ReturnsCasoWithDetails()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion { Id = 1, Direccion = "123 Calle Falsa", Estado = "Chiapas", Ciudad = "CDMX", CodigoPostal = 20043 };
            var cliente = new Cliente { Id = 1, Nombre = "Juan", Apellido = "Perez", CorreoElectronico = "juan@example.com" };
            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Tipo 1" };
            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado 1", NumeroExpediente = 123454323 };
            var estado = new Estado { Id = 1, Nombre = "Estado 1" };

            context.Ubicacion.Add(ubicacion);
            context.Cliente.Add(cliente);
            context.TipoCaso.Add(tipoCaso);
            context.Juzgado.Add(juzgado);
            context.Estado.Add(estado);

            var caso = new Caso
            {
                Id = 1,
                TipoCasoId = 1,
                JuzgadoId = 1,
                UbicacionId = 1,
                EstadoId = 1,
                ClienteId = 1,
                Descripcion = "Esto es una prueba",
                FechaInicio = DateTime.Now,
                FechaTermino = DateTime.Now,
            };

            context.Caso.Add(caso);
            await context.SaveChangesAsync();

            var controller = new CasoController(context, mockEmailSender.Object);

            // Act
            var result = await controller.GetCaso(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Caso>>(result);
            var casoResult = Assert.IsType<Caso>(actionResult.Value);
            Assert.NotNull(casoResult);
            Assert.Equal("Tipo 1", casoResult.TipoCaso.Nombre);
            Assert.Equal("Juzgado 1", casoResult.Juzgado.Nombre);
            Assert.Equal("123 Calle Falsa", casoResult.Ubicacion.Direccion);
            Assert.Equal("Estado 1", casoResult.Estado.Nombre);
            Assert.Equal("Juan", casoResult.Cliente.Nombre);
        }

        [Fact]
        public async Task PostCaso_CreatesNewCasoAndSendsEmail()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion { Id = 1, Direccion = "123 Calle Falsa", Estado = "Chiapas", Ciudad = "CDMX", CodigoPostal = 20043 };
            context.Ubicacion.Add(ubicacion);

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Maria",
                Apellido = "Lopez",
                CorreoElectronico = "maria@example.com",
                UbicacionId = 1,
                FechaNacimiento = DateTime.Now.AddYears(-25),
                Telefono = 9876543210,
            };
            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Tipo 1" };
            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado 1", NumeroExpediente = 123454323 };
            var estado = new Estado { Id = 1, Nombre = "Estado 1" };

            context.Cliente.Add(cliente);
            context.TipoCaso.Add(tipoCaso);
            context.Juzgado.Add(juzgado);
            context.Estado.Add(estado);
            await context.SaveChangesAsync();

            var caso = new Caso
            {
                TipoCasoId = 1,
                JuzgadoId = 1,
                UbicacionId = 1,
                EstadoId = 1,
                ClienteId = 1,
                Descripcion = "Esto es una prueba",
                FechaInicio = DateTime.Now,
                FechaTermino = DateTime.Now,
                Cliente = cliente,
                Ubicacion = ubicacion,
                Juzgado = juzgado,
                TipoCaso = tipoCaso,
                Estado = estado
            };

            var controller = new CasoController(context, mockEmailSender.Object);

            // Act
            var result = await controller.PostCaso(caso);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCaso = Assert.IsType<Caso>(createdResult.Value);
            Assert.Equal(1, createdCaso.Id);

            mockEmailSender.Verify(m => m.SendEmailAsync(
                caso.Cliente.CorreoElectronico,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task PutCaso_UpdatesExistingCaso()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion
            {
                Id = 1,
                Direccion = "123 Calle Falsa",
                Estado = "Chiapas",
                Ciudad = "CDMX",
                CodigoPostal = 20043
            };
            context.Ubicacion.Add(ubicacion);

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Maria",
                Apellido = "Lopez",
                CorreoElectronico = "maria@example.com",
                UbicacionId = 1,
                FechaNacimiento = DateTime.Now.AddYears(-25),
                Telefono = 9876543210,
            };
            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Tipo 1" };
            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado 1", NumeroExpediente = 123454323 };
            var estado = new Estado { Id = 1, Nombre = "Estado 1" };
            var estado2 = new Estado { Id = 2, Nombre = "Estado 2" };

            context.Cliente.Add(cliente);
            context.TipoCaso.Add(tipoCaso);
            context.Juzgado.Add(juzgado);
            context.Estado.Add(estado);
            context.Estado.Add(estado2);

            var caso = new Caso
            {
                Id = 1,
                TipoCasoId = 1,
                JuzgadoId = 1,
                UbicacionId = 1,
                EstadoId = 1,
                ClienteId = 1,
                Descripcion = "Esto es una prueba",
                FechaInicio = DateTime.Now,
                FechaTermino = DateTime.Now
            };

            context.Caso.Add(caso);
            await context.SaveChangesAsync();

            var controller = new CasoController(context, mockEmailSender.Object);

            // Act
            caso.EstadoId = 2;
            caso.Estado = estado2;

            var result = await controller.PutCaso(1, caso);

            // Assert
            Assert.IsType<OkResult>(result);

            var updatedCaso = await context.Caso.FindAsync(1);
            Assert.NotNull(caso);
            Assert.Equal(2, caso.EstadoId);
        }


        [Fact]
        public async Task DeleteCaso_RemovesCaso()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var caso = new Caso
            {
                Id = 1,
                TipoCasoId = 1,
                JuzgadoId = 1,
                UbicacionId = 1,
                EstadoId = 1,
                ClienteId = 1,
                Descripcion = "Caso de prueba",
                FechaInicio = DateTime.Now,
                FechaTermino = DateTime.Now.AddMonths(1)
            };

            context.Caso.Add(caso);
            await context.SaveChangesAsync();

            var controller = new CasoController(context, mockEmailSender.Object);

            // Act
            var result = await controller.DeleteCaso(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedCaso = await context.Caso.FindAsync(1);
            Assert.Null(deletedCaso);
        }
    }
}
