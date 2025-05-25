
    const lightbox = document.getElementById('lightbox');
    const lightboxImage = document.getElementById('lightbox-image');
    const lightboxCaption = document.getElementById('lightbox-caption');
    const images = document.querySelectorAll('.inside img'); // Assuming images are inside an element with class "inside"
    const closeBtn = document.querySelector('.close');

    // Function to open the lightbox
    function openLightbox(imageSrc, caption) {
        lightboxImage.src = imageSrc;
        lightboxCaption.textContent = caption;
        lightbox.classList.remove('hide'); // Show the lightbox
    }

    // Function to close the lightbox
    function closeLightbox() {
        lightbox.classList.add('hide'); // Hide the lightbox
    }

    // Add event listeners to images
    images.forEach(img => {
        img.addEventListener('click', function() {
            openLightbox(this.src, this.alt);
        });
    });

    // Add event listener to close button
    closeBtn.addEventListener('click', closeLightbox);

    // Add event listener to close the lightbox when clicking outside the image
    lightbox.addEventListener('click', function(event) {
        if (event.target === lightbox) {
            closeLightbox();
        }
    });
