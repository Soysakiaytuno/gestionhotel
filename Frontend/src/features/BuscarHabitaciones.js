import { HabitacionApi } from '../entities/HabitacionApi.js';
import { formatters } from '../shared/utils/formatters.js';

export class BuscarHabitaciones {
    // Recibe el 'estado' global del formulario para modificarlo directamente
    static async ejecutar(estado) {
        const inputIngreso = document.getElementById('input-ingreso').value;
        const inputSalida = document.getElementById('input-salida').value;
        const contenedor = document.getElementById('contenedor-habitaciones');

        if (!inputIngreso || !inputSalida) {
            alert("Por favor, selecciona las fechas de ingreso y salida.");
            return;
        }

        // Actualizamos el estado del widget padre
        estado.ingreso = inputIngreso;
        estado.salida = inputSalida;
        estado.idsHabitacionesSeleccionadas = [];

        contenedor.innerHTML = '<p class="text-center">Buscando habitaciones...</p>';

        try {
            const habitaciones = await HabitacionApi.obtenerDisponibles(inputIngreso, inputSalida);
            
            if (habitaciones.length === 0) {
                contenedor.innerHTML = '<p class="text-center text-danger">No hay habitaciones disponibles para estas fechas.</p>';
                return;
            }

            let html = '';
            habitaciones.forEach(hab => {
                html += `
                    <div class="item-seleccion">
                        <div>
                            <strong>Habitación ${hab.numero}</strong> <span class="text-muted">(${hab.tipo} - Capacidad: ${hab.capacidad})</span>
                            <div style="color: var(--success); font-weight: bold;">${formatters.moneda(hab.precioNoche)} / noche</div>
                        </div>
                        <input type="checkbox" class="chk-habitacion" value="${hab.idHabitacion}" style="transform: scale(1.5);">
                    </div>
                `;
            });
            contenedor.innerHTML = html;

            document.querySelectorAll('.chk-habitacion').forEach(chk => {
                chk.addEventListener('change', (e) => {
                    const id = parseInt(e.target.value);
                    if (e.target.checked) {
                        estado.idsHabitacionesSeleccionadas.push(id);
                    } else {
                        estado.idsHabitacionesSeleccionadas = estado.idsHabitacionesSeleccionadas.filter(hId => hId !== id);
                    }
                });
            });

        } catch (error) {
            contenedor.innerHTML = `<p class="text-center text-danger">Error: ${error.message}</p>`;
        }
    }
}