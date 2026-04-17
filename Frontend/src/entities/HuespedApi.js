import { CONFIG } from '../app/config.js';

export class HuespedApi {
    static async buscarPorTermino(termino) {
        const url = `${CONFIG.API_URL}/huespedes/buscar?termino=${termino}`;
        
        const respuesta = await fetch(url);
        
        if (!respuesta.ok) {
            // Si el backend devuelve un 404 (No encontrado), devolvemos una lista vacía
            // en lugar de hacer explotar la aplicación con un error genérico.
            if (respuesta.status === 404) {
                return []; 
            }
            const dataError = await respuesta.json().catch(() => ({}));
            throw new Error(dataError.error || 'Error al buscar el huésped.');
        }
        
        return await respuesta.json(); // Devuelve la lista de huéspedes que coinciden
    }
}