import { BuscarHabitaciones } from '../features/BuscarHabitaciones.js';
import { BuscarHuesped } from '../features/BuscarHuesped.js';
import { GuardarReserva } from '../features/GuardarReserva.js';

export class FormularioReserva {
    
    static estado = {
        ingreso: null,
        salida: null,
        idsHabitacionesSeleccionadas: [],
        huespedesAgregados: [], 
        idHuespedTitular: null
    };

    static iniciar() {
        document.getElementById('btn-buscar-habs').addEventListener('click', () => {
            BuscarHabitaciones.ejecutar(this.estado);
        });

        document.getElementById('btn-buscar-huesped').addEventListener('click', () => {
            BuscarHuesped.ejecutar(this.estado, () => this.renderizarHuespedes());
        });

        document.getElementById('btn-guardar-reserva').addEventListener('click', () => {
            GuardarReserva.ejecutar(this.estado);
        });
    }

    static renderizarHuespedes() {
        const contenedor = document.getElementById('contenedor-huespedes');
        
        if (this.estado.huespedesAgregados.length === 0) {
            contenedor.innerHTML = '<p class="text-muted text-center">Aún no hay huéspedes agregados.</p>';
            return;
        }

        let html = '<p class="text-muted" style="margin-bottom: 0.5rem;">Huéspedes en esta reserva:</p>';
        
        this.estado.huespedesAgregados.forEach(h => {
            const esTitular = this.estado.idHuespedTitular === h.idHuesped;
            html += `
                <div class="item-seleccion" style="${esTitular ? 'border-color: var(--primary); background-color: #f0fdf4;' : ''}">
                    <div>
                        <strong>${h.nombreCompleto}</strong> <br>
                        <small class="text-muted">DNI/CI: ${h.documento} | Tel: ${h.telefono}</small>
                    </div>
                    <div>
                        <label style="cursor: pointer;">
                            <input type="radio" name="titular" class="radio-titular" value="${h.idHuesped}" ${esTitular ? 'checked' : ''}>
                            Es Titular
                        </label>
                    </div>
                </div>
            `;
        });
        contenedor.innerHTML = html;

        document.querySelectorAll('.radio-titular').forEach(radio => {
            radio.addEventListener('change', (e) => {
                this.estado.idHuespedTitular = parseInt(e.target.value);
                this.renderizarHuespedes(); 
            });
        });
    }
}