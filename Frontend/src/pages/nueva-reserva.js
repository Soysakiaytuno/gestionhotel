import { FormularioReserva } from '../widgets/FormularioReserva.js';

document.addEventListener('DOMContentLoaded', () => {
    // Cuando el HTML termina de cargar, le decimos al Widget que "conecte los cables"
    FormularioReserva.iniciar();
});