document.body.addEventListener("input", async e => {
    if (e.target.type != "number") {
        return;
    }
    await calc();
});
document.addEventListener("DOMContentLoaded", async e => {
    await calc();
})
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