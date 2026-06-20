using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HotelBackend.Controllers;
using HotelBackend.Repository;
using HotelBackend.Models.ModuloUsuarios;
using HotelBackend.Models.ModuloEstadias;

namespace MiProyectoBackend.Tests
{
    public class UserStoriesTests
    {
        [Fact]
        public async Task RF02_BuscarHuesped_ConTerminoVacio_RetornaBadRequest()
        {
            var controller = new HuespedesController(null!);

            var resultado = await controller.Buscar("");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);
        }
/*
        [Fact]
        public async Task RF02_BuscarHuesped_ConTerminoInexistente_RetornaNotFound()
        {
            var mockRepo = new Mock<RepositorioHuesped>();
            
            mockRepo.Setup(r => r.BuscarPorTerminoAsync(It.IsAny<string>()))
                    .ReturnsAsync(new List<Huesped>()); // Simula BD vacía

            var controller = new HuespedesController(mockRepo.Object);

            var resultado = await controller.Buscar("DocumentoFalso123");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Contains("Usuario no encontrado", notFoundResult.Value!.ToString());
        }
*/
        [Fact]
        public void RF05_MarcarCheckIn_EstadiaEnEstadoProgramada_DebeCambiarAEnCurso()
        {
            var estadia = Estadia.CrearNuevaReserva(DateTime.Now.AddDays(1), DateTime.Now.AddDays(5));

            estadia.MarcarCheckIn();

            Assert.Equal("En Curso", estadia.Estado);
        }

        [Fact]
        public void RF05_MarcarCheckIn_EstadiaYaEnCurso_DebeLanzarExcepcion()
        {
            var estadia = Estadia.CrearNuevaReserva(DateTime.Now.AddDays(1), DateTime.Now.AddDays(5));
            
            estadia.MarcarCheckIn();
            
            Assert.Throws<Exception>(() => estadia.MarcarCheckIn());
        }


        [Fact]
        public void RF06_MarcarCheckOut_EstadiaEnCurso_CalculaCobroYFinaliza()
        {
            var estadia = Estadia.CrearNuevaReserva(DateTime.Now, DateTime.Now.AddDays(3));
            estadia.MarcarCheckIn(); // Cambia a "En Curso" y asigna FechaCheckInReal
            decimal precioPorNoche = 150.0m; // Precio simulado de la habitación

            estadia.MarcarCheckOut(precioPorNoche);

            Assert.Equal("Finalizada", estadia.Estado);
            Assert.NotNull(estadia.FechaCheckOutReal);
            Assert.Equal(150.0m, estadia.MontoTotal);
        }
        [Fact]
        public void RF06_MarcarCheckOut_CalculaCobroYFinaliza()
        {
            var estadia = Estadia.CrearNuevaReserva(DateTime.Now, DateTime.Now.AddDays(3));
            estadia.MarcarCheckIn();
            decimal precioPorNoche = 150.0m; 
            estadia.MarcarCheckOut(precioPorNoche);
            var cobro = new Cobro();
            decimal resultado = cobro.calcularCobro(estadia, precioPorNoche);
            Assert.Equal(150.0m, resultado);
        }
        [Fact]
        public void RF03_CrearNuevaReserva_FechaSalidaMenorOIgualAFechaIngreso()
        {
            DateTime fechaIngreso = DateTime.Now.AddDays(10);
            DateTime fechaSalida = DateTime.Now.AddDays(5); 
            var resultado = Assert.Throws<ArgumentException>(() => Estadia.ValidarFechaEstadia(fechaIngreso, fechaSalida));
            Assert.Contains("La fecha de salida no puede ser mayor a la de ingreso", resultado.Message);
        }
    }
}
