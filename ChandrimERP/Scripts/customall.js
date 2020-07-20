/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbookSalesInvoice() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/Salesbook/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $('#createCustomer').modal('hide');
                    alert("Add Customer successfully");
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Salesbook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Salesbook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });
        });
    });
    //Get Closing Balance
    $(document).ready(function () {
        $("#customer").change(function () {
            var customerid = $(this).val();
            $("#closingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/Salesbook/GetClosingBalance/" + customerid,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#closingbalance").append($("#closingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

        });

    });
    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseFloat(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt;
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".totalDiscount").val()) || $.trim($(".totalDiscount").val()) == "") {
            $(".totalDiscount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())).toFixed(2) - parseFloat($(".lessAmount").val()).toFixed(2)
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });


    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DueDate = $("#duedate").val(),
        order.DeliveryDate = $("#deliverydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Salesbook/SalesInvoice",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order QTN !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbookSalesInvoiceReport() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });

    $("#search").keyup(function () {
        var value = this.value.toLowerCase().trim();

        $("table tr").each(function (index) {
            if (!index) return;
            $(this).find("td").each(function () {
                var id = $(this).text().toLowerCase().trim();
                var not_found = (id.indexOf(value) == -1);
                $(this).closest('tr').toggle(!not_found);
                return not_found;
            });
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbookSalesOrder() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/Salesbook/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function () {
                    $('#createCustomer').modal('hide');
                    alert("Data has been added successfully.");
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Salesbook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Salesbook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });


    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DueDate = $("#duedate").val(),
        order.DeliveryDate = $("#delivarydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Salesbook/SalesOrder",
            data: data,
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbookSalesReturn() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Salesbook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Salesbook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });


    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {
        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DeliveryDate = $("#deliverydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Salesbook/SalesReturn",
            data: data,
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order QTN !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorPosInterface() {
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorSalesInvoice() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/SalesbookTailor/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $('#createCustomer').modal('hide');
                    alert(result);
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/SalesbookTailor/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');
                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                        $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);

                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);
                        $('#quantity').val(1);
                        $('#discount').val(0);
                        $('#tablecus tbody th').append(rows);

                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });
        });
    });
    //Get Closing Balance
    $(document).ready(function () {
        $("#customer").change(function () {
            var customerid = $(this).val();
            $("#closingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/GetClosingBalance/" + customerid,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#closingbalance").append($("#closingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

        });

    });
    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseFloat(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())).toFixed(2) - parseFloat($(".lessAmount").val()).toFixed(2)
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DueDate = $("#duedate").val(),
        order.DeliveryDate = $("#deliverydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/SalesbookTailor/SalesInvoice",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order QTN !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorSeawingInvoice() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/SalesbookTailor/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function () {
                    $('#createCustomer').modal('hide');
                    alert("Data has been added successfully.");
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/SalesbookTailor/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                        $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            })

        })

    })

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseFloat(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error")
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100).toFixed(2);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });


    function totalAmountsw() {

        if (isNaN($("#qntsw").val()) || $.trim($("#qntsw").val()) == "") {
            $("#qntsw").val(0);
        }
        if (isNaN($("#totalsw").val()) || $.trim($("#totalsw").val()) == "") {
            $("#totalsw").val(0);
        }

        var qntsw = $("#qntsw").val(),
            totalsw = $("#totalsw").val(),
             Amountsw = (parseFloat(qntsw) * parseInt(totalsw));

        $("#netAmountsw").val(Amountsw);
    };



    //Add Multiple Order.
    $("#addToListsw").click(function (e) {
        e.preventDefault();

        if ($.trim($("#tul").val()) == "" || $.trim($("#arrad").val()) == "" ||
            $.trim($("#kocchor").val()) == "" || $.trim($("#hip").val()) == "" || $.trim($("#qntsw").val()) == "" || $.trim($("#totalsw").val()) == "") return;

        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var tul = $("#tul").val(),
            arrad = $("#arrad").val(),
            kocchor = $("#kocchor").val(),
            hip = $("#hip").val(),
            khetab = $("#khetab").val(),
            ibad = $("#ibad").val(),
            ordun = $("#ordun").val(),
            orgoba = $("#orgoba").val(),
            gher = $("#gher").val(),
            throat = $("#throat").val(),
            shelha = $("#shelha").val(),
            mohra = $("#mohra").val(),
            modelStyle = $("#modelStyle").val(),
            qntsw = $("#qntsw").val(),
            totalsw = $("#totalsw").val(),

            netAmountsw = $("#netAmountsw").val(),

            detailsTableBodysw = $("#seawingInvoiceDetails tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        };


        var productItemsw =
             '<tr><td witdh="23%"> <div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"> <label>Tul Long:</label> </div> <div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + tul + '" id="tul"  class="form-group TulLong" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Arrad Body:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + arrad + '" id="arrad"  class="form-group ArradBody" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Kocchor Waist:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + kocchor + '" id="kocchor"  class="form-group KocchorWaist" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Hip:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + hip + '" id="hip"  class="form-group Hip" readonly style="width:100%;border:inherit;" /></div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Khetab Shoulder:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + khetab + '" id="khetab" class="form-group KhetabShoulder" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ibad Mohra:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + ibad + '" id="ibad" class="form-group Ibad" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ordun Hand:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + ordun + '" id="ordun"  class="form-group OrdunHand" readonly style="width:100%;border:inherit;" /></div> </div> <div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Orgoba Throat:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + orgoba + '" id="orgoba"  class="form-group Orgoba" readonly style="width:100%;border:inherit;" /> </div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Gher Enclorure:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + gher + '" id="gher"  class="form-group GherEnclorure" readonly style="width:100%;border:inherit;" /> </div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Orgoba Throat:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + throat + '" id="throat"  class="form-group Throat" readonly style="width:100%;border:inherit;" /> </div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Shelha:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + shelha + '" id="shelha"  class="form-group shelha" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ibad Mohra:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + mohra + '" id="mohra"  class="form-group IbadMohra" readonly style="width:100%;border:inherit;" /></div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Model No:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + modelStyle + '" id="modelStyle"  class="form-group modelStyle" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Quantity:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + qntsw + '" id="qntsw"  class="form-group Qty" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Rate:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + totalsw + '" id="totalsw"  class="form-group Rate" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Total Amount:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + netAmountsw + '" id="netAmountsw"  class="form-group totalpricesw TotalAmountSe" readonly style="width:100%;border:inherit;" /></div></div> </td><td width="8%"><button data-itemId="0" type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button></td></tr>'

        detailsTableBodysw.append(productItemsw);
        clearItemsw();
        invoiceAmount();


    });
    function clearItemsw() {
        $("#tul").val('');
        $("#arrad").val('');
        $("#kocchor").val('');
        $("#hip").val('');
        $("#khetab").val('');
        $("#ibad").val('');
        $("#ordun").val('');
        $("#orgoba").val('');
        $("#gher").val('');
        $("#throat").val('');
        $("#shelha").val('');
        $("#mohra").val('');
        $("#modelStyle").val('');
        $("#qntsw").val('');
        $("#totalsw").val('');
        $("#netAmountsw").val('');
    }


    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumValBonusqnt = 0;
        var sumDiscount = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseInt(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseInt(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseInt(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseInt(table.rows[i].cells[11].innerHTML);
        }

        var sum = 0;
        $('.totalpricesw').each(function () {
            sum += parseFloat($(this).val());
        });
        var totalsw = sum;


        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalsw").val(totalsw);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalsw").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    //Get Closing Balance
    $(document).ready(function () {
        $("#customer").change(function () {
            var customerid = $(this).val();
            $("#closingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/GetClosingBalance/" + customerid,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#closingbalance").append($("#closingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

        });

    });

    //After Click Save Button Pass All Data View To Controller For Save Database
    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    //Collect Multiple Order List For Pass To Controller
    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DueDate = $("#duedate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.SeawingAmount = $("#TotalAmountSW").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var orderArrsw = [];
        orderArrsw.length = 0;
        $.each($("#seawingInvoiceDetails tbody tr"), function () {
            orderArrsw.push({
                TulLong: $(this).find('td .TulLong:eq(0)').val(),
                ArradBody: $(this).find('td .ArradBody:eq(0)').val(),
                KocchorWaist: $(this).find('td .KocchorWaist:eq(0)').val(),
                Hip: $(this).find('td .Hip:eq(0)').val(),
                KhetabShoulder: $(this).find('td .KhetabShoulder:eq(0)').val(),
                OrdunHand: $(this).find('td .OrdunHand:eq(0)').val(),
                Orgoba: $(this).find('td .Orgoba:eq(0)').val(),
                Throat: $(this).find('td .Throat:eq(0)').val(),
                GherEnclorure: $(this).find('td .GherEnclorure:eq(0)').val(),
                IbadMohra: $(this).find('td .IbadMohra:eq(0)').val(),
                Ibad: $(this).find('td .Ibad:eq(0)').val(),
                Qty: $(this).find('td .Qty:eq(0)').val(),
                Rate: $(this).find('td .Rate:eq(0)').val(),
                TotalAmount: $(this).find('td .TotalAmountSe:eq(0)').val(),
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orderdatasw: orderArrsw,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/SalesbookTailor/SeawingInvoice",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorSeawing() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/SalesbookTailor/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function () {
                    $('#createCustomer').modal('hide');
                    alert("Data has been added successfully.");
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });

    function totalAmountsw() {

        if (isNaN($("#qntsw").val()) || $.trim($("#qntsw").val()) == "") {
            $("#qntsw").val(0);
        }
        if (isNaN($("#totalsw").val()) || $.trim($("#totalsw").val()) == "") {
            $("#totalsw").val(0);
        }

        var qntsw = $("#qntsw").val(),
            totalsw = $("#totalsw").val(),
             Amountsw = (parseFloat(qntsw) * parseInt(totalsw));

        $("#netAmountsw").val(Amountsw);
    };



    //Add Multiple Order.
    $("#addToListsw").click(function (e) {
        e.preventDefault();

        if ($.trim($("#tul").val()) == "" || $.trim($("#arrad").val()) == "" ||
            $.trim($("#kocchor").val()) == "" || $.trim($("#hip").val()) == "" || $.trim($("#qntsw").val()) == "" || $.trim($("#totalsw").val()) == "") return;

        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var tul = $("#tul").val(),
            arrad = $("#arrad").val(),
            kocchor = $("#kocchor").val(),
            hip = $("#hip").val(),
            khetab = $("#khetab").val(),
            ibad = $("#ibad").val(),
            ordun = $("#ordun").val(),
            orgoba = $("#orgoba").val(),
            gher = $("#gher").val(),
            throat = $("#throat").val(),
            shelha = $("#shelha").val(),
            mohra = $("#mohra").val(),
            modelStyle = $("#modelStyle").val(),
            qntsw = $("#qntsw").val(),
            totalsw = $("#totalsw").val(),

            netAmountsw = $("#netAmountsw").val(),

            detailsTableBodysw = $("#seawingInvoiceDetails tbody");


        var productItemsw =
             '<tr><td witdh="23%"> <div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"> <label>Tul Long:</label> </div> <div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + tul + '" id="tul"  class="form-group TulLong" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Arrad Body:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + arrad + '" id="arrad"  class="form-group ArradBody" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Kocchor Waist:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + kocchor + '" id="kocchor"  class="form-group KocchorWaist" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Hip:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + hip + '" id="hip"  class="form-group Hip" readonly style="width:100%;border:inherit;" /></div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Khetab Shoulder:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + khetab + '" id="khetab" class="form-group KhetabShoulder" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ibad Mohra:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + ibad + '" id="ibad" class="form-group Ibad" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ordun Hand:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + ordun + '" id="ordun"  class="form-group OrdunHand" readonly style="width:100%;border:inherit;" /></div> </div> <div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Orgoba Throat:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + orgoba + '" id="orgoba"  class="form-group Orgoba" readonly style="width:100%;border:inherit;" /> </div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Gher Enclorure:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + gher + '" id="gher"  class="form-group GherEnclorure" readonly style="width:100%;border:inherit;" /> </div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Orgoba Throat:</label> </div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + throat + '" id="throat"  class="form-group Throat" readonly style="width:100%;border:inherit;" /> </div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Shelha:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + shelha + '" id="shelha"  class="form-group shelha" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Ibad Mohra:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + mohra + '" id="mohra"  class="form-group IbadMohra" readonly style="width:100%;border:inherit;" /></div></div></td><td witdh="23%"><div class="col-md-12"><div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Model No:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + modelStyle + '" id="modelStyle"  class="form-group modelStyle" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Quantity:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + qntsw + '" id="qntsw"  class="form-group Qty" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Rate:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + totalsw + '" id="totalsw"  class="form-group Rate" readonly style="width:100%;border:inherit;" /></div></div><div class="col-md-12"> <div class="col-md-8 text-right" style="font-weight:bold;text-transform:uppercase;width:70%;"><label>Total Amount:</label></div><div class="col-md-4 text-left" style="font-weight:bold;text-transform:uppercase;width:30%;padding:0px!important"><input type="text" value="' + netAmountsw + '" id="netAmountsw"  class="form-group totalpricesw TotalAmountSe" readonly style="width:100%;border:inherit;" /></div></div> </td><td width="8%"><button data-itemId="0" type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button></td></tr>'

        detailsTableBodysw.append(productItemsw);
        clearItemsw();
        invoiceAmount();


    });
    function clearItemsw() {
        $("#tul").val('');
        $("#arrad").val('');
        $("#kocchor").val('');
        $("#hip").val('');
        $("#khetab").val('');
        $("#ibad").val('');
        $("#ordun").val('');
        $("#orgoba").val('');
        $("#gher").val('');
        $("#throat").val('');
        $("#shelha").val('');
        $("#mohra").val('');
        $("#modelStyle").val('');
        $("#qntsw").val('');
        $("#totalsw").val('');
        $("#netAmountsw").val('');
    }


    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumValBonusqnt = 0;
        var sumDiscount = 0;
        var vatAmount = 0;
        var sum = 0;
        $('.totalpricesw').each(function () {
            sum += parseFloat($(this).val());
        });
        var totalsw = sum;
        $(".total").val(sumVal);
        $(".totalsw").val(totalsw);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalsw").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            }); l

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    //Get Closing Balance
    $(document).ready(function () {
        $("#customer").change(function () {
            var customerid = $(this).val();
            $("#closingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/GetClosingBalance/" + customerid,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#closingbalance").append($("#closingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

        });

    });

    //After Click Save Button Pass All Data View To Controller For Save Database
    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    //Collect Multiple Order List For Pass To Controller
    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DueDate = $("#duedate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.SeawingAmount = $("#TotalAmountSW").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var orderArrsw = [];
        orderArrsw.length = 0;
        $.each($("#seawingInvoiceDetails tbody tr"), function () {
            orderArrsw.push({
                TulLong: $(this).find('td .TulLong:eq(0)').val(),
                ArradBody: $(this).find('td .ArradBody:eq(0)').val(),
                KocchorWaist: $(this).find('td .KocchorWaist:eq(0)').val(),
                Hip: $(this).find('td .Hip:eq(0)').val(),
                KhetabShoulder: $(this).find('td .KhetabShoulder:eq(0)').val(),
                OrdunHand: $(this).find('td .OrdunHand:eq(0)').val(),
                Orgoba: $(this).find('td .Orgoba:eq(0)').val(),
                Throat: $(this).find('td .Throat:eq(0)').val(),
                GherEnclorure: $(this).find('td .GherEnclorure:eq(0)').val(),
                IbadMohra: $(this).find('td .IbadMohra:eq(0)').val(),
                Ibad: $(this).find('td .Ibad:eq(0)').val(),
                Qty: $(this).find('td .Qty:eq(0)').val(),
                Rate: $(this).find('td .Rate:eq(0)').val(),
                TotalAmount: $(this).find('td .TotalAmountSe:eq(0)').val(),
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orderdatasw: orderArrsw,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/SalesbookTailor/seawing",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorSalesReturn() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/SalesbookTailor/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/SalesbookTailor/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {

        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });


    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {
        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.CustomerID = $("#customer").val(),
        order.DeliveryDate = $("#deliverydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/SalesbookTailor/SalesReturn",
            data: data,
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order QTN !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function salesbooktailorSalesInvoieReport() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function suppliersCreate() {
    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);
                }
            });
        });
    });
    /* City dropdown list*/
    $(document).ready(function () {
        $("#State").change(function () {
            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);
                }
            });
        });
    });

    /* Date Picker Looltip */
    $(document).ready(function () {
        $('[data_toggle="tooltip"]').tooltip();
    });

    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {
        var readUrl = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }
        $(".file-upload").on('change', function () {
            readUrl(this);
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function purchasebookPurchaseInvoice() {
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Purchasebook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Purchasebook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#cost").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val();
        a = parseFloat($("#cost").val()),
        b = parseFloat($("#ecost").val());
        if (isNaN(b) || $.trim($("#ecost").val()) == " ") {
            b = 0;
        };
        trate = (a + b),
        productUnit = $("#pUnit").val(),
        discount = $("#discount").val(),

        tAmount = parseFloat(parseFloat(trate) * parseInt(quantity)),
        netTotal = "",
        bonusQnt = $("#bonusQnt").val(),
        batch = $("#batch").val(),

        vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

        detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + trate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();
    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt;
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
        $("#cost").val('');
        $("#ecost").val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });
    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {
        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.SupplierID = $("#supplier").val(),
        //order.DueDate = $("#duedate").val(),
        order.DalevaryDate = $("#delivarydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });

            var data = JSON.stringify({
                orderdata: orderArr,
                orders: order
            });

            $.ajax({
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: 'POST',
                url: "/Purchasebook/PurchaseInvoice",
                data: data,
                //data: '{orders: ' + JSON.stringify(order) + '}',
                success: function (result) {
                    alert(result);
                    location.reload();
                },
                error: function () {
                    alert("Your request not  valid, Please check Order QTN !");
                }
            });
        });
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function purchasebookPurchaseInvoiceReport() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function purchasebookPurchaseOrder() {
    $(function () {
        //LoadData();
        $("#saveCousromer").click(function () {
            var std = {};
            std.CompanyId = $("#CompanyId").val();
            std.CompanyName = $("#CompanyName").val();
            std.ContactFirstName = $("#ContactFirstName").val();
            std.ContactLastName = $("#ContactLastName").val();
            std.OpeningBlance = $("#OpeningBlance").val();
            std.Genders = $("#Genders").val();
            std.Phone = $("#Phone").val();
            std.Email = $("#Email").val();
            std.WebPage = $("#WebPage").val();
            std.Country = $("#Country").val();
            std.State = $("#State").val();
            std.City = $("#City").val();
            std.AddressLineOne = $("#AddressLineOne").val();
            std.AddressLineTwo = $("#AddressLineTwo").val();
            std.Status = $("#Status").val();
            std.Notes = $("#Notes").val();
            $.ajax({
                type: "POST",
                url: "/Purchasebook/addcustomer",
                data: '{customer: ' + JSON.stringify(std) + '}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function () {
                    $('#createCustomer').modal('hide');
                    alert("Data has been added successfully.");
                    location.reload();
                },
                error: function () {
                    alert("Error while inserting data");
                }
            });
            return false;
        });

    });

    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#createCustomer").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#createCustomer").modal('hide');
        });
    });


    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Purchasebook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Purchasebook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {

        if (parseInt($("#quantity").val()) < 0) {
            alert("Quantity must be 1 or greater then 1");
            $("#quantity").val(" ");
            return false;
        }
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#rate").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            rate = $("#rate").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val(),

            tAmount = parseFloat(parseFloat(rate) * parseInt(quantity)),
            netTotal = "",
            bonusQnt = $("#bonusQnt").val(),
            batch = $("#batch").val(),

            vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

            detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + rate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
            + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
            '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
            + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
            + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();


    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });


    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
            order.CustomerID = $("#customer").val(),
            order.DueDate = $("#duedate").val(),
            order.DeliveryDate = $("#delivarydate").val(),
            order.BranchId = $("#branch").val(),
            order.WarehouseId = $("#warehouse").val(),
            order.LedgerId = $("#LedgerId").val(),
            order.SalesAgentId = $("#SalesAgentId").val(),
            order.Narration = $("#Narration").val(),
            order.TotalQNT = $("#totalQNT").val(),
            order.TotalDiscount = $("#totalDiscount").val(),
            order.TotalAmount = $("#TotalAmount").val(),
            order.VatAmount = $("#VatAmount").val(),
            order.LaseAmount = $("#LessAmount").val(),
            order.Addamount = $("#AddAmount").val(),
            order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Purchasebook/PurchaseOrder",
            data: data,
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function purchasebookPurchaseReturn() {
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Purchasebook/getProductList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (result) {
                $(result).each(function (i, item) {
                    $(".productList").append('<option value="' + item.ProductId + '"  data-subtext="' + item.ProductCode + ' ' + item.Description + '" >' + item.ProductName + '</option>');

                });
            },
            error: function (data) { }
        });
    });
    $(document).ready(function () {

        $("#barcode").change(function () {

            var barcode = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Purchasebook/getProduct/" + barcode,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#productName').val(item.ProductName);
                        $('#pCode').val(item.ProductCode);
                        $('#pDesc').val(item.Description);
                        $('#rate').val(item.ProductRate);
                        $('#pUnit').val(item.ProductMeasure);
                        $("#discount").val();
                        $('#pVAT').val(item.ProductVat);

                        $('#tablecus tbody th').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    //Add Multiple Order.
    $("#addToList").click(function (e) {
        e.preventDefault();

        if ($.trim($("#barcode").val()) == "" || $.trim($("#productName").val()) == "" ||
            $.trim($("#quantity").val()) == "" || $.trim($("#cost").val()) == "") return;
        //|| $.trim($("#discount").val()) == "" || $.trim($("#bonusQnt").val()) == "" || $.trim($("#batch").val()) == "" || $.trim($("#pVAT").val()) == ""

        var barcode = $("#barcode").val(),
            productCode = $("#pCode").val(),
            productName = $("#productName").val(),
            productDesc = $("#pDesc").val(),
            quantity = $("#quantity").val(),
            productUnit = $("#pUnit").val(),
            discount = $("#discount").val();
        a = parseFloat($("#cost").val()),
        b = parseFloat($("#ecost").val());
        if (isNaN(b) || $.trim($("#ecost").val()) == " ") {
            b = 0;
        };
        trate = (a + b),
        tAmount = parseFloat(parseFloat(trate) * parseInt(quantity)),
        netTotal = "",
        bonusQnt = $("#bonusQnt").val(),
        batch = $("#batch").val(),
        vatamount = parseFloat(($("#pVAT").val()) * parseInt(tAmount) / 100).toFixed(2),

        detailsTableBody = $("#detailsTable tbody");

        if (isNaN(bonusQnt) || $.trim($("#bonusQnt").val()) == "") {
            bonusQnt = 0;
        }
        if (discount.indexOf("%") <= -1) {
            var pFdiscount = parseFloat(discount);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number') {
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
            }
        }
        else {
            var discountPers = discount.replace(/%/g, "");
            var pFdiscount = parseFloat(discountPers);
            if (typeof pFdiscount === 'string' || isNaN(pFdiscount)) {
                alert("Please Check Your Discount!! Only alow Number or %")
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else if (typeof pFdiscount === 'number' && pFdiscount <= 100) {
                pFdiscount = ((tAmount * pFdiscount) / 100);
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
            else {
                alert("Error");
                pFdiscount = 0;
                netTotal = (tAmount - pFdiscount).toFixed(2);
            }
        }


        var productItem = '<tr><td>' + productCode + '</td><td>' + productName + '</td><td>' + productDesc + '</td><td>' + quantity +
            '</td><td>' + trate + '</td><td>' + productUnit + '</td><td>' + tAmount + '</td><td>' + pFdiscount + '</td><td>'
        + netTotal + '</td><td>' + batch + '</td><td>' + parseInt(bonusQnt) + '</td><td class="">' + vatamount +
        '</td><td class="hidden">' + barcode + '</td><td><div class="btn-group">'
                + '<button  data-itemId="0"  type="button" class="deleteItem btn btn-default glyphicon glyphicon-remove"></button>'
                + '</div></td></tr>';
        detailsTableBody.append(productItem);
        clearItem();
        invoiceAmount();
    });
    function invoiceAmount() {
        var sumVal = 0;
        var sumValqnt = 0;
        var sumDiscount = 0;
        var sumValBonusqnt = 0;
        var vatAmount = 0;
        var table = document.getElementById("detailsTable");
        for (var i = 1; i < table.rows.length; i++) {
            sumVal = sumVal + parseFloat(table.rows[i].cells[8].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValqnt = sumValqnt + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumDiscount = sumDiscount + parseFloat(table.rows[i].cells[7].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            sumValBonusqnt = sumValBonusqnt + parseFloat(table.rows[i].cells[10].innerHTML);
        }
        for (var i = 1; i < table.rows.length; i++) {
            vatAmount = vatAmount + parseFloat(table.rows[i].cells[11].innerHTML);
        }
        sumValqnt = sumValqnt + sumValBonusqnt
        $(".total").val(sumVal);
        $(".totalQNT").val(sumValqnt);
        $(".totalDiscount").val(sumDiscount);
        $(".totalVAT").val(vatAmount);
        if (isNaN($(".total").val()) || $.trim($(".total").val()) == "") {
            $(".total").val(0);
        }
        if (isNaN($(".totalVAT").val()) || $.trim($(".totalVAT").val()) == "") {
            $(".totalVAT").val(0);
        }
        if (isNaN($(".addAmount").val()) || $.trim($(".addAmount").val()) == "") {
            $(".addAmount").val(0);
        }
        if (isNaN($(".lessAmount").val()) || $.trim($(".lessAmount").val()) == "") {
            $(".lessAmount").val(0);
        }
        var invoicedTotal = (parseFloat($(".total").val()) + parseFloat($(".totalVAT").val()) + parseFloat($(".addAmount").val())) - parseFloat($(".lessAmount").val())
        $(".invoicedTotal").val(invoicedTotal)
    }
    function clearItem() {
        $("#barcode").val('');
        $("#productName").val('');
        $("#quantity").val('');
        $("#rate").val('');
        $("#discount").val('');
        $("#bonusQnt").val('');
        $("#batch").val('');
        $('#pVAT').val('');
        $("#cost").val('');
        $("#ecost").val('');
    }
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                invoiceAmount();
            });
        }
    });

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.LedgerList').chosen(config['.LedgerList']);
    }
    for (var selector in config) {
        $('.BranchList').chosen(config['.BranchList']);
    }
    for (var selector in config) {
        $('.WarehouseList').chosen(config['.WarehouseList']);
    }
    for (var selector in config) {
        $('.SalesAgentList').chosen(config['.SalesAgentList']);
    }
    for (var selector in config) {
        $('.productlist').chosen(config['.productlist']);
    }

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);
                }
            });
        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);
                }
            });

        });

    });


    //Collect Multiple Order List For Pass To Controller
    $("#saveOrder").click(function (e) {
        e.preventDefault();
        var r = confirm("Confirm to Submit!");
        if (r == true) {
            saveData();
        };
    });

    function saveData() {

        var order = {};
        order.OrderDate = $("#orderdate").val(),
        order.SupplierID = $("#supplier").val(),
        //order.DueDate = $("#duedate").val(),
        order.DalevaryDate = $("#delivarydate").val(),
        order.BranchId = $("#branch").val(),
        order.WarehouseId = $("#warehouse").val(),
        order.LedgerId = $("#LedgerId").val(),
        order.SalesAgentId = $("#SalesAgentId").val(),
        order.Narration = $("#Narration").val(),
        order.TotalQNT = $("#totalQNT").val(),
        order.TotalDiscount = $("#totalDiscount").val(),
        order.TotalAmount = $("#TotalAmount").val(),
        order.VatAmount = $("#VatAmount").val(),
        order.LaseAmount = $("#LessAmount").val(),
        order.Addamount = $("#AddAmount").val(),
        order.InvoicedAmount = $("#invoicedTotal").val()

        var orderArr = [];
        orderArr.length = 0;
        $.each($("#detailsTable tbody tr"), function () {
            orderArr.push({
                ProductCode: $(this).find('td:eq(0)').html(),
                ProductName: $(this).find('td:eq(1)').html(),
                ProductDescription: $(this).find('td:eq(2)').html(),
                Quantity: $(this).find('td:eq(3)').html(),
                Rate: $(this).find('td:eq(4)').html(),
                MeasureUnit: $(this).find('td:eq(5)').html(),
                TotalAmount: $(this).find('td:eq(6)').html(),
                Discount: $(this).find('td:eq(7)').html(),
                NetTotal: $(this).find('td:eq(8)').html(),
                BatchOrSerial: $(this).find('td:eq(9)').html(),
                BonusQuantity: $(this).find('td:eq(10)').html(),
                VAT: $(this).find('td:eq(11)').html(),
                ProductId: $(this).find('td:eq(12)').html()
            });
        });

        var data = JSON.stringify({
            orderdata: orderArr,
            orders: order
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Purchasebook/PurchaseReturn",
            data: data,
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Your request not  valid, Please check Order QTN !");
            }
        });
    };
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function productCreate() {
    /* country dropdown list*/
    //$(document).ready(function () {
    //    $.ajax({
    //        type: "GET",
    //        url: "/Products/GetProductCategory",
    //        contentType: "application/json; charset=utf-8",
    //        datatype: 'json',
    //        success: function (response) {
    //            $("#ProductCategoryId").empty();
    //            $("#ProductCategoryId").append(response);
    //        },
    //        error: function (data) { }
    //    });
    //});
    //$(function () {
    //    $("#btnShowModalProductSubCategory").click(function () {
    //        $.ajax({
    //            type: "GET",
    //            url: "/Products/GetProductCategory",
    //            contentType: "application/json; charset=utf-8",
    //            datatype: 'json',
    //            success: function (response) {
    //                $("#ModelProductCategoryId").empty();
    //                $("#ModelProductCategoryId").append(response);
    //            },
    //            error: function (data) { }
    //        });
    //    });
    //});
    /* State dropdown list*/
    $(document).ready(function () {

        $("#ProductCategoryId").change(function () {

            var pscId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Products/GetProductSubCategory?PCId=" + pscId,
                contentType: "html",
                success: function (response) {
                    $("#ProductSubCategoryId").empty();
                    $("#ProductSubCategoryId").append(response);
                }
            });
        });
    });
    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {


        var readURL = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }


        $(".file-upload").on('change', function () {
            readURL(this);
        });
    });
    /* For Show & hide Cate Modal*/
    $(document).ready(function () {
        $("#btnShowModalProductCategory").click(function () {
            $("#createCategory").modal('show');
        });
    });
    /* For Show & hide sub catModal*/
    $(document).ready(function () {
        $("#btnShowModalProductSubCategory").click(function () {
            $("#createSubCategory").modal('show');
        });
    });
    /* For Show & hide Unit Modal*/
    $(document).ready(function () {
        $("#btnShowModalMeasurement").click(function () {
            $("#createMeasureUnit").modal('show');
        });
    });
    /* For Show & hide Brand Modal*/
    $(document).ready(function () {
        $("#btnShowModalProductBrand").click(function () {
            $("#createBrand").modal('show');
        });
    });
    /* For Show & hide Rack Modal*/
    $(document).ready(function () {
        $("#btnShowModalProductRack").click(function () {
            $("#createRack").modal('show');
        });
    });

    $(function () {
        //LoadData();
        $("#saveCat").click(function () {
            var cat = {};
            cat.Name = $("#CatName").val();
            cat.CompanyId = $("#companyname").val();
            if (cat.Name !== null && cat.Name !== '') {
                $.ajax({
                    type: "POST",
                    url:"/Products/addCategory",
                    data: '{category: ' + JSON.stringify(cat) + '}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        $('#createCategory').modal('hide');
                        alert(data.Message);
                        location.reload();
                        // LoadData();
                    },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });

    });
    $(function () {
        //LoadData();
        $("#saveSubCat").click(function () {
            var cat = {};
            cat.ProductCategoryId = $("#ModelProductCategoryId").val();
            cat.Name = $("#CatSubName").val();
            if (cat.Name !== null && cat.Name !== '' && cat.ProductCategoryId !== null && cat.ProductCategoryId !== '') {
                $.ajax({
                    type: "POST",
                    url: "/Products/addSubCategory",
                    data: '{subcategory: ' + JSON.stringify(cat) + '}',
                    dataType: "json",
                    contentType: "application/json;charset=utf-8",
                    success: function (data) {
                        $('#createSubCategory').modal('hide');
                        alert(data.Message);
                        location.reload();
                        // LoadData();
                    },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });

    });
    $(function () {
        //LoadData();
        $("#saveUnit").click(function () {
            var cat = {};
            cat.Name = $("#UnitName").val();
            cat.CompanyId = $("#mcname").val();
            if (cat.Name !== null && cat.Name !== '') {
                $.ajax({
                    type: "POST",
                    url: "/Products/addMeasureUnit",
                    data: '{unit: ' + JSON.stringify(cat) + '}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        $('#createMeasureUnit').modal('hide');
                        alert(data.Message);
                        location.reload();
                        // LoadData();
                    },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });

    });

    $(function () {
        //LoadData();
        $("#saveBrand").click(function () {
            var cat = {};
            cat.Name = $("#BrandName").val();
            cat.CompanyId = $("#bcname").val();
            if (cat.Name !== null && cat.Name !== '') {
                $.ajax({
                    type: "POST",
                    url: "/Products/addBrand",
                    data: '{brand: ' + JSON.stringify(cat) + '}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        $('#createBrand').modal('hide');
                        alert(data.Message);
                        location.reload();
                        // LoadData();
                    },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });

    });
    $(function () {
        //LoadData();
        $("#saveRack").click(function () {
            var cat = {};
            cat.Name = $("#RackName").val();
            cat.CompanyId = $("#rcname").val();
            if (cat.Name !== null && cat.Name !== '') {
                $.ajax({
                    type: "POST",
                    url: "/Products/addRack",
                    data: '{rack: ' + JSON.stringify(cat) + '}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success:
                        function (data) {
                            $('#createRack').modal('hide');
                            location.reload();
                            alert(data.Message);
                        },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });

    });
    //Collect Multiple Order List For Pass To Controller

    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }

    for (var selector in config) {
        $('.companyList').chosen(config['.companyList']);
    }
    for (var selector in config) {
        $('.warehouseList').chosen(config['.warehouseList']);
    }
    for (var selector in config) {
        $('.supplierlist').chosen(config['.supplierlist']);
    }
    for (var selector in config) {
        $('.prcateList').chosen(config['.prcateList']);
    }
    //for (var selector in config) {
    //    $('.productscl').chosen(config['.productscl']);
    //}
    for (var selector in config) {
        $('.prmeaunit').chosen(config['.prmeaunit']);
    }
    for (var selector in config) {
        $('.prbrand').chosen(config['.prbrand']);
    }
    for (var selector in config) {
        $('.prrack').chosen(config['.prrack']);
    }

}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function productBarcodeIndex() {
    $("#search").keyup(function () {
        var value = this.value.toLowerCase().trim();

        $("table tr").each(function (index) {
            if (!index) return;
            $(this).find("td").each(function () {
                var id = $(this).text().toLowerCase().trim();
                var not_found = (id.indexOf(value) == -1);
                $(this).closest('tr').toggle(!not_found);
                return not_found;
            });
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsChartTree() {
    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsContra() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    $(document).ready(function () {

        $("#ladgername").change(function () {

            var ladgername = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Accounts/getLedger/" + ladgername,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#LName').val(item.LedgerName);
                        $('#LCode').val(item.LedgerCode);
                        $('#LId').val(item.LedgerId);


                        $('#detailsLedger thead tr').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });


    //Add Multiple Order.
    $("#addToList").click(function (e) {
        e.preventDefault();

        if ($.trim($("#ladgername").val()) == "" ||
            $.trim($("#amount").val()) == "") return;


        var date = $("#date").val(),
            ladgername = $("#LName").val(),
            ladgerid = $("#LId").val(),
            ladgercode = $("#LCode").val(),
            debit = "",
            credit = "",
            logic = $("input[name=drorcr]:checked"),
            detailsTableBody = $("#detailsLedger tbody");


        if (logic.val() == "dabit") {
            debit = $("#amount").val();
        } else {
            debit = 0;
        };
        if (logic.val() == "creadit") {
            credit = $("#amount").val();
        } else {
            credit = 0;
        };

        var ledgerdetails = '<tr><td colspan="3">' + ladgername + '</td><td>' + ladgercode + '</td><td>' + debit + '</td><td>' + credit +
            '</td><td class="text-right"><button  data-itemId="0"  type="button" class="deleteItem btn btn-warning"><i class="fa fa-fw fa-remove"></i>Remove</button></td><td><input type="hidden" id="lgID" value="' + ladgerid + '"/></td></tr>';
        detailsTableBody.append(ledgerdetails);
        // clearItem();
        transection();

    });
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                transection();
            });
        }
    });
    function transection() {
        var dabitsumVal = 0;
        var craditsumVal = 0;
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        for (var i = 0; i < table.rows.length; i++) {
            var craditsumVal = craditsumVal + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        $("#transamount").val(dabitsumVal.toFixed(2));
        $(".dabittotal").val(" Total Amount: " + dabitsumVal.toFixed(2) + "/-");
        $(".creadittotal").val(" Total Amount: " + craditsumVal.toFixed(2) + "/-");
        parseFloat($(".total").val(dabitsumVal.toFixed(2) - craditsumVal.toFixed(2)));

        if (dabitsumVal != craditsumVal || isNaN($(".dabittotal").val()) || isNaN($(".creadittotal").val()) || (isNaN($(".dabittotal").val()) == "")) {
            $("#saveData").addClass("disabled");
            $("#saveData").removeClass("saveData")
        } if ($(".total").val() == 0) {
            $("#saveData").addClass("saveData");
            $("#saveData").removeClass("disabled")
        }
    };
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('#ladgername').chosen(config['#ladgername']);
    }

    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveData").click(function (e) {
        e.preventDefault();
        if (!isNaN($(".total").val()) && $(".total").val() !== "" && $(".total").val() == 0) {
            var r = confirm("Confirm to Submit!");
            if (r == true) {
                saveData();
            };
        };
    });

    function saveData() {
        var trans = {};
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        var amount = dabitsumVal.toFixed(2);

        trans.TrasactionalAmount = $("#transamount").val(),
        trans.Narration = $("#Narration").val(),
        trans.BranchID = $("#branchName").val(),
        trans.TransactionDate = $("#date").val()

        var transArr = [];
        transArr.length = 0;
        $.each($("#detailsLedger tbody tr"), function () {
            transArr.push({
                LedgerName: $(this).find('td:eq(0)').html(),
                LedgerNo: $(this).find('td:eq(1)').html(),
                DebitAmount: $(this).find('td:eq(2)').html(),
                CreditAmount: $(this).find('td:eq(3)').html(),
                LedgerID: $(this).find('td #lgID:eq(0)').val(),
            });
        });

        var data = JSON.stringify({
            transactiondata: transArr,
            transaction: trans
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Accounts/Contra",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    };

    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsCreateLedger() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);
                }
            });
        });
    });
    //Date picker
    $(function () {
        $("#datepicker").datepicker();
    });


    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsJournal() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    $(document).ready(function () {

        $("#ladgername").change(function () {

            var ladgername = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Accounts/getLedger/" + ladgername,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#LName').val(item.LedgerName);
                        $('#LCode').val(item.LedgerCode);
                        $('#LId').val(item.LedgerId);


                        $('#detailsLedger thead tr').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });


    //Add Multiple Order.
    $("#addToList").click(function (e) {
        e.preventDefault();

        if ($.trim($("#ladgername").val()) == "" ||
            $.trim($("#amount").val()) == "") return;


        var date = $("#date").val(),
            ladgername = $("#LName").val(),
            ladgerid = $("#LId").val(),
            ladgercode = $("#LCode").val(),
            debit = "",
            credit = "",
            logic = $("input[name=drorcr]:checked"),
            detailsTableBody = $("#detailsLedger tbody");


        if (logic.val() == "dabit") {
            debit = $("#amount").val();
        } else {
            debit = 0;
        };
        if (logic.val() == "creadit") {
            credit = $("#amount").val();
        } else {
            credit = 0;
        };

        var ledgerdetails = '<tr><td colspan="3">' + ladgername + '</td><td>' + ladgercode + '</td><td>' + debit + '</td><td>' + credit +
            '</td><td class="text-right"><button  data-itemId="0"  type="button" class="deleteItem btn btn-warning"><i class="fa fa-fw fa-remove"></i>Remove</button></td><td><input type="hidden" id="lgID" value="' + ladgerid + '"/></td></tr>';
        detailsTableBody.append(ledgerdetails);
        // clearItem();
        transection();

    });
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                transection();
            });
        }
    });
    function transection() {
        var dabitsumVal = 0;
        var craditsumVal = 0;
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        for (var i = 0; i < table.rows.length; i++) {
            var craditsumVal = craditsumVal + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        $("#transamount").val(dabitsumVal.toFixed(2));
        $(".dabittotal").val(" Total Amount: " + dabitsumVal.toFixed(2) + "/-");
        $(".creadittotal").val(" Total Amount: " + craditsumVal.toFixed(2) + "/-");
        parseFloat($(".total").val(dabitsumVal.toFixed(2) - craditsumVal.toFixed(2)));

        if (dabitsumVal != craditsumVal || isNaN($(".dabittotal").val()) || isNaN($(".creadittotal").val()) || (isNaN($(".dabittotal").val()) == "")) {
            $("#saveData").addClass("disabled");
            $("#saveData").removeClass("saveData")
        } if ($(".total").val() == 0) {
            $("#saveData").addClass("saveData");
            $("#saveData").removeClass("disabled")
        }
    };
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('#ladgername').chosen(config['#ladgername']);
    }

    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveData").click(function (e) {
        e.preventDefault();
        if (!isNaN($(".total").val()) && $(".total").val() !== "" && $(".total").val() == 0) {
            var r = confirm("Confirm to Submit!");
            if (r == true) {
                saveData();
            };
        };
    });

    function saveData() {
        var trans = {};
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        var amount = dabitsumVal.toFixed(2);

        trans.TrasactionalAmount = $("#transamount").val(),
        trans.Narration = $("#Narration").val(),
        trans.BranchID = $("#branchName").val(),
        trans.TransactionDate = $("#date").val()

        var transArr = [];
        transArr.length = 0;
        $.each($("#detailsLedger tbody tr"), function () {
            transArr.push({
                LedgerName: $(this).find('td:eq(0)').html(),
                LedgerNo: $(this).find('td:eq(1)').html(),
                DebitAmount: $(this).find('td:eq(2)').html(),
                CreditAmount: $(this).find('td:eq(3)').html(),
                LedgerID: $(this).find('td #lgID:eq(0)').val(),
            });
        });
        var data = JSON.stringify({
            transactiondata: transArr,
            transaction: trans
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Accounts/Journal",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    };


    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsLedgerdetails() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#showData").modal('show');

        $("#btnHideModal").click(function () {
            var lID = $("#LIDS").val();
            var dateformDate = $("#dateformDate").val();
            var datetoDatae = $("#datetoDatae").val();
            if (dateformDate != "" && datetoDatae != "" && lID != "") {
                $("#showData").modal('hide');
                $("#dateForm").append($("#dateformDate").val());
                $("#dateTo").append($("#datetoDatae").val());
                dataLoad();
            }
        });
    });
    function dataLoad() {
        var lID = $("#LIDS").val();
        var dateformDate = $("#dateformDate").val();
        var datetoDatae = $("#datetoDatae").val();

        // var pageObject = { DateformDate: DateformDate, DatetoDatae: DatetoDatae };
        //
        $("#my_table").DataTable({
            //"processing": true,
            //"serverSide":true,
            "ajax": {
                url: "/Accounts/LedgerdetailsData",
                //"url": "/Accounts/LedgerdetailsData",
                "type": "GET",
                data: { id: lID, DateformDate: dateformDate, DatetoDatae: datetoDatae },
                contentType: "application/json; charset=utf-8",
                "datatype": "json"
            },
            "columns": [
                {
                    "data": "Date",
                    "type": "date ",
                    "render": function (value) {
                        if (value === null) return "";

                        var pattern = /Date\(([^)]+)\)/;
                        var results = pattern.exec(value);
                        var dt = new Date(parseFloat(results[1]));

                        return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                    }
                },
                { "data": "Name" },
                { "data": "Vouchertype" },
                { "data": "Voucher" },
                { "data": "Debit" },
                { "data": "Cradit" }
            ],
            "dom": "lrBftip",
            "buttons": [
                {
                    extend: 'copy',
                    className: 'Button',
                    text: '<i class="fa fa-copy" aria-hidden="true"></i> Copy'
                },
                {
                    extend: 'excel',
                    className: 'Button',
                    text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Excel'
                },
                {
                    extend: 'pdf',
                    className: 'Button',
                    text: '<i class="fa fa-file-pdf-o" aria-hidden="true"></i> Pdf'
                },
                {
                    extend: 'csv',
                    className: 'Button',
                    text: '<i class="fa fa-file-text"></i> CSV'
                },
                {
                    extend: 'print',
                    className: 'Button',
                    text: '<i class="fa fa-print" aria-hidden="true"></i> Print'
                }

            ],
            "initComplete": function (settings, json) {
                this.api().columns('.debit').every(function () {
                    var column = this;

                    var intVal = function (i) {
                        return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    var sum = column
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        });

                    $(column.footer()).html(sum);
                    balancecd();
                });
                this.api().columns('.cradit').every(function () {
                    var column = this;

                    var intVal = function (i) {
                        return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    var sum = column
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        });
                    $(column.footer()).html(sum);
                    balancecd();
                });
            }
        });
    };

    $(document).ready(function () {
        var config = {
            '.chosen-select-deselect': { allow_single_deselect: true },
            '.chosen-select-no-single': { disable_search_threshold: 10 },
            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
            '.chosen-select-rtl': { rtl: true },
            '.chosen-select-width': { width: '95%' }
        }
        for (var selector in config) {
            $('#LIDS').chosen(config['#LIDS']);
        };


        $("#LIDS").change(function () {

            var ledgerid = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Accounts/GetLedgerName/" + ledgerid,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#lName").append(item.LedgerName);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });

    function balancecd() {
        var damutotal = $(".dAmountTotal").text();
        if (isNaN(damutotal)) {
            damutotal = $(".dAmountTotal").text("0");
        }
        var camutotal = $(".cAmountTotal").text();
        if (isNaN(camutotal)) {
            camutotal = $(".cAmountTotal").text("0");
        }
        var sumValue = parseFloat(damutotal - camutotal).toFixed(2);
        if (sumValue >= 0) {
            var value = sumValue.replace(/-/g, "")
            $(".isCredit").html(value);
            $(".closingBL").html("Closing Balance   " + value + " Dr");
            $(".isDebit").html(0);
        } else {
            var value = sumValue.replace(/-/g, "")
            $(".isDebit").html(value);
            $(".closingBL").html("Closing Balance   " + value + " Cr");
            $(".isCredit").html(0);
        }
        var dramounttotal = $(".dAmountTotal").text();
        var cramounttotal = $(".cAmountTotal").text();

        var isCredit = $(".isCredit").text();
        var isDebit = $(".isDebit").text();

        var sumTotaldr = parseFloat(parseFloat(dramounttotal) + parseFloat(isDebit)).toFixed(2);
        var sumTotalcr = parseFloat(parseFloat(cramounttotal) + parseFloat(isCredit)).toFixed(2);
        $(".drTotal").text(sumTotaldr);
        $(".crTotal").text(sumTotalcr);
    };

}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsPayment() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    $(document).ready(function () {

        $("#ladgername").change(function () {

            var ladgername = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Accounts/getLedger/" + ladgername,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#LName').val(item.LedgerName);
                        $('#LCode').val(item.LedgerCode);
                        $('#LId').val(item.LedgerId);


                        $('#detailsLedger thead tr').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });


    //Add Multiple Order.
    $("#addToList").click(function (e) {
        e.preventDefault();

        if ($.trim($("#ladgername").val()) == "" ||
            $.trim($("#amount").val()) == "") return;


        var date = $("#date").val(),
            ladgername = $("#LName").val(),
            ladgerid = $("#LId").val(),
            ladgercode = $("#LCode").val(),
            debit = "",
            credit = "",
            logic = $("input[name=drorcr]:checked"),
            detailsTableBody = $("#detailsLedger tbody");


        if (logic.val() == "dabit") {
            debit = $("#amount").val();
        } else {
            debit = 0;
        };
        if (logic.val() == "creadit") {
            credit = $("#amount").val();
        } else {
            credit = 0;
        };

        var ledgerdetails = '<tr><td colspan="3">' + ladgername + '</td><td>' + ladgercode + '</td><td>' + debit + '</td><td>' + credit +
            '</td><td class="text-right"><button  data-itemId="0"  type="button" class="deleteItem btn btn-warning"><i class="fa fa-fw fa-remove"></i>Remove</button></td><td><input type="hidden" id="lgID" value="' + ladgerid + '"/></td></tr>';
        detailsTableBody.append(ledgerdetails);
        // clearItem();
        transection();

    });
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                transection();
            });
        }
    });
    function transection() {
        var dabitsumVal = 0;
        var craditsumVal = 0;
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        for (var i = 0; i < table.rows.length; i++) {
            var craditsumVal = craditsumVal + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        $("#transamount").val(dabitsumVal.toFixed(2));
        $(".dabittotal").val(" Total Amount: " + dabitsumVal.toFixed(2) + "/-");
        $(".creadittotal").val(" Total Amount: " + craditsumVal.toFixed(2) + "/-");
        parseFloat($(".total").val(dabitsumVal.toFixed(2) - craditsumVal.toFixed(2)));

        if (dabitsumVal != craditsumVal || isNaN($(".dabittotal").val()) || isNaN($(".creadittotal").val()) || (isNaN($(".dabittotal").val()) == "")) {
            $("#saveData").addClass("disabled");
            $("#saveData").removeClass("saveData")
        } if ($(".total").val() == 0) {
            $("#saveData").addClass("saveData");
            $("#saveData").removeClass("disabled")
        }
    };
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('#ladgername').chosen(config['#ladgername']);
    }

    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveData").click(function (e) {
        e.preventDefault();
        if (!isNaN($(".total").val()) && $(".total").val() !== "" && $(".total").val() == 0) {
            var r = confirm("Confirm to Submit!");
            if (r == true) {
                saveData();
            };
        };
    });

    function saveData() {
        var trans = {};
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        var amount = dabitsumVal.toFixed(2);

        trans.TrasactionalAmount = $("#transamount").val(),
        trans.Narration = $("#Narration").val(),
        trans.BranchID = $("#branchName").val(),
        trans.TransactionDate = $("#date").val()

        var transArr = [];
        transArr.length = 0;
        $.each($("#detailsLedger tbody tr"), function () {
            transArr.push({
                LedgerName: $(this).find('td:eq(0)').html(),
                LedgerNo: $(this).find('td:eq(1)').html(),
                DebitAmount: $(this).find('td:eq(2)').html(),
                CreditAmount: $(this).find('td:eq(3)').html(),
                LedgerID: $(this).find('td #lgID:eq(0)').val(),
            });
        });

        var data = JSON.stringify({
            transactiondata: transArr,
            transaction: trans
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Accounts/Payment",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    };

    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function accountsReceived() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    $(document).ready(function () {

        $("#ladgername").change(function () {

            var ladgername = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Accounts/getLedger/" + ladgername,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        var rows =
                            $('#LName').val(item.LedgerName);
                        $('#LCode').val(item.LedgerCode);
                        $('#LId').val(item.LedgerId);


                        $('#detailsLedger thead tr').append(rows);
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }

            });

        });

    });


    //Add Multiple Order.
    $("#addToList").click(function (e) {
        e.preventDefault();

        if ($.trim($("#ladgername").val()) == "" ||
            $.trim($("#amount").val()) == "") return;


        var date = $("#date").val(),
            ladgername = $("#LName").val(),
            ladgerid = $("#LId").val(),
            ladgercode = $("#LCode").val(),
            debit = "",
            credit = "",
            logic = $("input[name=drorcr]:checked"),
            detailsTableBody = $("#detailsLedger tbody");


        if (logic.val() == "dabit") {
            debit = $("#amount").val();
        } else {
            debit = 0;
        };
        if (logic.val() == "creadit") {
            credit = $("#amount").val();
        } else {
            credit = 0;
        };

        var ledgerdetails = '<tr><td colspan="3">' + ladgername + '</td><td>' + ladgercode + '</td><td>' + debit + '</td><td>' + credit +
            '</td><td class="text-right"><button  data-itemId="0"  type="button" class="deleteItem btn btn-warning"><i class="fa fa-fw fa-remove"></i>Remove</button></td><td><input type="hidden" id="lgID" value="' + ladgerid + '"/></td></tr>';
        detailsTableBody.append(ledgerdetails);
        // clearItem();
        transection();

    });
    $(document).on('click', 'button.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
                transection();
            });
        }
    });
    function transection() {
        var dabitsumVal = 0;
        var craditsumVal = 0;
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        for (var i = 0; i < table.rows.length; i++) {
            var craditsumVal = craditsumVal + parseFloat(table.rows[i].cells[3].innerHTML);
        }
        $("#transamount").val(dabitsumVal.toFixed(2));
        $(".dabittotal").val(" Total Amount: " + dabitsumVal.toFixed(2) + "/-");
        $(".creadittotal").val(" Total Amount: " + craditsumVal.toFixed(2) + "/-");
        parseFloat($(".total").val(dabitsumVal.toFixed(2) - craditsumVal.toFixed(2)));

        if (dabitsumVal != craditsumVal || isNaN($(".dabittotal").val()) || isNaN($(".creadittotal").val()) || (isNaN($(".dabittotal").val()) == "")) {
            $("#saveData").addClass("disabled");
            $("#saveData").removeClass("saveData")
        } if ($(".total").val() == 0) {
            $("#saveData").addClass("saveData");
            $("#saveData").removeClass("disabled")
        }
    };
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('#ladgername').chosen(config['#ladgername']);
    }

    //After Click Save Button Pass All Data View To Controller For Save Database

    $("#saveData").click(function (e) {
        e.preventDefault();
        if (!isNaN($(".total").val()) && $(".total").val() !== "" && $(".total").val() == 0) {
            var r = confirm("Confirm to Submit!");
            if (r == true) {
                saveData();
            };
        };
    });
    function saveData() {
        var trans = {};
        var table = document.getElementById("details");
        for (var i = 0; i < table.rows.length; i++) {
            var dabitsumVal = dabitsumVal + parseFloat(table.rows[i].cells[2].innerHTML);
        }
        var amount = dabitsumVal.toFixed(2);

        trans.TrasactionalAmount = $("#transamount").val(),
        trans.Narration = $("#Narration").val(),
        trans.BranchID = $("#branchName").val(),
        trans.TransactionDate = $("#date").val()

        var transArr = [];
        transArr.length = 0;
        $.each($("#detailsLedger tbody tr"), function () {
            transArr.push({
                LedgerName: $(this).find('td:eq(0)').html(),
                LedgerNo: $(this).find('td:eq(1)').html(),
                DebitAmount: $(this).find('td:eq(2)').html(),
                CreditAmount: $(this).find('td:eq(3)').html(),
                LedgerID: $(this).find('td #lgID:eq(0)').val(),
            });
        });

        var data = JSON.stringify({
            transactiondata: transArr,
            transaction: trans
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Accounts/Received",
            data: data,
            //data: '{orders: ' + JSON.stringify(order) + '}',
            success: function (result) {
                alert(result);
                location.reload();
            },
            error: function () {
                alert("Insert Valid Data!");
            }
        });
    };

    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function businessAddNewBusiness() {
    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    /* Date Picker Looltip */
    $(document).ready(function () {
        $('[data_toggle="tooltip"]').tooltip();
    });

    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {


        var readURL = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }


        $(".file-upload").on('change', function () {
            readURL(this);
        });
    });

    /* Tav Panel*/
    $(document).ready(function () {
        //Initialize tooltips
        $('.nav-tabs > li a[title]').tooltip();

        //Wizard
        $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

            var $target = $(e.target);

            if ($target.parent().hasClass('disabled')) {
                return false;
            }
        });

        $(".next-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            $active.next().removeClass('disabled');
            nextTab($active);

        });
        $(".prev-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            prevTab($active);

        });
    });

    function nextTab(elem) {
        $(elem).next().find('a[data-toggle="tab"]').click();
    }
    function prevTab(elem) {
        $(elem).prev().find('a[data-toggle="tab"]').click();
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function businessIndex() {
    $(function () {
        //Loop through all Child Grids.
        $("#gridT .ChildGrid").each(function () {
            //Copy the Child Grid to DIV.
            var childGrid = $(this).clone();
            $(this).closest("TR").find("TD").eq(0).find("DIV").append(childGrid);

            //Remove the Last Column from the Row.
            $(this).parent().remove(0);
        });

        //Remove Last Column from Header Row.
        $(".img").parent().css({ "width": "20px" });
        $("#gridT TH:last-child").eq(0).remove(0);
        $("#gridT  Tr th:last-child").eq(0).remove(0);
    });
    //Assign Click event to Plus Image.
    $("body").on("click", "img[src*='plus.png']", function () {
        $(this).closest("tr").after("</td><td colspan = '6'>" + $(this).next().html() + "</td></tr>");
        $(this).attr("src", "/images/minus.png");
    });
    //Assign Click event to Minus Image.
    $("body").on("click", "img[src*='minus.png']", function () {
        $(this).attr("src", "/images/plus.png");
        $(this).closest("tr").next().remove();
    });

    $("#search").keyup(function () {
        var value = this.value.toLowerCase().trim();

        $("table tr").each(function (index) {
            if (!index) return;
            $(this).find("td").each(function () {
                var id = $(this).text().toLowerCase().trim();
                var not_found = (id.indexOf(value) == -1);
                $(this).closest('tr').toggle(!not_found);
                return not_found;
            });
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function customerCreate() {
    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);
                }
            });
        });
    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {

        var readUrl = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }

        $(".file-upload").on('change', function () {
            readUrl(this);
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function dressmodelsCreate() {
    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {


        var readURL = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        $(".file-upload").on('change', function () {
            readURL(this);
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function easyaccountExpense() {
    //Get Closing Balance
    $(document).ready(function () {
        $("#closingbalance").change(function () {
            var id = $(this).val();
            $("#getclosingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/EasyAccount/GetClosingBalance/" + id,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#getclosingbalance").append($("#getclosingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }            
            });
        });
        $("#payAmount").change(function () {
            var id = document.getElementById('payAmount').value;
            var preAmount = document.getElementById('getclosingbalance').value;
            var result = parseFloat(id) + parseFloat(preAmount);
            if (!isNaN(result)) {
                document.getElementById('getCurentBalance').value = result;
            }
        });
    });
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('#closingbalance').chosen(config['#closingbalance']);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function easyaccountReceived() {
    //Get Closing Balance
    $(document).ready(function () {
        $("#closingbalance").change(function () {
            var id = $(this).val();
            $("#getclosingbalance").val(" ");
            $("#details").val(" ");
            $.ajax({
                type: "Post",
                url: "/EasyAccount/GetClosingBalance/" + id,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#getclosingbalance").append($("#getclosingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

            $.ajax({
                type: "Post",
                url: "/EasyAccount/GetCustomerInformation/" + id,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#details").append($("#details").val(data[0].ContactFirstName + " " + data[0].ContactLastName + "   " + data[0].PhoneNo));
                    }
                },
                error: function (ex) {
                   
                }
            });
            
        });
        $("#payAmount").change(function () {
            var id = document.getElementById('payAmount').value;
            var preAmount = document.getElementById('getclosingbalance').value;
            var result = parseFloat(preAmount)-parseFloat(id);
            if(!isNaN(result)) {
                document.getElementById('getCurentBalance').value = result;
            }
        });
    });
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }

    for (var selector in config) {
        $('#closingbalance').chosen(config['#closingbalance']);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function easyaccountPaymentReport() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });
    $("#search").keyup(function () {
        var value = this.value.toLowerCase().trim();

        $("table tr").each(function (index) {
            if (!index) return;
            $(this).find("td").each(function () {
                var id = $(this).text().toLowerCase().trim();
                var not_found = (id.indexOf(value) == -1);
                $(this).closest('tr').toggle(!not_found);
                return not_found;
            });
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function easyaccountReceivedReport() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });
    $("#search").keyup(function () {
        var value = this.value.toLowerCase().trim();

        $("table tr").each(function (index) {
            if (!index) return;
            $(this).find("td").each(function () {
                var id = $(this).text().toLowerCase().trim();
                var not_found = (id.indexOf(value) == -1);
                $(this).closest('tr').toggle(!not_found);
                return not_found;
            });
        });
    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function easyaccountSupplierPayment() {
    //Get Closing Balance
    $(document).ready(function () {
        $("#closingbalance").change(function () {
            var id = $(this).val();
            $("#getclosingbalance").val(" ");
            $.ajax({
                type: "Post",
                url: "/EasyAccount/GetClosingBalance/" + id,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var items = '';
                    var int = 0;
                    $.each(data, function (i, item) {
                        $("#getclosingbalance").append($("#getclosingbalance").val(item.ClosinngBalance));
                    });
                },
                error: function (ex) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });
            $.ajax({
                type: "Post",
                url: "/EasyAccount/GetSupplierInformation/" + id,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#details").append($("#details").val(data[0].ContactFirstName + " " + data[0].ContactLastName + "   " + data[0].PhoneNo));
                    }
                },
                error: function (ex) {

                }
            });
        });
        $("#payAmount").change(function () {
            var id = document.getElementById('payAmount').value;
            var preAmount = document.getElementById('getclosingbalance').value;
            var result = parseFloat(id) + parseFloat(preAmount);
            if (!isNaN(result)) {
                document.getElementById('getCurentBalance').value = result;
            }
        });

    });
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }

    for (var selector in config) {
        $('#closingbalance').chosen(config['#closingbalance']);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function employeeEmployee() {
    $(function () {
        //LoadData();
        $("#saveEmpType").click(function () {
            var emt = {};
            emt.Name = $("#empt").val();
            emt.CompanyId = $("#companyname").val();
            if (emt.Name !== null && emt.Name !== '') {
                $.ajax({
                    type: "POST",
                    url: "/Employees/addEmptypes",
                    data: '{empTypes: ' + JSON.stringify(emt) + '}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        $('#Emptypemodel').modal('hide');
                        alert(data.Message);
                        location.reload();
                    },
                    error: function () {
                        alert("Error while inserting data");
                    }
                });
            } else {
                alert("Error while inserting data");
            }
            return false;
        });
    });
    $(document).ready(function () {
        $("#Emptypebtn").click(function () {
            $("#Emptypemodel").modal('show');
        });
    });
    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    /* Date Picker Looltip */
    $(document).ready(function () {
        $('[data_toggle="tooltip"]').tooltip();
    });

    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {


        var readUrl = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }


        $(".file-upload").on('change', function () {
            readUrl(this);
        });
    });

    /* Tav Panel*/
    $(document).ready(function () {
        //Initialize tooltips
        $('.nav-tabs > li a[title]').tooltip();

        //Wizard
        $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

            var $target = $(e.target);

            if ($target.parent().hasClass('disabled')) {
                return false;
            }
        });

        $(".next-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            $active.next().removeClass('disabled');
            nextTab($active);

        });
        $(".prev-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            prevTab($active);

        });
    });

    function nextTab(elem) {
        $(elem).next().find('a[data-toggle="tab"]').click();
    }
    function prevTab(elem) {
        $(elem).prev().find('a[data-toggle="tab"]').click();
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function empnomineeCreate() {
    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);

                }

            });

        });

    });

    /* Date Picker Looltip */
    $(document).ready(function () {
        $('[data_toggle="tooltip"]').tooltip();
    });

    /* File Upload validate*/
    $('#file').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {

        var readUrl = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }


        $(".file-upload").on('change', function () {
            readUrl(this);
        });
    });


    /* File Upload validate*/
    $('#file2').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 2; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('#alert').html('The file is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        } else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('#alert').html('An not allowed file format has been added \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {

        var readUrl = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.avatar2').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }


        $(".file-upload2").on('change', function () {
            readUrl(this);
        });
    });
    /* Tav Panel*/
    $(document).ready(function () {
        //Initialize tooltips
        $('.nav-tabs > li a[title]').tooltip();

        //Wizard
        $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

            var $target = $(e.target);

            if ($target.parent().hasClass('disabled')) {
                return false;
            }
        });

        $(".next-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            $active.next().removeClass('disabled');
            nextTab($active);

        });
        $(".prev-step").click(function (e) {

            var $active = $('.wizard .nav-tabs li.active');
            prevTab($active);

        });
    });

    function nextTab(elem) {
        $(elem).next().find('a[data-toggle="tab"]').click();
    }
    function prevTab(elem) {
        $(elem).prev().find('a[data-toggle="tab"]').click();
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function fileUploadFiles() {
    $(document).ready(function () {

        $("#comid").val(" ");
        $("#companyId").change(function () {
            var comid = $("#companyId").val();
            $("#comid").val(comid);
        });

        $(".radioList").click(function () {
            var changes = $(this).attr("change");
            var inputValue = $(this).attr("chandrim");
            var targetBox = $("." + inputValue);
            $(".boxes").not(targetBox).hide();
            $(targetBox).show();

            if (changes == "customer") {
                document.getElementById("reftype").value = changes;
                $("#refid").val(" ");
                $("#CustomerId").change(function () {
                    var refid = $("#CustomerId").val();
                    $("#refid").val(refid);
                });

                $('[change="' + changes + '"]').addClass("fa fa-circle");
                $('.emp').removeClass("fa fa-circle");
                $('.emp').addClass("fa fa-circle-o");
                $('.sup').removeClass("fa fa-circle");
                $('.sup').addClass("fa fa-circle-o");
                $('.bra').removeClass("fa fa-circle");
                $('.bra').addClass("fa fa-circle-o");
                $('.com').removeClass("fa fa-circle");
                $('.com').addClass("fa fa-circle-o");

            } else if (changes == "employee") {
                document.getElementById("reftype").value = changes;
                $("#refid").val(" ");
                $("#EmployeeId").change(function () {
                    var refid = $("#EmployeeId").val();
                    $("#refid").val(refid);
                });
                $('[change="' + changes + '"]').addClass("fa fa-circle");
                $('.sup').removeClass("fa fa-circle");
                $('.sup').addClass("fa fa-circle-o");
                $('.bra').removeClass("fa fa-circle");
                $('.bra').addClass("fa fa-circle-o");
                $('.com').removeClass("fa fa-circle");
                $('.com').addClass("fa fa-circle-o");
                $('.cust').removeClass("fa fa-circle");
                $('.cust').addClass("fa fa-circle-o");
            } else if (changes == "supplier") {
                document.getElementById("reftype").value = changes;
                $("#refid").val(" ");
                $("#SupplierId").change(function () {
                    var refid = $("#SupplierId").val();
                    $("#refid").val(refid);
                });
                $('[change="' + changes + '"]').addClass("fa fa-circle");
                $('.emp').removeClass("fa fa-circle");
                $('.emp').addClass("fa fa-circle-o");
                $('.bra').removeClass("fa fa-circle");
                $('.bra').addClass("fa fa-circle-o");
                $('.com').removeClass("fa fa-circle");
                $('.com').addClass("fa fa-circle-o");
                $('.cust').removeClass("fa fa-circle");
                $('.cust').addClass("fa fa-circle-o");
            } else if (changes == "branch") {
                document.getElementById("reftype").value = changes;
                $("#refid").val(" ");
                $("#BranchId").change(function () {
                    var refid = $("#BranchId").val();
                    $("#refid").val(refid);
                });
                $('[change="' + changes + '"]').addClass("fa fa-circle");
                $('.emp').removeClass("fa fa-circle");
                $('.emp').addClass("fa fa-circle-o");
                $('.sup').removeClass("fa fa-circle");
                $('.sup').addClass("fa fa-circle-o");
                $('.com').removeClass("fa fa-circle");
                $('.com').addClass("fa fa-circle-o");
                $('.cust').removeClass("fa fa-circle");
                $('.cust').addClass("fa fa-circle-o");
            } else {
                document.getElementById("reftype").value = changes;
                $("#refid").val(" ");
                $("#CompanyId").change(function () {
                    var refid = $("#CompanyId").val();
                    $("#refid").val(refid);
                });
                $('[change="' + changes + '"]').addClass("fa fa-circle");
                $('.emp').removeClass("fa fa-circle");
                $('.emp').addClass("fa fa-circle-o");
                $('.sup').removeClass("fa fa-circle");
                $('.sup').addClass("fa fa-circle-o");
                $('.bra').removeClass("fa fa-circle");
                $('.bra').addClass("fa fa-circle-o");
                $('.cust').removeClass("fa fa-circle");
                $('.cust').addClass("fa fa-circle-o");
            }
        });
    });
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    for (var selector in config) {
        $('.customerList').chosen(config['.customerList']);
    }
    for (var selector in config) {
        $('.companyList').chosen(config['.companyList']);
    }
    for (var selector in config) {
        $('.employeeList').chosen(config['.employeeList']);
    }
    for (var selector in config) {
        $('.branchList').chosen(config['.branchList']);
    }
    for (var selector in config) {
        $('.supplierList').chosen(config['.supplierList']);
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function ledgerCreate() {
    /* For Show & hide Modal*/
    $(document).ready(function () {
        $("#btnShowModal").click(function () {
            $("#charttree").modal('show');
        });

        $("#btnHideModal").click(function () {
            $("#charttree").modal('hide');
        });
    });

    /* country dropdown list*/
    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Country/GetCountryList",
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                $("#Country").empty();
                $("#Country").append(response);
            },
            error: function (data) { }
        });
    });
    /* State dropdown list*/
    $(document).ready(function () {

        $("#Country").change(function () {

            var countryId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetStateList?CountryId=" + countryId,
                contentType: "html",
                success: function (response) {
                    $("#State").empty();
                    $("#State").append(response);

                }

            });

        });

    });
    /* City dropdown list*/
    $(document).ready(function () {

        $("#State").change(function () {

            var stateId = $(this).val();
            $.ajax({
                type: "Post",
                url: "/Country/GetCityList?StateId=" + stateId,
                contentType: "html",
                success: function (response) {
                    $("#City").empty();
                    $("#City").append(response);
                }
            });
        });
    });
    //Date picker
    $(function () {
        $("#datepicker").datepicker();
    });


    $(function () {
        $('#treebody').jstree({
            'core': {
                'data': {
                    'url': '/Accounts/Nodes',
                    'dataType': 'json'
                },
            },
            'types': {
                "root": {
                    "icon": "glyphicon glyphicon-folder-close"
                },
                "chart": {
                    "icon": "glyphicon glyphicon-folder-open"
                },
                "cat": {
                    "icon": "glyphicon glyphicon-duplicate"
                },
                "ledger": {
                    "icon": "glyphicon glyphicon-file"
                },
                "default": {

                }
            },
            plugins: ["search", "themes", "types"]
        })
        var to = false;
        $('#tree_q').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#tree_q').val();
                $('#treebody').jstree(true).search(v);
            }, 250);
        });

        $('#treebody').on('changed.jstree',
            function (e, data) {
                var i, j, r = [];
                for (i = 0, j = data.selected.length; i < j; i++) {
                    r.push(data.instance.get_node(data.selected[i]).text)
                };
                alert('Selected: ' + r.join(', '));
                // $('#event_result').html('Selected: ' + r.join(', '));
            }).jstree();

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function orderSalesOrder() {
    $(document).ready(function () {
        var size = $("#main #gridT > thead > tr >th").length; // get total column
        $("#main #gridT > thead > tr >th").last().remove(); // remove last column

        $("#main #gridT > thead > tr").prepend("<th></th>"); // add one column at first for collapsible column
        $("#main #gridT > tbody > tr").each(function (i, el) {
            $(this).prepend(
                    $("<td></td>")
                    .addClass("expand")
                    .addClass("hoverEff in")
                    .attr('title', "click for show/hide")
                );
            //Now get sub table from last column and add this to the next new added row
            var table = $("table", this).parent().html();
            //add new row with this subtable
            $(this).after("<tr><td></td><td style='padding:5px; margin:0px;' colspan='" + (size - 1) + "'>" + table + "</td></tr>");
            $("table", this).parent().remove();
            // ADD CLICK EVENT FOR MAKE COLLAPSIBLE
            $(".hoverEff", this).on("click", null, function () {
                $(this).parent().closest("tr").next().slideToggle(100);
                $(this).toggleClass("expand collapse");
            });
        });

        //by default make all subgrid in collapse mode
        $("#main #gridT > tbody > tr td.expand").each(function (i, el) {
            $(this).toggleClass("expand collapse");
            $(this).parent().closest("tr").next().slideToggle(100);
        });

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////