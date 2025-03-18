class ItemSelectDialog {
    option;
    dialog;
    rowTemplate;

    constructor(option) {
        this.option = option ?? {};
    }

    async show() {
        this.dialog = document.querySelector("#itemSelectDialogTemplate").content.cloneNode(true).firstElementChild;
        this.rowTemplate = this.dialog.querySelector("tbody tr").cloneNode(true);

        this.dialog.querySelector("[name=IsPurchaseTarget]").value = this.option.isPurchaseTarget ?? "";
        this.dialog.querySelector("[name=IsSalesTarget]").value = this.option.isSalesTarget ?? "";
        this.dialog.querySelector("[name=IsBundleTarget]").value = this.option.isBundleTarget ?? "";

        document.body.append(this.dialog);
        this.dialog.querySelector("tbody").replaceChildren();
        this.dialog.addEventListener("input", this.#search.bind(this));
        await this.#search();
        return new Promise((resolve) => {
            this.dialog.showModal();
            this.dialog.addEventListener('close', () => {
                this.dialog.remove();

                resolve(this.items[parseInt(this.dialog.returnValue)]);
            }, { once: true });

            this.dialog.addEventListener('click', (event) => {
                if (event.target === this.dialog) {
                    this.dialog.close();
                    this.dialog.remove();
                    resolve();
                }
            });
        });
    }

    async #search() {
        const formData = new FormData(this.dialog.querySelector("form"));
        const response = await fetch("/api/Item/Search", {
            method: 'POST',
            body: formData
        });
        this.items = await response.json();
        this.dialog.querySelector("tbody").replaceChildren();
        for (let i = 0; i < this.items.length; i++) {
            const tr = this.rowTemplate.cloneNode(true);
            tr.querySelector(".selectButton button").value = i;
            tr.querySelector(".sku").append(this.items[i].sku);
            tr.querySelector(".name").append(this.items[i].name);
            tr.querySelector(".stock").append(this.items[i].stock ?? "");
            tr.querySelector(".unitPrice").append(this.items[i].unitPrice ? this.items[i].unitPrice + "円" : "");
            this.dialog.querySelector("tbody").append(tr);
        }
    }
}