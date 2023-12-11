    $(document).ready(function () {        

        function ReloadPage() {
            LoadAllUnitOfMeasures();
        }

        function LoadAllUnitOfMeasures() {
            var fromUnitOfMeasureSelect = $("#UnitConversion_FromUnitOfMeasureID");
            var toUnitOfMeasureSelect = $("#UnitConversion_ToUnitOfMeasureID");

            fromUnitOfMeasureSelect.empty().append("<option></option>");
            toUnitOfMeasureSelect.empty();

            GetOrPostAsync("GET", "/Inventory/GetAllUnitOfMeasures/", "", "").then(response => {
                unitMeasuresTable.clear().draw();

                if (!$.isEmptyObject(response)) {
                    var tableRows = "";

                    for (let i = 0; i < response.length; i++) {
                        let isSmallestUnit = response[i].isSmallestUnit === 1 ? "Yes" : "No";
                        tableRows += `<tr id="${response[i].unitOfMeasureID}">
                                        <td data-title="No">${response[i].unitOfMeasureID}</td>
                                        <td data-title="Name">${response[i].unitOfMeasureName}</td>
                                        <td data-title="Is Smallest Unit">${isSmallestUnit}</td>
                                    </tr>`;

                        let option = new Option(response[i].unitOfMeasureName, response[i].unitOfMeasureID);
                        fromUnitOfMeasureSelect.append(option);
                    }

                    unitMeasuresTable.rows.add($(tableRows)).draw(false);
                    let options = fromUnitOfMeasureSelect.find('option');
                    options.clone().appendTo(toUnitOfMeasureSelect);
                }
            }).catch(error => console.log(error));
        }

        function GetUnitOfMeasureDetails(unitOfMeasure) {
                var token = $("#UnitOfMeasureForm input[name=__RequestVerificationToken]").val();

                GetOrPostAsync("POST", "/Inventory/GetUnitOfMeasureDetails/", unitOfMeasure, token).then(response => {
                    UpdateUnitOfMeasureFields(response);
                })
                .catch(error => {
                    console.error(error);
                });
        }

        function UpdateUnitOfMeasureFields(unitOfMeasureData) {
            $("#UnitOfMeasure_UnitOfMeasureID").val(unitOfMeasureData.unitOfMeasureID);
            $("#UnitOfMeasure_Name").val(unitOfMeasureData.unitOfMeasureName);
            $("#IsSmallestUnit").prop("checked", unitOfMeasureData.isSmallestUnit === 1);
            $("#addunitofmeasure").hide();
            $("#updateunitofmeasure").show();
        }

        function DeleteUnitOfMeasure(unitOfMeasureID) {
                var token = $("#UnitOfMeasureForm input[name=__RequestVerificationToken]").val();

                AjaxServerCallAsync("POST", "/Inventory/DeleteUnitOfMeasure/", unitOfMeasureID, token, function (response) {
                    if (response.status) {
                        LoadAllUnitOfMeasures();
                        Notify(true, "Unit Of Measure Deleted Successfully");
                    } else {
                        Notify(false, response.response);
                    }
                });
        }

        var unitMeasuresTable;
        $(document).ready(function () {
            unitMeasuresTable.clear().draw();
            $("#UnitOfMeasure_UnitOfMeasureID").val("0");
            ReloadPage()
        });

        unitMeasuresTable = $("#unitofmeasurestable").DataTable({
            drawCallback: function () {
                $.contextMenu("destroy", `#${$(this).prop("id")} tbody tr td`);
                $.contextMenu({
                    selector: "#unitofmeasurestable tbody tr td",
                    trigger: "right",
                    delay: 500,
                    autoHide: !0,
                    callback: function (n, t) {
                        var i = t.$trigger[0].parentElement.id, r;
                        switch (n) {
                            case "select":
                                GetUnitOfMeasureDetails(i);
                            break;
                            case "delete":
                                r = confirm("Are You Sure You Want To Delete The Selected Unit Of Measure?");
                                r === !0 && DeleteUnitOfMeasure(i)
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
            bInfo: !1,
            select: !0,
            scrollY: "33vh",
            sScrollX: "100%",
            scrollX: !0
        });

        unitMeasuresTable.on("select", function (event, table, type, indexes) {
            if (type === "row") {
                let rowData = unitMeasuresTable.rows(indexes).data().toArray();
                let rowId = rowData[0].DT_RowId;
                if (rowId !== undefined) {
                    GetUnitOfMeasureDetails(rowId);
                }
            }
        });

        $("#UnitOfMeasureForm").submit(function (event) {
            event.preventDefault(); 

            var unitOfMeasureID = $("#UnitOfMeasure_UnitOfMeasureID").val();
            var unitOfMeasureName = $("#UnitOfMeasure_Name").val();
            var isSmallestUnit = $("#IsSmallestUnit").is(":checked") ? 1 : 0;

            var submitButton = (unitOfMeasureID > 0) ? "#btnupdateunitofmeasure" : "#btnaddunitofmeasure";
            var laddaInstance = Ladda.create(document.querySelector(submitButton));

            laddaInstance.start();
            laddaInstance.isLoading();
            laddaInstance.setProgress(-1);

            var requestData = {
                UnitOfMeasureID: unitOfMeasureID,
                UnitOfMeasureName: unitOfMeasureName,
                IsSmallestUnit: isSmallestUnit
            };

            var requestVerificationToken = $("#UnitOfMeasureForm input[name=__RequestVerificationToken]").val();

            GetOrPostAsync("POST", "/Inventory/CreateUpdateUnitOfMeasure/", requestData, requestVerificationToken).then(function (response) {
                laddaInstance.stop(); 

                if (unitOfMeasureID > 0) {
                    Notify(!0, "Unit Of Measure Updated Successfully");
                } else {
                    Notify(!0, "Unit Of Measure Created Successfully");
                    $("#btnCreateNewUnitOfMeasure").click();
                }

                LoadAllUnitOfMeasures();
            })
            .catch(function (error) {
                laddaInstance.stop(); 
                Notify(!1, error);
            });
        });

        $("#btnCreateNewUnitOfMeasure").click(function () {
            $("#UnitOfMeasureForm")[0].reset();
            $("#UnitOfMeasure_UnitOfMeasureID").val("0");
            $("#updateunitofmeasure").hide();
            $("#addunitofmeasure").show()
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