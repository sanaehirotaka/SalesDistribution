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
