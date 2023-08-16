document.addEventListener("DOMContentLoaded", function () {
    // Edit button logic
    document.querySelectorAll('.editBtn').forEach(function (button) {
        button.addEventListener('click', function (e) {
            var row = e.target.closest('tr');
            row.querySelectorAll('.text').forEach(el => el.style.display = 'none');
            row.querySelectorAll('.edit').forEach(el => el.style.display = 'block');
            e.target.style.display = 'none'; // Hide the edit button
            row.querySelector('.edit-actions').style.display = 'block'; // Show save and cancel buttons
        });
    });

    // Delete button logic
    $(document).ready(function () {
        $('.deleteBtn').on('click', function (e) {
            if (confirm('Are you sure you want to delete this item?')) {
                var url = $(this).data('url');
                var button = $(this); // Store a reference to the clicked button

                $.post(url, function () {
                    // Remove the closest parent row of the clicked button
                    button.closest('tr').remove();
                });
            } else {
                // Optional: Handle the cancellation if needed
                e.preventDefault();
            }
        });
    });



    // Add button logic
    document.querySelectorAll('.newButton').forEach(function (button) {
        button.addEventListener('click', function (e) {
            var formId = e.target.getAttribute('data-form');
            var form = document.getElementById(formId);
            form.style.display = form.style.display === "none" ? "block" : "none";
            e.target.style.display = 'none';
        });
    });

    // Cancel button logic for table rows
    document.querySelectorAll('.cancelBtn').forEach(function (button) {
        button.addEventListener('click', function (e) {
            var row = e.target.closest('tr');
            row.querySelectorAll('.text').forEach(el => el.style.display = 'block');
            row.querySelectorAll('.edit').forEach(el => el.style.display = 'none');
            row.querySelector('.edit-actions').style.display = 'none';
            row.querySelector('.editBtn').style.display = 'block'; // Show the edit button
        });
    });


    // Cancel Form button logic 
    document.querySelectorAll('.cancelButton').forEach(function (button) {
        button.addEventListener('click', function (e) {
            e.target.closest('.newForm').style.display = 'none';
            document.querySelector('.newButton').style.display = '';
        });
    });

    // Save button logic
    document.querySelectorAll('.saveBtn').forEach(function (button) {
        button.addEventListener('click', function (e) {
            var row = e.target.closest('tr');
            // Logic to save the edited data
            // You may need to call an API endpoint to update the data in the database
            row.querySelectorAll('.text').forEach(el => el.style.display = 'block');
            row.querySelectorAll('.edit').forEach(el => el.style.display = 'none');
            row.querySelector('.edit-actions').style.display = 'none'; // Hide save and cancel buttons
            row.querySelector('.editBtn').style.display = 'block'; // Show the edit button
        });
    });

});
