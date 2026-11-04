const buttons = document.querySelectorAll("[data-copy]");

for (const button of buttons) {
    button.addEventListener("click", async () => {
        const value = button.getAttribute("data-copy") ?? "";
        const previous = button.textContent;
        const copied = await writeClipboard(value);
        button.textContent = copied ? "Copied" : "Copy failed";
        window.setTimeout(() => {
            button.textContent = previous;
        }, 1600);
    });
}

async function writeClipboard(value) {
    try {
        await navigator.clipboard.writeText(value);
        return true;
    } catch {
        const area = document.createElement("textarea");
        area.value = value;
        area.setAttribute("readonly", "");
        area.style.position = "fixed";
        area.style.left = "-9999px";
        document.body.append(area);
        area.select();
        let copied = false;
        try {
            copied = document.execCommand("copy");
        } catch {
            copied = false;
        }
        area.remove();
        return copied;
    }
}
