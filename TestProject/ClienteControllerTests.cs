using JRC_Abogados.Server.Controllers;
using JRC_Abogados.Server.DataBaseContext;
using JRC_Abogados.Server.Models;
using JRC_Abogados.Server.Models.EmailHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestProject
{
    public class ClienteControllerTests
    {
        private DBaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DBaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DBaseContext(options);
        }


        [Fact]
        public async Task GetClientes_ReturnsClientesWithUbicacion()
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
                Nombre = "Juan",
                Apellido = "Perez",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Telefono = 1234567890,
                CorreoElectronico = "juan@example.com",
                UbicacionId = 1
            };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.GetClientes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Cliente>>>(result);
            var clientes = Assert.IsAssignableFrom<IEnumerable<Cliente>>(actionResult.Value);
            var clienteResult = clientes.FirstOrDefault();
            Assert.NotNull(clienteResult);
            Assert.Equal("Juan", clienteResult.Nombre);
            Assert.NotNull(clienteResult.Ubicacion);
            Assert.Equal("123 Calle Falsa", clienteResult.Ubicacion.Direccion);
            Assert.Equal("Chiapas", clienteResult.Ubicacion.Estado);
            Assert.Equal("CDMX", clienteResult.Ubicacion.Ciudad);
            Assert.Equal(20043, clienteResult.Ubicacion.CodigoPostal);
        }


        [Fact]
        public async Task GetCliente_ReturnsClienteWithUbicacion()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            // Agregar datos de prueba
            var ubicacion = new Ubicacion { Id = 1, Direccion = "123 Calle Falsa", Estado = "Chiapas", Ciudad = "CDMX", CodigoPostal = 20043 };
            context.Ubicacion.Add(ubicacion);
            context.Cliente.Add(new Cliente { Id = 1, Nombre = "Juan", Apellido = "Perez", CorreoElectronico = "juan@example.com", UbicacionId = 1 });
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.GetCliente(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Cliente>>(result);
            var cliente = Assert.IsType<Cliente>(actionResult.Value);
            Assert.Equal("Juan", cliente.Nombre);
            Assert.NotNull(cliente.Ubicacion);
            Assert.Equal("123 Calle Falsa", cliente.Ubicacion.Direccion);
        }

        [Fact]
        public async Task PostCliente_CreatesNewClienteAndSendsEmail()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var cliente = new Cliente
            {
                Nombre = "Maria",
                Apellido = "Lopez",
                CorreoElectronico = "maria@example.com",
                UbicacionId = 1
            };

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.PostCliente(cliente);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCliente = Assert.IsType<Cliente>(createdResult.Value);
            Assert.Equal("Maria", createdCliente.Nombre);

            mockEmailSender.Verify(m => m.SendEmailAsync(
                cliente.CorreoElectronico,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task PutCliente_UpdatesExistingCliente()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Luis",
                Apellido = "Martinez",
                FechaNacimiento = DateTime.Now.AddYears(-25),
                Telefono = 9876543210,
                CorreoElectronico = "luis@example.com",
                UbicacionId = 1
            };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            cliente.Nombre = "Luis Updated";
            var result = await controller.PutCliente(1, cliente);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedCliente = await context.Cliente.FindAsync(1);
            Assert.NotNull(updatedCliente);
            Assert.Equal("Luis Updated", updatedCliente.Nombre);
        }



        [Fact]
        public async Task DeleteCliente_RemovesCliente()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var cliente = new Cliente { Id = 1, Nombre = "Carlos", Apellido = "Sanchez", CorreoElectronico = "carlos@example.com" };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.DeleteCliente(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedCliente = await context.Cliente.FindAsync(1);
            Assert.Null(deletedCliente);
        }
    }

}
