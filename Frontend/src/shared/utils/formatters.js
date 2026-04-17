// Herramientas genéricas para que los datos se vean bien en pantalla

export const formatters = {
    fecha: (fechaIso) => {
        if (!fechaIso) return "-";
        const fecha = new Date(fechaIso);
        return fecha.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
    },
    
    moneda: (monto) => {
        if (monto == null) return "-";
        return new Intl.NumberFormat('es-BO', { style: 'currency', currency: 'BOB' }).format(monto);
    }
};