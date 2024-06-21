document.querySelectorAll('.password').forEach(input => {
    let icon = input.nextElementSibling;
    if (icon && icon.tagName === 'I') {
        icon.style.cursor = 'pointer';
        icon.addEventListener('click', function () {
            if (input.type === 'password') {
                input.type = 'text';
                icon.className = 'bi bi-unlock-fill'; // Ýkonu unlock olarak deðiþtir
            } else {
                input.type = 'password';
                icon.className = 'bi bi-lock-fill'; // Ýkonu lock olarak deðiþtir
            }
        });
    }
});