(function (window) {
    "use strict";

    function createPad(canvas, options) {
        const opts = options || {};
        const ctx = canvas.getContext("2d");
        let drawing = false;
        let lastX = 0;
        let lastY = 0;
        let dirty = false;
        let enabled = opts.enabled !== false;
        const wrap = canvas.closest("[data-pen-pad]");

        function resize() {
            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            const rect = canvas.getBoundingClientRect();
            const width = Math.max(1, Math.floor(rect.width));
            const height = Math.max(1, Math.floor(rect.height));
            const snapshot = dirty ? canvas.toDataURL() : null;

            canvas.width = Math.floor(width * ratio);
            canvas.height = Math.floor(height * ratio);
            ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
            ctx.lineCap = "round";
            ctx.lineJoin = "round";
            ctx.strokeStyle = opts.strokeStyle || "#1f2430";
            ctx.lineWidth = opts.lineWidth || 2.2;

            if (snapshot) {
                const img = new Image();
                img.onload = function () {
                    ctx.drawImage(img, 0, 0, width, height);
                };
                img.src = snapshot;
            }
        }

        function pointerPos(event) {
            const rect = canvas.getBoundingClientRect();
            const point = event.touches && event.touches[0] ? event.touches[0] : event;
            return {
                x: point.clientX - rect.left,
                y: point.clientY - rect.top
            };
        }

        function start(event) {
            if (!enabled) return;
            event.preventDefault();
            drawing = true;
            if (canvas.setPointerCapture && event.pointerId != null) {
                try { canvas.setPointerCapture(event.pointerId); } catch (e) { /* ignore */ }
            }
            const pos = pointerPos(event);
            lastX = pos.x;
            lastY = pos.y;
        }

        function move(event) {
            if (!enabled || !drawing) return;
            event.preventDefault();
            const pos = pointerPos(event);
            ctx.beginPath();
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(pos.x, pos.y);
            ctx.stroke();
            lastX = pos.x;
            lastY = pos.y;
            dirty = true;
            canvas.dataset.hasInk = "true";
        }

        function end(event) {
            if (!drawing) return;
            event.preventDefault();
            drawing = false;
        }

        function clear() {
            if (!enabled) return;
            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
            ctx.lineCap = "round";
            ctx.lineJoin = "round";
            ctx.strokeStyle = opts.strokeStyle || "#1f2430";
            ctx.lineWidth = opts.lineWidth || 2.2;
            dirty = false;
            canvas.dataset.hasInk = "false";
        }

        function setEnabled(isEnabled) {
            enabled = !!isEnabled;
            drawing = false;
            canvas.style.pointerEvents = enabled ? "auto" : "none";
            canvas.style.cursor = enabled ? "crosshair" : "not-allowed";
            if (wrap) {
                wrap.classList.toggle("is-disabled", !enabled);
            }
        }

        canvas.style.touchAction = "none";
        canvas.addEventListener("pointerdown", start);
        canvas.addEventListener("pointermove", move);
        canvas.addEventListener("pointerup", end);
        canvas.addEventListener("pointerleave", end);
        canvas.addEventListener("pointercancel", end);

        resize();
        window.addEventListener("resize", resize);
        setEnabled(enabled);

        return {
            clear: clear,
            resize: resize,
            setEnabled: setEnabled,
            isEmpty: function () { return !dirty; },
            toDataURL: function () { return dirty ? canvas.toDataURL("image/png") : ""; }
        };
    }

    function initAll(root, options) {
        const scope = root || document;
        const opts = options || {};
        const pads = {};
        const enabled = opts.enabled !== false;

        scope.querySelectorAll("[data-pen-pad]").forEach(function (wrap) {
            const canvas = wrap.querySelector("canvas");
            const clearBtn = wrap.querySelector("[data-pen-clear]");
            if (!canvas) return;

            const size = wrap.getAttribute("data-pen-size") || "line";
            const lineWidth = size === "initials" ? 1.8 : (size === "signature" ? 2.4 : 2.1);
            const pad = createPad(canvas, { lineWidth: lineWidth, enabled: enabled });
            const id = wrap.getAttribute("data-pen-pad") || ("pad_" + Math.random().toString(36).slice(2));
            pads[id] = pad;

            if (clearBtn) {
                clearBtn.addEventListener("click", function () {
                    pad.clear();
                });
            }
        });

        return pads;
    }

    function setAllEnabled(pads, isEnabled) {
        Object.keys(pads || {}).forEach(function (key) {
            if (pads[key] && typeof pads[key].setEnabled === "function") {
                pads[key].setEnabled(isEnabled);
            }
        });
    }

    window.TreatmentConsentPenPads = {
        initAll: initAll,
        setAllEnabled: setAllEnabled
    };
})(window);
