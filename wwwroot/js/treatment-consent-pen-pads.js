(function (window) {
    "use strict";

    function isElementVisible(el) {
        if (!el || !el.isConnected) {
            return false;
        }
        const style = window.getComputedStyle(el);
        if (style.display === "none" || style.visibility === "hidden") {
            return false;
        }
        const rect = el.getBoundingClientRect();
        return rect.width >= 2 && rect.height >= 2;
    }

    function createPad(canvas, options) {
        const opts = options || {};
        const ctx = canvas.getContext("2d");
        let drawing = false;
        let lastX = 0;
        let lastY = 0;
        let dirty = false;
        let inkSnapshot = null;
        let enabled = opts.enabled !== false;
        const wrap = canvas.closest("[data-pen-pad]");

        function applyStrokeStyle(ratio) {
            ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
            ctx.lineCap = "round";
            ctx.lineJoin = "round";
            ctx.strokeStyle = opts.strokeStyle || "#1f2430";
            ctx.lineWidth = opts.lineWidth || 2.2;
        }

        function persistInk() {
            if (!dirty || canvas.width < 2 || canvas.height < 2) {
                return;
            }
            try {
                inkSnapshot = canvas.toDataURL("image/png");
            } catch (e) {
                /* ignore */
            }
        }

        function restoreInk(width, height) {
            if (!inkSnapshot) {
                return;
            }
            const img = new Image();
            img.onload = function () {
                ctx.drawImage(img, 0, 0, width, height);
            };
            img.src = inkSnapshot;
        }

        function resize(force) {
            // Hidden tab panes report 0x0; resizing then wipes the bitmap.
            if (!force && !isElementVisible(canvas)) {
                return false;
            }

            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            const rect = canvas.getBoundingClientRect();
            const width = Math.max(1, Math.floor(rect.width));
            const height = Math.max(1, Math.floor(rect.height));

            if (width < 2 || height < 2) {
                return false;
            }

            if (dirty && !inkSnapshot) {
                persistInk();
            }

            canvas.width = Math.floor(width * ratio);
            canvas.height = Math.floor(height * ratio);
            applyStrokeStyle(ratio);

            if (inkSnapshot) {
                restoreInk(width, height);
            }

            return true;
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
            if (dirty) {
                persistInk();
            }
        }

        function clear() {
            if (!enabled) return;
            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            applyStrokeStyle(ratio);
            dirty = false;
            inkSnapshot = null;
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
        window.addEventListener("resize", function () {
            resize();
        });
        setEnabled(enabled);

        return {
            canvas: canvas,
            wrap: wrap,
            clear: clear,
            resize: resize,
            setEnabled: setEnabled,
            isEmpty: function () { return !dirty; },
            toDataURL: function () {
                if (inkSnapshot) {
                    return inkSnapshot;
                }
                return dirty ? canvas.toDataURL("image/png") : "";
            }
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

    function resizePadsIn(pads, container) {
        Object.keys(pads || {}).forEach(function (key) {
            const pad = pads[key];
            if (!pad || typeof pad.resize !== "function") {
                return;
            }
            if (container && pad.wrap && !container.contains(pad.wrap)) {
                return;
            }
            pad.resize();
        });
    }

    window.TreatmentConsentPenPads = {
        initAll: initAll,
        setAllEnabled: setAllEnabled,
        resizePadsIn: resizePadsIn
    };
})(window);
