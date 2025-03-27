document.querySelector("#Item_BundleTarget").addEventListener("input", e => {
    if (e.target.checked) {
        document.querySelector("#Bundles").removeAttribute("disabled");
        document.querySelector("#Bundles").removeAttribute("hidden");
    } else {
        document.querySelector("#Bundles").setAttribute("disabled", "");
        document.querySelector("#Bundles").setAttribute("hidden", "");
    }
});
document.querySelectorAll("[name=Search]").forEach(button => {
    button.addEventListener("click", async e => {
        e.preventDefault();
        const ret = await new ItemSelectDialog().show();
        if (!ret)
            return;
        document.querySelector(`[name="Item.Bundles[${e.target.closest("[name]").value}].Sku"]`).value = ret.sku;
        document.querySelector(`[name="Item.Bundles[${e.target.closest("[name]").value}].Name"]`).value = ret.name;
    }, { capture : true });
});