class MessageDialog {
    message;
    buttons;
    dialog;
    constructor(message, ...buttons) {
        this.message = message;
        this.buttons = buttons;
    }

    async show() {
        this.dialog = document.querySelector("#messageDialogTemplate").content.cloneNode(true).firstElementChild;

        this.dialog.querySelector("p").replaceChildren(this.message);
        this.dialog.querySelector("nav").replaceChildren(...this.buttons.map(value => {
            const button = document.createElement("button");
            button.append(value);
            button.classList.add("btn", "btn-primary");
            button.value = value;
            return button;
        }));

        document.body.append(this.dialog);

        return new Promise((resolve) => {
            this.dialog.showModal();
            this.dialog.addEventListener('close', () => {
                this.dialog.remove();

                resolve(this.dialog.returnValue);
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
}