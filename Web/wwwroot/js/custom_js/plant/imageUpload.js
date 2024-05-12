


document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById('file-upload');
    const submitButton = document.querySelector('.btn[type="submit"]');
    const notImageDiv = document.getElementById('notimage');
    const fileImage = document.getElementById('file-image');

    // Baþlangýçta submit butonunu devre dýþý býrak
    submitButton.disabled = true;

    fileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file && file.type.match(/image.*/)) {
            // Resim dosyasý ise
            const reader = new FileReader();
            reader.onload = function (e) {
                fileImage.src = e.target.result;
                fileImage.classList.remove("hidden");
                notImageDiv.classList.add("hidden");
                notImageDiv.classList.add("text-danger");
                submitButton.disabled = false; // Butonu etkinleþtir
            };
            reader.readAsDataURL(file);
        } else {
            // Resim dosyasý deðilse
            fileImage.classList.add("hidden");
            notImageDiv.classList.remove("hidden");
            /*notImageDiv.classList.remove("text-danger");*/
            submitButton.disabled = true; // Butonu devre dýþý býrak
        }
    });
});







function ekUpload() {
    function Init() {
        console.log("Upload Initialised");

        var fileSelect = document.getElementById('file-upload'),
            fileDrag = document.getElementById('file-drag');

        fileSelect.addEventListener('change', fileSelectHandler, false);

        // Drag and Drop listeners
        fileDrag.addEventListener('dragover', fileDragHover, false);
        fileDrag.addEventListener('dragleave', fileDragHover, false);
        fileDrag.addEventListener('drop', fileSelectHandler, false);
    }

    function fileDragHover(e) {
        e.stopPropagation();
        e.preventDefault();
        e.target.className = (e.type === 'dragover' ? 'hover' : '');
    }

    function fileSelectHandler(e) {
        // Prevent the form from submitting if the user drops the file
        e.stopPropagation();
        e.preventDefault();

        // Get the files from the input or drop event
        var files = e.target.files || e.dataTransfer.files;

        // Process all files
        for (var i = 0, f; f = files[i]; i++) {
            parseFile(f);
        }
    }

    function parseFile(file) {
        console.log("Parsing file:", file.name);
        // Check for file type
        if (/\.(jpe?g|png|gif)$/i.test(file.name)) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('file-image').src = e.target.result;
                document.getElementById('file-image').classList.remove("hidden");
            };
            reader.readAsDataURL(file);
        }
    }

    // Check for File API support
    if (window.File && window.FileList && window.FileReader) {
        Init();
    } else {
        document.getElementById('file-drag').style.display = 'none';
    }
}



ekUpload();




