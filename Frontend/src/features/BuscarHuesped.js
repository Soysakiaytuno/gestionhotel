import { HuespedApi } from '../entities/HuespedApi.js';

export class BuscarHuesped {
    static resultadosTemporales = [];

    // Recibe el estado y una función (callback) para repintar la lista en el widget padre
    static async ejecutar(estado, onHuespedAgregado) {
        const inputBusqueda = document.getElementById('input-dni').value.trim();
        const contenedor = document.getElementById('contenedor-huespedes');

        if (!inputBusqueda) {
            alert("Ingresa un nombre, apellido o documento para buscar.");
            return;
        }

        contenedor.innerHTML = '<p class="text-center">Buscando...</p>';

        try {
            const resultados = await HuespedApi.buscarPorTermino(inputBusqueda);
            
            if (resultados.length === 0) {
                contenedor.innerHTML = '<p class="text-center text-danger">Huésped no encontrado.</p>';
                if (estado.huespedesAgregados.length > 0) {
                    setTimeout(() => onHuespedAgregado(), 2000); // Vuelve a mostrar los agregados
                }
                return;
            }

            this.resultadosTemporales = resultados;
            let html = '<p class="text-muted" style="margin-bottom: 0.5rem;">Resultados de búsqueda:</p>';
            
            resultados.forEach(h => {
                html += `
                    <div class="item-seleccion">
                        <div>
                            <strong>${h.nombreCompleto}</strong> <br>
                            <small class="text-muted">DNI/CI: ${h.documento} | Tel: ${h.telefono}</small>
                        </div>
                        <button class="btn btn-outline btn-agregar-huesped" data-id="${h.idHuesped}" style="padding: 0.25rem 0.75rem; font-size: 0.9rem;">
                            Añadir
                        </button>
                    </div>
                `;
            });
            contenedor.innerHTML = html;

            document.querySelectorAll('.btn-agregar-huesped').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const id = parseInt(e.target.getAttribute('data-id'));
                    this.agregarSeleccionado(id, estado, onHuespedAgregado);
                });
            });

        } catch (error) {
            contenedor.innerHTML = `<p class="text-center text-danger">Error: ${error.message}</p>`;
        }
    }

    static agregarSeleccionado(idHuesped, estado, onHuespedAgregado) {
        const huesped = this.resultadosTemporales.find(h => h.idHuesped === idHuesped);
        if (!huesped) return;

        if (estado.huespedesAgregados.find(h => h.idHuesped === huesped.idHuesped)) {
            alert("Este huésped ya fue agregado a la lista.");
            onHuespedAgregado(); 
            return;
        }

        estado.huespedesAgregados.push(huesped);
        
        if (estado.idHuespedTitular === null) {
            estado.idHuespedTitular = huesped.idHuesped;
        }

        document.getElementById('input-dni').value = ''; 
        onHuespedAgregado(); // Le avisa al padre que redibuje la lista de seleccionados
    }
}