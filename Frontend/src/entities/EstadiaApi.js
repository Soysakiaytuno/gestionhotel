import { CONFIG } from '../app/config.js';

export class EstadiaApi {
    static async obtenerActivas() {
        try {
            const respuesta = await fetch(`${CONFIG.API_URL}/estadias/activas`);
            if (!respuesta.ok) throw new Error('Error al conectar con el servidor');
            return await respuesta.json();
        } catch (error) {
            console.error("Error en EstadiaApi:", error);
            return [];
        }
    }

    static async crearReserva(peticionDatos) {
        const respuesta = await fetch(`${CONFIG.API_URL}/estadias`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(peticionDatos)
        });

        if (!respuesta.ok) {
            const error = await respuesta.json();
            throw new Error(error.error || 'Error al registrar la estadía.');
        }
        
        return await respuesta.json();
    }

    static async marcarCheckIn(idEstadia) {
        const respuesta = await fetch(`${CONFIG.API_URL}/estadias/${idEstadia}/checkin`, {
            method: 'POST'
        });
        if (!respuesta.ok) {
            const error = await respuesta.json();
            throw new Error(error.error || 'Error desconocido');
        }
        return await respuesta.json();
    }

    static async marcarCheckOut(idEstadia) {
        const respuesta = await fetch(`${CONFIG.API_URL}/estadias/${idEstadia}/checkout`, {
            method: 'POST'
        });
        if (!respuesta.ok) {
            const error = await respuesta.json();
            throw new Error(error.error || 'Error desconocido');
        }
        return await respuesta.json();
    }
}