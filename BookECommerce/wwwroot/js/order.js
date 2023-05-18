var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var statuses = ["inprocess", "completed", "pending", "approved", "all"];

    for (var i = 0; i < statuses.length; i++) {
        if (url.includes(statuses[i])) {
            loadDataTable(statuses[i]);
            return;
        }
    }
    
    loadDataTable("all");
});


function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            {"data": "id", "width": "5%"},
            {"data": "name", "width": "15%"},
            {"data": "phoneNumber", "width": "15%"},
            {"data": "applicationUser.email", "width": "15%"},
            {"data": "orderStatus", "width": "10%"},
            {"data": "orderTotal", "width": "10%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/order/details?orderId=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
					</div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}
