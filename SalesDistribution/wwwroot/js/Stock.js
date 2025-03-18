document.querySelector(`[name="StockHistory.UnitPrice"]`).addEventListener("input", e => {
    document.querySelector(`[name="StockHistory.Price"]`).value = "";
});
document.querySelector(`[name="StockHistory.Price"]`).addEventListener("input", e => {
    document.querySelector(`[name="StockHistory.UnitPrice"]`).value = "";
});
document.querySelector(`[name="StockHistory.UnitPrice"]`).addEventListener("focusin", e => {
    document.querySelector(`[name="StockHistory.Price"]`).setAttribute("disabled", "");
});
document.querySelector(`[name="StockHistory.Price"]`).addEventListener("focusin", e => {
    document.querySelector(`[name="StockHistory.UnitPrice"]`).setAttribute("disabled", "");
});
document.querySelector(`[name="StockHistory.UnitPrice"]`).addEventListener("focusout", e => {
    document.querySelector(`[name="StockHistory.Price"]`).removeAttribute("disabled");
});
document.querySelector(`[name="StockHistory.Price"]`).addEventListener("focusout", e => {
    document.querySelector(`[name="StockHistory.UnitPrice"]`).removeAttribute("disabled");
});
