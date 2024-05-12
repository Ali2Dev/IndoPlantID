/*jshint esversion: 6*/

document.addEventListener('DOMContentLoaded', function () {
    const video = document.querySelector('video');
    let model;
    const cameraMode = 'environment'; // or 'user'

    const startVideoStreamPromise = navigator.mediaDevices
        .getUserMedia({
            audio: false,
            video: { facingMode: cameraMode }
        })
        .then(function (stream) {
            return new Promise(function (resolve) {
                video.srcObject = stream;
                video.onloadedmetadata = function () {
                    video.play();
                    resolve();
                };
            });
        });

    const publishable_key = 'rf_eBE2SLq9dEZYXkDRdsjU75PtjQt1';
    const toLoad = {
        model: 'ev-bitkisi-tanima',
        version: 1
    };

    const loadModelPromise = new Promise(function (resolve, reject) {
        roboflow.auth({ publishable_key })
            .load(toLoad)
            .then(function (m) {
                model = m;
                resolve();
            });
    });

    Promise.all([startVideoStreamPromise, loadModelPromise]).then(function () {
        document.body.classList.remove('loading');
        resizeCanvas();
        detectFrame();
    });

    let canvas, ctx;
    const font = '16px sans-serif';

    function videoDimensions(video) {
        const videoRatio = video.videoWidth / video.videoHeight;
        let width = video.offsetWidth,
            height = video.offsetHeight;
        const elementRatio = width / height;

        if (elementRatio > videoRatio) {
            width = height * videoRatio;
        } else {
            height = width / videoRatio;
        }

        return { width, height };
    }

    window.addEventListener('resize', resizeCanvas);

    function resizeCanvas() {
        if (canvas) {
            canvas.remove();
        }

        canvas = document.createElement('canvas');
        ctx = canvas.getContext('2d');

        const dimensions = videoDimensions(video);
        console.log(video.videoWidth, video.videoHeight, video.offsetWidth, video.offsetHeight, dimensions);

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        canvas.style.width = `${dimensions.width}px`;
        canvas.style.height = `${dimensions.height}px`;
        canvas.style.left = `${(window.innerWidth - dimensions.width) / 2}px`;
        canvas.style.top = `${(window.innerHeight - dimensions.height) / 2}px`;

        document.body.appendChild(canvas);
    }

    function renderPredictions(predictions) {
        const dimensions = videoDimensions(video);
        const scale = 1;

        ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);

        predictions.forEach(prediction => {
            const x = prediction.bbox.x;
            const y = prediction.bbox.y;
            const width = prediction.bbox.width;
            const height = prediction.bbox.height;

            ctx.strokeStyle = prediction.color;
            ctx.lineWidth = 4;
            ctx.strokeRect(
                (x - width / 2) / scale,
                (y - height / 2) / scale,
                width / scale,
                height / scale
            );

            ctx.fillStyle = prediction.color;
            const textWidth = ctx.measureText(prediction.class).width;
            const textHeight = parseInt(font, 10); // base 10
            ctx.fillRect(
                (x - width / 2) / scale,
                (y - height / 2) / scale,
                textWidth + 8,
                textHeight + 4
            );
        });

        predictions.forEach(prediction => {
            const x = prediction.bbox.x;
            const y = prediction.bbox.y;
            const width = prediction.bbox.width;
            const height = prediction.bbox.height;

            ctx.font = font;
            ctx.textBaseline = 'top';
            ctx.fillStyle = '#000';
            ctx.fillText(
                prediction.class,
                (x - width / 2) / scale + 4,
                (y - height / 2) / scale + 1
            );
        });
    }

    let prevTime;
    const pastFrameTimes = [];

    function detectFrame() {
        if (!model) {
            requestAnimationFrame(detectFrame);
            return;
        }

        model.detect(video).then(predictions => {
            requestAnimationFrame(detectFrame);
            renderPredictions(predictions);

            if (prevTime) {
                const currentTime = Date.now();
                pastFrameTimes.push(currentTime - prevTime);
                if (pastFrameTimes.length > 30) pastFrameTimes.shift();

                const total = pastFrameTimes.reduce((acc, time) => acc + time / 1000, 0);
                const fps = pastFrameTimes.length / total;
                document.getElementById('fps').textContent = Math.round(fps);
                prevTime = currentTime;
            } else {
                prevTime = Date.now();
            }
        }).catch(e => {
            console.log('CAUGHT', e);
            requestAnimationFrame(detectFrame);
        });
    }
});
