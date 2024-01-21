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









