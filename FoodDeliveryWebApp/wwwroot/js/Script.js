$(document).ready(function () {
    // Load team list on page load
    loadTeamList();

    // Function to load team list using Ajax
    function loadTeamList() {
        $.ajax({
            url: '@Url.Action("Index", "Team")',
            type: 'GET',
            success: function (result) {
                // Update the content of the teamListContainer
                $('#teamListContainer').html(result);
            },
            error: function (error) {
                console.error('Error loading teams: ', error);
            }
        });
    }
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


// Using jQuery for simplicity, ensure jQuery is included in your project

// Attach a click event handler to the delete button
//$(".delete-link").click(function (e) {
//    e.preventDefault(); // Prevent the default behavior of the link

//    var deleteUrl = $(this).attr("href"); // Get the URL from the href attribute

//    // Show SweetAlert confirmation dialog
//    Swal.fire({
//        title: 'Are you sure?',
//        text: 'You won\'t be able to revert this!',
//        icon: 'warning',
//        showCancelButton: true,
//        confirmButtonColor: '#d33',
//        cancelButtonColor: '#3085d6',
//        confirmButtonText: 'Yes, delete it!'
//    }).then((result) => {
//        if (result.isConfirmed) {
//            // If the user confirms, prevent the default link behavior and proceed with the deletion
//            e.preventDefault();

//            $.ajax({
//                url: deleteUrl,
//                type: "POST",
//                dataType: "json",
//                success: function (ajaxResult) {
//                    if (ajaxResult.success) {
//                        // Reload data and show success message
//                        loadData();
//                        toastr.success(ajaxResult.message);
//                    } else {
//                        // Show error message
//                        Swal.fire({
//                            title: 'Error!',
//                            text: ajaxResult.message,
//                            icon: 'error'
//                        });
//                    }
//                },
//                error: function (errormessage) {
//                    toastr.error("An error occurred while deleting the Team: " + errormessage.responseText);
//                }
//            });
//        }
//    });
//});

function Delete(teamId) {
    // Show the SweetAlert confirmation dialog
    Swal.fire({
        title: 'Are you sure?',
        text: 'You won\'t be able to revert this!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            // If the user confirms, proceed with the deletion
            $.ajax({
                url: "/Team/Delete/" + teamId,
                type: "POST",
                dataType: "json",
                success: function (ajaxResult) {
                    if (ajaxResult.success) {
                        // Show success message
                        toastr.success(ajaxResult.message);

                        // Redirect to the index page
                        window.location.href = '/Team/Index';
                    } else {
                        // Show error message
                        Swal.fire({
                            title: 'Error!',
                            text: ajaxResult.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (errormessage) {
                    toastr.error("An error occurred while deleting the Team: " + errormessage.responseText);
                }
            });
        }
    });

    // Prevent the default link behavior
    return false;
}




