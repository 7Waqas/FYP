var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes('inprocess')) {
        loadDataTable('inprocess');

    }
    else {
        if (url.includes('pending')) {
            loadDataTable('pending');

        }
        else {
            if (url.includes('completed')) {
                loadDataTable('completed');

            }
            else {
                if (url.includes('approved')) {
                    loadDataTable('approved');

                }
                else {

                    loadDataTable('all');
                }


            }


        }



    }

   
});

function loadDataTable(status) {
    let columns = [
        { data: 'id', "width": "5%" },
        { data: 'name', "width": "25%" },
        { data: 'phoneNumber', "width": "20%" },
        { data: 'applicationUser.email', "width": "20%" },
        { data: 'orderStatus', "width": "10%" },
        { data: 'orderTotal', "width": "10%" },
        {
            data: 'id',
            render: function (data) {
                return `
                    <div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                    </div>
                `;
            },
            "width": "10%"
        }
    ];

    // Only show "Review" column if current user is Customer
    if (currentUserRole === "Customer") {
        columns.push({
            data: null,
            render: function (data, type, row) {
                if (row.orderStatus === "Shipped") {
                    return `
                        <div class="btn-group btn-sm" role="group">
                            <a href="/Seller/Review/Review?orderId=${row.id}" class="btn btn-warning mx-2">
                                Review
                            </a>
                        </div>
                    `;
                } else {
                    return "";
                }
            },
            "width": "10%"
        });
    }

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": columns
    });
}
