function displayMessage(selector) {
    const message = document.querySelector(selector);
    if (message) {



        message.style.display = 'block';
        setTimeout(function () {
            message.style.display = 'none';
        }, 20000); // Adjust the time (in milliseconds) to display the message
    }
    // if (document.getElementById('MsgId').value == "File Uploaded Success") {
    if (document.getElementById('MsgId').value.includes("Success")) {
       // document.getElementById('generateBtn').removeAttribute('disabled');
    }

    setTimeout(function () {
        HideGrid()
    }, 20000);
}
function generateSessionId() {
    return uuidv4(); // Generate a UUID v4
}


var sessionId = "";
function SetSession() {

     sessionId = generateSessionId();
    $.ajax({
        type: 'POST',
        url: '/Home/SetSession',
        data: { sessionId: sessionId },
        success: function (response) {
            sessionId = response;
            console.log('Session ID sent to the server successfully');
        },
        error: function (error) {
            console.error('Error sending session ID to the server', error);
        }
    });
}




$(document).ready(function () {
    SetSession();
})




document.getElementById('uploadForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent default form submission

    document.getElementById('loader').style.display = 'block';

    // Simulating a delay before form submission for demonstration purposes (you may remove this)
    setTimeout(function () {
        event.target.submit(); // Submit the form after a slight delay
    }, 500); // Adjust the delay time as needed
});

document.getElementById('generateForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent default form submission

    document.getElementById('loader').style.display = 'block';

    // Simulating a delay before form submission for demonstration purposes (you may remove this)
    setTimeout(function () {
        event.target.submit(); // Submit the form after a slight delay
    }, 500); // Adjust the delay time as needed
});

//let percentageValue = 0;
//setInterval(function () {
//    percentageValue = (percentageValue + 10) % 101; // Increment percentage value (capped at 100)
//    document.querySelector('.percentage').textContent = percentageValue + '%'; // Update text content
//}, 1000); // Change the timing as needed
//// Call the function to display success and error messages
displayMessage('.message.success');
displayMessage('.message.error');



function displayFileName(input) {
    const fileNameDisplay = document.getElementById('fileNameDisplay');
    if (input.files.length > 0) {
        fileNameDisplay.value = input.files[0].name;
        const fileSize = (input.files[0].size/1024 / 1024).toFixed(2);
        $('#fileSizeDisplay').val(`${fileSize} MB`);
    } else {
        fileNameDisplay.value = '';
    }
}

 
//$('#uploadBtn').click(function () {

//    var Size = $('#fileSizeDisplay').val().split(' ');

//    if (parseInt(Size[0]) > 20) {
//        alert("The file size exceeds 20 MB.");
//        return false;
//    }
//});
function loadData() {
    fetch('/Home/GetAllCsvFiles')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // Assuming the response is JSON data
        })
        .then(data => {
            // Process the retrieved data
            console.log('Data from server:', data);

            var totalRows = 0;
            var totalFileSize = 0;

            for (var i = 0; i < data.length; i++) {
                var mb = data[i].fileSizeBytes / 1024;
                var kb = data[i].fileSizeBytes;
                var Kb_Mb = (data[i].fileSizeBytes > 1024 ? mb + "MB" : kb + "KB");

                // Populate the table body
                $('#Tbody').append(
                    "<tr>" +
                    "<td>" + data[i].fileName + "</td>" +
                    "<td>" + data[i].rowCount + "</td>" +
                    "<td>" + Kb_Mb  + "</td>" +
                    "<td><a href='#' class='download-btn' onclick='downloadFile(\"" + data[i].fileName + "\")'>Download</a></td>" +
                    "</tr>"
                );

                // Calculate totals
                totalRows += data[i].rowCount;
                totalFileSize += data[i].fileSizeBytes;
            }

            // Display totals in the table footer
            $('#tfoot').html(
                "<tr>" +
                "<td>Total:&nbsp;" + data.length+"</td>" +
                "<td>" + totalRows + "</td>" +
                "<td>" + (totalFileSize > 1024 ? (totalFileSize / 1024).toFixed(2) + "MB" : totalFileSize.toFixed(2) + "KB") + "</td>" +
                "<td><a href='#' class='download-btn' onclick='downloadZipFile()' style='color: white; background-color: #3498db; padding: 5px 10px; border-radius: 5px;'>Download all</a></td>" + // Empty cell for download column in footer
                "</tr>"
            );
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}


// Call the function to load data when needed
loadData();



function downloadFile(fileName) {
    fetch(`/Home/DownloadFile?fileName=${fileName}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.blob(); 
        })
        .then(blob => {

            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = fileName; 
            link.click();


            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

downloadFile('YourFileName.csv');




function deleteAllFiles(file) {
    fetch(`/Home/DeleteAllFile?record=${file}`, {
        method: 'POST', 
        headers: {
            'Content-Type': 'application/json' 
        },
    })
        .then(response => {
            if (response.ok) {
                console.log('Files deleted successfully');
            } else {
                console.error('Failed to delete files');
            }
            location.reload();
        })
        .catch(error => {
            console.error('Error occurred:', error);
        });
}

function openModal() {
    var modal = document.getElementById('confirmationModal');
    modal.style.display = 'block';
}

function closeModal() {
    var modal = document.getElementById('confirmationModal');
    modal.style.display = 'none';
}

 


function downloadZipFile() {

    if ($('#Tbody tr').length == 0) {
        return false;
    }

    var currentDate = new Date();
    var formattedDate = currentDate.toISOString().slice(0, 19).replace(/[-T:/]/g, ''); // Format the date

    var fileName = 'zip_' + formattedDate; // Concatenate 'zip_' with the formatted date
    var link = document.createElement('a');
    link.href = '/Home/DownloadZipFile?fileName=' + fileName;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.setAttribute('download', fileName + '.zip'); // Set the download attribute with the filename
    link.click();
    document.body.removeChild(link);
}



function HideGrid() {

    $('.HideGrid').hide();

}