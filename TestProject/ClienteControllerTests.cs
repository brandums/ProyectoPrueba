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
        {  // Historia de Usuario HU005, Visualizar cliente
           // Verifica que muestra una lista completa de clientes con la información de su ubicación.

            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion
            {
                Id = 1,
                Direccion = "206 Av Clouthier",
                Estado = "Jalisco",
                Ciudad = "Guadalajara",
                CodigoPostal = 20043
            };
            context.Ubicacion.Add(ubicacion);

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Alejandra",
                Apellido = "Nava",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Telefono = "(33) 2344-2345",
                CorreoElectronico = "nalejandra@hotmail.com",
                UbicacionId = 1,
                EmpleadoId = 1,
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
            Assert.Equal("Alejandra", clienteResult.Nombre);
            Assert.NotNull(clienteResult.Ubicacion);
            Assert.Equal("206 Av Clouthier", clienteResult.Ubicacion.Direccion);
            Assert.Equal("Jalisco", clienteResult.Ubicacion.Estado);
            Assert.Equal("Guadalajara", clienteResult.Ubicacion.Ciudad);
            Assert.Equal(20043, clienteResult.Ubicacion.CodigoPostal);
        }


        [Fact]
        public async Task GetCliente_ReturnsClienteWithUbicacion()
        {   // Historia de Usuario HU005, Visualizar cliente
            //muestra la información de un cliente específico con su ubicación.
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var ubicacion = new Ubicacion { Id = 1, Direccion = "206 Av Clouthier", Estado = "Jalisco", Ciudad = "Guadalajara", CodigoPostal = 20043 };
            context.Ubicacion.Add(ubicacion);

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Alejandra",
                Apellido = "Nava",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Telefono = "(33) 2344-2345",
                CorreoElectronico = "nalejandra@hotmail.com",
                UbicacionId = 1,
                EmpleadoId = 1,
            };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.GetCliente(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Cliente>>(result);
            var clientePrueba = Assert.IsType<Cliente>(actionResult.Value);
            Assert.Equal("Alejandra", clientePrueba.Nombre);
            Assert.NotNull(clientePrueba.Ubicacion);
            Assert.Equal("206 Av Clouthier", cliente.Ubicacion.Direccion);
        }

        [Fact]
        public async Task PostCliente_CreatesNewClienteAndSendsEmail()
        {   // Historia de Usuario HU001, Crear Cliente
            // Historia de Usuario HU007, Enviar correo bienvenida
            //Verifica que crea un nuevo cliente y envía un correo electrónico de bienvenida al cliente recién creado.


            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Rigoberto",
                Apellido = "Lopez",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Telefono = "(33) 2344-2345",
                CorreoElectronico = "rigoberto@hotmail.com",
                UbicacionId = 1,
                EmpleadoId = 1,
            };

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.PostCliente(cliente);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCliente = Assert.IsType<Cliente>(createdResult.Value);
            Assert.Equal("Rigoberto", createdCliente.Nombre);

            mockEmailSender.Verify(m => m.SendEmailAsync(
                cliente.CorreoElectronico,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task PutCliente_UpdatesExistingCliente()
        {   // Historia de Usuario HU002, Actualizar cliente
            //Verifica que actualiza un cliente existente correctamente.
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var rol1 = new Rol
            {
                Id = 1,
                Nombre = "Admin"
            };
            var rol2 = new Rol
            {
                Id = 2,
                Nombre = "Admin"
            };
            context.Rol.Add(rol1);
            context.Rol.Add(rol2);

            var empleado = new Empleado
            {
                Id = 1,
                Nombre = "Jorge Reyna",
                Contraseña = "contraseña123.",
                CorreoElectronico = "jrcabogadospt@gmail.com",
                RolId = 1,
            };
            context.Empleado.Add(empleado);

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Luisa",
                Apellido = "Rodriguez",
                FechaNacimiento = DateTime.Now.AddYears(-25),
                Telefono = "(33) 2344-2745",
                CorreoElectronico = "luisa@hotmail.com",
                UbicacionId = 1,
                EmpleadoId = 1,
            };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            cliente.Nombre = "Luisa Updated";
            var result = await controller.PutCliente(1, 1, cliente);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedCliente = await context.Cliente.FindAsync(1);
            Assert.NotNull(updatedCliente);
            Assert.Equal("Luisa Updated", updatedCliente.Nombre);
        }



        [Fact]
        public async Task DeleteCliente_RemovesCliente()
        {   // Historia de Usuario HU003 , Eliminar cliente
            //Verifica que elimina un cliente existente de la base de datos.
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailSender = new Mock<IEmailSender>();

            var cliente = new Cliente
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Sanchez",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                Telefono = "(33) 2344-2345",
                CorreoElectronico = "carlos@hotmail.com",
                UbicacionId = 1
            };
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var controller = new ClienteController(context, mockEmailSender.Object);

            // Act
            var result = await controller.DeleteCliente(1, 1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedCliente = await context.Cliente.FindAsync(1);
            Assert.Null(deletedCliente);
        }


    }

}
