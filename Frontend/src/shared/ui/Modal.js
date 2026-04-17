// Clase genérica para manipular la ventana emergente en pantalla
export class Modal {
    static init() {
        this.overlay = document.getElementById('modal-root');
        this.titulo = document.getElementById('modal-titulo');
        this.cuerpo = document.getElementById('modal-cuerpo');
        this.footer = document.getElementById('modal-footer');
        this.btnCerrar = document.getElementById('modal-btn-cerrar');

        // Cerrar al hacer clic en la "X"
        this.btnCerrar.addEventListener('click', () => this.cerrar());
        
        // Cerrar al hacer clic fuera de la tarjeta (en el fondo oscuro)
        this.overlay.addEventListener('click', (e) => {
            if (e.target === this.overlay) this.cerrar();
        });
    }

    static mostrar(titulo, htmlCuerpo, htmlBotones) {
        if (!this.overlay) this.init(); // Inicializa si es la primera vez

        this.titulo.textContent = titulo;
        this.cuerpo.innerHTML = htmlCuerpo;
        this.footer.innerHTML = htmlBotones;
        
        this.overlay.classList.remove('hidden');
    }

    static cerrar() {
        if (this.overlay) {
            this.overlay.classList.add('hidden');
            // Limpiamos el contenido para el próximo uso
            this.cuerpo.innerHTML = '';
            this.footer.innerHTML = '';
        }
    }
}