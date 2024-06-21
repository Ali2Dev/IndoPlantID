document.querySelectorAll('.password').forEach(input => {
    let icon = input.nextElementSibling;
    if (icon && icon.tagName === 'I') {
        icon.style.cursor = 'pointer';
        icon.addEventListener('click', function () {
            if (input.type === 'password') {
                input.type = 'text';
                icon.className = 'bi bi-unlock-fill'; // �konu unlock olarak de�i�tir
            } else {
                input.type = 'password';
                icon.className = 'bi bi-lock-fill'; // �konu lock olarak de�i�tir
            }
        });
    }
});