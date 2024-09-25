//using JRC_Abogados.Server.Controllers;
//using JRC_Abogados.Server.DataBaseContext;
//using JRC_Abogados.Server.Models;
//using JRC_Abogados.Server.Models.EmailHelper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;

//namespace TestProject
//{
//    public class CasoControllerTests
//    {
//        private DBaseContext GetInMemoryDbContext()
//        {
//            var options = new DbContextOptionsBuilder<DBaseContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;

//            return new DBaseContext(options);
//        }

//        [Fact]
//        public async Task GetCasos_ReturnsCasosWithDetails()

//        {    //HU013 Verifica que pueda filtrar casos por su información para ubicar registro rápidamente.
//             // Arrange

//            var context = GetInMemoryDbContext();
//            var mockEmailSender = new Mock<IEmailSender>();

//            var ubicacion = new Ubicacion { Id = 1, Direccion = "206 Av Clouthier", Estado = "Jalisco", Ciudad = "Guadalajara", CodigoPostal = 20043 };
//            var cliente = new Cliente
//            {
//                Id = 1,
//                Nombre = "Alejandra",
//                Apellido = "Nava",
//                FechaNacimiento = DateTime.Now.AddYears(-30),
//                Telefono = "(33) 2344-2345",
//                CorreoElectronico = "nalejandra@hotmail.com",
//                UbicacionId = 1
//            };
//            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Demandante" };
//            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado primero", NumeroExpediente = "X/220100" };
//            var estado = new Estado { Id = 1, Nombre = "Abierto" };

//            context.Ubicacion.Add(ubicacion);
//            context.Cliente.Add(cliente);
//            context.TipoCaso.Add(tipoCaso);
//            context.Juzgado.Add(juzgado);
//            context.Estado.Add(estado);

//            var caso = new Caso
//            {
//                Id = 1,
//                TipoCasoId = 1,
//                JuzgadoId = 1,
//                UbicacionId = 1,
//                EstadoId = 1,
//                ClienteId = 1,
//                Descripcion = "Esto es una prueba",
//                FechaInicio = DateTime.Now,
//                FechaTermino = DateTime.Now,
//            };

//            context.Caso.Add(caso);
//            await context.SaveChangesAsync();

//            var controller = new CasoController(context, mockEmailSender.Object);

//            // Act
//            var result = await controller.GetCasos();

//            // Assert
//            var actionResult = Assert.IsType<ActionResult<IEnumerable<Caso>>>(result);
//            var casos = Assert.IsAssignableFrom<IEnumerable<Caso>>(actionResult.Value);
//            var casoResult = casos.FirstOrDefault();
//            Assert.NotNull(casoResult);
//            Assert.Equal("Demandante", casoResult.TipoCaso.Nombre);
//            Assert.Equal("Juzgado primero", casoResult.Juzgado.Nombre);
//            Assert.Equal("206 Av Clouthier", casoResult.Ubicacion.Direccion);
//            Assert.Equal("Abierto", casoResult.Estado.Nombre);
//            Assert.Equal("Alejandra", casoResult.Cliente.Nombre);
//        }

//        [Fact]
//        public async Task GetCaso_ReturnsCasoWithDetails()
//        {
//            //HU012, Verifica que se puedad obtner detalles de un caso en especifico y se pueda visualizar.
//            // Arrange

//            var context = GetInMemoryDbContext();
//            var mockEmailSender = new Mock<IEmailSender>();

//            var ubicacion = new Ubicacion { Id = 1, Direccion = "206 Av Clouthier", Estado = "Jalisco", Ciudad = "Guadalajara", CodigoPostal = 20043 };
//            var cliente = new Cliente
//            {
//                Id = 1,
//                Nombre = "Alejandra",
//                Apellido = "Nava",
//                FechaNacimiento = DateTime.Now.AddYears(-30),
//                Telefono = "(33) 2344-2345",
//                CorreoElectronico = "nalejandra@hotmail.com",
//                UbicacionId = 1
//            };
//            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Demandante" };
//            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado primero", NumeroExpediente = "X/220100" };
//            var estado = new Estado { Id = 1, Nombre = "Abierto" };

//            context.Ubicacion.Add(ubicacion);
//            context.Cliente.Add(cliente);
//            context.TipoCaso.Add(tipoCaso);
//            context.Juzgado.Add(juzgado);
//            context.Estado.Add(estado);

//            var caso = new Caso
//            {
//                Id = 1,
//                TipoCasoId = 1,
//                JuzgadoId = 1,
//                UbicacionId = 1,
//                EstadoId = 1,
//                ClienteId = 1,
//                Descripcion = "Esto es una prueba",
//                FechaInicio = DateTime.Now,
//                FechaTermino = DateTime.Now,
//            };

//            context.Caso.Add(caso);
//            await context.SaveChangesAsync();

//            var controller = new CasoController(context, mockEmailSender.Object);

//            // Act
//            var result = await controller.GetCaso(1);

//            // Assert
//            var actionResult = Assert.IsType<ActionResult<Caso>>(result);
//            var casoResult = Assert.IsType<Caso>(actionResult.Value);
//            Assert.NotNull(casoResult);
//            Assert.Equal("Demandante", casoResult.TipoCaso.Nombre);
//            Assert.Equal("Juzgado primero", casoResult.Juzgado.Nombre);
//            Assert.Equal("206 Av Clouthier", casoResult.Ubicacion.Direccion);
//            Assert.Equal("Abierto", casoResult.Estado.Nombre);
//            Assert.Equal("Alejandra", casoResult.Cliente.Nombre);
//        }

