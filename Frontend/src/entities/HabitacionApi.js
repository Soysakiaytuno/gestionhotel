import { CONFIG } from '../app/config.js';

export class HabitacionApi {
    static async obtenerDisponibles(ingreso, salida) {
        const url = `${CONFIG.API_URL}/habitaciones/disponibles?ingreso=${ingreso}&salida=${salida}`;
        
        const respuesta = await fetch(url);
        
        if (!respuesta.ok) {
            const dataError = await respuesta.json().catch(() => ({}));
            throw new Error(dataError.error || 'Error al buscar habitaciones disponibles.');
        }
        
        return await respuesta.json();
    }
}