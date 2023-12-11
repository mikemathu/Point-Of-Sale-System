    $(document).ready(function () {        

        function ReloadPage() {
            LoadAllItemCategories();
            LoadAllItemClasses();
            LoadAllInventorySubAccounts();
            LoadAllRevenueSubAccounts();
            LoadAllCostOfSaleSubAccounts();
            LoadAllVatTypes();
            LoadAllUnitOfMeasures();
            LoadPOSCategories();
            FilterProducts()
        }

        function LoadAllItemCategories() {
            var itemCategorySelect = $("#Item_ItemCategoryID");
            var openStockItemCategoryIds = $("#OpenStockItemCategoryIds");

            itemCategorySelect.empty().append("<option></option>");
            openStockItemCategoryIds.empty().append("<option></option>");

            AjaxServerCallAsync("GET", "/Inventory/GetAllItemCategories/", "", "", function (response) {
                var itemCategories = response.response;

                if (response.status && !$.isEmptyObject(itemCategories)) {
                    var tableRows = "";

                    $("#itemcategoriestable tbody").empty();

                    itemCategories.forEach(category => {
                        itemCategorySelect.append(new Option(category.itemCategoryName, category.itemCategoryID));
                        openStockItemCategoryIds.append(new Option(category.itemCategoryName, category.itemCategoryID));

                        tableRows += '<tr id="' + category.itemCategoryID + '"><td data-title="Item Category ID">' + category.itemCategoryID + '</td><td data-title="Name">' + category.itemCategoryName + '</td></tr>';
                    });

                    $("#itemcategoriestable").append(tableRows);
                }
            });
        }

        function GetItemCategoryDetails(categoryID) {
            var requestVerificationToken = $("#ItemCategoryForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/GetItemCategoryDetails/", categoryID, requestVerificationToken, function (response) {
                var categoryDetails = response.response;

                if (response.status) {
                    $("#ItemCategory_ItemCategoryID").val(categoryDetails.itemCategoryID);
                    $("#ItemCategory_Name").val(categoryDetails.itemCategoryName);
                    $("#ItemCategory_Description").val(categoryDetails.description);
                    $("#additemcat").hide();
                    $("#updateitemcat").show();
                }
            });
        }

        function DeleteItemCategory(itemCategoryID) {
            var requestToken = $("#ItemCategoryForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/DeleteItemCategory/", itemCategoryID, requestToken, function (response) {
                var itemResponse = response.response;

                if (response.status) {
                    $("#itemcategoriestable #" + itemCategoryID).remove();
                    Notify(!0, "Item Category Deleted Successfully");
                } else {
                    Notify(!1, itemResponse);
                }
            });
        }


        function LoadAllItemClasses() {
            AjaxServerCallAsync("GET", "/Inventory/GetAllItemClasses/", "", "", function (response) {
                var itemClasses = response.response;

                if (response.status && !$.isEmptyObject(itemClasses)) {
                    var itemClassSelect = $("#Item_ItemClassID");
                    itemClassSelect.empty().append("<option></option>");

                    $("#itemclassestable tbody").empty();

                    var rowsToAdd = "";

                    for (var i = 0; i < itemClasses.length; i++) {
                        let newOption = new Option(itemClasses[i].itemClassName, itemClasses[i].itemClassID);
                        itemClassSelect.append(newOption);

                        rowsToAdd += '<tr id="' + itemClasses[i].itemClassID + '"><td data-title="Item Class ID">' + itemClasses[i].itemClassID + '<\/td><td data-title="Name">' + itemClasses[i].itemClassName + "<\/td><\/tr>";
                    }

                    $("#itemclassestable").append(rowsToAdd);
                }
            });
        }

     function GetItemClassDetails(itemID) {
        var requestToken = $("#ItemClassForm input[name=__RequestVerificationToken]").val();

        AjaxServerCallAsync("POST", "/Inventory/GetItemClassDetails/", itemID, requestToken, function (response) {
            var itemClassDetails = response.response;

            if (response.status) {
                $("#ItemClass_ItemClassID").val(itemClassDetails.itemClassID);
                $("#ItemClass_Name").val(itemClassDetails.itemClassName);
                $("#ItemClass_Description").val(itemClassDetails.description);
                $("#ItemClass_ItemClassType").val(itemClassDetails.itemClassTypeID);
                $("#additemclass").hide();
                $("#updateitemclass").show();
            }
        });
    }

         function DeleteItemClass(itemId) {
            var token = $("#ItemClassForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/DeleteItemClass/", itemId, token, function (response) {
                var itemResponse = response.response;
                if (response.status) {
                    $("#itemclassestable #" + itemId).remove();
                    Notify(true, "Item Class Deleted Successfully");
                } else {
                    Notify(false, itemResponse);
                }
            });
        }

        function LoadPOSCategories() {
            var t = $("#InventoryItemForm input[name=__RequestVerificationToken]").val()
                , n = $("#CompanyBranchItem_PointOfSaleCategoryID");
            n.empty();
            n.append("<option><\/option>");
            GetOrPostAsync("GET", "/Inventory/GetPointOfSaleCategories/", "", t).then(t => {
                if (!$.isEmptyObject(t))
                    for (var i = 0; i < t.length; i++)
                        n.append(new Option(t[i].pointOfSaleCategoryName, t[i].pointOfSaleCategoryID))
            }
            ).catch(n => Notify(!1, n))
        }

        function GetInventoryItemDetails(n) {
            var t = n
                , i = $("#InventoryItemForm input[name=__RequestVerificationToken]").val();
            AjaxServerCallAsync("POST", "/Inventory/GetItemDetails/", t, i, function (n) {
                var t = n.response;
                if (n.status) {
                    //$("#btnCreateNewProduct").click();
                    //let n = new Date(t.itemStorageLocation.expiryDate);
                    let n = new Date(t.expiryDate);
                    $(`#Item_ItemCategoryID option[value='${t.itemCategoryID}']`).length > 0 ? $("#Item_ItemCategoryID").val(t.itemCategoryID) : AddItemCategory(t.itemCategoryID, t.storageLocationID);
                    $("#Item_ItemID").val(t.itemID);
                    $("#Item_Name").val(t.itemName);
                    $(".prod-name").text(t.itemName);
                    $("#Item_ItemClassID").val(t.itemClassID);
                    $("#CompanyBranchItem_AssetSubAccountID").val(t.assetSubAccountID);
                    $("#CompanyBranchItem_CompanyBranchItemID").val(t.companyBranchItemID);
                    $("#CompanyBranchItem_CostOfSaleSubAccountID").val(t.costOfSaleSubAccountID);
                    $("#CompanyBranchItem_RevenueSubAccountID").val(t.revenueSubAccountID);
                    $("#CompanyBranchItem_VATTypeID").val(t.vatTypeID);
                    $("#Item_UnitOfMeasureID").val(t.unitOfMeasureID);
                    $("#CompanyBranchItem_ItemCode").val(t.itemCode);
                    $("#CompanyBranchItem_Barcode").val(t.barcode);
                    $("#CompanyBranchItem_PointOfSaleCategoryID").val(t.pointOfSaleCategoryID);
                    $("#CompanyBranchItem_Weight").val(t.weight);
                    $("#CompanyBranchItem_Length").val(t.length);
                    $("#CompanyBranchItem_Width").val(t.width);
                    $("#CompanyBranchItem_Height").val(t.height);
                    t.canBeSold === 1 ? $("#CanBeSold").prop("checked", !0) : $("#CanBeSold").prop("checked", !1);
                    t.canBePurchased === 1 ? $("#CanBePurchased").prop("checked", !0) : $("#CanBePurchased").prop("checked", !1);
                    t.showInPOS === 1 ? $("#ShowInPOS").prop("checked", !0) : $("#ShowInPOS").prop("checked", !1);
                    $("#ItemStorageLocation_ItemStorageLocationID").val(t.itemStorageLocationID);
                    $("#ItemStorageLocation_CompanyBranchItemID").val(t.companyBranchItemID);
                    $("#ItemStorageLocation_StorageLocationID").val(t.storageLocationID);
                    $("#ItemStorageLocation_UnitCost").val(t.unitCost);
                    $("#ItemStorageLocation_Batch").val(t.batch);
                    $("#ItemStorageLocation_ExpiryDate").val(n.toLocalISOString().substr(0, 10));
                    $("#ItemStorageLocation_AvailableQuantity").val(t.availableQuantity);
                    $("#ItemStorageLocation_TotalQuantity").val(t.totalQuantity);
                    $("#ItemStorageLocation_ReorderLevel").val(t.reorderLevel);
                    $("#UnitPrice").val(t.unitPrice);
                    $("#PreviousUnitCost").val(t.unitCost);
                    $("#PreviousQuantity").val(t.totalQuantity);
                    $("#PreviousUnitPrice").val(t.unitPrice);
                    $("#CompanyBranchItem_AssetSubAccountID").prop("disabled", !0);
                    $("#CompanyBranchItem_CostOfSaleSubAccountID").prop("disabled", !0);
                    $("#CompanyBranchItem_RevenueSubAccountID").prop("disabled", !0);
                    //LoadImage(t.image);
                    $("#addproduct").hide();
                    $("#updateproduct").show();
                    goToByScroll("backUp")
                } else
                    Notify(!1, t)
            })
        }

        function DeleteInventoryItem(itemID) {
            var token = $("#InventoryItemForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/DeleteItem/", itemID, token, function (response) {
                var responseData = response.response;

                if (response.status) {
                    Notify(true, "Inventory Item Deleted Successfully");
                    productTable.row(`#${itemID}`).remove().draw();
                } else {
                    Notify(false, responseData);
                }
            });
        }   

        function LoadAllInventorySubAccounts() {
            AjaxServerCallAsync("GET", "/Inventory/GetInventorySubAccounts/", "", "", function (response) {
                var subAccounts = response.response;
                var assetSubAccountSelect = $("#CompanyBranchItem_AssetSubAccountID");

                if (response.status && !$.isEmptyObject(subAccounts)) {
                    assetSubAccountSelect.empty().append("<option></option>");

                    for (var i = 0; i < subAccounts.length; i++) {
                        var option = new Option(subAccounts[i].subAccountName, subAccounts[i].subAccountID);
                        assetSubAccountSelect.append(option);
                    }
                }
            });
        }

          function LoadAllCostOfSaleSubAccounts() {
            AjaxServerCallAsync("GET", "/Inventory/GetCostOfSalesSubAccounts/", "", "", function (response) {
                var subAccounts = response.response;
                var costOfSaleSubAccountSelect = $("#CompanyBranchItem_CostOfSaleSubAccountID");
        
                if (response.status && !$.isEmptyObject(subAccounts)) {
                    costOfSaleSubAccountSelect.empty().append("<option></option>");

                    for (var i = 0; i < subAccounts.length; i++) {
                        var option = new Option(subAccounts[i].subAccountName, subAccounts[i].subAccountID);
                        costOfSaleSubAccountSelect.append(option);
                    }
                }
            });
        }

        function LoadAllRevenueSubAccounts() {
            AjaxServerCallAsync("GET", "/Inventory/GetIncomeSubAccounts/", "", "", function (response) {
                var subAccounts = response.response;
                var revenueSubAccountSelect = $("#CompanyBranchItem_RevenueSubAccountID");

                if (response.status && !$.isEmptyObject(subAccounts)) {
                    revenueSubAccountSelect.empty().append("<option></option>");

                    for (var i = 0; i < subAccounts.length; i++) {
                        var option = new Option(subAccounts[i].subAccountName, subAccounts[i].subAccountID);
                        revenueSubAccountSelect.append(option);
                    }
                }
            });
        }

        function LoadAllVatTypes() {
            AjaxServerCallAsync("GET", "/Inventory/GetAllVATTypes/", "", "", function (response) {
                var vatTypes = response.response;
                var vatTypeSelect = $("#CompanyBranchItem_VATTypeID");
        
                if (response.status && !$.isEmptyObject(vatTypes)) {
                    vatTypeSelect.empty().append("<option></option>");

                    for (var i = 0; i < vatTypes.length; i++) {
                        var option = new Option(vatTypes[i].vatTypeName, vatTypes[i].vatTypeID);
                        vatTypeSelect.append(option);
                    }
                }
            });
        }

      function LoadAllOtherTaxes() {
            var otherTaxSelect = $("#OtherTaxID");
            otherTaxSelect.empty().append("<option></option>");

            AjaxServerCallAsync("GET", "/Inventory/GetAllOtherTaxes/", "", "", function (response) {
                var otherTaxes = response.response;

                if (response.status && !$.isEmptyObject(otherTaxes)) {
                    otherTaxes.forEach(tax => {
                        var option = new Option(`${tax.otherTaxName} at ${tax.perRate}%`, tax.otherTaxID);
                        otherTaxSelect.append(option);
                    });
                }
            });
        }

        function LoadAllUnitOfMeasures() {
            AjaxServerCallAsync("GET", "/Inventory/GetAllUnitOfMeasures/", "", "", function (response) {
                var unitOfMeasures = response.response;

                if (response.status && !$.isEmptyObject(unitOfMeasures)) {
                    var unitOfMeasureSelect = $("#Item_UnitOfMeasureID");
                    unitOfMeasureSelect.empty().append("<option></option>");

                    for (var i = 0; i < unitOfMeasures.length; i++) {
                        let newOption = new Option(unitOfMeasures[i].unitOfMeasureName, unitOfMeasures[i].unitOfMeasureID);
                        unitOfMeasureSelect.append(newOption);
                    }
                }
            });
        }

        function FilterProducts() {
            var storageLocationID = $("#ItemStorageLocation_StorageLocationID").val();
            var flagElement = $("#flag");
            var laddaButton = Ladda.create(document.querySelector("#btnfilterproducts"));

            if (storageLocationID <= 0) {
                Notify(!1, "First select the storage location");
                return;
            }

            laddaButton.start();
            laddaButton.isLoading();
            laddaButton.setProgress(-1);

            flagElement.prop("disabled", true);

            var requestData = {
                FilterFlag: flagElement.val()
            };

            var requestVerificationToken = $("#FilterForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Inventory/FilterItems/", requestData, requestVerificationToken).then(response => {
                    LoadProducts(response);
                    flagElement.prop("disabled", false);
                    laddaButton.stop();
                })
                .catch(error => {
                    flagElement.prop("disabled", false);
                    laddaButton.stop();
                    Notify(!1, error);
                });
        }


        function LoadProducts(n) {
            if (productTable.clear().draw(),
                $.isEmptyObject(n))
                Inform("No products found!!");
            else if (0) {
                let t = 0
                    , i = 0;
                try {
                    i = n[0].companyBranchItem.companyBranchItemID
                } catch (t) {
                    i = 0
                }
                try {
                    t = n[0].itemStorageLocation.itemStorageLocationID
                } catch (t) {
                    t = 0
                }
                if (t > 0)
                    GetInventoryItemDetails(t);
                else if (i === 0) {
                    let t = ""
                        , i = n[0].itemName.replace(/[^\w ]/, "").replace(/\s/g, "").replace(/`/g, "").replace(/'/g, "").replace(/"/g, "");
                    t += '<tr style="color: red;" id="' + i + '" data-branchItemID="0" data-itemID="' + n[0].itemID + '"><td data-title="Name">' + n[0].itemName + '<\/td><td data-title="Batch No"><\/td><td data-title="Unit Cost"><\/td><td data-title="Unit Price"><\/td><td data-title="Total Quantity"><\/td><td data-title="Available Quantity"><\/td><td data-title="Expires On"><\/td><\/tr>';
                    productTable.rows.add($(t)).draw(!1);
                    goToByScroll("scrolltohere")
                } else {
                    let t = ""
                        , r = n[0].itemName.replace(/[^\w ]/, "").replace(/\s/g, "").replace(/`/g, "").replace(/'/g, "").replace(/"/g, "");
                    t += '<tr style="color: #FFBF00;" id="' + r + '" data-branchItemID="' + i + '" data-itemID="' + n[0].itemID + '"><td data-title="Name">' + n[0].itemName + '<\/td><td data-title="Batch No"><\/td><td data-title="Unit Cost"><\/td><td data-title="Unit Price"><\/td><td data-title="Total Quantity"><\/td><td data-title="Available Quantity"><\/td><td data-title="Expires On"><\/td><\/tr>';
                    productTable.rows.add($(t)).draw(!1);
                    goToByScroll("scrolltohere");
                    Notify(!1, "Create opening stock for the item first")
                }
            } else {
                let t = "";
                for (let i = 0; i < n.length; i++) {
                    let u = 0
                        , r = 0;
                 
                    k = 1;

                    if (k > 0)
                        t += '<tr style="color: green;"  data-itemID="' + n[i].itemID + '" id="' + n[i].itemID + '" ><td class="dt-body-center"><input type="checkbox" /><\/td><td data-title="Name">' + n[i].itemName + '<\/td><td data-title="Unit Cost" class="currency">' + formatCurrency(n[i].unitCost) + '<\/td><td data-title="Unit Price" class="currency">' + formatCurrency(n[i].unitPrice) + '<\/td><td data-title="Total Quantity">' + n[i].totalQuantity + '<\/td><td data-title="Available Quantity">' + n[i].availableQuantity + '<\/td><td data-title="Expires On">' + formatDate(n[i].expiryDate) + "<\/td><\/tr>";
                    else if (r === 0) {
                        let r = n[i].itemName.replace(/[^\w ]/, "").replace(/\s/g, "").replace(/`/g, "").replace(/'/g, "").replace(/"/g, "");
                        t += '<tr style="color: red;" id="' + r + '" data-branchItemID="0" data-itemID="' + n[i].itemID + '"><td>-<\/td><td data-title="Name">' + n[i].itemName + '<\/td><td data-title="Batch No"><\/td><td data-title="Unit Cost"><\/td><td data-title="Unit Price"><\/td><td data-title="Total Quantity"><\/td><td data-title="Available Quantity"><\/td><td data-title="Expires On"><\/td><\/tr>'
                    }
                    else {
                        let u = n[i].itemName.replace(/[^\w ]/, "").replace(/\s/g, "").replace(/`/g, "").replace(/'/g, "").replace(/"/g, "");
                        t += '<tr style="color: #FFBF00;" id="' + u + '" data-branchItemID="' + r + '" data-itemID="' + n[i].itemID + '"><td class="dt-body-center"><input type="checkbox" id="' + r + '"/><\/td><td data-title="Name">' + n[i].itemName + '<\/td><td data-title="Batch No"><\/td><td data-title="Unit Cost"><\/td><td data-title="Unit Price"><\/td><td data-title="Total Quantity"><\/td><td data-title="Available Quantity"><\/td><td data-title="Expires On"><\/td><\/tr>'
                    }
                }
                productTable.rows.add($(t)).draw(!1);
                goToByScroll("scrolltohere")
            }
        }      
   
        function ManageOtherTaxes() {
            if (SelectedItemIds.length === 0) {
                Notify(!1, "Make sure to select more than one item(s).");
                return
            }
            GetItemOtherTaxes();
            $("#txtNoOfItems").text(SelectedItemIds.length);
            $(".manage-taxes-modal").modal("toggle")
        }

        var SelectedItemIds, selectedids, selectedBatchIds;
        $(document).ready(function () {
            productTable.clear().draw();
            reservedTable.clear().draw();
            othertaxesTable.clear().draw();
            expiredbatchesTable.clear().draw();
            $("#itemcategoriestable tbody").empty();
            ReloadPage();
            $("#Item_ItemID").val("0");
            $("#ItemStorageLocation_ItemStorageLocationID").val("0");
            $("#ItemStorageLocation_CompanyBranchItemID").val("0");
            $("#CompanyBranchItem_CompanyBranchItemID").val("0");
            $("#PreviousUnitCost").val("0");
            $("#PreviousQuantity").val("0");
            $("#ItemStorageLocation_AvailableQuantity").val("0");
            $("#PreviousUnitPrice").val("0");
            $(".select-stoloc-modal").modal("toggle");
            $("#ItemCategory_ItemCategoryID").val("0");
            $("#ItemClass_ItemClassID").val("0");
            LoadAllOtherTaxes()
        });

        const productTable = $("#inventoryitemstable").DataTable({
            drawCallback: function () {
                $.contextMenu("destroy", `#${$(this).prop("id")} tbody tr td`);
                $.contextMenu({
                    selector: "#inventoryitemstable tbody tr td",
                    trigger: "right",
                    delay: 500,
                    autoHide: !0,
                    callback: function (n, t) {
                        var r = t.$trigger[0].parentElement.id
                            , u = Number($("#inventoryitemstable #" + r).data("itemid") || 0)
                            , i = Number($("#inventoryitemstable #" + r).data("branchitemid") || 0)
                            ,i = r;
                        isNaN(r) && (r = 0);
                        switch (n) {
                            case "select":
                                r > 0 ? GetInventoryItemDetails(r) : i > 0 ? Notify(!1, "First create the opening stock for this item") : Notify(!1, "This item is not in your branch. Make sure you add it first");
                                break;
                            case "delete":
                                if (r > 0) {
                                    let n = confirm("Are You Sure You Want To Delete The Selected Inventory Item?");
                                    n === !0 && DeleteInventoryItem(r)
                                } else
                                    Notify(!1, "Can only delete stock item(s) already in the selected storage location");
                                break;
                            case "openstock":
                                if (u > 0 && i > 0) {
                                    let n = confirm("Are You Sure You Want To Create an opening stock for the selected item?");
                                    n === !0 && CreateOpeningStock(u, i)
                                } else
                                    i > 0 && Notify(!1, "Item already in stock");
                                break;
                            case "createinbranch":
                                i > 0 ? Notify(!1, "Item already exist in this branch") : GetItemDetails(u);
                                break;
                            case "retire":
                                if (r > 0) {
                                    let n = confirm("You are about to retire the selected batch. Do you wish to continue?");
                                    n === !0 && RetireBatch(r)
                                } else
                                    Notify(!1, "Can only retire batch that is in the selected storage location");
                                break;
                            case "manage-taxes":
                                SelectedItemIds.length === 0 && i > 0 && SelectedItemIds.push(i);
                                ManageOtherTaxes();
                                break;
                            case "deactivate":
                                Number(i) > 0 ? confirm("You are about to deactivate the selected item. Do you wish to continue?") === !0 && DeactivateItem(i) : Notify(!1, "This item is not in your branch. Make sure you add it first");
                                break;
                            case "activate":
                                Number(i) > 0 ? confirm("You are about to activate the selected item. Do you wish to continue?") === !0 && ActivateItem(i) : Notify(!1, "This item is not in your branch. Make sure you add it first")
                        }
                    },
                    items: {
                        select: {
                            name: "Select"
                        },
                        "delete": {
                            name: "Delete"
                        }
                    }
                })
            },
            lengthChange: !1,
            buttons: ["excel", "csv", "pdf", "print"],
            paging: !1,
            searching: !0,
            ordering: !0,
            bInfo: !0,
            select: !0,
            scrollY: "250px",
            sScrollX: "100%",
            scrollX: !0,
            order: [[1, "asc"]]
        });
        productTable.on("select", function (n, t, i, r) {
            if (i === "row") {
                let t = productTable.rows(r).data().toArray()
                    , n = t[0].DT_RowId;
                if (n !== undefined)
                    if (n > 0)
                        GetInventoryItemDetails(n);
                    else {
                        let t = $("#inventoryitemstable #" + n).data("branchitemid");
                        t > 0 ? Notify(!1, "First create the opening stock for this item") : Notify(!1, "This item is not in your branch. Make sure you add it first")
                    }
            }
        });

        SelectedItemIds = [];

        const categoriesTable = $("#itemcategoriestable").DataTable({
            drawCallback: function () {
                $.contextMenu("destroy", `#${$(this).prop("id")} tbody tr td`);
                $.contextMenu({
                    selector: "#itemcategoriestable tbody tr td",
                    trigger: "right",
                    delay: 500,
                    autoHide: !0,
                    callback: function (n, t) {
                        var i = t.$trigger[0].parentElement.id, r;
                        switch (n) {
                            case "select":
                                GetItemCategoryDetails(i);
                                break;
                            case "delete":
                                r = confirm("Are You Sure You Want To Delete The Selected Item Category?");
                                r === !0 && DeleteItemCategory(i)
                        }
                    },
                    items: {
                        select: {
                            name: "Select"
                        },
                        "delete": {
                            name: "Delete"
                        }
                    }
                })
            },
            lengthChange: !1,
            buttons: ["excel", "csv", "pdf", "print"],
            paging: !1,
            searching: !1,
            ordering: !1,
            bInfo: !1,
            scrollY: "150px",
            sScrollX: "100%",
            scrollX: !0
        })
            , reservedTable = $("#reservedtable").DataTable({
                lengthChange: !1,
                paging: !1,
                searching: !0,
                ordering: !0,
                bInfo: !0,
                scrollY: "60vh",
                sScrollX: "100%",
                scrollX: !0
            })
            , expiredbatchesTable = $("#expiredbatchestable").DataTable({
                lengthChange: !1,
                paging: !1,
                searching: !0,
                ordering: !0,
                bInfo: !0,
                scrollY: "58vh",
                sScrollX: "100%",
                scrollX: !0,
                order: [1, "asc"]
            })
            , classesTable = $("#itemclassestable").DataTable({
                drawCallback: function () {
                    $.contextMenu("destroy", `#${$(this).prop("id")} tbody tr td`);
                    $.contextMenu({
                        selector: "#itemclassestable tbody tr td",
                        trigger: "right",
                        delay: 500,
                        autoHide: !0,
                        callback: function (n, t) {
                            var i = t.$trigger[0].parentElement.id, r;
                            switch (n) {
                                case "select":
                                    GetItemClassDetails(i);
                                    break;
                                case "delete":
                                    r = confirm("Are You Sure You Want To Delete The Selected Item Class?");
                                    r === !0 && DeleteItemClass(i)
                            }
                        },
                        items: {
                            select: {
                                name: "Select"
                            },
                            "delete": {
                                name: "Delete"
                            }
                        }
                    })
                },
                lengthChange: !1,
                buttons: ["excel", "csv", "pdf", "print"],
                paging: !1,
                searching: !1,
                ordering: !1,
                bInfo: !1,
                scrollY: "150px",
                sScrollX: "100%",
                scrollX: !0
            });

        const othertaxesTable = $("#otherTaxestable").DataTable({
            lengthChange: !1,
            paging: !1,
            searching: !1,
            ordering: !1,
            bInfo: !1,
            scrollY: "35vh",
            sScrollX: "100%",
            scrollX: !0
        });

        $("#ItemCategoryForm").submit(function (event) {
            event.preventDefault();

            var itemCategoryID = $("#ItemCategory_ItemCategoryID").val();
            var laddaButton = itemCategoryID > 0 ? Ladda.create(document.querySelector("#btnupdateitemcat")) : Ladda.create(document.querySelector("#btnadditemcat"));
            laddaButton.start();
            laddaButton.isLoading();
            laddaButton.setProgress(-1);

            var requestData = {
                    ItemCategoryID: $("#ItemCategory_ItemCategoryID").val(),
                    ItemCategoryName: $("#ItemCategory_Name").val(),
                    Description: $("#ItemCategory_Description").val()                
            };
            var requestVerificationToken = $("#ItemCategoryForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/CreateUpdateItemCategory/", requestData, requestVerificationToken, function (response) {
                var itemCategoryResponse = response.response;
                var isActiveText = itemCategoryResponse.isActive === 1 ? "Yes" : "No";

                if (response.status) {
                    if (itemCategoryID > 0) {
                        let tableRow = '<tr id="' + itemCategoryResponse.itemCategoryID + '"><td data-title="Item Category ID">' + itemCategoryResponse.itemCategoryID + '<\/td><td data-title="Name">' + itemCategoryResponse.itemCategoryName + "<\/td><\/tr>";
                        $("#itemcategoriestable #" + itemCategoryID).replaceWith(tableRow);

                        Notify(!0, "Item Category Updated Successfully");
                        let option = new Option(itemCategoryResponse.name, itemCategoryResponse.itemCategoryID, !0, !0);
                        $("#Item_ItemCategoryID option[value='" + itemCategoryResponse.itemCategoryID + "']").replaceWith(option);
                    } else {
                        let tableRow = '<tr id="' + itemCategoryResponse.itemCategoryID + '"><td data-title="Item Category ID">' + itemCategoryResponse.itemCategoryID + '<\/td><td data-title="Name">' + itemCategoryResponse.itemCategoryName + "<\/td><\/tr>";
                        $("#itemcategoriestable").append(tableRow);

                        Notify(!0, "Item Category Created Successfully");
                        let option = new Option(itemCategoryResponse.name, itemCategoryResponse.itemCategoryID, !0, !0);
                        $("#Item_ItemCategoryID").append(option);
                        $("#btnCreateNewItemCat").click();
                    }
                    laddaButton.stop();
                } else {
                    laddaButton.stop();
                    Notify(!1, itemCategoryResponse);
                }
            });
        });

        $("#btnCreateNewItemCat").click(function () {
            $("#ItemCategoryForm")[0].reset();
            $("#ItemCategory_ItemCategoryID").val("0");
            $("#updateitemcat").hide();
            $("#additemcat").show()
        });
        $("#ItemClassForm").submit(function (event) {
            event.preventDefault();

            var itemClassID = $("#ItemClass_ItemClassID").val();
            var laddaButton = itemClassID > 0 ? Ladda.create(document.querySelector("#btnupdateitemclass")) : Ladda.create(document.querySelector("#btnadditemclass"));
            laddaButton.start();
            laddaButton.isLoading();
            laddaButton.setProgress(-1);

            var requestData = {
                ItemClassID: $("#ItemClass_ItemClassID").val(),
                ItemClassName: $("#ItemClass_Name").val(),
                Description: $("#ItemClass_Description").val(),
                ItemClassTypeID: $("#ItemClass_ItemClassType").val()
            };
            var requestVerificationToken = $("#ItemClassForm input[name=__RequestVerificationToken]").val();

            AjaxServerCallAsync("POST", "/Inventory/CreateUpdateItemClass/", requestData, requestVerificationToken, function (response) {
                var itemClassResponse = response.response;

                if (response.status) {
                    if (itemClassID > 0) {
                        let tableRow = '<tr id="' + itemClassResponse.itemClassID + '"><td data-title="Item Class ID">' + itemClassResponse.itemClassID + '<\/td><td data-title="Name">' + itemClassResponse.itemClassName + "<\/td><\/tr>";
                        $("#itemclassestable #" + itemClassID).replaceWith(tableRow);

                        Notify(!0, "Item Class Updated Successfully");
                        let option = new Option(itemClassResponse.name, itemClassResponse.itemClassID, !0, !0);
                        $("#Item_ItemClassID option[value='" + itemClassResponse.itemClassID + "']").replaceWith(option);
                    } else {
                        let tableRow = '<tr id="' + itemClassResponse.itemClassID + '"><td data-title="Item Class ID">' + itemClassResponse.itemClassID + '<\/td><td data-title="Name">' + itemClassResponse.itemClassName + "<\/td><\/tr>";
                        $("#itemclassestable").append(tableRow);

                        Notify(!0, "Item Class Created Successfully");
                        let option = new Option(itemClassResponse.name, itemClassResponse.itemClassID, !0, !0);
                        $("#Item_ItemClassID").append(option);
                        $("#btnCreateNewItemClass").click();
                    }
                    laddaButton.stop();
                } else {
                    laddaButton.stop();
                    Notify(!1, itemClassResponse);
                }
            });
        });

        $("#btnCreateNewItemClass").click(function () {
            $("#ItemClassForm")[0].reset();
            $("#ItemClass_ItemClassID").val("0");
            $("#updateitemclass").hide();
            $("#additemclass").show()
        });

        $("#InventoryItemForm").submit(function (n) {
            var i, t, r, u;
            if (n.preventDefault(),
                i = Number($("#Item_ItemID").val()),
                t = Ladda.create(document.querySelector("#btnaddproduct")),
                i > 0 && (t = Ladda.create(document.querySelector("#btnupdateproduct"))),
                t.start(),
                t.isLoading(),
                t.setProgress(-1),
                r = $("#IsNewBatch").is(":checked"),
                !r || !(i > 0) || (u = confirm("You are about to create a new batch from this Item. Are you sure you want to continue?"),
                    u !== !1)) {
                var f = {
                    UnitCost: $("#ItemStorageLocation_UnitCost").val(),
                    Batch: $("#ItemStorageLocation_Batch").val(),
                    ExpiryDate: $("#ItemStorageLocation_ExpiryDate").val(),
                    AvailableQuantity: $("#ItemStorageLocation_AvailableQuantity").val(),
                    TotalQuantity: $("#ItemStorageLocation_TotalQuantity").val(),
                    ReorderLevel: $("#ItemStorageLocation_ReorderLevel").val(),
                    ItemID: $("#Item_ItemID").val(),
                    ItemName: $("#Item_Name").val(),
                    ItemCategoryID: $("#Item_ItemCategoryID").val(),
                    ItemClassID: $("#Item_ItemClassID").val(),
                    UnitOfMeasureID: $("#Item_UnitOfMeasureID").val(),
                    AssetSubAccountID: $("#CompanyBranchItem_AssetSubAccountID").val(),
                    CostOfSaleSubAccountID: $("#CompanyBranchItem_CostOfSaleSubAccountID").val(),
                    RevenueSubAccountID: $("#CompanyBranchItem_RevenueSubAccountID").val(),
                    VATTypeID: $("#CompanyBranchItem_VATTypeID").val(),
                    ItemCode: $("#CompanyBranchItem_ItemCode").val(),
                    Barcode: $("#CompanyBranchItem_Barcode").val(),
                    Weight: $("#CompanyBranchItem_Weight").val() || 0,
                    Length: $("#CompanyBranchItem_Length").val() || 0,
                    Width: $("#CompanyBranchItem_Width").val() || 0,
                    Height: $("#CompanyBranchItem_Height").val() || 0,
                    ShowInPOS: $("#ShowInPOS").is(":checked") ? 1 : 0,
                    UnitPrice: $("#UnitPrice").val(),
                    //Image: GetSelectedImage()
                }
                    , e = $("#InventoryItemForm input[name=__RequestVerificationToken]").val();
                GetOrPostAsync("POST", "/Inventory/CreateUpdateItem/", f, e).then(n => {
                    if (t.stop(),
                        i > 0) {
                        Notify(!0, "Product Updated Successfully");
                        let t = [`<input type="checkbox" id="${n.itemID}">`, n.itemName,  n.unitCost, n.unitPrice, n.totalQuantity, n.availableQuantity, formatDate(n.expiryDate)];
                        t.DT_RowId = n.itemID;
                        productTable.row(`#${i}`).data(t).draw()
                    } else {
                        Notify(!0, "Product Created Successfully");
                        $("#btnCreateNewProduct").click();
                        let t = '<tr style="color: green;" data-branchItemID="' + n.itemID + '" data-itemID="' + n.itemID + '" id="' + n.itemID + '"><td class="dt-body-center"><input type="checkbox" id="' + n.itemID + '"/><\/td><td data-title="Name">' + n.itemName + '<\/td><td data-title="Unit Cost">' + n.unitCost + '<\/td><td data-title="Unit Price">' + n.unitPrice + '<\/td><td data-title="Total Quantity">' + n.totalQuantity + '<\/td><td data-title="Available Quantity">' + n.availableQuantity + '<\/td><td data-title="Expires On">' + formatDate(n.expiryDate) + "<\/td><\/tr>";
                        productTable.row.add($(t)).draw()
                    }
                }
                ).catch(n => {
                    t.stop(),
                        Notify(!1, n)
                }
                )
            }
        });

        $("#btnCreateNewProduct").click(function () {
            $("#InventoryItemForm")[0].reset();
            $("#Item_ItemID").val("0");
            $("#ItemStorageLocation_ItemStorageLocationID").val("0");
            $("#ItemStorageLocation_CompanyBranchItemID").val("0");
            $("#CompanyBranchItem_CompanyBranchItemID").val("0");
            $("#PreviousUnitCost").val("0");
            $("#PreviousQuantity").val("0");
            $("#ItemStorageLocation_AvailableQuantity").val("0");
            $("#PreviousUnitPrice").val("0");
            $("#CompanyBranchItem_AssetSubAccountID").prop("disabled", !1);
            $("#CompanyBranchItem_CostOfSaleSubAccountID").prop("disabled", !1);
            $("#CompanyBranchItem_RevenueSubAccountID").prop("disabled", !1);
            $dropimg.css("background-image", "");
            $droptarget.removeClass("dropped");
            $remover.addClass("disabled");
            $("#updateproduct").hide();
            $("#addproduct").show()
        });
    
        $("#SearchProductForm").submit(function (n) {
            var t, i;
            if (n.preventDefault(),
                t = Ladda.create(document.querySelector("#btnsearchitem")),
                t.start(),
                t.isLoading(),
                t.setProgress(-1),
                i = $("#ItemStorageLocation_StorageLocationID").val(),
                i <= 0)
                Notify(!1, "First select the storage location"),
                    t.stop();
            else {
                var r = {
                    StorageLocationID: i,
                    SearchIndex: $("#searchIndex").val()
                }
                    , u = $("#SearchProductForm input[name=__RequestVerificationToken]").val();
                GetOrPostAsync("POST", "/Inventory/SearchProducts/", r, u).then(n => {
                    t.stop(),
                        LoadProducts(n),
                        $("#searchIndex").val(""),
                        $("#searchIndex").focus()
                }
                ).catch(n => {
                    t.stop(),
                        Notify(!1, n)
                }
                )
            }
        });
        $("#FilterForm").submit(function (event) {
            event.preventDefault();
            executeProductFiltering();
        });

        $("#flag").on("change", FilterProducts);

        $("#OtherTaxItemsForm").submit(function (n) {
            var t;
            n.preventDefault();
            t = Ladda.create(document.querySelector("#btnSubmitOtherTax"));
            t.start();
            t.isLoading();
            t.setProgress(-1);
            var i = {
                OtherTaxID: $("#OtherTaxID").val(),
                CompanyBranchItemIds: SelectedItemIds
            }
                , r = $("#OtherTaxItemsForm input[name=__RequestVerificationToken]").val();
            GetOrPostAsync("POST", "/Inventory/AssignOtherTaxesToItems/", i, r).then(n => {
                t.stop();
                Notify(!0, "Tax assigned Successfully.");
                let i = '<tr id="' + n.otherTaxID + '"><td data-title="Tax">' + n.name + '<\/td><td data-title="Rate">' + n.perRate + '%<\/td><td><a href="Javascript:RemoveOtherTax(' + n.otherTaxID + ')"><span class="glyphicon glyphicon-trash"><\/span><\/a><\/td><\/tr>';
                othertaxesTable.row.add($(i)).draw(!1);
                $("#OtherTaxID > option").each(function () {
                    if (!isNaN(this.value)) {
                        let t = Number(this.value);
                        n.otherTaxID === t && $(this).prop("disabled", !0)
                    }
                });
                $("#OtherTaxItemsForm")[0].reset()
            }
            ).catch(n => {
                t.stop(),
                    Notify(!1, n)
            }
            )
        });

        $(function () {
            $.contextMenu({
                selector: '#body',
                trigger: 'right',
                autoHide: true,
                zIndex: 9999,
                reposition: false,
                callback: function (key, options) {
                    switch (key) {
                        case 'refresh':
                            if (typeof ReloadPage === "function") {
                                ReloadPage();
                            }
                            break;
                    }
                },
                items: {
                    "refresh": {
                        name: "Refresh",
                        icon: "fas fa-sync-alt"
                    }
                }
            });
        });

});