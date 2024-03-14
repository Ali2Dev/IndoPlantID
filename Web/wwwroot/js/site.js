

//  JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var closeButtons = document.querySelectorAll('.message .close');
    closeButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            var message = this.closest('.message');
            message.style.opacity = '0.5';
            setTimeout(function () {
                message.style.display = 'none';
            }, 200);
        });
    });
});