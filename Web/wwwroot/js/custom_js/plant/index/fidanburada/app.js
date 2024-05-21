//async function fetchContent(plantName) {
//    const encodedQuery = encodeURIComponent(plantName);
//    const url = `https://www.fidanburada.com/arama?q=${encodedQuery}`;
//    const proxyUrl = `http://localhost:3000/proxy?url=${encodeURIComponent(url)}`;
//    try {
//        const response = await fetch(proxyUrl);
//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }
//        const text = await response.text();
//        const parser = new DOMParser();
//        const doc = parser.parseFromString(text, 'text/html');
//        const resultsDiv = document.getElementById('fidan-burada-result');
//        resultsDiv.innerHTML = '';

//        const catalogElement = doc.querySelector('#catalog335');
//        if (catalogElement) {
//            // 3 kere içeri gir ve <a> elemanını bul
//            let currentElement = catalogElement;
//            for (let i = 0; i < 3; i++) {
//                if (currentElement.children.length > 0) {
//                    currentElement = currentElement.children[0];
//                } else {
//                    resultsDiv.textContent = 'Sonuç bulunamadı. catalog335';
//                    return;
//                }
//            }

//            const anchorElement = currentElement.querySelector('a');
//            if (anchorElement) {
//                const fullUrl = `https://www.fidanburada.com${anchorElement.getAttribute('href')}`;
//                resultsDiv.textContent = fullUrl;

//                // Yeni sayfadan istenilen içeriği al
//                fetchNewPageContent(fullUrl);
//            } else {
//                resultsDiv.textContent = 'Sonuç bulunamadı. aaa';
//            }
//        } else {
//            resultsDiv.textContent = 'Element mevcut değil.';
//        }
//    } catch (error) {
//        console.error('Fetch error:', error);
//        document.getElementById('fidan-burada-result').textContent = 'Bir hata oluştu.';
//    }
//}

//async function fetchNewPageContent(url) {
//    const proxyUrl = `http://localhost:3000/proxy?url=${encodeURIComponent(url)}`;
//    try {
//        const response = await fetch(proxyUrl);
//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }
//        const text = await response.text();
//        const parser = new DOMParser();
//        const doc = parser.parseFromString(text, 'text/html');
//        const resultsDiv = document.getElementById('fidan-burada-result');

