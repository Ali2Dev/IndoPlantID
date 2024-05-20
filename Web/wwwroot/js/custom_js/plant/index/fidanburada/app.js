async function fetchContent(plantName) {
    const encodedQuery = encodeURIComponent(plantName);
    const url = `https://www.fidanburada.com/arama?q=${encodedQuery}`;
    const proxyUrl = `http://localhost:3000/proxy?url=${encodeURIComponent(url)}`;
    try {
        const response = await fetch(proxyUrl);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const text = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(text, 'text/html');
        const resultsDiv = document.getElementById('fidan-burada-result');
        resultsDiv.innerHTML = '';

        const catalogElement = doc.querySelector('#catalog335');
        if (catalogElement) {
            // 3 kere içeri gir ve <a> elemanýný bul
            let currentElement = catalogElement;
            for (let i = 0; i < 3; i++) {
                if (currentElement.children.length > 0) {
                    currentElement = currentElement.children[0];
                } else {
                    resultsDiv.textContent = 'Sonuç bulunamadý. catalog335';
                    return;
                }
            }

            const anchorElement = currentElement.querySelector('a');
            if (anchorElement) {
                const fullUrl = `https://www.fidanburada.com${anchorElement.getAttribute('href')}`;
                resultsDiv.textContent = fullUrl;

                // Yeni sayfadan istenilen içeriði al
                fetchNewPageContent(fullUrl);
            } else {
                resultsDiv.textContent = 'Sonuç bulunamadý. aaa';
            }
        } else {
            resultsDiv.textContent = 'Element mevcut deðil.';
        }
    } catch (error) {
        console.error('Fetch error:', error);
        document.getElementById('fidan-burada-result').textContent = 'Bir hata oluþtu.';
    }
}

async function fetchNewPageContent(url) {
    const proxyUrl = `http://localhost:3000/proxy?url=${encodeURIComponent(url)}`;
    try {
        const response = await fetch(proxyUrl);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const text = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(text, 'text/html');
        const resultsDiv = document.getElementById('fidan-burada-result');

        const trElement = doc.querySelector('tr[style="height: 53px;"]');
        if (trElement) {
            const tdElement = trElement.querySelector('td');
            if (tdElement) {
                // Ýlk span elementini kaldýr
                const firstSpan = tdElement.querySelector('span');
                if (firstSpan) {
                    firstSpan.remove();
                }
                resultsDiv.innerHTML = tdElement.innerHTML; // Ýçeriði HTML olarak ekle
            } else {
                resultsDiv.textContent = 'TD element bulunamadý.';
            }
        } else {
            resultsDiv.textContent = 'TR element bulunamadý.';
        }
    } catch (error) {
        console.error('Fetch error:', error);
        document.getElementById('fidan-burada-result').textContent = 'Bir hata oluþtu.';
    }
}


async function fetchContentPrice(query) {
    console.log("price calisiyo");
    const url = `https://www.fidanburada.com/arama?q=${query}`;
    const proxyUrl = `http://localhost:3000/proxy?url=${encodeURIComponent(url)}`;
    try {
        const response = await fetch(proxyUrl);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const text = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(text, 'text/html');
        const resultsDiv = document.getElementById('fidan-burada-price-result');
        resultsDiv.innerHTML = '';

        const priceRangeDiv = document.getElementById('price-range');
        priceRangeDiv.innerHTML = 'Fiyatlar burada görünecek';

        const catalogElement = doc.querySelector('#catalog335');
        if (catalogElement) {
            const productItems = Array.from(catalogElement.querySelectorAll('.col-6.col-sm-6.col-md-4.col-lg-3.col-xl-3.col-xxl-3.mb-2.product-item'));
            let totalPrices = 0;
            let minPrice = Number.MAX_VALUE;
            let maxPrice = 0;
            let prices = [];

            // Ürün bilgilerini toplama
            const products = productItems.map(item => {
                const imgElement = item.querySelector('picture.image-inner img');
                const imgSrc = imgElement ? imgElement.src : '';

                const titleElement = item.querySelector('.product-detail-card .col-12.product-title');
                const productTitle = titleElement ? titleElement.innerText : '';
                const truncatedTitle = productTitle.length > 23 ? productTitle.substring(0, 20) + "..." : productTitle;
                const productHref = titleElement ? `https://www.fidanburada.com${titleElement.getAttribute('href')}` : '';

                const priceElement = item.querySelector('.current-price strong');
                const priceText = priceElement ? priceElement.innerText : '';
                const price = parseFloat(priceText.replace(/[^\d,]/g, '').replace(',', '.'));

                if (!isNaN(price)) {
                    prices.push(price);
                    totalPrices += price;
                    if (price < minPrice) {
                        minPrice = price;
                    }
                    if (price > maxPrice) {
                        maxPrice = price;
                    }
                }

                return { imgSrc, truncatedTitle, productHref, priceText, price };
            });

            // Ürünleri fiyatlarýna göre sýralama
            products.sort((a, b) => a.price - b.price);

            // Sýralanmýþ ürünleri HTML olarak ekleme
            products.forEach(product => {
                const cardHtml = `
                    <a class="col-md-3 me-5" href="${product.productHref}" style="text-decoration: none;" target="_blank">
                        <div class="border rounded border-1 border-opacity-25 border-success mb-4 opacity-hover" style="width: 18rem; cursor: pointer;">
                            <img src="${product.imgSrc}" class="card-img-top" width="100%" height="200px" loading="lazy">
                            <div class="card-body p-2">
                                <p id="plant-name" class="text-primary">${product.truncatedTitle}</p>
                                <p class="card-text text-dark" id="price"><strong>Fiyat:</strong> ${product.priceText}</p>
                            </div>
                        </div>
                    </a>
                `;
                resultsDiv.insertAdjacentHTML('beforeend', cardHtml);
            });

            // Ortalama, minimum ve maksimum fiyatlarý hesaplama ve gösterme
            if (prices.length > 0) {
                const averagePrice = totalPrices / prices.length;
                priceRangeDiv.innerHTML = `
                    <p><span class="text-primary">Ortalama Fiyat:</span>  ${averagePrice.toFixed(2)} TL</p>
                    <p><span class="text-success">Minimum Fiyat:</span> ${minPrice.toFixed(2)} TL</p>
                    <p><span class="text-danger">Maksimum Fiyat:</span> ${maxPrice.toFixed(2)} TL</p>
                `;
            } else {
                priceRangeDiv.innerHTML = 'Fiyat bilgisi bulunamadý.';
            }
        } else {
            resultsDiv.textContent = 'Element mevcut deðil.';
        }
    } catch (error) {
        console.error('Fetch error:', error);
        document.getElementById('fidan-burada-price-result').textContent = 'Bir hata oluþtu.';
    }
}

