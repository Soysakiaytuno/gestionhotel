import { EstadiaApi } from '../entities/EstadiaApi.js';
import { formatters } from '../shared/utils/formatters.js';
import { CheckIn } from '../features/CheckIn.js';
import { CheckOut } from '../features/CheckOut.js';

export class TablaDashboard {
    static async renderizar() {
        const contenedor = document.getElementById('contenedor-tabla');
        contenedor.innerHTML = '<p class="loading-text">Cargando estadías desde la base de datos...</p>';

        const estadias = await EstadiaApi.obtenerActivas();

        if (estadias.length === 0) {
            contenedor.innerHTML = '<div style="text-align:center; padding: 2rem; background: var(--card-bg); border-radius: 0.5rem;">No hay estadías activas ni programadas en este momento.</div>';
            return;
        }

        let html = `
            <div class="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Nº Reserva</th>
                            <th>Titular</th>
                            <th>Habitaciones</th>
                            <th>Ingreso</th>
                            <th>Salida</th>
                            <th>Estado</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
        `;

        estadias.forEach(e => {
            const claseBadge = e.estado.toLowerCase().replace(' ', '-'); 
            html += `
                <tr>
                    <td><strong>#${e.idEstadia}</strong></td>
                    <td>${e.titular}</td>
                    <td>${e.habitaciones}</td>
                    <td>${formatters.fecha(e.ingreso)}</td>
                    <td>${formatters.fecha(e.salida)}</td>
                    <td><span class="badge ${claseBadge}">${e.estado}</span></td>
                    <td>
                        ${e.estado === 'Programada' ? `<button class="btn btn-success btn-checkin" data-id="${e.idEstadia}" data-titular="${e.titular}">Ingresar</button>` : ''}
                        ${e.estado === 'En Curso' ? `<button class="btn btn-danger btn-checkout" data-id="${e.idEstadia}" data-titular="${e.titular}">Salida</button>` : ''}
                    </td>
                </tr>
            `;
        });

        html += `</tbody></table></div>`;
        contenedor.innerHTML = html;

        // ==========================================
        // VINCULACIÓN DE EVENTOS
        // ==========================================
        
        // Escuchamos los clics en todos los botones verdes
        document.querySelectorAll('.btn-checkin').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const id = e.target.getAttribute('data-id');
                const titular = e.target.getAttribute('data-titular');
                CheckIn.ejecutar(id, titular, () => this.renderizar()); 
            });
        });

        // Escuchamos los clics en todos los botones rojos
        document.querySelectorAll('.btn-checkout').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const id = e.target.getAttribute('data-id');
                const titular = e.target.getAttribute('data-titular');
                CheckOut.ejecutar(id, titular, () => this.renderizar());
            });
        });
    }
}