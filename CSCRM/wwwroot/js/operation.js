
window.onload = function () {
    if (performance.navigation.type === 2) {
        location.reload();
    }
};





function handleKeyUp(event) {
    const input = document.getElementById('client-searchInput').value;

    // Enter tuşuna basılmışsa
    if (event.key === 'Enter') {
        if (input.length >= 4) {
            searchClients();


        }

        return;
    }

}

function searchClients() {
    const code = document.getElementById('client-searchInput').value;
    fetch(`/operation/client/GetClientByMailOrInvCode?code=${code}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.text())
        .then(data => {
            document.getElementById('clients-page-content').innerHTML = data;
            document.getElementById('client-searchInput').value = code

        });

}







function deleteTourOrder(tourOrderId, clientId) {
    const confirmDelete = confirm('Are you sure you want to delete this client?');
    if (confirmDelete) {
        fetch(`/operation/client/DeleteTourOrderOfClient?clientId=${clientId}&tourOrderId=${tourOrderId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                document.getElementById('tour-service-section').innerHTML = data;


            });
    }


}


function addNewTourOrder(clientId) {
    var tourId = document.getElementById('newTourName').value;
    var carType = document.getElementById('newCarTypeName').value;
    var guide = document.getElementById('newGuide').value;
    var date = document.getElementById('newTourDate').value;

    if (!tourId || !carType || !guide || !date) {
        alert('Please Fill All Inputs.');
        return;
    }

    var tourOrder = {
        ClientId: clientId,
        TourId: parseInt(tourId),
        CarType: carType,
        Date: date,
        Guide: guide === 'True'
    };


    fetch('/operation/client/AddNewTourOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(tourOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('tour-service-section').innerHTML = data


        });



}


function deleteRestaurantOrder(restaurantOrderId, clientId) {
    const confirmDelete = confirm('Are you sure you want to delete this client?');
    if (confirmDelete) {
        fetch(`/operation/client/DeleteRestaurantOrderOfClient?clientId=${clientId}&restaurantOrderId=${restaurantOrderId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                document.getElementById('restaurant-service-section').innerHTML = data;


            });
    }


}



function addNewRestaurantOrder(clientId) {
    var restaurantName = document.getElementById('add-new-restaurantOrder-restName').value;
    var mealType = document.getElementById('add-new-restaurantOrder-mealType').value;
    var count = document.getElementById('add-new-restaurantOrder-count').value;
    var date = document.getElementById('add-new-restaurantOrder-date').value;

    if (!restaurantName || !mealType || !count || !date) {
        alert('Please fill all inputs.');
        return;
    }

    var restaurantOrder = {
        ClientId: clientId,
        RestaurantName: restaurantName,
        MealType: mealType,
        Count: parseInt(count),
        Date: date
    };

    fetch('/operation/client/AddNewRestaurantOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(restaurantOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('restaurant-service-section').innerHTML = data


        });


}









//PDF UPLOADER
//PDF UPLOADER
//PDF UPLOADER
//PDF UPLOADER



function downloadPDF() {
    removeButtons();





    const { jsPDF } = window.jspdf;
    // Input değerlerini güncelle


    html2canvas(document.getElementById('section-to-print'), { allowTaint: true, useCORS: true }).then(canvas => {
        html2canvas(document.getElementById('textareaId'), {
            scale: 2,
            logging: true,
            letterRendering: 1,
            allowTaint: false
        }).then(function (canvas) {
            var imgData = canvas.toDataURL('image/png');
            var pdf = new jsPDF('p', 'mm', 'a4');
            pdf.addImage(imgData, 'PNG', 10, 10);
            pdf.save("output.pdf");
        });




        const pdf = new jsPDF('p', 'mm', 'a4');
        const imgData = canvas.toDataURL('image/png');
        const imgWidth = 210; // A4 width in mm
        const pageHeight = 297; // A4 height in mm
        const imgHeight = canvas.height * imgWidth / canvas.width;
        let heightLeft = imgHeight;
        let position = 0;

        pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;

        while (heightLeft >= 0) {
            position = heightLeft - imgHeight;
            pdf.addPage();
            pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
            heightLeft -= pageHeight;
        }

        pdf.save("download.pdf");

        // Input değerlerini geri yükle
        document.querySelectorAll('input').forEach(input => {
            input.style.visibility = 'visible';
            const span = input.parentElement.querySelector('span');
            if (span) {
                span.remove();
            }
        });
    });
}



function removeBQ(orik) {
    const element = document.getElementById(orik);
    console.log(orik)

    if (element) {
        element.parentNode.removeChild(element);
    } else {
        console.log('Element with id "5" not found.');
    }
}

function removeButtons() {
    const elements = document.getElementsByClassName('voucher-remove-button');
    console.log(elements)


    if (elements) {
        for (let element of elements) {

            element.style.visibility = "hidden";
        }

    } else {
        console.log('Element with id "5" not found.');
    }
}




