/* ----------------- Start Document ----------------- */
"use strict";
function initApp() {
    

    /*----------------------------------------------------*/
    /* Dashboard Scripts
    /*----------------------------------------------------*/

    // Dashboard Nav Submenus
    $('.sidebar_inner ul li a').on('click', function (e) {
        if ($(this).closest("li").children("ul").length) {
            if ($(this).closest("li").is(".active-submenu")) {
                $('.sidebar_inner ul li').removeClass('active-submenu');
            } else {
                $('.sidebar_inner ul li').removeClass('active-submenu');
                $(this).parent('li').addClass('active-submenu');
            }
            e.preventDefault();
        }
    });


    /*--------------------------------------------------*/
    /*  Tippy JS 
    /*--------------------------------------------------*/
    /* global tippy */
    tippy('[data-tippy-placement]', {
        delay: 100,
        arrow: true,
        arrowType: 'sharp',
        size: 'regular',
        duration: 200,

        // 'shift-toward', 'fade', 'scale', 'perspective'
        animation: 'shift-away',

        animateFill: true,
        theme: 'dark',

        // How far the tooltip is from its reference element in pixels 
        distance: 10,

    });

    (function (window, document, undefined) {
        'use strict';
        if (!('localStorage' in window)) return;
        var nightMode = localStorage.getItem('gmtNightMode');
        if (nightMode) {
            document.documentElement.className += ' dark';
        }
    })(window, document);


    (function (window, document, undefined) {

        'use strict';

        // Feature test
        if (!('localStorage' in window)) return;

        // Get our newly insert toggle
        var nightMode = document.querySelector('#night-mode');
        if (!nightMode) return;

        // When clicked, toggle night mode on or off
        nightMode.addEventListener('click', function (event) {
            event.preventDefault();
            document.documentElement.classList.toggle('dark');
            if (document.documentElement.classList.contains('dark')) {
                localStorage.setItem('gmtNightMode', true);
                return;
            }
            localStorage.removeItem('gmtNightMode');
        }, false);

    })(window, document);
}