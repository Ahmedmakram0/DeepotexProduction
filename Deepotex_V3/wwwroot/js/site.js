// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * Deepotex Site JavaScript
 * Contains all functionality for the Deepotex website
 */

// Initialize all components when DOM is fully loaded
document.addEventListener('DOMContentLoaded', function() {
    initProductHoverEffects();
    initWhatsAppLink();
    initImageZoom();
    initLazyLoading();
    initMobileNavigation();
    initAlertDismissal();
    enhancedDropdownFunctionality();
});

/**
 * Product hover effects for product cards
 */
function initProductHoverEffects() {
    const productBoxes = document.querySelectorAll('.productbox');
    
    productBoxes.forEach(box => {
        // For desktop: mouseenter/mouseleave
        box.addEventListener('mouseenter', function() {
            const captionShop = this.querySelector('.captionshop');
            if (captionShop) {
                captionShop.style.display = 'flex';
                setTimeout(() => {
                    captionShop.style.opacity = '1';
                    captionShop.style.transform = 'translateY(0)';
                }, 10);
            }
        });

        box.addEventListener('mouseleave', function() {
            const captionShop = this.querySelector('.captionshop');
            if (captionShop) {
                captionShop.style.opacity = '0';
                captionShop.style.transform = 'translateY(10px)';
                setTimeout(() => {
                    captionShop.style.display = 'none';
                }, 300);
            }
        });

        // For mobile: touch events
        box.addEventListener('touchstart', function(e) {
            const captionShop = this.querySelector('.captionshop');
            if (captionShop) {
                if (captionShop.style.display === 'none' || captionShop.style.display === '') {
                    e.preventDefault(); // Prevent default only on first touch
                    captionShop.style.display = 'flex';
                    setTimeout(() => {
                        captionShop.style.opacity = '1';
                        captionShop.style.transform = 'translateY(0)';
                    }, 10);
                }
            }
        });
    });
}

/**
 * WhatsApp link handler
 */
function initWhatsAppLink() {
    const whatsappLink = document.getElementById('whatsappLink');
    if (whatsappLink) {
        const whatsappNumber = whatsappLink.getAttribute('data-whatsapp-number');
        if (whatsappNumber) {
            try {
                const cleanNumber = whatsappNumber.replace(/[^\d]/g, '');
                const whatsappUrl = `https://wa.me/${+201061212027}?text=${encodeURIComponent('Hello from Deepotex – How can we assist you?')}`;
                whatsappLink.href = whatsappUrl;
            } catch (error) {
                console.error('Error setting up WhatsApp link:', error);
            }
        }
    }
}

/**
 * Image zoom functionality for product details page
 */
function initImageZoom() {
    const productImage = document.querySelector('.product-image');
    if (productImage) {
        productImage.addEventListener('click', function() {
            // Create modal if it doesn't exist
            let modal = document.getElementById('imageZoomModal');
            if (!modal) {
                modal = document.createElement('div');
                modal.id = 'imageZoomModal';
                modal.className = 'image-zoom-modal';
                modal.innerHTML = `
                    <div class="image-zoom-content">
                        <span class="image-zoom-close">&times;</span>
                        <img class="image-zoom-img">
                    </div>
                `;
                document.body.appendChild(modal);
                
                // Add close functionality
                const closeBtn = modal.querySelector('.image-zoom-close');
                closeBtn.addEventListener('click', function() {
                    modal.style.display = 'none';
                });
                
                // Close when clicking outside the image
                modal.addEventListener('click', function(e) {
                    if (e.target === modal) {
                        modal.style.display = 'none';
                    }
                });
            }
            
            // Set the image source and show modal
            const modalImg = modal.querySelector('.image-zoom-img');
            modalImg.src = this.src;
            modal.style.display = 'flex';
        });
    }
}

/**
 * Lazy loading for images
 */
function initLazyLoading() {
    if ('IntersectionObserver' in window) {
        const lazyImages = document.querySelectorAll('img[loading="lazy"]');
        
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    const src = img.getAttribute('data-src');
                    
                    if (src) {
                        img.src = src;
                        img.removeAttribute('data-src');
                    }
                    
                    imageObserver.unobserve(img);
                }
            });
        });
        
        lazyImages.forEach(img => {
            imageObserver.observe(img);
        });
    }
}

/**
 * Mobile navigation enhancements
 */
function initMobileNavigation() {
    const navbarToggler = document.querySelector('.navbar-toggler');
    if (navbarToggler) {
        navbarToggler.addEventListener('click', function() {
            this.classList.toggle('active');
        });
    }
    
    // Add smooth scrolling for mobile menu items
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', function() {
            const navbarCollapse = document.querySelector('.navbar-collapse');
            if (navbarCollapse && navbarCollapse.classList.contains('show')) {
                // Close the menu when a link is clicked on mobile
                document.querySelector('.navbar-toggler').click();
            }
        });
    });
}

/**
 * Alert dismissal functionality
 */
function initAlertDismissal() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        const closeBtn = alert.querySelector('.close');
        if (closeBtn) {
            closeBtn.addEventListener('click', function() {
                alert.classList.remove('show');
                setTimeout(() => {
                    alert.remove();
                }, 300);
            });
        }
        
        // Auto-dismiss alerts after 5 seconds
        setTimeout(() => {
            alert.classList.remove('show');
            setTimeout(() => {
                alert.remove();
            }, 300);
        }, 5000);
    });
}

// Sticky Navigation
$(window).on('scroll', function() {
    if ($(window).scrollTop() > 100) {
        $('.navbar').addClass('sticky');
    } else {
        $('.navbar').removeClass('sticky');
    }
});

// Initialize tooltips
$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// WhatsApp link handler
$(document).ready(function() {
    $('.whatsapp-icon, .nav-btn-whatsapp').on('click', function(e) {
        const whatsappNumber = $(this).data('whatsapp-number') || '+966569634996';
        window.open(`https://wa.me/${whatsappNumber}`, '_blank');
    });
});

// Dropdown fix for both desktop and mobile
function enhancedDropdownFunctionality() {
    // Enhanced dropdown functionality
    const dropdownToggles = document.querySelectorAll('.dropdown-toggle');
    
    // Add click event handler for mobile
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            if (window.innerWidth < 992) {
                e.preventDefault();
                e.stopPropagation();
                
                const parent = this.parentElement;
                const menu = this.nextElementSibling;
                
                // Close other open dropdowns first
                document.querySelectorAll('.dropdown-menu.show').forEach(openMenu => {
                    if (openMenu !== menu) {
                        openMenu.classList.remove('show');
                        openMenu.parentElement.classList.remove('show');
                    }
                });
                
                // Toggle current dropdown
                parent.classList.toggle('show');
                menu.classList.toggle('show');
                
                const expanded = this.getAttribute('aria-expanded') === 'true';
                this.setAttribute('aria-expanded', !expanded);
            }
        });
    });
    
    // Handle document clicks to close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.dropdown')) {
            document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                menu.classList.remove('show');
                menu.parentElement.classList.remove('show');
                menu.previousElementSibling.setAttribute('aria-expanded', 'false');
            });
        }
    });
    
    // Handle dropdown items click to close mobile menu
    document.querySelectorAll('.dropdown-item').forEach(item => {
        item.addEventListener('click', function() {
            if (window.innerWidth < 992) {
                document.querySelector('.navbar-collapse').classList.remove('show');
                
                const toggler = document.querySelector('.navbar-toggler');
                if (toggler.classList.contains('active')) {
                    toggler.classList.remove('active');
                }
            }
        });
    });
}
