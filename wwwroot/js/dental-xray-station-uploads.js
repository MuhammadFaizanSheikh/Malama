(function (window, $) {
    "use strict";

    const DEFAULTS = {
        downloadImageUrl: "/DentalXRay/DownloadXRayImage",
        formSelector: "#SaveDentalXRayStation",
        isFemale: false,
        bwxNeeded: false,
        paCardIndex: 1,
        initialBwxReason: "",
        initialPaReason: "",
        enableGoToVitalStation: false
    };

    const MAX_PA_CARDS = 8;
    const XRAY_ZOOM_STEP = 25;
    const XRAY_ZOOM_MIN = 50;
    const XRAY_ZOOM_MAX = 300;
    const BWX_UPLOAD_PREFIXES = ["bwx_left_molar", "bwx_left_premolar", "bwx_right_molar", "bwx_right_premolar"];
    const BWX_UPLOAD_LABELS = {
        bwx_left_molar: "BW Left Molar",
        bwx_left_premolar: "BW Left Premolar",
        bwx_right_molar: "BW Right Molar",
        bwx_right_premolar: "BW Right Premolar"
    };

    let config = Object.assign({}, DEFAULTS);
    let paCardIndex = 1;
    const xrayZoomLevels = {};
    const xrayPanState = {
        active: false,
        viewport: null,
        startX: 0,
        startY: 0,
        scrollLeft: 0,
        scrollTop: 0
    };
    let goToVitalStationClicked = false;
    let initialized = false;

    function cfg() {
        return config;
    }

    function buildDownloadUrl(downloadPrefix, fileName) {
        return cfg().downloadImageUrl
            + "?prefix=" + encodeURIComponent(downloadPrefix)
            + "&fileName=" + encodeURIComponent(fileName);
    }

    function syncPaAddButton() {
        const count = $("#paCardsRow .pa-card").length;
        const atMax = count >= MAX_PA_CARDS;
        $("#addPaCardBtn").prop("disabled", atMax);
        $("#addPaCardBtn").attr("title", atMax ? "Maximum of 8 PA X-Ray uploads allowed" : "");
    }

    function setFlatpickrVisibility(input, show) {
        if (!input) return;
        input.style.display = show ? "block" : "none";
        if (input._flatpickr?.altInput) {
            input._flatpickr.altInput.style.display = show ? "block" : "none";
        }
    }

    function getCurrentDateTimeValue() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, "0");
        const day = String(now.getDate()).padStart(2, "0");
        const hour = String(now.getHours()).padStart(2, "0");
        const minute = String(now.getMinutes()).padStart(2, "0");
        return `${year}-${month}-${day} ${hour}:${minute}`;
    }

    function toggleXRayUploadDate(input, prefix) {
        const datePicker = document.getElementById(prefix + "_uploadDate");
        const hiddenUploaded = document.getElementById(prefix + "_uploaded");
        const now = getCurrentDateTimeValue();

        if (input.files && input.files.length > 0) {
            setFlatpickrVisibility(datePicker, true);
            if (hiddenUploaded) hiddenUploaded.value = "true";
            if (datePicker) {
                datePicker.value = now;
                if (datePicker._flatpickr) {
                    datePicker._flatpickr.setDate(now, true, "Y-m-d H:i");
                }
            }
        }
        updateXRayFileUI(prefix);
        updateOverallStatus();
    }

    function setXRayZoom(prefix, level) {
        const previewImg = document.getElementById(prefix + "_previewImg");
        const zoomLabel = document.getElementById(prefix + "_zoomLevel");
        if (!previewImg || !zoomLabel) return;

        const clamped = Math.max(XRAY_ZOOM_MIN, Math.min(XRAY_ZOOM_MAX, level));
        xrayZoomLevels[prefix] = clamped;
        previewImg.style.width = clamped + "%";
        previewImg.style.height = "auto";
        previewImg.style.transform = "";
        previewImg.draggable = false;
        zoomLabel.textContent = clamped + "%";
        requestAnimationFrame(function () {
            updateXRayPanAvailability(prefix);
        });
    }

    function updateXRayPanAvailability(prefix) {
        const viewport = document.getElementById(prefix + "_previewViewport");
        const previewImg = document.getElementById(prefix + "_previewImg");
        if (!viewport) return;

        const canPan = !!previewImg?.src &&
            (viewport.scrollWidth > viewport.clientWidth + 1 ||
             viewport.scrollHeight > viewport.clientHeight + 1);

        viewport.classList.toggle("can-pan", canPan);
        viewport.title = canPan ? "Click and drag to move the image" : "";
        if (previewImg) {
            previewImg.draggable = false;
        }
    }

    function startXRayPan(clientX, clientY, viewport) {
        xrayPanState.active = true;
        xrayPanState.viewport = viewport;
        xrayPanState.startX = clientX;
        xrayPanState.startY = clientY;
        xrayPanState.scrollLeft = viewport.scrollLeft;
        xrayPanState.scrollTop = viewport.scrollTop;
        viewport.classList.add("is-panning");
    }

    function moveXRayPan(clientX, clientY) {
        if (!xrayPanState.active || !xrayPanState.viewport) return;
        const dx = clientX - xrayPanState.startX;
        const dy = clientY - xrayPanState.startY;
        xrayPanState.viewport.scrollLeft = xrayPanState.scrollLeft - dx;
        xrayPanState.viewport.scrollTop = xrayPanState.scrollTop - dy;
    }

    function endXRayPan() {
        if (!xrayPanState.active) return;
        xrayPanState.viewport?.classList.remove("is-panning");
        xrayPanState.active = false;
        xrayPanState.viewport = null;
    }

    function updateXRayPreview(prefix) {
        const previewSection = document.getElementById(prefix + "_previewSection");
        const previewImg = document.getElementById(prefix + "_previewImg");
        const status = document.getElementById(prefix + "_fileStatus");
        const hiddenFileName = document.getElementById(prefix + "_fileName");
        const removedInput = document.getElementById(prefix + "_removed");

        if (!previewSection || !previewImg || !status) return;

        const removed = removedInput?.value === "true";
        let src = "";

        if (status.dataset.previewMode === "selected" && status.dataset.selectedPreviewUrl) {
            src = status.dataset.selectedPreviewUrl;
        } else if (!removed && status.dataset.previewMode === "server") {
            const fileName = (hiddenFileName?.value || status.dataset.existingFilename || "").trim();
            const downloadPrefix = status.dataset.downloadPrefix || prefix;
            if (fileName) {
                src = buildDownloadUrl(downloadPrefix, fileName);
            }
        }

        if (src) {
            previewImg.onload = function () {
                setXRayZoom(prefix, xrayZoomLevels[prefix] || 100);
                updateXRayPanAvailability(prefix);
            };
            previewImg.src = src;
            previewSection.style.display = "block";
            if (previewImg.complete) {
                setXRayZoom(prefix, xrayZoomLevels[prefix] || 100);
            }
        } else {
            previewImg.removeAttribute("src");
            previewSection.style.display = "none";
            delete xrayZoomLevels[prefix];
        }
    }

    function updateXRayFileUI(prefix) {
        const fileInput = document.querySelector(`.xray-file[data-prefix='${prefix}']`);
        const status = document.getElementById(prefix + "_fileStatus");
        const fileNameDisplay = document.getElementById(prefix + "_fileNameDisplay");
        const hiddenUploaded = document.getElementById(prefix + "_uploaded");
        const hiddenFileName = document.getElementById(prefix + "_fileName");
        const hiddenOriginalFileName = document.getElementById(prefix + "_originalFileName");
        const removedInput = document.getElementById(prefix + "_removed");

        if (!fileInput || !status || !fileNameDisplay || !hiddenUploaded || !hiddenFileName) return;

        const selectedFile = fileInput.files && fileInput.files.length > 0 ? fileInput.files[0] : null;
        const existingFileName = (status.dataset.existingFilename || "").trim();
        const existingOriginalFileName = (status.dataset.existingOriginalFilename || "").trim();
        const removedExisting = removedInput?.value === "true";

        if (selectedFile) {
            fileNameDisplay.textContent = selectedFile.name;
            status.style.display = "flex";
            hiddenUploaded.value = "true";
            hiddenFileName.value = "";
            if (hiddenOriginalFileName) hiddenOriginalFileName.value = selectedFile.name;
            if (removedInput) removedInput.value = "false";

            const prevUrl = status.dataset.selectedPreviewUrl;
            if (prevUrl) URL.revokeObjectURL(prevUrl);
            status.dataset.selectedPreviewUrl = URL.createObjectURL(selectedFile);
            status.dataset.previewMode = "selected";
            updateXRayPreview(prefix);
            return;
        }

        if (status.dataset.selectedPreviewUrl) {
            URL.revokeObjectURL(status.dataset.selectedPreviewUrl);
            status.dataset.selectedPreviewUrl = "";
        }

        if (existingFileName && !removedExisting) {
            fileNameDisplay.textContent = existingOriginalFileName || existingFileName;
            status.style.display = "flex";
            hiddenUploaded.value = "true";
            hiddenFileName.value = existingFileName;
            if (hiddenOriginalFileName) hiddenOriginalFileName.value = existingOriginalFileName;
            status.dataset.previewMode = "server";
            updateXRayPreview(prefix);
            return;
        }

        fileNameDisplay.textContent = "";
        status.style.display = "none";
        hiddenUploaded.value = "false";
        hiddenFileName.value = "";
        if (hiddenOriginalFileName) hiddenOriginalFileName.value = "";
        status.dataset.previewMode = "";
        updateXRayPreview(prefix);
    }

    function toggleReasonAndUpload(statusSelector, reasonContainer, uploadContainer, reasonSelector) {
        const status = $(statusSelector).val();
        $(reasonContainer).hide();
        $(uploadContainer).hide();
        $(reasonSelector).prop("required", false);

        if (status === "Not Completed") {
            $(reasonContainer).show();
            $(reasonSelector).prop("required", true);
        } else if (status === "Completed") {
            $(uploadContainer).show();
        }
    }

    function toggleBwxSection() {
        const status = $("#BwxStatus").val();
        $("#bwxReasonContainer").hide();
        $("#bwxUploadModeSection").hide();
        $("#bwxConsolidatedUploadContainer").hide();
        $("#bwxSeparateUploadContainer").hide();
        $("#BwxReason").prop("required", false);

        if (status === "Not Completed") {
            $("#bwxReasonContainer").show();
            $("#BwxReason").prop("required", true);
        } else if (status === "Completed") {
            $("#bwxUploadModeSection").show();
            toggleBwxUploadContainers();
        }
    }

    function toggleBwxUploadContainers() {
        if ($("#BwxStatus").val() !== "Completed") {
            return;
        }

        const mode = $("input[name='BwxUploadMode']:checked").val();
        $("#bwxConsolidatedUploadContainer").toggle(mode === "Consolidated");
        $("#bwxSeparateUploadContainer").toggle(mode === "Separate");
    }

    function getSelectedBwxUploadMode() {
        return $("input[name='BwxUploadMode']:checked").val() || "";
    }

    function isBwxUploadCompleteForStatus() {
        const mode = getSelectedBwxUploadMode();
        if (mode === "Consolidated") {
            return isUploadComplete("bwx_consolidated");
        }
        if (mode === "Separate") {
            return BWX_UPLOAD_PREFIXES.every(isUploadComplete);
        }
        return false;
    }

    function togglePregnancyFlow() {
        if (!cfg().isFemale) {
            $("#clinicalStationSection").show();
            return;
        }

        const pregnant = $("#AreYouPregnant").val();
        $("#pregnancyApprovalSection").hide();
        $("#PregnancyApproval").prop("required", false);
        $("#clinicalStationSection").hide();

        if (pregnant === "Yes") {
            $("#pregnancyApprovalSection").show();
            $("#PregnancyApproval").prop("required", true);
            if ($("#PregnancyApproval").val() === "Approved") {
                $("#clinicalStationSection").show();
            }
        } else if (pregnant === "No") {
            $("#clinicalStationSection").show();
        }
    }

    function isGoToVitalStationSubmit(submitter) {
        return goToVitalStationClicked || (submitter && submitter.name === "GoToVitalStation");
    }

    function isUploadComplete(prefix) {
        const hiddenFileName = document.getElementById(prefix + "_fileName");
        const removedInput = document.getElementById(prefix + "_removed");
        const fileInput = document.querySelector(`.xray-file[data-prefix='${prefix}']`);

        if (fileInput?.files && fileInput.files.length > 0) return true;
        if (removedInput?.value === "true") return false;
        return !!(hiddenFileName?.value || "").trim();
    }

    function shouldValidateXRaySections() {
        return cfg().bwxNeeded && $("#xrayStationSection").is(":visible");
    }

    function validateXRayUploadsBeforeSave() {
        const errors = [];
        if (!shouldValidateXRaySections()) {
            return errors;
        }

        const bwxStatus = $("#BwxStatus").val();
        if (bwxStatus === "Completed") {
            const mode = getSelectedBwxUploadMode();
            if (!mode) {
                errors.push("BWX upload type selection is required.");
            } else if (mode === "Consolidated" && !isUploadComplete("bwx_consolidated")) {
                errors.push("BWX Status requires consolidated X-Ray image upload.");
            } else if (mode === "Separate") {
                const missingBwx = BWX_UPLOAD_PREFIXES.filter(function (prefix) {
                    return !isUploadComplete(prefix);
                }).map(function (prefix) {
                    return BWX_UPLOAD_LABELS[prefix];
                });

                if (missingBwx.length > 0) {
                    errors.push("BWX Status requires all 4 X-Ray uploads. Missing: " + missingBwx.join(", ") + ".");
                }
            }
        }

        const paStatus = $("#PaStatus").val();
        if (paStatus === "Completed") {
            const paCards = $(".pa-card:visible");
            if (paCards.length === 0) {
                errors.push("Periapical (PA) X-Rays Status requires at least one PA X-Ray image.");
            } else {
                const missingPa = [];
                paCards.each(function () {
                    const prefix = $(this).find(".xray-file").data("prefix");
                    if (prefix && !isUploadComplete(prefix)) {
                        missingPa.push("PA card " + (missingPa.length + 1));
                    }
                });
                if (missingPa.length > 0) {
                    errors.push("Periapical (PA) X-Rays Status requires an uploaded image on every PA card.");
                }
            }
        }

        return errors;
    }

    function isSectionDone(statusVal, reasonVal, uploadCheckFn) {
        if (!statusVal) return false;
        if (statusVal === "Not Completed") return !!reasonVal;
        if (statusVal === "Completed") return uploadCheckFn();
        return false;
    }

    function updateOverallStatus() {
        if (!$("#xrayStationSection").is(":visible")) {
            $("#StatusHidden").val("Pending");
            $("#Status").val("Pending");
            return;
        }

        let overallStatus = "Pending";

        if (cfg().bwxNeeded) {
            const bwxStatus = $("#BwxStatus").val();
            const paStatus = $("#PaStatus").val();

            if (bwxStatus && paStatus) {
                const bwxDone = isSectionDone(bwxStatus, $("#BwxReason").val(), isBwxUploadCompleteForStatus);
                const paDone = isSectionDone(paStatus, $("#PaReason").val(), function () {
                    return $(".pa-card:visible").length > 0 &&
                        $(".pa-card:visible").toArray().every(function (card) {
                            const prefix = $(card).find(".xray-file").data("prefix");
                            return isUploadComplete(prefix);
                        });
                });
                overallStatus = bwxDone && paDone ? "Completed" : "Pending";
            }
        }
        $("#StatusHidden").val(overallStatus);
        $("#Status").val(overallStatus);
    }

    function reindexPaCards() {
        $("#paCardsRow .pa-card").each(function (index) {
            const card = $(this);
            card.attr("data-index", index);
            card.find(".pa-sort-order").val(index);

            card.find("[name^='PaImages']").each(function () {
                const el = $(this);
                const name = el.attr("name");
                if (!name) return;
                el.attr("name", name.replace(/PaImages\[\d+\]/, "PaImages[" + index + "]"));
            });

            const prefix = "pa_" + index;
            card.find(".xray-file").attr("data-prefix", prefix).attr("onchange", "toggleXRayUploadDate(this, '" + prefix + "')");
            card.find(".open-xray-picker").attr("data-prefix", prefix);
            card.find(".preview-xray-file").attr("data-prefix", prefix);
            card.find(".remove-xray-file").attr("data-prefix", prefix);
            card.find(".xray-zoom-in, .xray-zoom-out, .xray-zoom-reset").attr("data-prefix", prefix);

            ["_fileStatus", "_uploaded", "_fileName", "_originalFileName", "_removed", "_uploadDate", "_fileNameDisplay",
             "_previewSection", "_previewViewport", "_previewImg", "_zoomLevel"].forEach(function (suffix) {
                const el = card.find("[id$='" + suffix + "']");
                if (el.length) el.attr("id", prefix + suffix);
            });

            updateXRayFileUI(prefix);
        });
        paCardIndex = $("#paCardsRow .pa-card").length;
        syncPaAddButton();
    }

    function buildPaCardHtml(index) {
        const prefix = "pa_" + index;
        return `
            <div class="card pa-card h-100 xray-upload-card" data-index="${index}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <h6 class="card-title mb-0">PA of tooth</h6>
                        <button type="button" class="btn btn-sm btn-outline-danger remove-pa-card" title="Remove">
                            <i class="fas fa-trash-alt" aria-hidden="true"></i>
                        </button>
                    </div>
                    <input type="hidden" name="PaImages[${index}].Id" value="0" />
                    <input type="hidden" name="PaImages[${index}].SortOrder" value="${index}" class="pa-sort-order" />
                    <input type="file" class="xray-file d-none pa-file" name="PaImages[${index}].ImageFile"
                           data-prefix="${prefix}" accept=".jpg,.jpeg,image/jpeg"
                           onchange="toggleXRayUploadDate(this, '${prefix}')" />
                    <button type="button" class="btn btn-outline-primary btn-sm mt-1 open-xray-picker" data-prefix="${prefix}">
                        <i class="fas fa-upload" aria-hidden="true"></i> Upload
                    </button>
                    <div class="align-items-center gap-2 mt-2 xray-file-status" id="${prefix}_fileStatus"
                         style="display:none;" data-download-prefix="pa_tooth">
                        <span id="${prefix}_fileNameDisplay" class="small text-muted"></span>
                        <button type="button" class="btn btn-sm btn-outline-secondary preview-xray-file" data-prefix="${prefix}">Preview</button>
                        <button type="button" class="btn btn-sm btn-outline-danger remove-xray-file" data-prefix="${prefix}">Remove</button>
                    </div>
                    <input type="hidden" id="${prefix}_uploaded" name="PaImages[${index}].Uploaded" value="false" />
                    <input type="hidden" id="${prefix}_fileName" name="PaImages[${index}].FileName" value="" />
                    <input type="hidden" id="${prefix}_originalFileName" name="PaImages[${index}].OriginalFileName" value="" />
                    <input type="hidden" id="${prefix}_removed" name="PaImages[${index}].Removed" value="false" />
                    <input name="PaImages[${index}].UploadedDateTime" id="${prefix}_uploadDate"
                           class="form-control flatpickr mt-2" type="text" style="display:none;" readonly />
                    <div class="xray-preview-panel mt-3" id="${prefix}_previewSection" style="display:none;">
                        <label class="form-label fw-semibold mb-1">Image Preview:</label>
                        <div class="xray-preview-viewport" id="${prefix}_previewViewport">
                            <img id="${prefix}_previewImg" alt="X-Ray preview" />
                        </div>
                        <label class="form-label fw-semibold mt-2 mb-1">Zoom Controls:</label>
                        <div class="d-flex flex-wrap gap-2 mb-2">
                            <button type="button" class="btn btn-sm btn-primary xray-zoom-in" data-prefix="${prefix}">
                                <i class="fas fa-search-plus" aria-hidden="true"></i> Zoom In
                            </button>
                            <button type="button" class="btn btn-sm btn-primary xray-zoom-out" data-prefix="${prefix}">
                                <i class="fas fa-search-minus" aria-hidden="true"></i> Zoom Out
                            </button>
                            <button type="button" class="btn btn-sm btn-outline-secondary xray-zoom-reset" data-prefix="${prefix}">
                                <i class="fas fa-undo" aria-hidden="true"></i> Reset
                            </button>
                        </div>
                        <p class="mb-0 small">Zoom Level: <span id="${prefix}_zoomLevel" class="text-primary fw-semibold">100%</span></p>
                    </div>
                </div>
            </div>`;
    }

    function initFlatpickrForUploadDates() {
        if (typeof flatpickr !== "function") return;
        document.querySelectorAll(".flatpickr").forEach(function (el) {
            if (!el._flatpickr) {
                flatpickr(el, {
                    altInput: true,
                    altFormat: "m/d/Y h:i K",
                    dateFormat: "Y-m-d H:i",
                    allowInput: false,
                    enableTime: true,
                    time_24hr: true
                });
            }
            el.setAttribute("readonly", "readonly");
        });
    }

    function bindEvents() {
        if (initialized) return;
        initialized = true;

        $(document).on("change", ".xray-file", function () {
            const prefix = $(this).data("prefix");
            if (!prefix) return;
            updateXRayFileUI(prefix);
            updateOverallStatus();
        });

        $(document).on("click", ".open-xray-picker", function () {
            const prefix = $(this).data("prefix");
            const input = document.querySelector(`.xray-file[data-prefix='${prefix}']`);
            if (input) input.click();
        });

        $(document).on("click", ".remove-xray-file", function () {
            const prefix = $(this).data("prefix");
            const input = document.querySelector(`.xray-file[data-prefix='${prefix}']`);
            const status = document.getElementById(prefix + "_fileStatus");
            const removedInput = document.getElementById(prefix + "_removed");
            const datePicker = document.getElementById(prefix + "_uploadDate");

            if (input) input.value = "";
            if (status?.dataset.existingFilename) {
                status.dataset.existingFilename = "";
                status.dataset.existingOriginalFilename = "";
            }
            if (removedInput) removedInput.value = "true";

            if (datePicker) {
                datePicker.value = "";
                setFlatpickrVisibility(datePicker, false);
                if (datePicker._flatpickr) datePicker._flatpickr.clear();
            }

            updateXRayFileUI(prefix);
            updateOverallStatus();
        });

        $(document).on("click", ".xray-zoom-in", function () {
            const prefix = $(this).data("prefix");
            setXRayZoom(prefix, (xrayZoomLevels[prefix] || 100) + XRAY_ZOOM_STEP);
        });

        $(document).on("click", ".xray-zoom-out", function () {
            const prefix = $(this).data("prefix");
            setXRayZoom(prefix, (xrayZoomLevels[prefix] || 100) - XRAY_ZOOM_STEP);
        });

        $(document).on("click", ".xray-zoom-reset", function () {
            const prefix = $(this).data("prefix");
            setXRayZoom(prefix, 100);
        });

        $(document).on("mousedown", ".xray-preview-viewport.can-pan", function (e) {
            if (e.button !== 0) return;
            e.preventDefault();
            startXRayPan(e.clientX, e.clientY, this);
        });

        $(document).on("mousemove", function (e) {
            if (!xrayPanState.active) return;
            e.preventDefault();
            moveXRayPan(e.clientX, e.clientY);
        });

        $(document).on("mouseup", function () {
            endXRayPan();
        });

        $(document).on("touchstart", ".xray-preview-viewport.can-pan", function (e) {
            if (!e.touches || e.touches.length !== 1) return;
            const touch = e.touches[0];
            startXRayPan(touch.clientX, touch.clientY, this);
        });

        $(document).on("touchmove", function (e) {
            if (!xrayPanState.active || !e.touches || e.touches.length !== 1) return;
            e.preventDefault();
            moveXRayPan(e.touches[0].clientX, e.touches[0].clientY);
        });

        $(document).on("touchend touchcancel", function () {
            endXRayPan();
        });

        $(document).on("click", ".preview-xray-file", function () {
            const prefix = $(this).data("prefix");
            const status = document.getElementById(prefix + "_fileStatus");
            const hiddenFileName = document.getElementById(prefix + "_fileName");
            if (!status || !hiddenFileName) return;

            if (status.dataset.previewMode === "selected") {
                const localUrl = status.dataset.selectedPreviewUrl;
                if (localUrl) window.open(localUrl, "_blank");
                return;
            }

            const fileName = hiddenFileName.value || status.dataset.existingFilename;
            const downloadPrefix = status.dataset.downloadPrefix || prefix;
            if (!fileName) return;

            window.open(buildDownloadUrl(downloadPrefix, fileName), "_blank");
        });

        $(document).on("click", "#addPaCardBtn", function () {
            if ($("#paCardsRow .pa-card").length >= MAX_PA_CARDS) {
                return;
            }

            const index = paCardIndex;
            $("#paCardsRow").append(buildPaCardHtml(index));
            paCardIndex++;
            initFlatpickrForUploadDates();
            updateOverallStatus();
            syncPaAddButton();
        });

        $(document).on("click", ".remove-pa-card", function () {
            $(this).closest(".pa-card").remove();
            reindexPaCards();
            updateOverallStatus();
        });

        $(document).on("click", "#goToVitalStationBtn", function () {
            if (!cfg().enableGoToVitalStation) return;
            goToVitalStationClicked = true;
            $("#GoToVitalStationFlag").val("true");
        });

        $(document).on("click", cfg().formSelector + " button[type='submit']:not(#goToVitalStationBtn)", function () {
            goToVitalStationClicked = false;
            $("#GoToVitalStationFlag").val("false");
        });

        $(document).on("submit", cfg().formSelector, function (e) {
            if (cfg().enableGoToVitalStation) {
                const submitter = e.originalEvent && e.originalEvent.submitter;
                if (isGoToVitalStationSubmit(submitter)) {
                    $("#GoToVitalStationFlag").val("true");
                    return true;
                }
            }

            goToVitalStationClicked = false;
            updateOverallStatus();

            const uploadErrors = validateXRayUploadsBeforeSave();
            if (uploadErrors.length > 0) {
                e.preventDefault();
                Swal.fire({
                    icon: "error",
                    title: "Missing X-Ray Uploads",
                    html: uploadErrors.join("<br>"),
                    confirmButtonText: "OK",
                    confirmButtonColor: "#FF0000",
                    iconColor: "#FF0000"
                });
                return false;
            }
        });
    }

    function init(options) {
        config = Object.assign({}, DEFAULTS, options || {}, window.DentalXRayStationConfig || {});
        paCardIndex = config.paCardIndex || 1;

        initFlatpickrForUploadDates();

        BWX_UPLOAD_PREFIXES.forEach(updateXRayFileUI);
        updateXRayFileUI("bwx_consolidated");
        $(".pa-card .xray-file").each(function () {
            updateXRayFileUI($(this).data("prefix"));
        });

        if ($("#BwxReason").length && config.initialBwxReason) {
            $("#BwxReason").val(config.initialBwxReason);
        }
        if ($("#PaReason").length && config.initialPaReason) {
            $("#PaReason").val(config.initialPaReason);
        }

        togglePregnancyFlow();
        toggleBwxSection();
        toggleReasonAndUpload("#PaStatus", "#paReasonContainer", "#paUploadContainer", "#PaReason");
        updateOverallStatus();
        syncPaAddButton();

        $("#AreYouPregnant, #PregnancyApproval").off("change.dentalXRayUploads").on("change.dentalXRayUploads", function () {
            togglePregnancyFlow();
            updateOverallStatus();
        });

        $("#BwxStatus").off("change.dentalXRayUploads").on("change.dentalXRayUploads", function () {
            toggleBwxSection();
            updateOverallStatus();
        });
        $("input[name='BwxUploadMode']").off("change.dentalXRayUploads").on("change.dentalXRayUploads", function () {
            toggleBwxUploadContainers();
            updateOverallStatus();
        });
        $("#PaStatus").off("change.dentalXRayUploads").on("change.dentalXRayUploads", function () {
            toggleReasonAndUpload("#PaStatus", "#paReasonContainer", "#paUploadContainer", "#PaReason");
            updateOverallStatus();
        });
        $("#BwxReason, #PaReason").off("change.dentalXRayUploads").on("change.dentalXRayUploads", updateOverallStatus);

        bindEvents();
    }

    window.toggleXRayUploadDate = toggleXRayUploadDate;
    window.DentalXRayStationUploads = {
        init: init,
        togglePregnancyFlow: togglePregnancyFlow,
        updateOverallStatus: updateOverallStatus
    };
})(window, jQuery);
