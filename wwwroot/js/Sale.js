let deletedSaleDetailIds = [];
function addNewRow() {
    const saleDetailTable = document.getElementById('saleDetailTable');
    const tbody = saleDetailTable.querySelector('tbody');
    const templateRow = tbody.rows[0];
    const newRow = templateRow.cloneNode(true);

    newRow.querySelectorAll('input', 'selet').forEach(el => {
        if (el.tagName == "SELECT") {
            el.selectedIndex = 0;
        } else {
            el.value = '';
        }
    })

    tbody.appendChild(newRow);
}
function deleteRow(button) {
    const row = button.closest('tr');
    const saleDetailId = row.querySelector(".saleDetailId")?.value;

    const tbody = row.parentElement;
    const totalRows = tbody.querySelectorAll('tr').length;

    if (totalRows <= 1) {
        alert('At least one row must remain.');
        return;
    }

    // Keep track of deleted existing sale details
    if (saleDetailId && saleDetailId !== "0") {
        deletedSaleDetailIds.push(parseInt(saleDetailId));
    }

    row.remove();
}

async function populateItemDropdowns() {
    try {
        const res = await fetch('/Sale/GetItems'); // fetch API
        const response = await res.json(); // parse JSON

        // Ensure we have a successful response and a data array
        if (!response.success || !response.data) return;

        const items = response.data; // List<Item> from ServiceResponse

        // Populate all dropdowns with class 'itemId'
        const itemSelects = document.querySelectorAll('.itemId');
        itemSelects.forEach(select => {
            // Clear previous options if needed
            select.innerHTML = '';

            items.forEach((item, rowIndex) => {
                if (rowIndex == 0) {
                    const option = document.createElement('option');
                    option.value = 0;
                    option.text = "-- Select Item --";
                    select.appendChild(option);
                }
                const option = document.createElement('option');
                option.value = item.id;
                option.text = item.name;
                select.appendChild(option);
            });
        });
    } catch (error) {
        console.error('Error fetching item list:', error);
    }
}

async function populatePartyDropdowns() {
    try {
        const res = await fetch('/Sale/GetParties'); // fetch API
        const response = await res.json(); // parse JSON

        // Ensure we have a successful response and a data array
        if (!response.success || !response.data) return;

        const parties = response.data; // List<PartyMaster> from ServiceResponse

        // Populate a dropdown with id 'partyId'
        const partySelect = document.getElementById('partyId');

        partySelect.innerHTML = '';

        parties.forEach((party, rowIndex) => {            
            if (rowIndex == 0) {
                const option = document.createElement('option');
                option.value = 0;
                option.text = "-- Select Party --";
                partySelect.appendChild(option);
            } 
            const option = document.createElement('option');
            option.value = party.id;
            option.text = party.name;
            partySelect.appendChild(option);
        });

    } catch (error) {
        console.error('Error fetching party list:', error);
    }
}

window.onload = function () {

    populatePartyDropdowns();

    populateItemDropdowns();

    let today = new Date().toISOString().split('T')[0]; // format: yyyy-MM-dd

    document.getElementById("saleDate").value = today;
    document.getElementById("dueDate").value = today;

    const saleDateInput = document.getElementById("saleDate");
    const dueDaysInput = document.getElementById("dueDays");
    const dueDateInput = document.getElementById("dueDate");

    // Format date as yyyy-MM-dd
    function formatDate(date) {
        return date.toISOString().split("T")[0];
    }

    // When Due Days changes → calculate Due Date
    dueDaysInput.addEventListener("input", () => {
        const saleDate = new Date(saleDateInput.value);
        const dueDays = parseInt(dueDaysInput.value, 10);

        if (!isNaN(dueDays)) {
            const dueDate = new Date(saleDate);
            dueDate.setDate(dueDate.getDate() + dueDays);

            if (dueDate < saleDate) {
                alert("Due Date cannot be earlier than Sale Date.");
                dueDateInput.value = "";
                dueDaysInput.value = "";
            } else {
                dueDateInput.value = formatDate(dueDate);
            }
        }
    });

    // When Due Date changes → calculate Due Days
    dueDateInput.addEventListener("change", () => {
        const saleDate = new Date(saleDateInput.value);
        const dueDate = new Date(dueDateInput.value);

        if (dueDate < saleDate) {
            alert("Due Date cannot be earlier than Sale Date.");
            dueDateInput.value = "";
            dueDaysInput.value = "";
        } else {
            const diffTime = dueDate - saleDate;
            const diffDays = Math.round(diffTime / (1000 * 60 * 60 * 24));
            dueDaysInput.value = diffDays;
        }
    });

    // When sale date changes -> set due days and due date blank
    saleDateInput.addEventListener("change", function () {
        dueDateInput.value = ""; // reset due date
        dueDaysInput.value = "";
    });

    const saleDetailTable = document.getElementById('saleDetailTable');

    saleDetailTable.addEventListener('change', function (e) {
        if (e.target.tagName === 'SELECT' && e.target.classList.contains('itemId')) {
            const row = e.target.closest('tr');
            updateItemPrice(row, e.target.value);
        }
    });

    saleDetailTable.addEventListener('input', function (e) {
        if (e.target.classList.contains('itemQty')) {
            const row = e.target.closest('tr');
            if (!row) return;

            const qty = parseFloat(row.querySelector('.itemQty').value) || 0;
            const price = parseFloat(row.querySelector('.itemPrice').value) || 0;
            const amountInput = row.querySelector('.itemAmount');

            const amount = qty * price;
            amountInput.value = amount.toFixed(2);
        }
    });

    document.getElementById('saleDate')?.focus();
}

