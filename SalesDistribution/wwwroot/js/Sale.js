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
document.querySelector(".images").addEventListener("click", async e => {
    const button = e.target.closest("button");
    if (!button) {
        return;
    }
    e.stopPropagation();
    e.preventDefault();
    if (await new MessageDialog("画像を削除しますか？", "はい", "いいえ").show() == "はい") {
        button.closest(".image").remove();
    }
});
window.addEventListener('dragenter', (event) => {
    event.stopPropagation();
    event.preventDefault();
    document.body.classList.add('dragover');
});
window.addEventListener('dragover', (event) => {
    event.stopPropagation();
    event.preventDefault();
    event.dataTransfer.dropEffect = 'copy';
});
window.addEventListener('dragleave', (event) => {
    event.stopPropagation();
    event.preventDefault();
    document.body.classList.remove('dragover');
});
window.addEventListener('drop', async (event) => {
    event.stopPropagation();
    event.preventDefault();
    document.body.classList.remove('dragover');

    const files = [...event.dataTransfer.files].filter(file => file.type.startsWith('image/'));
    if (files.length === 0) {
        return;
    }
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
        formData.append(`Images`, files[i], files[i].name);
    }
    const imageIds = await (await fetch("/api/Image/Upload", { method: "POST", body: formData })).json();
    for (let i = 0; i < imageIds.length; i++) {
        const container = document.querySelector("#dropImageContainer").content.cloneNode(true);

        const input = container.querySelector(`input[type="hidden"]`);
        input.setAttribute("name", `Sale.Images`);
        input.setAttribute("value", imageIds[i]);

        const url = "/api/Image/Download/" + imageIds[i];

        const link = container.querySelector(`a`);
        link.setAttribute("href", url);

        const img = container.querySelector(`img`);
        img.setAttribute("src", url);

        document.querySelector("form .images").append(container);
    }
});