//        const trElement = doc.querySelector('tr[style="height: 53px;"]');
//        if (trElement) {
//            const tdElement = trElement.querySelector('td');
//            if (tdElement) {
//                // İlk span elementini kaldır
//                const firstSpan = tdElement.querySelector('span');
//                if (firstSpan) {
//                    firstSpan.remove();
//                }
//                resultsDiv.innerHTML = tdElement.innerHTML; // İçeriği HTML olarak ekle
//            } else {
//                resultsDiv.textContent = 'TD element bulunamadı.';
//            }
//        } else {
//            resultsDiv.textContent = 'TR element bulunamadı.';
//        }
//    } catch (error) {
//        console.error('Fetch error:', error);
//        document.getElementById('fidan-burada-result').textContent = 'Bir hata oluştu.';
//    }
//}


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
                const truncatedTitle = productTitle.length > 36 ? productTitle.substring(0, 33) + "..." : productTitle;
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

            // Ürünleri fiyatlarına göre sıralama
            products.sort((a, b) => a.price - b.price);

            // Sıralanmış ürünleri HTML olarak ekleme
            products.forEach(product => {
                const cardHtml = `
                    <a class="col-md-2" href="${product.productHref}" style="text-decoration: none;" target="_blank">
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

            // Ortalama, minimum ve maksimum fiyatları hesaplama ve gösterme
            if (prices.length > 0) {
                const averagePrice = totalPrices / prices.length;
                priceRangeDiv.innerHTML = `
                    <p class="me-4"><span class="text-success">Minimum Fiyat:</span> ${minPrice.toFixed(2)} TL</p>
                    <p class="me-4"><span class="text-primary">Ortalama Fiyat:</span>  ${averagePrice.toFixed(2)} TL</p>
                    <p><span class="text-danger">Maksimum Fiyat:</span> ${maxPrice.toFixed(2)} TL</p>
                `;
            } else {
                priceRangeDiv.innerHTML = 'Fiyat bilgisi bulunamadi.';
                fetchContentPriceBahceMarket(query);
            }
        } else {
            resultsDiv.textContent = 'Element mevcut degil.';
            fetchContentPriceBahceMarket(query);
        }
    } catch (error) {
        console.error('Fetch error:', error);
        document.getElementById('fidan-burada-price-result').textContent = 'Bir hata olustu.';
        fetchContentPriceBahceMarket(query);
    }
}



async function fetchContentPriceBahceMarket(query) {
    const encodedQuery = encodeURIComponent(query);
    const url = `https://www.bahcemarket.com/Arama?1&kelime=${encodedQuery}`;
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
        const priceRangeDiv = document.getElementById('price-range');
        resultsDiv.innerHTML = '';
        priceRangeDiv.innerHTML = '';

        const productPageElement = doc.querySelector('#ProductPageProductList');
        if (productPageElement) {
            console.log('ProductPageProductList element found');
            const allChildren = productPageElement.querySelectorAll('*');
            console.log('All child elements:', allChildren);

            const productItems = Array.from(allChildren).filter(element =>
                element.classList.contains('ItemOrj') &&
                (element.classList.contains('col-lg-3') ||
                    element.classList.contains('col-md-3') ||
                    element.classList.contains('col-sm-6') ||
                    element.classList.contains('col-xs-6'))
            );
            console.log(`Found ${productItems.length} items`);

            const products = [];

            productItems.forEach((item, index) => {
                console.log(`Processing item ${index + 1}/${productItems.length}`, item);

                const productImageElement = item.querySelector('.productImage');
                console.log('ProductImage element:', productImageElement);
                const detailLinkElement = productImageElement ? productImageElement.querySelector('a.detailLink.detailUrl') : null;
                console.log('DetailLink element:', detailLinkElement);
                const imgElement = detailLinkElement ? detailLinkElement.querySelector('img[data-resize-target=".productImage"]') : null;
                console.log('Img element:', imgElement);
                const productPriceElement = item.querySelector('.productPrice .discountPrice span');
                console.log('ProductPrice element:', productPriceElement);

                if (detailLinkElement && imgElement && productPriceElement) {
                    const productHref = `https://www.bahcemarket.com${detailLinkElement.getAttribute('href')}`;
                    const productTitle = detailLinkElement.getAttribute('title');
                    const imgSrc = imgElement.getAttribute('data-original');
                    const priceText = productPriceElement.innerText.replace('₺', '').trim();

                    // Fiyatı doğru parse et
                    const price = parseFloat(priceText.replace('.', '').replace(',', '.'));

                    if (!isNaN(price)) {
                        products.push({
                            href: productHref,
                            title: productTitle,
                            src: imgSrc,
                            price: price,
                            priceText: productPriceElement.innerText
                        });
                    }

                    console.log('Product details:', {
                        href: productHref,
                        title: productTitle,
                        src: imgSrc,
                        price: priceText
                    });
                } else {
                    console.log(`Required elements not found for item ${index + 1}/${productItems.length}`);
                }
            });

            // Ürünleri fiyatlarına göre sıralayalım
            products.sort((a, b) => a.price - b.price);

            // Sıralanmış ürünleri ekrana yazdıralım
            products.forEach(product => {
                const cardHtml = `

                    <a class="col-md-2" href="${product.href}" style="text-decoration: none;" target="_blank">
    <div class="border rounded border-1 border-opacity-25 border-success mb-4 opacity-hover" style="width: 18rem; cursor: pointer;">
        <img src="${product.src}" class="card-img-top" width="100%" height="200px" loading="lazy">
        <div class="card-body p-2">
            <p id="plant-name" class="text-primary">${product.title}</p>
            <p class="card-text text-dark" id="price"><strong>Fiyat:</strong> ${product.priceText}</p>
        </div>
    </div>
</a>
                `;
                resultsDiv.insertAdjacentHTML('beforeend', cardHtml);
            });

            // Fiyat hesaplamaları
            const totalPrice = products.reduce((total, product) => total + product.price, 0);
            const averagePrice = totalPrice / products.length;
            const minPrice = Math.min(...products.map(product => product.price));
            const maxPrice = Math.max(...products.map(product => product.price));

            const priceRangeHtml = `
                <p class="me-4"><span class="text-success">Minimum Fiyat:</span> ${minPrice.toFixed(2)} TL</p>
                <p class="me-4"><span class="text-primary">Ortalama Fiyat:</span>  ${averagePrice.toFixed(2)} TL</p>
                <p><span class="text-danger">Maksimum Fiyat:</span> ${maxPrice.toFixed(2)} TL</p>
            `;
            priceRangeDiv.insertAdjacentHTML('beforeend', priceRangeHtml);
        } else {
            resultsDiv.textContent = 'Fiyat bilgisi alinamadi.';
        }
    } catch (error) {
        console.error('Fetch error:', error);
        document.getElementById('fidan-burada-price-result').textContent = 'Bir hata olustu.';
    }
}