async function saveSalesData() {
    const saleDate = document.getElementById("saleDate").value;
    const dueDays = document.getElementById("dueDays").value;
    const dueDate = document.getElementById("dueDate").value;
    const partyId = document.getElementById("partyId").value;

    if (partyId == "") {
        alert('Party should not be blank!');
        return;
    }

    const saleDetails = [];
    const rows = document.querySelectorAll("#saleDetailTable tbody tr");

    rows.forEach(row => {
        const itemId = row.querySelector(".itemId").value;
        const qty = row.querySelector(".itemQty").value;

        if (itemId == "") {
            alert('Item should not be blank!');
            return;
        }

        if (itemId) {
            if (qty == "") {
                alert('Item Qty should not be blank!');
                return;
            }

            saleDetails.push({
                ItemId: parseInt(itemId),
                Qty: parseInt(qty)
            });
        }
    });

    const saleData = {
        SaleDate: saleDate,
        DueDays: parseInt(dueDays),
        DueDate: dueDate,
        PartyMasterId: parseInt(partyId),
        SalesDetailRequests: saleDetails
    };

    await fetch('/Sale/SaveSalesData', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(saleData)
    })
        .then(res => res.json())
        .then(data => {
            alert('Saved Successfully.');
            window.location.href = '/Sale/Sale';
            //document.getElementById('saleMasterId').value = data.id;
        })
        .catch(error => {
            console.error("Error Saving Data.", error)
            alert('Error Saving Data.');
        });
}
function editSale(id, saleDate, dueDays, dueDate, partyId, button) {
    const salesDetails = JSON.parse(button.getAttribute("data-salesdetails"));
    document.getElementById("saleMasterId").value = id;
    //let saleTDate = new Date(saleDate).toISOString().split('T')[0];
    document.getElementById("saleDate").value = saleDate;
    document.getElementById("dueDays").value = dueDays;
    //let dueTDate = new Date(dueDate).toISOString().split('T')[0];
    document.getElementById("dueDate").value = dueDate;
    document.getElementById("partyId").value = partyId;
    document.getElementById("btnSave").onclick = updateSalesData;
    document.getElementById("btnSave").innerText = "Update";

    const rows = document.querySelectorAll("#saleDetailTable tbody tr");
    const tbody = document.querySelector("#saleDetailTable tbody");
    const templateRow = tbody.rows[0];

    rows.forEach(row => {
        row.remove();
    });
    const totalRows = tbody.querySelectorAll('tr').length;
    if (totalRows == salesDetails.length) return;

    salesDetails.forEach((select, rowIndex) => {
        const row = templateRow.cloneNode(true);
        const item = row.querySelector(".itemId");
        const qty = row.querySelector(".itemQty");
        const saleMasterId = row.querySelector(".saleMasterId");
        const saleDetailId = row.querySelector(".saleDetailId");

        if (select.ItemId) {
            item.value = select.ItemId;
            qty.value = select.Qty;
            saleMasterId.value = select.SalesMasterId;
            saleDetailId.value = select.Id;
            updateItemPrice(row, select.ItemId);
        }

        tbody.appendChild(row);
    });
}

async function updateItemPrice(row, itemId) {
    if (!itemId) return;

    await fetch('/Sale/GetItemPrice?id=' + itemId)
        .then(res => {
            if (!res.ok) throw new Error('Network Error');
            return res.json();
        })
        .then(data => {
            const price = parseFloat(data.price) || 0;
            row.querySelector('.itemPrice').value = price.toFixed(2);

            const qty = parseFloat(row.querySelector('.itemQty').value) || 0;
            row.querySelector('.itemAmount').value = (qty * price).toFixed(2);
        })
        .catch(error => {
            alert('Failed to fetch price: ' + error.message);
        });
}

async function updateSalesData() {
    const saleMasterId = document.getElementById("saleMasterId").value;
    const saleDate = document.getElementById("saleDate").value;
    const dueDays = document.getElementById("dueDays").value;
    const dueDate = document.getElementById("dueDate").value;
    const partyId = document.getElementById("partyId").value;

    const saleDetails = [];
    const rows = document.querySelectorAll("#saleDetailTable tbody tr");

    rows.forEach(row => {
        const saleDetailId = row.querySelector(".saleDetailId").value;
        const itemId = row.querySelector(".itemId").value;
        const qty = row.querySelector(".itemQty").value;

        if (itemId == "") {
            alert('Item should not be blank!');
            return;
        }

        if (itemId) {
            if (qty == "") {
                alert('Qty should not be blank!');
                return;
            }

            saleDetails.push({
                Id: saleDetailId ? parseInt(saleDetailId) : 0,
                SaleMasterId: parseInt(saleMasterId),
                ItemId: parseInt(itemId),
                Qty: parseInt(qty)
            });
        }
    });

    const saleData = {
        Id: saleMasterId,
        SaleDate: saleDate,
        DueDays: parseInt(dueDays),
        DueDate: dueDate,
        PartyMasterId: parseInt(partyId),
        SalesDetailRequests: saleDetails,
        DeletedSaleDetailIds: deletedSaleDetailIds
    };

    await fetch('/Sale/UpdateSalesData', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(saleData)
    })
        .then(res => res.json())
        .then(data => {
            alert('Saved Successfully.');
            window.location.href = '/Sale/Sale';
            //document.getElementById('saleMasterId').value = data.id;
        })
        .catch(error => {
            console.error("Error Saving Data.", error)
            alert('Error Saving Data.');
        });
}

async function deleteSalesData(id) {
    await fetch('/Sale/DeleteSalesData', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: 'id=' + id
    })
        .then(res => res.json())
        .then(data => {
            alert('Data deleted successfully');
            window.location.href = '/Sale/Sale';
        })
        .catch(error => {
            console.error("Error deleting data", error);
            alert('Error deleting data');
        })
}