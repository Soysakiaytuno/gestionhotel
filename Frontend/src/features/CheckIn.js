import { Modal } from '../shared/ui/Modal.js';
import { EstadiaApi } from '../entities/EstadiaApi.js';

export class CheckIn {
    // Recibimos el ID, el nombre del titular, y una función para recargar la tabla si todo sale bien
    static ejecutar(idEstadia, titular, onSuccess) {
        const cuerpo = `<p>¿Confirmar la llegada del titular <strong>${titular}</strong> para la reserva #${idEstadia}?</p>`;
        const botones = `
            <button class="btn btn-outline" id="btn-cancelar-modal">Cancelar</button>
            <button class="btn btn-success" id="btn-confirmar-checkin">Confirmar Ingreso</button>
        `;

        // 1. Abrimos el modal
        Modal.mostrar(`Check-In - Reserva #${idEstadia}`, cuerpo, botones);

        // 2. Evento para cancelar
        document.getElementById('btn-cancelar-modal').addEventListener('click', () => Modal.cerrar());
        
        // 3. Evento para confirmar enviando al Backend
        document.getElementById('btn-confirmar-checkin').addEventListener('click', async () => {
            try {
                // Cambiamos el texto del botón mientras carga
                document.getElementById('btn-confirmar-checkin').innerText = "Procesando...";
                
                await EstadiaApi.marcarCheckIn(idEstadia);
                
                Modal.cerrar();
                onSuccess(); // ¡Recargamos la tabla!
            } catch (error) {
                alert("Error: " + error.message);
                Modal.cerrar();
            }
        });
    }
}