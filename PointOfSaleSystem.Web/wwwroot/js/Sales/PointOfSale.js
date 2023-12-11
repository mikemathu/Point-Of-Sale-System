$(document).ready(function () {
         
        function ReloadPage() {
            LoadUserPendingOrders();
            LoadInventoryItems();
            GetPaymentModes();
           
        }

        function LoadInventoryItems() {
            var selectedPriceList = $(".select-pricelist").val();
            var requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();
            let categoryList = $(".category-list");

            GetOrPostAsync("GET", "/Sales/GetAllItemsOnPOS/", selectedPriceList, requestVerificationToken)
                .then(response => {
                    ___Items___ = [];
                    categoryList.empty();
                    let categoriesHTML = "";

                    if (!$.isEmptyObject(response)) {
                        for (let i = 0; i < response.categories.length; i++) {
                            categoriesHTML += `
                            <span class="category-simple-button js-category-switch" data-categoryid="${response.categories[i].itemCategoryID}">
                                ${response.categories[i].itemCategoryName}
                            </span>
                        `;
                        }

                        categoryList.html(categoriesHTML);
                        $(".category-simple-button").on("click", OnCategorySelect);
                        ___Items___ = response.items;
                        $(".category-list").find(`[data-categoryid="${response.categories[0].itemCategoryID}"]`).click();
                    }
                })
                .catch(error => Notify(!1, error));
        }

          function LoadUserPendingOrders() {
            var token = $("#SampleForm input[name=__RequestVerificationToken]").val();
            let $orders = $(".orders");
            $orders.empty();
            $(".order-items").empty();
            $(".summary").empty();
            ___PendingOrders____ = [];

            GetOrPostAsync("GET", "/Sales/GetUserPendingOrders/", "", token).then(data => {
                    if (!$.isEmptyObject(data)) {
                        ___PendingOrders____ = data;
                        let ordersHTML = "";
                        for (let i = 0; i < data.length; i++) {
                            ordersHTML += `
                                <span class="order-button select-order${i + 1 === data.length ? ' selected' : ''}" data-time="${formatTime(data[i].dateTimeCreated)}" data-uid="${data[i].customerOrderID}">
                                    <span class="order-sequence">
                                        ${data[i].customerOrderID}
                                    </span>
                                    <span class="order-time">${formatTime(data[i].dateTimeCreated)}</span>
                                </span>
                            `;
                            ___OrderCount___++;
                            if (i + 1 === data.length) {
                                ___SelectedOrderID___ = data[i].customerOrderID;
                            }
                        }

                        $orders.html(ordersHTML);
                        $(".order-button").on("click", ChangeOrder);
                        LoadOrderItems();
                        GetOrderDetails();
                    }
                })
                .catch(error => {
                    Notify(false, error);
                });
        }

        function NewOrder() {
            var token = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("GET", "/Sales/CreatePosOrder/", "", token)
                .then(() => {
                    LoadUserPendingOrders();
                })
                .catch(error => {
                    Notify(false, error);
                });
        }

        function DeleteOrder() {
            var selectedOrderID = ___SelectedOrderID___;
            var token = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/RemoveOrder/", selectedOrderID, token).then(() => {
                    LoadUserPendingOrders();
            })
            .catch(error => {
                Notify(false, error);
            });
        }


        function GetItemHtml(item) {
            let imageElement = (item.image === "none.png") ? "<span>Image</span>" : `<img src="/Sales/images/${item.image}" loading="lazy" alt="Product image">`;
            return `
            <article class="product" data-branchitemid="${item.itemID}"  tabindex="0">
                <div class="product-img">
                    ${imageElement}
                    <span class="price-tag">${item.unitPrice}</span>
                </div>
                <div class="product-name">
                    ${item.itemName}
                </div>
            </article>
        `;
        }

        function LoadItemsOnView(filterText = "") {
            let productList = $(".product-list");
            productList.empty();
            let itemsHtml = "";

            if (___Searching___) {
                let filteredItems = ___Items___.filter(function (item) {
                    return item.itemName.toLowerCase().includes(filterText);
                });

                itemsHtml = filteredItems.reduce((html, item) => html + GetItemHtml(item), "");
            } else if (___SelectedCategoryID___ === -1) {
                itemsHtml = ___Items___.reduce((html, item) => html + GetItemHtml(item), "");
            } else {
                let categoryItems = ___Items___.filter(function (item) {
                    return item.pointOfSaleCategoryID === ___SelectedCategoryID___;
                });

                itemsHtml = categoryItems.reduce((html, item) => html + GetItemHtml(item), "");
            }

            productList.html(itemsHtml);
            $(".product").on("click", SelectProduct);
        }

        function ChangeOrder() {
            let selectedOrder = $(".orders").find(".selected");
            selectedOrder.removeClass("selected");
            selectedOrder.find(".order-time").text("");

            $(this).addClass("selected");
            $(this).find(".order-time").text($(this).data("time"));

            ___SelectedOrderID___ = Number($(this).data("uid"));
            __EnteredKeys__ = "";
            __ItemEditMode___ = false;
            ___SelectedOrderItemID___ = 0;
            __Payments___ = [];

            $(".order-items").empty();
            $(".summary").empty();

            LoadOrderItems();
            GetOrderDetails();
        }

        function GetPaymentModes() {
            var paymentMethods = $(".paymentmethods");
            paymentMethods.empty();

            GetOrPostAsync("GET", "/Sales/GetAllPaymentModes/", "", "").then(paymentModesData => {
                    if (!$.isEmptyObject(paymentModesData)) {
                        let paymentModesHTML = "";

                        for (let i = 0; i < paymentModesData.length; i++) {
                            if (Number(paymentModesData[i].canBeReceived) === 1) {
                                paymentModesHTML += `
                                <div class="button paymentmethod"
                                     data-name="${paymentModesData[i].paymentMethodName}"
                                     data-id="${paymentModesData[i].paymentMethodID}"
                                     data-api="${paymentModesData[i].api}">
                                    ${paymentModesData[i].paymentMethodName}
                                </div>
                            `;
                            }
                        }

                        paymentMethods.html(paymentModesHTML);
                        $(".paymentmethod").on("click", SelectPaymentMode);
                    }
                })
                .catch(error => {
                    Notify(false, error);
                });
        }

        function OnCategorySelect() {
            ___SelectedCategoryID___ = $(this).data("categoryid");
            ___CategoryItems___ = [];

            let categoryName = $(this).text();
            let selectedCategoryElement = $(".sel-category");
            selectedCategoryElement.empty();
            selectedCategoryElement.html(`<span class="breadcrumb"><span class="breadcrumb-button js-category-switch" data-categoryid="${___SelectedCategoryID___}">${categoryName}</span></span>`);

            $(".categories").addClass("oe_hidden");
            LoadItemsOnView();
        }

        function SelectProduct() {
            let selectedProduct = $(this);
            let itemId = selectedProduct.data("branchitemid");
            AddItemToOrder(itemId);
        }

        function AddItemToOrder(orderItemID) {
            var orderData = {
                CustomerOrderID: ___SelectedOrderID___,
                CustomerOrderItemID: orderItemID,
                PriceListID: $(".select-pricelist").val()
            };

            var requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/AddPosItemToOrder/", orderData, requestVerificationToken)
                .then(() => {
                    LoadOrderItems();
                })
                .catch(error => Notify(!1, error));
        }


        function LoadOrderItems() {
            var selectedOrderID = ___SelectedOrderID___;
            var token = $("#SampleForm input[name=__RequestVerificationToken]").val();
            let orderItems = $(".order-items");
            let summary = $(".summary");

            ___SelectedOrderAmount___ = 0;
            __ItemsToSplit__ = [];

            GetOrPostAsync("POST", "/Sales/GetPosOrderItems/", selectedOrderID, token)
                .then(orderData => {
                    orderItems.empty();
                    summary.empty();

                    if (!$.isEmptyObject(orderData)) {
                        let orderItemsList = "";
                        let totalVAT = 0;
                        let totalAmount = 0;
                        let isInEditMode = !orderData.some(item => item.itemID === ___SelectedOrderItemID___) && __ItemEditMode___;

                        if (isInEditMode) {
                            __ItemEditMode___ = false;
                        }

                        for (let i = 0; i < orderData.length; i++) {
                            __ItemsToSplit__.push({
                                CustomerOrderItemID: orderData[i].itemID,
                                Amount: orderData[i].subTotal,
                                Quantity: orderData[i].quantity,
                                SplitQuantity: 0,
                                UnitPrice: orderData[i].unitPrice,
                                Uom: orderData[i].unitOfMeasureName,
                                PercentageDiscount: orderData[i].discount,
                                Name: orderData[i].itemName
                            });

                            let isSelected = orderData[i].itemID === ___SelectedOrderItemID___;

                            orderItemsList += `
                            <li class="orderline ${isSelected ? 'selected' : ''}" data-orderitemid="${orderData[i].itemID}" data-qty="${orderData[i].quantity}">
                                <span class="product-name">${orderData[i].itemName}</span>
                                <span class="price">${formatCurrency(orderData[i].subTotal)}</span>
                                <ul class="info-list">
                                    <li class="info">
                                        <em>${orderData[i].quantity}</em> ${orderData[i].unitOfMeasureName}s at ${formatCurrency(orderData[i].unitPrice)}/${orderData[i].unitOfMeasureName}
                                    </li>
                                </ul>
                            </li>
                        `;

                            if (isSelected) {
                                ___SelectedOrderItemID___ = orderData[i].itemID;
                            }

                            totalVAT += orderData[i].vat;
                            totalAmount += orderData[i].subTotal;
                        }

                        orderItems.html(orderItemsList);

                        summary.html(`
                        <div class="line">
                            <div class="entry total">
                                <span class="badge">Total: </span> <span class="value">${formatCurrency(totalAmount)}</span>
                                <div class="subentry">Taxes: <span class="value">${formatCurrency(totalVAT)}</span></div>
                            </div>
                        </div>
                    `);

                        $(".orderline").on("click", SelectOrderItem);
                        $(".pay-total").text(formatCurrency(totalAmount));
                        ___SelectedOrderAmount___ = totalAmount;
                    }
                })
                .catch(error => {
                    Notify(false, error);
                });
        }

        function PrintSalesOrder() {
            var selectedOrderID = ___SelectedOrderID___;
            var requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/PrintSalesOrder/", selectedOrderID, requestVerificationToken)
                .then(printData => {
                    let printContainer = $("#silent-print");
                    printContainer.empty();

                    let copies = GetPrintCopies();
                    for (let copy = 0; copy < copies; copy++) {
                        printContainer.append(printData);
                        if (copy + 1 !== copies) {
                            printContainer.append('<p style="page-break-before: always">');
                        }
                    }

                    PrintSilently(() => { });
                })
                .catch(error => {
                    Notify(!1, error);
                });
        }


        function SelectOrderItem() {
            let selectedOrderItem = $(".order-items").find(".selected");
            selectedOrderItem.removeClass("selected");

            $(this).addClass("selected");
            ___SelectedOrderItemID___ = Number($(this).data("orderitemid"));

            __EnteredKeys__ = ``;
            console.log('selectedItemID selestOrderItem():', ___SelectedOrderItemID___);
        }


        function ChangeKeypadMode(event) {
            event.preventDefault();

            __EnteredKeys__ = "";
            __ItemEditMode___ = false;

            let clickedElement = $(this);
            ModePermission(clickedElement);
        }

        function ModePermission(element) {
            if (element !== undefined) {
                let mode = Number(element.data("mode"));
                var requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

                GetOrPostAsync("POST", "/Sales/CheckPosKeypadMode/", mode, requestVerificationToken)
                    .then(() => {
                        let selectedMode = $(".selected-mode");
                        selectedMode.removeClass("selected-mode");

                        ___KeypadMode___ = mode;
                        element.addClass("selected-mode");
                    })
                    .catch(error => {
                        Notify(false, error);
                    });
            }
        }

        function delay(func, time) {
            return function (...args) {
                clearTimeout(timer);
                timer = setTimeout(func.bind(this, ...args), time || 0);
            };
        }

       
        $(document).on('keydown', event => {
            if (__ReceivingPayment__) {
                if (___HandlingMpesaAPI___) return;

                const { keyCode, key } = event;

                if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105)) {
                    addCodePayment(key);
                } else if (keyCode === 110 || keyCode === 190) {
                    addCodePayment('.');
                } else if (keyCode === 8) {
                    addCodePayment('<');
                }
            } else if (___OnMainPanel___) {
                if (___IsSearching____) return;

                console.log('Keydown event:', event);

                const { keyCode, key } = event;

                if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105)) {
                    addCode(key);
                } else if (keyCode === 110 || keyCode === 190) {
                    addCode('.');
                } else if (keyCode === 8) {
                    addCode('<');
                }
            }
        });

        function addCode(input) {
            switch (input) {
                default:
                    if (Number(input) >= 0 && Number(input) <= 9) {
                        __EnteredKeys__ += input;
                        delay(ApplyKey, 250)();
                    } else if (input === ".") {
                        __EnteredKeys__ += ".";
                    } else if (input === "<") {
                        if (__EnteredKeys__.length > 0) {
                            __EnteredKeys__ = __EnteredKeys__.substring(0, __EnteredKeys__.length - 1);
                            delay(ApplyKey, 250)();
                        }
                    }
            }
        }


        function ApplyKey() {
            let enteredNumber = Number(__EnteredKeys__);

            if (!isNaN(enteredNumber)) {
                if (enteredNumber > 1e9) {
                    __EnteredKeys__ = "";
                    return;
                }

                __ItemEditMode___ = true;

                if (___KeypadMode___ === 1) {
                    UpdateQuantity(enteredNumber);
                } else if (___KeypadMode___ === 2) {
                    if (enteredNumber > 100) {
                        enteredNumber = 100;
                    }
                    UpdateDiscount(enteredNumber);
                } else if (___KeypadMode___ === 3) {
                    UpdatePrice(enteredNumber);
                }
            }
        }

        function UpdateQuantity(quantity) {
            let orderItem = {
                CustomerOrderID: ___SelectedOrderID___,
                ItemID: ___SelectedOrderItemID___,
                Quantity: quantity
            };

            let requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/UpdatePosItemQuantity/", orderItem, requestVerificationToken)
                .then(() => {
                    LoadOrderItems();
                })
                .catch(error => {
                    Notify(false, error);
                });
        }


        function UpdateDiscount(amountBeforeDiscount) {
            let orderItem = {
                CustomerOrderItemID: ___SelectedOrderItemID___,
                AmountBeforeDiscount: amountBeforeDiscount
            };

            let requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/UpdatePosItemDiscount/", orderItem, requestVerificationToken)
                .then(() => {
                    LoadOrderItems();
                })
                .catch(error => {
                    Notify(false, error);
                });
        }

        function UpdatePrice(unitPrice) {
            let orderItem = {
                CustomerOrderItemID: ___SelectedOrderItemID___,
                UnitPrice: unitPrice
            };

            let requestVerificationToken = $("#SampleForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Sales/UpdatePosItemPrice/", orderItem, requestVerificationToken)
                .then(() => {
                    LoadOrderItems();
                })
                .catch(error => {
                    Notify(false, error);
                });
        }    

        function FilterCustomer() {
        }

        function UpdatePaymentView() {
            if (__Payments___.length === 0) {
                $(".paymentlines-empty").removeClass("oe_hidden");
                $(".paymentlines").addClass("oe_hidden");
            } else {
                $(".paymentlines-empty").addClass("oe_hidden");
                $(".paymentlines").removeClass("oe_hidden");

                let paymentLinesBody = $(".paymentlines-body");
                paymentLinesBody.empty();
                let paymentLinesHTML = "";

                for (let index = 0; index < __Payments___.length; index++) {
                    let payment = __Payments___[index];
                    let selectedPaymentClass = payment.Selected ? "selected" : "";

                    paymentLinesHTML += `
                    <tr class="paymentline pay-mode mode-${payment.paymentMethodID} ${selectedPaymentClass}" data-id='${payment.paymentMethodID}'>
                        <td class="col-due">${payment.AmountDue}</td>
                        <td class="col-tendered edit">${payment.AmountTendered}</td>
                        <td class="col-change">${payment.ChangeAmount}</td>
                        <td class="col-name">${payment.Name}</td>
                        <td class="delete-button delete-payment" data-ind="${index}" aria-label="Delete" title="Delete">
                            <i class="fa fa-times-circle"></i>
                        </td>
                    </tr>`;
                }

                if (___SelectedOrderAmount___ - __PaymentMade__ > 0) {
                    paymentLinesHTML += `
                    <tr class="paymentline extra">
                        <td class="col-due">${___SelectedOrderAmount___ - __PaymentMade__}</td>
                    </tr>`;
                    $(".btn-save-payments").prop("disabled", true);
                    $(".btn-save-payments").removeClass("highlight");
                } else {
                    $(".btn-save-payments").prop("disabled", false);
                    $(".btn-save-payments").addClass("highlight");
                }

                paymentLinesBody.html(paymentLinesHTML);
                $(".pay-mode").on("click", ChangeSelectedPayMode);
                $(".delete-payment").on("click", RemovePayment);
            }
        }



       function SelectPaymentMode() {
            let n = Number($(this).data("id")), t = $(this).data("name"), i = Number($(this).data("api"));
            if (___SelectedOrderAmount___ - __PaymentMade__ != 0 && !__Payments___.some(t => t.paymentMethodID === n)) {
                if (i === 1) {
                    $("#pmodeid").val(n);
                    $("#pmodename").val(t);
                    $("#PhoneNumber").val("");
                    $("#mpesapaymentcust").text("");
                    $("#mpesapaymentamnt").text("");
                    $("#pDetails").hide();
                    modal.style.display = "block";
                    ___HandlingMpesaAPI___ = !0;
                    return
                }

                if (__Payments___.length > 0) {
                    let n = __Payments___.find(n => n.Selected === !0);
                    n !== undefined && (n.Selected = !1)
                }

                __Payments___.push({
                    paymentMethodID: n,
                    Name: t,
                    AmountTendered: 0,
                    ChangeAmount: 0,
                    AmountDue: ___SelectedOrderAmount___ - __PaymentMade__,
                    Selected: !0,
                    CanSelect: !0,
                    MpesaPaymentID: 0
                });
                __EnteredKeysPayment__ = "";
                UpdatePaymentView()
            }
        }


    function addCodePayment(inputKey) {
        switch (inputKey) {
            default:
                if (Number(inputKey) >= 0 && Number(inputKey) <= 9) {
                    __EnteredKeysPayment__ += inputKey;
                } else if (inputKey === ".") {
                    __EnteredKeysPayment__ += ".";
                } else if (inputKey === "<" && __EnteredKeysPayment__.length > 0) {
                    __EnteredKeysPayment__ = __EnteredKeysPayment__.substring(0, __EnteredKeysPayment__.length - 1);
                }
                ApplyKeyPayment();
        }
    }


        function ApplyKeyPayment() {
            let enteredAmount = Number(__EnteredKeysPayment__);

            if (!isNaN(enteredAmount)) {
                let selectedPaymentLine = $(".paymentline.selected");

                if (selectedPaymentLine.length !== 0) {
                    let paymentMethodID = Number(selectedPaymentLine.data("id"));
                    let paymentMethod = __Payments___.find(payment => payment.paymentMethodID === paymentMethodID);

                    if (paymentMethod !== null && paymentMethod !== undefined) {
                        __PaymentMade__ = 0;
                        paymentMethod.AmountTendered = enteredAmount;

                        for (let index = 0; index < __Payments___.length; index++) {
                            __PaymentMade__ += __Payments___[index].AmountTendered;
                        }

                        if (__Payments___.length === 1) {
                            paymentMethod.ChangeAmount = ___SelectedOrderAmount___ > __PaymentMade__ ? 0 : __PaymentMade__ - ___SelectedOrderAmount___;
                            __PaymentMade__ = paymentMethod.AmountTendered - paymentMethod.ChangeAmount;
                        } else {
                            for (let index = 0; index < __Payments___.length; index++) {
                                __Payments___[index].ChangeAmount = 0;
                            }
                        }

                        UpdatePaymentView();
                    }
                }
            }
        }


        function ChangeSelectedPayMode() {
            let selectedPaymentMethodID = Number($(this).data("id"));
            let selectedPaymentMethod = __Payments___.find(payment => payment.paymentMethodID === selectedPaymentMethodID);

            if (selectedPaymentMethod.CanSelect) {
                let previouslySelectedPayment = __Payments___.find(payment => payment.Selected === true);

                if (previouslySelectedPayment !== undefined) {
                    previouslySelectedPayment.Selected = false;
                }

                selectedPaymentMethod.Selected = true;
                __EnteredKeysPayment__ = "";
                UpdatePaymentView();
            }
        }

        function RemovePayment() {
            let indexToRemove = Number($(this).data("ind"));

            if (!isNaN(indexToRemove)) {
                __Payments___.splice(indexToRemove, 1);
                __PaymentMade__ = 0;
                __EnteredKeysPayment__ = "";
                let remainingAmount = ___SelectedOrderAmount___;

                for (let index = 0; index < __Payments___.length; index++) {
                    __Payments___[index].AmountDue = remainingAmount;
                    remainingAmount -= __Payments___[index].AmountTendered;
                    __PaymentMade__ += __Payments___[index].AmountTendered;
                }

                UpdatePaymentView();
            }
        }

        var ___SelectedCategoryID___ = -1, ___SelectedOrderID___ = 0, ___SelectedOrderAmount___ = 0, ___OrderSequence___ = 1, ___Items___ = [], ___CategoryItems___ = [], ___Searching___ = !1, ___OrderCount___ = 1, ___SelectedOrderItemID___ = 0, ___KeypadMode___ = 1, __EnteredKeys__ = "", __ItemEditMode___ = !1, __Payments___ = [], ___PendingOrders____ = [], ___IsSearching____ = !1, ___HandlingMpesaAPI___ = !1, ___OnMainPanel___ = !0, timer, __PaymentMade__, __ReceivingPayment__, __EnteredKeysPayment__, __ItemsToSplit__, modal, span, p;
        $(document).ready(function () {
            ___SelectedOrderID___ = Number($(".orders").find(".selected").data("uid"));
            ReloadPage()
        });

        $(".neworder-button").on("click", NewOrder);
        $(".deleteorder-button").on("click", DeleteOrder);
        $(".breadcrumb-home").on("click", () => {
            let selectedCategoryElement = $(".sel-category");
            selectedCategoryElement.empty();
            ___SelectedCategoryID___ = -1;
            ___CategoryItems___ = [];
            $(".categories").removeClass("oe_hidden");
            LoadItemsOnView();
        });


        $(".search-products").on("focus", () => {
            ___IsSearching____ = !0
        });
        $(".search-products").on("blur", () => {
            ___IsSearching____ = !1
        });

        $(".search-products").on("keyup", function () {
            let inputValue = $(this).val();
            if (isEmpty(inputValue)) {
                ___Searching___ = false;
                LoadItemsOnView();
                return;
            }
            ___Searching___ = true;
            LoadItemsOnView(inputValue.toLowerCase());
        });

        $(".clear-search-box").on("click", () => {
            $(".search-products").val(""),
                ___Searching___ = !1,
                LoadItemsOnView()
        });
        $(".print-order").on("click", PrintSalesOrder);
        $(".mode-button").on("click", ChangeKeypadMode);
        timer = 0;
        $(".make-payment").on("click", () => {
            $(".product-screen").addClass("oe_hidden"),
                $(".payment-screen").removeClass("oe_hidden"),
                __ReceivingPayment__ = !0,
                __EnteredKeysPayment__ = "",
                UpdatePaymentView()
        });
        $(".back").on("click", () => {
            $(".product-screen").removeClass("oe_hidden"),
                $(".payment-screen").addClass("oe_hidden"),
                __ReceivingPayment__ = !1,
                __EnteredKeysPayment__ = ""
        });
        __PaymentMade__ = 0;
        __ReceivingPayment__ = !1;
 
        __EnteredKeysPayment__ = "";
        $(".btn-save-payments").on("click", () => {
            var t = {
                CustomerOrderID: ___SelectedOrderID___,
                PosPaymentItems: __Payments___
            }, i = $("#SampleForm input[name=__RequestVerificationToken]").val(), n;
            if (__PaymentMade__ !== ___SelectedOrderAmount___) {
                Notify(!1, "Full amount has to be covered.");
                return
            }
            n = Ladda.create(document.querySelector(".btn-save-payments"));
            n.start();
            n.isLoading();
            n.setProgress(-1);
            GetOrPostAsync("POST", "/Sales/ReceivePosPayments/", t, i).then(t => {
                n.stop();
                Notify(!0, "Payment saved successfully.");
                __Payments___ = [];
                __ReceivingPayment__ = !1;
                __EnteredKeysPayment__ = "";
                __PaymentMade__ = 0;
                UpdatePaymentView();
                $(".product-screen").removeClass("oe_hidden");
                $(".payment-screen").addClass("oe_hidden");
                let i = $("#silent-print");
                i.empty();
                for (let n = 0; n < t.length; n++)
                    i.append(t[n]),
                        n + 1 !== t.length && i.append('<p style="page-break-before: always">');
                PrintSilently(() => {
                    LoadUserPendingOrders()
                })
            })
            .catch(t => {
                n.stop(),
                    Notify(!1, t)
            })
        });
    
        $("#btnSubmitMpesaC2b").on("click", () => {
            __EnteredKeysPayment__ = "";
            __PaymentMade__ = 0;
            for (let n = 0; n < __Payments___.length; n++)
                __PaymentMade__ += __Payments___[n].AmountTendered;
            if (__Payments___.length === 0)
                p.ChangeAmount = ___SelectedOrderAmount___ > __PaymentMade__ ? 0 : __PaymentMade__ - ___SelectedOrderAmount___,
                    __PaymentMade__ = p.AmountTendered - p.ChangeAmount;
            else
                for (let n = 0; n < __Payments___.length; n++)
                    __Payments___[n].ChangeAmount = 0;
            __Payments___.push(p);
            UpdatePaymentView();
            modal.style.display = "none";
            ___HandlingMpesaAPI___ = !1
        }); 


});