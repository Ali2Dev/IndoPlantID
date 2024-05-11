document.querySelectorAll('.password').forEach(input => {
    let icon = input.nextElementSibling; // input'un hemen yan�ndaki ikonu se�er
    icon.style.cursor = 'pointer'; // �kona t�klanabilirlik hissi vermek i�in cursor'u pointer yapar
    icon.addEventListener('click', function () {
        if (input.type === 'password') {
            input.type = 'text';
            icon.className = 'bi bi-unlock-fill'; // �konu unlock olarak de�i�tir
        } else {
            input.type = 'password';
            icon.className = 'bi bi-lock-fill'; // �konu lock olarak de�i�tir
        }
    });
});


// Yeni �ifre ve �ifre tekrar inputlar� i�in event listener ekleniyor
document.getElementById('new-password').addEventListener('keyup', function () {
    updatePasswordStrength(this, document.getElementById('strong').querySelector('span'));
});

document.getElementById('confirm-password').addEventListener('keyup', function () {
    updatePasswordStrength(this, document.getElementById('strong').querySelector('span'));
});

// �ifre g�c�n� g�ncelleyen fonksiyon
function updatePasswordStrength(inputElement, strengthIndicator) {
    var strength = 'weak'; // Ba�lang�� g�� de�eri
    var value = inputElement.value; // Input de�eri
    if (value.length > 5 && /\d/.test(value)) { // �ifre uzunlu�u ve rakam i�erip i�ermedi�i
        strength = 'medium';
    }
    if (value.length > 6 && /[^\w\s]/gi.test(value)) { // �ifre uzunlu�u ve �zel karakter i�erip i�ermedi�i
        strength = 'strong';
    }
    strengthIndicator.className = strength; // G�� g�stergesi i�in class g�ncellemesi
    strengthIndicator.innerHTML = strength; // G�� g�stergesi i�in text g�ncellemesi
}

