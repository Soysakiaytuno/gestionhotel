import { TablaDashboard } from '../widgets/TablaDashboard.js';

// Este archivo es el que "enciende" la página index.html
document.addEventListener('DOMContentLoaded', () => {
    // Al cargar la página, le decimos al Widget que dibuje la tabla
    TablaDashboard.renderizar();
});