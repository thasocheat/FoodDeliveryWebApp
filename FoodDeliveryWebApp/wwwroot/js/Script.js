$(document).ready(function () {
    $('#btnLogout').click(function (e) {
        e.preventDefault(); // Prevent the default button behavior

        // Make an AJAX request for logout
        $.ajax({
            url: '/Account/Logout',
            type: 'POST',
            success: function (result) {
                toastr.success("Logout successful!");
                // Optionally, redirect to another page after successful logout
                window.location.href = '/Home/Index';
            },
            error: function (err) {
                toastr.error("Error during logout!");
            }
        });
    });

    // Initialize Toastr
    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-right',
        preventDuplicates: true,
        showMethod: 'slideDown',
        hideMethod: 'slideUp',
        timeOut: 5000
    };
});

//Image preview function
function showImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

// Initialize Toastr
toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: 'toast-top-right',
    preventDuplicates: true,
    showMethod: 'slideDown',
    hideMethod: 'slideUp',
    timeOut: 5000
};


$(document).ready(function () {
    highlightActiveLink();
});

function highlightActiveLink() {
    var pathName = window.location.pathname;

    // Remove the active class from all nav-links
    $(".navbar-nav .nav-link").removeClass("active");

    // Add the active class to the corresponding nav-link based on the URL
    $(".navbar-nav .nav-link").each(function () {
        var href = $(this).attr("href");
        if (pathName.includes(href)) {
            $(this).addClass("active");
        }
    });
}
// Click event handlers to add active class to clicked link
$("#homeLink").click(function () {
    $(this).addClass("active");
    $(".navbar-nav .nav-link").removeClass("active");
});

$("#aboutUsLink").click(function () {
    $(this).addClass("active");
    $(".navbar-nav .nav-link").removeClass("active");
    redirectToAboutUs();
});

$("#menuLink").click(function () {
    $(this).addClass("active");
    $(".navbar-nav .nav-link").removeClass("active");
    redirectToFrontViewProduct();
});

$("#contactUsLink").click(function () {
    $(this).addClass("active");
    $(".navbar-nav .nav-link").removeClass("active");
});






