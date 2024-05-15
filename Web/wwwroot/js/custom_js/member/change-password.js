document.querySelectorAll('.password').forEach(input => {
    let icon = input.nextElementSibling; // input'un hemen yanýndaki ikonu seçer
    icon.style.cursor = 'pointer'; // Ýkona týklanabilirlik hissi vermek için cursor'u pointer yapar
    icon.addEventListener('click', function () {
        if (input.type === 'password') {
            input.type = 'text';
            icon.className = 'bi bi-unlock-fill'; // Ýkonu unlock olarak deðiþtir
        } else {
            input.type = 'password';
            icon.className = 'bi bi-lock-fill'; // Ýkonu lock olarak deðiþtir
        }
    });
});


// Yeni þifre ve þifre tekrar inputlarý için event listener ekleniyor
document.getElementById('new-password').addEventListener('keyup', function () {
    updatePasswordStrength(this, document.getElementById('strong').querySelector('span'));
});

document.getElementById('confirm-password').addEventListener('keyup', function () {
    updatePasswordStrength(this, document.getElementById('strong').querySelector('span'));
});

// Þifre gücünü güncelleyen fonksiyon
function updatePasswordStrength(inputElement, strengthIndicator) {
    var strength = 'ZAYIF'; // Baþlangýç güç deðeri
    var value = inputElement.value; // Input deðeri
    if (value.length > 7 && /\d/.test(value)) { // Þifre uzunluðu ve rakam içerip içermediði
        strength = 'NORMAL';
    }
    if (value.length > 10 && /[^\w\s]/gi.test(value)) { // Þifre uzunluðu ve özel karakter içerip içermediði
        strength = 'HARIKA';
    }
    strengthIndicator.className = strength; // Güç göstergesi için class güncellemesi
    strengthIndicator.innerHTML = strength; // Güç göstergesi için text güncellemesi
}

