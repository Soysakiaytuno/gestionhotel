import { Modal } from '../shared/ui/Modal.js';
import { EstadiaApi } from '../entities/EstadiaApi.js';
import { formatters } from '../shared/utils/formatters.js';

export class CheckOut {
    static ejecutar(idEstadia, titular, onSuccess) {
        const cuerpo = `<p>¿Procesar la salida del titular <strong>${titular}</strong> de la reserva #${idEstadia}?</p>`;
        const botones = `
            <button class="btn btn-outline" id="btn-cancelar-modal">Cancelar</button>
            <button class="btn btn-danger" id="btn-confirmar-checkout">Procesar Salida</button>
        `;

        Modal.mostrar(`Check-Out - Reserva #${idEstadia}`, cuerpo, botones);

        document.getElementById('btn-cancelar-modal').addEventListener('click', () => Modal.cerrar());
        
        document.getElementById('btn-confirmar-checkout').addEventListener('click', async () => {
            try {
                document.getElementById('btn-confirmar-checkout').innerText = "Calculando...";
                
                const resultado = await EstadiaApi.marcarCheckOut(idEstadia);
                
                const cuerpoExito = `
                    <div style="text-align: center;">
                        <h3 style="color: var(--success); margin-bottom: 1rem;">¡Check-Out Exitoso!</h3>
                        <p>Días cobrados: <strong>${resultado.diasCobrados}</strong></p>
                        <p style="font-size: 1.5rem; margin-top: 1rem;">Total: <strong>${formatters.moneda(resultado.totalCobrado)}</strong></p>
                    </div>
                `;
                const btnCerrar = `<button class="btn btn-primary" id="btn-cerrar-exito">Finalizar</button>`;
                
                Modal.mostrar("Resumen de Cobro", cuerpoExito, btnCerrar);
                
                document.getElementById('btn-cerrar-exito').addEventListener('click', () => {
                    Modal.cerrar();
                    onSuccess();
                });

            } catch (error) {
                alert("Error: " + error.message);
                Modal.cerrar();
            }
        });
    }
}