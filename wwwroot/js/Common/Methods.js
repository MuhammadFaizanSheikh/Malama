function getDateOnlyGlobal(dateTimeString) {
    if (!dateTimeString) return "";

    // Works for: "YYYY-MM-DD" or "YYYY-MM-DDTHH:mm:ss"
    return dateTimeString.split('T')[0];
}

function getyyyymmddDateGlobalAgainstDate(date) {
    if (!date) return "";
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0"); // months 0-11
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`; // YYYY-MM-DD
}


function formatDateToMMDDYYYYGlobal(dateStr) {
    const [y, m, d] = dateStr.split('-');
    return `${m}/${d}/${y}`; // MM/DD/YYYY
}

function formatDateToYYYYMMDDGlobal(dateString) {
    const [month, day, year] = dateString.split('/');
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
}

function formatDateTimeToMMDDYYYY_HHMMSSGlobal(date) {
    const pad = (num) => num.toString().padStart(2, '0');

    const month = pad(date.getMonth() + 1);  // Months are zero-based
    const day = pad(date.getDate());
    const year = date.getFullYear();

    const hours = pad(date.getHours());       // 24-hour format by default
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    return `${month}/${day}/${year} ${hours}:${minutes}:${seconds}`;
}

function formatDateTimeStringToMMDDYYYY_HHMMSS(dateString) {
    if (!dateString) return "";

    const [datePart, timePart] = dateString.split('T');
    if (!datePart || !timePart) return dateString; // fallback

    const [year, month, day] = datePart.split('-');
    const [hour, minute, second] = timePart.split(':');

    return `${month}/${day}/${year} ${hour}:${minute}:${second}`;
}

function getCurrentDateTimeNumericLocal() {
    const now = new Date();

    const YYYY = now.getFullYear();
    const MM = String(now.getMonth() + 1).padStart(2, '0');
    const DD = String(now.getDate()).padStart(2, '0');
    const hh = String(now.getHours()).padStart(2, '0');
    const mm = String(now.getMinutes()).padStart(2, '0');
    const ss = String(now.getSeconds()).padStart(2, '0');

    return `${YYYY}${MM}${DD}${hh}${mm}${ss}`; // Format: YYYYMMDDHHMMSS
}




