window.showYesNoModal = function (message = "Are you sure?") {
    return new Promise((resolve) => {

        const modalElement = document.getElementById("yesNoModal");
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

        document.getElementById("yesNoModalMessage").textContent = message;

        // YES button
        const yesBtn = document.getElementById("yesBtn");
        const newYesBtn = yesBtn.cloneNode(true);
        yesBtn.replaceWith(newYesBtn);

        newYesBtn.addEventListener("click", () => {
            resolve(true);
            modal.hide();
        });

        // NO button
        const noBtn = document.getElementById("noBtn");
        const newNoBtn = noBtn.cloneNode(true);
        noBtn.replaceWith(newNoBtn);

        newNoBtn.addEventListener("click", () => {
            resolve(false);
            modal.hide();
        });

        modal.show();
    });
};
