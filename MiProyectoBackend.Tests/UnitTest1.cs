using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBackend.Controllers;
using HotelBackend.Repository;
using HotelBackend.Models.ModuloUsuarios;
using HotelBackend.Models.ModuloEstadias;

namespace MiProyectoBackend.Tests
{
    public class UserStoriesTests
    {
        // =========================================================================
        // HISTORIA DE USUARIO: RF02 - Búsqueda de Personas (Huéspedes)
        // =========================================================================

        [Fact]
        public async Task RF02_BuscarHuesped_ConTerminoVacio_RetornaBadRequest()
        {
            // Arrange: No necesitamos mockear el repositorio porque el controlador 
            // detiene la petición antes de consultar la BD.
            var controller = new HuespedesController(null!);

            // Act
            var resultado = await controller.Buscar("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task RF02_BuscarHuesped_ConTerminoInexistente_RetornaNotFound()
        {
            // Arrange: CA2 - Dado que el documento no existe en la base de datos...
            // Utilizamos Moq para crear un repositorio falso que retorne una lista vacía.
            var mockRepo = new Mock<RepositorioHuesped>();
            
            mockRepo.Setup(r => r.BuscarPorTerminoAsync(It.IsAny<string>()))
                    .ReturnsAsync(new List<Huesped>()); // Simula BD vacía

            var controller = new HuespedesController(mockRepo.Object);

            // Act: Cuando busco un documento que no existe
            var resultado = await controller.Buscar("DocumentoFalso123");

            // Assert: Entonces muestra un mensaje indicando que no hay resultados
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Contains("Usuario no encontrado", notFoundResult.Value!.ToString());
        }

        // =========================================================================
        // HISTORIA DE USUARIO: RF05 - Ejecución de Check-In
        // =========================================================================
        
        [Fact]
        public void RF05_MarcarCheckIn_EstadiaEnEstadoProgramada_DebeCambiarAEnCurso()
        {
            // Arrange: CA1 - Dado que tengo una estadía en estado "Programada"
            var estadia = new Estadia
            {
                IdEstadia = 1,
                Estado = "Programada"
            };

            // Act: Cuando ejecuto la acción de Check-In
            estadia.MarcarCheckIn();

            // Assert: Entonces el estado cambia a "En Curso"
            Assert.Equal("En Curso", estadia.Estado);
        }

        [Fact]
        public void RF05_MarcarCheckIn_EstadiaYaEnCurso_DebeLanzarExcepcion()
        {
            // Arrange: CA2 - Dado que una estadía ya se encuentra "En Curso"
            var estadia = new Estadia 
            { 
                IdEstadia = 2,
                Estado = "En Curso" 
            };

            // Act & Assert: Cuando intento realizar la acción, lanza un error / no me deja
            Assert.Throws<InvalidOperationException>(() => estadia.MarcarCheckIn());
        }
    }
}
