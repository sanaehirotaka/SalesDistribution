document.addEventListener("DOMContentLoaded", calc);
document.body.addEventListener("input", async e => {
    if (e.target.getAttribute("type") != "number" && e.target.getAttribute("inputmode") != "numeric") {
        return;
    }
    await calc();
});
async function calc() {
    const formData = new FormData(document.querySelector("form"));
    const response = await fetch("/api/Sale/Calc", {
        method: 'POST',
        body: formData
    });
    const sale = await response.json();
    for (let i = 0; i < sale.items.length; i++) {
        document.querySelector(`[name="Sale.Items[${i}].PurchasePrice"]`).value = sale.items[i].purchasePrice;
        document.querySelector(`[name="Sale.Items[${i}].SalePrice"]`).value = sale.items[i].salePrice;
    }
    document.querySelector(`[name="Sale.PurchasePrice"]`).value = sale.purchasePrice;
    document.querySelector(`[name="Sale.SalePrice"]`).value = sale.salePrice;
    document.querySelector(`[name="Sale.IncomePrice"]`).value = sale.incomePrice;
    document.querySelector(`[name="Sale.SalesProfit"]`).value = sale.salesProfit;
    document.querySelector(`[name="Sale.SalesProfitRatio"]`).value = sale.salesProfitRatio;
}
document.querySelectorAll("[name=Search]").forEach(button => {
    button.addEventListener("click", async e => {
        e.preventDefault();
        const ret = await new ItemSelectDialog({
            isSalesTarget: true
        }).show();
        if (!ret)
            return;
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].Sku"]`).value = ret.sku;
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].Name"]`).value = ret.name;
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].Amount"]`).value = "";
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].PurchaseUnitPrice"]`).value = ret.unitPrice;
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].SaleUnitPrice"]`).value = "";
        document.querySelector(`[name="Sale.Items[${e.target.closest("[name]").value}].Stock"]`).replaceChildren(ret.stock)
    }, { capture: true });
});
document.querySelector(`[name="Sale.Status"][value="Shipped"]`).addEventListener("input", e => {
    if (e.target.checked) {
        document.querySelector(`[name="Sale.ShipDate"]`).value = dayjs().format("YYYY-MM-DD");
    }
});
document.querySelector(`[name="Sale.Status"][value="Closed"]`).addEventListener("input", e => {
    if (e.target.checked) {
        document.querySelector(`[name="Sale.CloseDate"]`).value = dayjs().format("YYYY-MM-DD");
    }
});
document.querySelector("form").addEventListener("submit", async e => {
    if (e.target.dataset.submitted == "true") {
        return;
    }
    const message = (() => {
        switch (e.submitter.textContent) {
            case "登録":
                return "登録します。よろしいですか？";
            case "削除":
                return "削除します。よろしいですか？";
        }
    })();
    if (!message) {
        return;
    }
    e.preventDefault();
    e.submitter?.setAttribute("disabled", "");
    e.target.dataset.submitted = "true";
    try {
        if (await new MessageDialog(message, "はい", "いいえ").show() == "はい") {
            setTimeout(() => {
                e.target.requestSubmit(e.submitter);
            });
        } else {
            e.submitter?.removeAttribute("disabled");
            delete e.target.dataset.submitted;
        }
    } catch (ex) {
        e.submitter?.removeAttribute("disabled");
        delete e.target.dataset.submitted;
    }
});
