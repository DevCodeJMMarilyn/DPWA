// favoritos.js - Funcionalidad del corazón de favoritos
document.addEventListener('DOMContentLoaded', function () {
    // Seleccionar todos los corazones en la página
    const hearts = document.querySelectorAll('.favorite');

    hearts.forEach(heart => {
        heart.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const icon = this.querySelector('i');

            if (icon.classList.contains('fa-regular')) {
                // Cambiar a corazón lleno
                icon.classList.remove('fa-regular');
                icon.classList.add('fa-solid');
                this.classList.add('active');
                console.log('Añadido a favoritos');

            } else {
                // Cambiar a corazón vacío
                icon.classList.remove('fa-solid');
                icon.classList.add('fa-regular');
                this.classList.remove('active');
                console.log('Quitado de favoritos');

            }
        });
    });
});