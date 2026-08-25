(function (window, $) {
    "use strict";

    const DEFAULTS = {
        chartSelector: "#psrInteractiveToothChart",
        editable: false,
        isToothBlocked: null,
        onBlocked: null,
        onSelectionChanged: null
    };

    function isBlocked(options, toothNumber) {
        if (typeof options.isToothBlocked !== "function") {
            return false;
        }
        return !!options.isToothBlocked(toothNumber);
    }

    function notifyBlocked(options, toothNumber) {
        if (typeof options.onBlocked === "function") {
            options.onBlocked(toothNumber);
            return;
        }
        if (typeof Swal !== "undefined" && Swal.fire) {
            Swal.fire(
                "Cannot select missing tooth",
                "Tooth " + toothNumber + " is already used in a Dental Finding.",
                "warning"
            );
        }
    }

    function notifySelectionChanged(options, toothNumber, selected) {
        if (typeof options.onSelectionChanged === "function") {
            options.onSelectionChanged(toothNumber, selected);
        }
    }

    function setToothSelected($chart, options, toothNumber, selected) {
        const tooth = String(toothNumber);
        if (selected && isBlocked(options, tooth)) {
            notifyBlocked(options, tooth);
            return false;
        }

        const $input = $("#psrToothCheck_" + tooth);
        const $btn = $chart.find('.psr-tooth-image-btn[data-tooth="' + tooth + '"]');
        const $img = $btn.find(".psr-tooth-img");

        $input.prop("checked", !!selected);
        $btn.attr("aria-pressed", selected ? "true" : "false");
        if ($img.length) {
            $img.attr("src", selected ? $img.attr("data-selected") : $img.attr("data-normal"));
        }

        notifySelectionChanged(options, tooth, !!selected);
        return true;
    }

    function toggleTooth($chart, options, toothNumber) {
        const $input = $("#psrToothCheck_" + toothNumber);
        if (!$input.length) {
            return;
        }
        const willSelect = !$input.is(":checked");
        if (willSelect && isBlocked(options, toothNumber)) {
            notifyBlocked(options, toothNumber);
            return;
        }
        setToothSelected($chart, options, toothNumber, willSelect);
    }

    function syncVisualState($chart) {
        $chart.find(".psr-tooth-input").each(function () {
            const tooth = $(this).attr("data-tooth");
            const selected = $(this).is(":checked");
            const $btn = $chart.find('.psr-tooth-image-btn[data-tooth="' + tooth + '"]');
            const $img = $btn.find(".psr-tooth-img");
            $btn.attr("aria-pressed", selected ? "true" : "false");
            if ($img.length) {
                $img.attr("src", selected ? $img.attr("data-selected") : $img.attr("data-normal"));
            }
        });
    }

    function preloadSelectedImages($chart) {
        $chart.find(".psr-tooth-img").each(function () {
            const selectedSrc = $(this).attr("data-selected");
            if (selectedSrc) {
                const preload = new Image();
                preload.src = selectedSrc;
            }
        });
    }

    function bindInteractions($chart, options) {
        $chart.off("click.psrTooth").on("click.psrTooth", ".psr-tooth-image-btn", function (e) {
            e.preventDefault();
            toggleTooth($chart, options, $(this).attr("data-tooth"));
        });

        $chart.off("change.psrTooth").on("change.psrTooth", ".psr-tooth-input", function () {
            const tooth = $(this).attr("data-tooth");
            if ($(this).is(":checked") && isBlocked(options, tooth)) {
                $(this).prop("checked", false);
                notifyBlocked(options, tooth);
                setToothSelected($chart, options, tooth, false);
                return;
            }
            setToothSelected($chart, options, tooth, $(this).is(":checked"));
        });
    }

    function init(userOptions) {
        const options = $.extend({}, DEFAULTS, userOptions || {});
        const $chart = $(options.chartSelector);
        if (!$chart.length) {
            return null;
        }

        preloadSelectedImages($chart);
        syncVisualState($chart);

        const readonlyAttr = $chart.attr("data-readonly") === "true";
        const editable = !!options.editable && !readonlyAttr;

        $chart.toggleClass("psr-tooth-chart-editable", editable);

        if (!editable) {
            $chart.attr("data-readonly", "true");
            $chart.find(".psr-tooth-input, .psr-tooth-image-btn").prop("disabled", true);
            $chart.off("click.psrTooth change.psrTooth");
            return {
                setSelected: function (toothNumber, selected) {
                    return setToothSelected($chart, options, toothNumber, selected);
                }
            };
        }

        $chart.removeAttr("data-readonly");
        $chart.find(".psr-tooth-input, .psr-tooth-image-btn").prop("disabled", false);
        bindInteractions($chart, options);

        return {
            setSelected: function (toothNumber, selected) {
                return setToothSelected($chart, options, toothNumber, selected);
            }
        };
    }

    window.DentalPsrToothChart = {
        init: init
    };
})(window, jQuery);
