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
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Seller/SellerOrder/getOrderList?status=' + status },
        "columns": [
            { data: 'orderId', "width": "5%" },
            { data: 'customerName', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'totalAmount', "width": "10%" },
            {
                data: 'orderId',
                render: function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Seller/SellerOrder/details?orderId=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            
                        </div>
                    `;
                },
                "width": "10%"
            }
        ]
    });
}
