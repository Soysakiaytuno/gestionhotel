// Herramientas genéricas para que los datos se vean bien en pantalla

export const formatters = {
    // Convierte "2023-10-01T00:00:00" a "01/10/2023"
    fecha: (fechaIso) => {
        if (!fechaIso) return "-";
        const fecha = new Date(fechaIso);
        return fecha.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
    },
    
    // Convierte 150 a "$ 150.00" (Puedes cambiar la moneda si gustas)
    moneda: (monto) => {
        if (monto == null) return "-";
        return new Intl.NumberFormat('es-BO', { style: 'currency', currency: 'BOB' }).format(monto);
    }
};