import { CONFIG } from '../app/config.js';

export class HabitacionApi {
    static async obtenerDisponibles(ingreso, salida) {
        // Armamos la URL con los parámetros de búsqueda (?ingreso=...&salida=...)
        const url = `${CONFIG.API_URL}/habitaciones/disponibles?ingreso=${ingreso}&salida=${salida}`;
        
        const respuesta = await fetch(url);
        
        if (!respuesta.ok) {
            const dataError = await respuesta.json().catch(() => ({}));
            throw new Error(dataError.error || 'Error al buscar habitaciones disponibles.');
        }
        
        return await respuesta.json(); // Devuelve la lista de habitaciones libres
    }
}