import { EstadiaApi } from '../entities/EstadiaApi.js';

export class GuardarReserva {
    // Solo toma el estado final, valida y manda a la base de datos
    static async ejecutar(estado) {
        if (!estado.ingreso || !estado.salida) return alert("Faltan las fechas.");
        if (estado.idsHabitacionesSeleccionadas.length === 0) return alert("Debes seleccionar al menos una habitación.");
        if (estado.huespedesAgregados.length === 0) return alert("Debes agregar al menos un huésped.");
        if (estado.idHuespedTitular === null) return alert("Debes seleccionar un huésped titular.");

        const peticion = {
            Ingreso: estado.ingreso,
            Salida: estado.salida,
            IdsHabitaciones: estado.idsHabitacionesSeleccionadas,
            IdsHuespedes: estado.huespedesAgregados.map(h => h.idHuesped),
            IdHuespedTitular: estado.idHuespedTitular
        };

        const btn = document.getElementById('btn-guardar-reserva');
        btn.innerText = "Registrando...";
        btn.disabled = true;

        try {
            await EstadiaApi.crearReserva(peticion);
            alert("¡Estadía programada con éxito!");
            window.location.href = "index.html";
        } catch (error) {
            alert("Error al guardar: " + error.message);
            btn.innerText = "Registrar Estadía";
            btn.disabled = false;
        }
    }
}