//        [Fact]
//        public async Task PostCaso_CreatesNewCasoAndSendsEmail()
//        {
//            //HU008 y HU014 valida que se pueda registrar un nuevo caso en el sistema y que se envíe un correo electrónico al cliente para notificacion.
//            // Arrange
//            var context = GetInMemoryDbContext();
//            var mockEmailSender = new Mock<IEmailSender>();

//            var ubicacion = new Ubicacion { Id = 1, Direccion = "206 Av Clouthier", Estado = "Jalisco", Ciudad = "Guadalajara", CodigoPostal = 20043 };
//            context.Ubicacion.Add(ubicacion);

//            var cliente = new Cliente
//            {
//                Id = 1,
//                Nombre = "Rigoberto",
//                Apellido = "Lopez",
//                CorreoElectronico = "rigoberto@hotmail.com",
//                UbicacionId = 1,
//                FechaNacimiento = DateTime.Now.AddYears(-25),
//                Telefono = "(33) 2344-2348",
//            };
//            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Demandante" };
//            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado primero", NumeroExpediente = "X/220100" };
//            var estado = new Estado { Id = 1, Nombre = "Abierto" };

//            context.Cliente.Add(cliente);
//            context.TipoCaso.Add(tipoCaso);
//            context.Juzgado.Add(juzgado);
//            context.Estado.Add(estado);
//            await context.SaveChangesAsync();

//            var caso = new Caso
//            {
//                TipoCasoId = 1,
//                JuzgadoId = 1,
//                UbicacionId = 1,
//                EstadoId = 1,
//                ClienteId = 1,
//                Descripcion = "Esto es una prueba",
//                FechaInicio = DateTime.Now,
//                FechaTermino = DateTime.Now,
//                Cliente = cliente,
//                Ubicacion = ubicacion,
//                Juzgado = juzgado,
//                TipoCaso = tipoCaso,
//                Estado = estado
//            };

//            var controller = new CasoController(context, mockEmailSender.Object);

//            // Act
//            var result = await controller.PostCaso(caso);

//            // Assert
//            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
//            var createdCaso = Assert.IsType<Caso>(createdResult.Value);
//            Assert.Equal(1, createdCaso.Id);

//            mockEmailSender.Verify(m => m.SendEmailAsync(
//                caso.Cliente.CorreoElectronico,
//                It.IsAny<string>(),
//                It.IsAny<string>()), Times.Once);
//        }

//        [Fact]
//        public async Task PutCaso_UpdatesExistingCaso()
//        {   //HU009 valida que se puedad modificar los datos de un caso.
//            // Arrange
//            var context = GetInMemoryDbContext();
//            var mockEmailSender = new Mock<IEmailSender>();

//            var ubicacion = new Ubicacion
//            {
//                Id = 1,
//                Direccion = "206 Av Clouthier",
//                Estado = "Jalisco",
//                Ciudad = "Guadalajara",
//                CodigoPostal = 20043
//            };
//            context.Ubicacion.Add(ubicacion);

//            var cliente = new Cliente
//            {
//                Id = 1,
//                Nombre = "Maria",
//                Apellido = "Lopez",
//                CorreoElectronico = "maria@hotmail.com",
//                UbicacionId = 1,
//                FechaNacimiento = DateTime.Now.AddYears(-25),
//                Telefono = "(33) 2344-2348",
//            };
//            var tipoCaso = new TipoCaso { Id = 1, Nombre = "Demandante" };
//            var juzgado = new Juzgado { Id = 1, Nombre = "Juzgado primero", NumeroExpediente = "X/220100" };
//            var estado = new Estado { Id = 1, Nombre = "Abierto" };
//            var estado2 = new Estado { Id = 2, Nombre = "Cerrado" };

//            context.Cliente.Add(cliente);
//            context.TipoCaso.Add(tipoCaso);
//            context.Juzgado.Add(juzgado);
//            context.Estado.Add(estado);
//            context.Estado.Add(estado2);

//            var caso = new Caso
//            {
//                Id = 1,
//                TipoCasoId = 1,
//                JuzgadoId = 1,
//                UbicacionId = 1,
//                EstadoId = 1,
//                ClienteId = 1,
//                Descripcion = "Esto es una prueba",
//                FechaInicio = DateTime.Now,
//                FechaTermino = DateTime.Now
//            };

//            context.Caso.Add(caso);
//            await context.SaveChangesAsync();

//            var controller = new CasoController(context, mockEmailSender.Object);

//            // Act
//            caso.EstadoId = 2;
//            caso.Estado = estado2;

//            var result = await controller.PutCaso(1, caso);

//            // Assert
//            Assert.IsType<OkResult>(result);

//            var updatedCaso = await context.Caso.FindAsync(1);
//            Assert.NotNull(caso);
//            Assert.Equal(2, caso.EstadoId);
//        }


//        [Fact]
//        public async Task DeleteCaso_RemovesCaso()
//        {   //HU010 verifica que se pueda eliminar un caso.
//            // Arrange
//            var context = GetInMemoryDbContext();
//            var mockEmailSender = new Mock<IEmailSender>();

//            var caso = new Caso
//            {
//                Id = 1,
//                TipoCasoId = 1,
//                JuzgadoId = 1,
//                UbicacionId = 1,
//                EstadoId = 1,
//                ClienteId = 1,
//                Descripcion = "Caso de prueba",
//                FechaInicio = DateTime.Now,
//                FechaTermino = DateTime.Now.AddMonths(1)
//            };

//            context.Caso.Add(caso);
//            await context.SaveChangesAsync();

//            var controller = new CasoController(context, mockEmailSender.Object);

//            // Act
//            var result = await controller.DeleteCaso(1);

//            // Assert
//            Assert.IsType<NoContentResult>(result);
//            var deletedCaso = await context.Caso.FindAsync(1);
//            Assert.Null(deletedCaso);
//        }
//    }
//}