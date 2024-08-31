
window.onload = function () {
    if (performance.navigation.type === 2) {
        location.reload();
    }
};

function addHotel() {
    const name = document.getElementById('add-hotel-name-input').value;
    const singlePrice = document.getElementById('add-hotel-snglprc-input').value;
    const doublePrice = document.getElementById('add-hotel-dblprc-input').value;
    const triplePrice = document.getElementById('add-hotel-trplprc-input').value;
    const contactName = document.getElementById('add-hotel-contactName-input').value;
    const contactPhone = document.getElementById('add-hotel-contactPhone-input').value;
    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/hotel/AddNewHotel', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
            SinglePrice: parseFloat(singlePrice),
            DoublePrice: parseFloat(doublePrice),
            TriplePrice: parseFloat(triplePrice),
            ContactPerson: contactName,
            ContactNumber: contactPhone
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {
            console.log(data)
            document.getElementById('hotels-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}

function deleteHotel(hotelId) {
    console.log(hotelId);
    const confirmDelete = confirm('Are you sure you want to delete this hotel?');
    if (confirmDelete) {
        console.log(hotelId);
        fetch(`/sale/hotel/DeleteHotel?hotelId=${hotelId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                console.log(data);
                document.getElementById('hotels-page-content').innerHTML = data;

                /*$('hotels-page-content').html(data)*/
            });
    }
}

function updateHotel(Id) {
    console.log(Id)
    const name = document.getElementById('editHotelNameInput').value;
    const singlePrice = document.getElementById('editHotelSnglPriceInput').value;
    const doublePrice = document.getElementById('editHotelDblPriceInput').value;
    const triplePrice = document.getElementById('editHotelTrplPriceInput').value;
    const contactName = document.getElementById('editHotelContPersonInput').value;
    const contactPhone = document.getElementById('editHotelContNumInput').value;
    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/hotel/EditHotel', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            SinglePrice: parseFloat(singlePrice),
            DoublePrice: parseFloat(doublePrice),
            TriplePrice: parseFloat(triplePrice),
            ContactPerson: contactName,
            ContactNumber: contactPhone
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {
            console.log(data)
            document.getElementById('hotel-edit-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}





function deleteCompany(companyId) {
    const confirmDelete = confirm('Are you sure you want to delete this company?');
    if (confirmDelete) {
        fetch(`/sale/company/DeleteCompany?companyId=${companyId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                console.log(data);
                document.getElementById('companies-page-content').innerHTML = data;

                /*$('hotels-page-content').html(data)*/
            });
    }
}

function addCompany() {
    const name = document.getElementById('add-companyName-input').value;
    const contactName = document.getElementById('add-company-contactName-input').value;
    const address = document.getElementById('add-company-address-input').value;
    const phoneNumber = document.getElementById('add-company-phone-input').value;
    const email = document.getElementById('add-company-Email-input').value;

    if (!name) {
        alert('Name field is required.');
        return;
    }

    fetch('/sale/company/addnewcompany', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
            ContactPerson: contactName,
            Address: address,
            Phone: phoneNumber,
            Email: email
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('companies-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}

function editCompany(Id) {
    const name = document.getElementById('company-edit-name-input').value;
    const contactName = document.getElementById('company-edit-contactName-input').value;
    const address = document.getElementById('company-edit-address-input').value;
    const email = document.getElementById('company-edit-email-input').value;
    const phone = document.getElementById('company-edit-phone-input').value;

    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/company/EditCompany', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            ContactPerson: contactName,
            Address: address,
            Email: email,
            Phone: phone
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('company-edit-page-content').innerHTML = data


        });
}









let itineraryCount = 1;

function addItineraryInput() {
    const itineraryInputs = document.getElementById('add-itineraryInputs');
    const input = document.createElement('input');
    input.type = 'text';
    input.name = `itinerary${itineraryCount}`;
    input.placeholder = 'Itinerary';
    input.className = 'tours-input';
    itineraryInputs.appendChild(input);
    itineraryCount++;
}

function addTour() {
    const name = document.getElementById('add-tourName-input').value;
    const itineraries = [];

    for (let i = 1; i < itineraryCount; i++) {
        const itineraryInput = document.getElementsByName(`itinerary${i}`)[0];
        if (itineraryInput.value.trim() !== '') {
            itineraries.push(itineraryInput.value.trim());
        }
    }

    console.log(itineraries)

    if (!name || itineraries.length === 0) {
        alert('All fields are required.');
        return;
    }

    itineraryCount = 1;

    fetch('/sale/tour/AddNewTour', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
            Itineraries: itineraries
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('tours-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}

function deleteTour(tourId) {
    const confirmDelete = confirm('Are you sure you want to delete this tour?');
    if (confirmDelete) {
        fetch(`/sale/Tour/DeleteTour?tourId=${tourId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                console.log(data);
                document.getElementById('tours-page-content').innerHTML = data;

                /*$('hotels-page-content').html(data)*/
            });
    }
}







let editItineraryCount = -3;

edit_itinerary_inputs = document.getElementsByClassName('tour-edit-page-itinerary-input')
for (let input in edit_itinerary_inputs) {
    editItineraryCount += 1;
}



function addEditItineraryInput() {
    const itineraryInputs = document.getElementById('edit-itineraryInputs');
    const input = document.createElement('input');
    input.type = 'text';
    input.name = `edit-itinerary-${editItineraryCount}`;
    input.placeholder = 'Itinerary';
    input.className = 'tour-edit-page-input';
    itineraryInputs.appendChild(input);

    editItineraryCount++;
}

function updateTour(Id) {
    const name = document.getElementById('edit-tourName-input').value;
    const itineraries = [];

    for (let i = 0; i < editItineraryCount; i++) {
        const itineraryInput = document.getElementsByName(`edit-itinerary-${i}`)[0];
        if (itineraryInput && itineraryInput.value.trim() !== '') {
            itineraries.push(itineraryInput.value.trim());
        }
    }
    console.log(itineraries)

    if (!name || itineraries.length === 0) {
        alert('All fields are required.');
        return;
    }

    console.log(name, itineraries);

    //editItineraryCount = -3;

    fetch('/sale/tour/EditTour', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            Itineraries: itineraries
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('tour-edit-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });

}






//function addCarType() {
//    const name = document.getElementById('add-carTypeName-input').value;
//    const capacity = document.getElementById('add-carTypeCapacity-input').value;

//    if (!name || !capacity) {
//        alert('All fields are required.');
//        return;
//    }



//    // Burada eklenti için gerekli API isteği yapılabilir, fetch kullanılabilir.
//    fetch('/manage/car/AddNewCar', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify({
//            Name: name,
//            Capacity: parseInt(capacity)
//        })
//    })
//        .then(res => {
//            return res.text();
//        })
//        .then(data => {

//            document.getElementById('cartypes-page-content').innerHTML = data

//            /*$('hotels-page-content').html(data)*/
//        });
//}

//function deleteCar(carId) {
//    const confirmDelete = confirm('Are you sure you want to delete this car?');
//    if (confirmDelete) {
//        fetch(`/manage/car/DeleteCar?carId=${carId}`, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json'
//            }
//        })
//            .then(res => res.text())
//            .then(data => {
//                console.log(data);
//                document.getElementById('cartypes-page-content').innerHTML = data;

//                /*$('hotels-page-content').html(data)*/
//            });
//    }
//}


//function editCar(Id) {
//    const name = document.getElementById('car-edit-name-input').value;
//    const capacity = document.getElementById('car-edit-capacity-input').value;


//    if (!name) {
//        alert('Name field cannot be empty.');
//        return;
//    }

//    fetch('/manage/car/EditCar', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify({
//            Id: Id,
//            Name: name,
//            Capacity: capacity
//        })
//    })
//        .then(res => {
//            return res.text();
//        })
//        .then(data => {

//            document.getElementById('car-type-edit-page-content').innerHTML = data


//        });
//}



function UpdateTourCar(tourName) {
    console.log(tourName)
    const inputs = document.getElementsByClassName('tourCarType-edit-page-input')
    const carPrices = {};

    for (let input of inputs) {
        console.log(input.name)
        console.log(input.value)
        carPrices[input.name] = parseFloat(input.value);
    }

    const editTourCarVM = {
        TourName: tourName,
        CarPrices: carPrices
    };

    console.log(editTourCarVM)

    fetch('/sale/TourCar/EditTourCar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(editTourCarVM)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('tourCarType-edit-page-content').innerHTML = data


        });
}





function AddInclusiveService() {
    const Name = document.getElementById('add-inclusive-service-name-input').value;
    const Price = document.getElementById('add-inclusive-service-price-input').value;

    if (!Name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/inclusive/AddNewInclusive', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: Name,
            Price: parseFloat(Price),

        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {
            console.log(data)
            document.getElementById('inclusive-services-page-content').innerHTML = data


        });
}

function deleteInclusiveService(inclusiveId) {

    const confirmDelete = confirm('Are you sure you want to delete this service?');
    if (confirmDelete) {

        fetch(`/sale/inclusive/DeleteInclusive?inclusiveId=${inclusiveId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {

                document.getElementById('inclusive-services-page-content').innerHTML = data;


            });
    }
}

function updateInclusive(Id) {
    const name = document.getElementById('editInclusiveNameInput').value;
    const price = document.getElementById('editInclusivePriceInput').value;

    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/inclusive/EditInclusive', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            Price: parseFloat(price),

        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {
            console.log(data)
            document.getElementById('inclusive-service-edit-page-content').innerHTML = data


        });
}




function addRestaurant() {
    const name = document.getElementById('add-restaurant-name-input').value;
    const lunch = document.getElementById('add-restaurant-lunch-input').value;
    const dinner = document.getElementById('add-restaurant-dinner-input').value;
    const galaSimple = document.getElementById('add-restaurant-gala-simple-input').value;
    const galaLocal = document.getElementById('add-restaurant-gala-local-input').value;
    const galaForeign = document.getElementById('add-restaurant-gala-foreign-input').value;
    const takeaway = document.getElementById('add-restaurant-takeaway-input').value;
    const contactName = document.getElementById('add-restaurant-contactName-input').value;
    const contactPhone = document.getElementById('add-restaurant-contactPhone-input').value;

    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/restaurant/AddNewRestaurant', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
            Lunch: parseFloat(lunch),
            Dinner: parseFloat(dinner),
            Gala_Dinner_Simple: parseFloat(galaSimple),
            Gala_Dinner_Local_Alc: parseFloat(galaLocal),
            Gala_Dinner_Foreign_Alc: parseFloat(galaForeign),
            TakeAway: parseFloat(takeaway),
            ContactPerson: contactName,
            ContactPhone: contactPhone
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('restaurants-page-content').innerHTML = data
        });
}
function deleteRestaurant(id) {
    const confirmDelete = confirm('Are you sure you want to delete this restaurant?');
    if (confirmDelete) {
        fetch(`/sale/Restaurant/DeleteRestaurant?restaurantId=${id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                console.log(data);
                document.getElementById('restaurants-page-content').innerHTML = data;


            });
    }
}


function updateRestaurant(Id) {
    const name = document.getElementById('editRestaurantNameInput').value;
    const lunch = document.getElementById('editRestaurantLunchInput').value;
    const dinner = document.getElementById('editRestaurantDinnerInput').value;
    const galaSimple = document.getElementById('editRestaurantGalaSimpleInput').value;
    const galaLocal = document.getElementById('editRestaurantGalaLocalInput').value;
    const galaForeign = document.getElementById('editRestaurantGalaForeignInput').value;
    const takeaway = document.getElementById('editRestaurantTakeawayInput').value;
    const contactName = document.getElementById('editRestaurantContPersonInput').value;
    const contactPhone = document.getElementById('editRestaurantContNumInput').value;

    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/sale/restaurant/EditRestaurant', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            Lunch: parseFloat(lunch),
            Dinner: parseFloat(dinner),
            Gala_Dinner_Simple: parseFloat(galaSimple),
            Gala_Dinner_Local_Alc: parseFloat(galaLocal),
            Gala_Dinner_Foreign_Alc: parseFloat(galaForeign),
            TakeAway: parseFloat(takeaway),
            ContactPerson: contactName,
            ContactPhone: contactPhone

        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {
            console.log(data)
            document.getElementById('restaurant-edit-page-content').innerHTML = data


        });
}




function addClient() {

    // Form alanlarındaki değerleri al
    const invoiceCode = document.getElementById('add-invoiceCode-input').value;
    const mailCode = document.getElementById('add-mailCode-input').value;
    const clientName = document.getElementById('add-clientName-input').value;
    const clientSurname = document.getElementById('add-clientSurname-input').value;
    const salesAmount = parseFloat(document.getElementById('add-salesAmount-input').value);
    const received = parseFloat(document.getElementById('add-received-input').value);
    const paymentSituation = document.getElementById('add-paymentSituation-select').value;
    const visaSituation = document.getElementById('add-visaSituation-select').value;
    const country = document.getElementById('add-country-input').value;
    const company = document.getElementById('add-company-input').value;
    const arrivalDate = document.getElementById('add-arrivalDate-input').value;
    const departureDate = document.getElementById('add-departureDate-input').value;
    const arrivalTime = document.getElementById('add-arrivalTime-input').value;
    const departurTime = document.getElementById('add-departureTime-input').value;
    const arrivalFlight = document.getElementById('add-arrivalFlight-input').value;
    const departureFlight = document.getElementById('add-departureFlight-input').value;
    const carType = document.getElementById('add-carType-input').value;
    const paxsSize = document.getElementById('add-paxsSize-input').value;

    if (!invoiceCode) {
        alert('Invoice Code cannot be empty.');
        return;
    }
    if (!mailCode) {
        alert('Mail Code cannot be empty.');
        return;
    }
    if (!clientName) {
        alert('Client Name cannot be empty.');
        return;
    }
    if (!clientSurname) {
        alert('Client Surname cannot be empty.');
        return;
    }

    console.log(departureDate)

    // Yeni bir Client nesnesi oluştur
    const newClient = {
        InvCode: invoiceCode,
        MailCode: mailCode,
        Name: clientName,
        Surname: clientSurname,
        SalesAmount: salesAmount,
        Received: received,
        PaySituation: paymentSituation,
        VisaSituation: visaSituation,
        Country: country,
        Company: company,
        ArrivalDate: arrivalDate,
        DepartureDate: departureDate,
        ArrivalTime: arrivalTime,
        DepartureTime: departurTime,
        ArrivalFlight: arrivalFlight,
        DepartureFlight: departureFlight,
        CarType: carType,
        PaxSize: paxsSize
    };

    // Sunucuya POST isteği gönder
    fetch('/sale/client/addclient', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newClient)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('clients-page-content').innerHTML = data


        });
}

function deleteClient(id) {
    const confirmDelete = confirm('Are you sure you want to delete this client?');
    if (confirmDelete) {
        fetch(`/sale/client/DeleteClient?clientId=${id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                console.log(data);
                document.getElementById('clients-page-content').innerHTML = data;


            });
    }
}

function updateClientInfo(clientId) {
    const invoiceCode = document.getElementById('edit-invoiceCode-input').value;
    const mailCode = document.getElementById('edit-mailCode-input').value;
    const name = document.getElementById('edit-clientName-input').value;
    const surname = document.getElementById('edit-clientSurname-input').value;
    const salesAmount = document.getElementById('edit-salesAmount-input').value;
    const received = document.getElementById('edit-received-input').value;
    const paymentSituation = document.getElementById('edit-paymentSituation-select').value;
    const visaSituation = document.getElementById('edit-visaSituation-select').value;
    const country = document.getElementById('edit-country-input').value;
    const company = document.getElementById('edit-company-input').value;
    const arrivalDate = document.getElementById('edit-arrivalDate-input').value;
    const departureDate = document.getElementById('edit-departureDate-input').value;
    const arrivalTime = document.getElementById('edit-arrivalTime-input').value;
    const departurTime = document.getElementById('edit-departureTime-input').value;
    const arrivalFlight = document.getElementById('edit-arrivalFlight-input').value;
    const departureFlight = document.getElementById('edit-departureFlight-input').value;
    const paxSize = document.getElementById('edit-clientPaxSize-input').value;
    const carType = document.getElementById('edit-clientCarType-select').value;

    if (!invoiceCode) {
        alert('Invoice Code cannot be empty.');
        return;
    }
    if (!mailCode) {
        alert('Mail Code cannot be empty.');
        return;
    }
    if (!name) {
        alert('Client Name cannot be empty.');
        return;
    }
    if (!surname) {
        alert('Client Surname cannot be empty.');
        return;
    }

    fetch('/sale/client/EditClientInfo', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: clientId,
            InvCode: invoiceCode,
            MailCode: mailCode,
            Name: name,
            Surname: surname,
            SalesAmount: salesAmount,
            Received: received ? received : 0,
            PaySituation: paymentSituation,
            VisaSituation: visaSituation,
            Country: country,
            Company: company,
            ArrivalDate: arrivalDate,
            DepartureDate: departureDate,
            ArrivalTime: arrivalTime,
            DepartureTime: departurTime,
            ArrivalFlight: arrivalFlight,
            DepartureFlight: departureFlight,
            CarType: carType,
            PaxSize: paxSize
        })
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('edit-client-info-page-content').innerHTML = data
        });
}

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
    fetch(`/sale/client/GetClientByMailOrInvCode?code=${code}`, {
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



function deleteHotelOrder(hotelOrderId, clientId) {
    const confirmDelete = confirm('Are you sure you want to delete this order?');
    if (confirmDelete) {
        fetch(`/sale/client/DeleteHotelOrderOfClient?clientId=${clientId}&hotelOrderId=${hotelOrderId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                document.getElementById('hotel-service-section').innerHTML = data;


            });
    }


}

function addNewHotelOrder(clientId) {
    var clientNameSurname = document.getElementById('clientNameSurnameForHotelOrder').value;
    var hotelName = document.getElementById('add-hotel-order-hotelName').value;
    var roomType = document.getElementById('add-hotel-order-roomType').value;
    var roomCount = document.getElementById('add-hotel-order-roomCount').value;
    var days = document.getElementById('add-hotel-order-days').value;
    var fromDate = document.getElementById('add-hotel-order-dateFrom').value;
    var toDate = document.getElementById('add-hotel-order-dateTo').value;

    if (!hotelName || !roomType || !roomCount || !days || !fromDate || !toDate) {
        alert('Please Fill All Inputs.');
        return;
    }

    var hotelOrder = {
        ClientNameSurname: clientNameSurname,
        ClientId: clientId,
        HotelName: hotelName,
        RoomCount: parseInt(roomCount),
        Days: parseInt(days),
        RoomType: roomType,
        FromDate: fromDate,
        ToDate: toDate
    };


    fetch('/sale/client/AddNewHotelOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(hotelOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('hotel-service-section').innerHTML = data


        });


}


function deleteTourOrder(tourOrderId, clientId) {
    const confirmDelete = confirm('Are you sure you want to delete this order?');
    if (confirmDelete) {
        fetch(`/sale/client/DeleteTourOrderOfClient?clientId=${clientId}&tourOrderId=${tourOrderId}`, {
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


    fetch('/sale/client/AddNewTourOrder', {
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


//function deleteRestaurantOrder(restaurantOrderId, clientId) {
//    const confirmDelete = confirm('Are you sure you want to delete this client?');
//    if (confirmDelete) {
//        fetch(`/sale/client/DeleteRestaurantOrderOfClient?clientId=${clientId}&restaurantOrderId=${restaurantOrderId}`, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json'
//            }
//        })
//            .then(res => res.text())
//            .then(data => {
//                document.getElementById('restaurant-service-section').innerHTML = data;


//            });
//    }


//}



//function addNewRestaurantOrder(clientId) {
//    var restaurantName = document.getElementById('add-new-restaurantOrder-restName').value;
//    var mealType = document.getElementById('add-new-restaurantOrder-mealType').value;
//    var count = document.getElementById('add-new-restaurantOrder-count').value;
//    var date = document.getElementById('add-new-restaurantOrder-date').value;

//    if (!restaurantName || !mealType || !count || !date) {
//        alert('Lütfen tüm alanları doldurun.');
//        return;
//    }

//    var restaurantOrder = {
//        ClientId: clientId,
//        RestaurantName: restaurantName,
//        MealType: mealType,
//        Count: parseInt(count),
//        Date: date
//    };

//    fetch('/sale/client/AddNewRestaurantOrder', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify(restaurantOrder)
//    })
//        .then(res => {
//            return res.text();
//        })
//        .then(data => {

//            document.getElementById('restaurant-service-section').innerHTML = data


//        });


//}



function deleteInclusiveOrder(inclusiveOrderId, clientId) {
    const confirmDelete = confirm('Are you sure you want to delete this order?');
    if (confirmDelete) {
        fetch(`/sale/client/DeleteInclusiveOrderOfClient?clientId=${clientId}&inclusiveOrderId=${inclusiveOrderId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(res => res.text())
            .then(data => {
                document.getElementById('inclusive-service-section').innerHTML = data;


            });
    }


}



function addNewInclusiveOrder(clientId) {
    var inclusiveName = document.getElementById('add-new-inclusiveOrder-inclusiveName').value;
    var count = document.getElementById('add-new-inclusiveOrder-count').value;
    var date = document.getElementById('add-new-inclusiveOrder-date').value;

    if (!inclusiveName || !count || !date) {
        alert('Please fill all inputs.');
        return;
    }

    var inclusiveOrder = {
        ClientId: clientId,
        InclusiveName: inclusiveName,
        Count: parseInt(count),
        Date: date
    };

    fetch('/sale/client/AddNewInclusiveOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(inclusiveOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('inclusive-service-section').innerHTML = data
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


function editHotelOrder(HotelOrderId) {
    var hotelName = document.getElementById('edit-hotel-service-hotelName').value;
    var roomType = document.getElementById('edit-hotel-service-roomTypeName').value;
    var roomCount = document.getElementById('edit-hotel-service-roomCount').value;
    var days = document.getElementById('edit-hotel-service-days').value;
    var fromDate = document.getElementById('edit-hotel-service-dateFrom').value;
    var toDate = document.getElementById('edit-hotel-service-dateTo').value;

    console.log(HotelOrderId + " " + hotelName + " " + roomType + " " + roomCount + " " + days + " " + fromDate + " " + toDate)

    if (!hotelName || !roomType || !roomCount || !days || !fromDate || !toDate) {
        alert('Please Fill All Inputs.');
        return;
    }

    var hotelOrder = {
        HotelOrderId: HotelOrderId,
        HotelName: hotelName,
        RoomCount: parseInt(roomCount),
        Days: parseInt(days),
        RoomType: roomType,
        DateFrom: fromDate,
        DateTo: toDate
    };




    fetch('/sale/client/EditHotelOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(hotelOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('edit-hotel-service-content').innerHTML = data


        });


}

function updateTourService(tourOrderId) {
    var tourId = document.getElementById('edit-tourService-tourName').value;
    var carType = document.getElementById('edit-tourService-carTypeName').value;
    var guide = document.getElementById('edit-tourService-guide').value;
    var date = document.getElementById('edit-tourService-tourDate').value;

    if (!tourId || !carType || !guide || !date) {
        alert('Please Fill All Inputs.');
        return;
    }

    var tourOrder = {
        Id: tourOrderId,
        TourId: parseInt(tourId),
        CarType: carType,
        Date: date,
        Guide: guide === 'True'
    };


    fetch('/sale/client/EditTourOrder', {
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

            document.getElementById('EditTourServiceContent').innerHTML = data


        });



}

function updateInclusiveOrder(inclusiveOrderId) {
    var inclusiveName = document.getElementById('edit-inclusive-order-name').value;
    var count = document.getElementById('edit-inclusive-order-count').value;
    var date = document.getElementById('edit-inclusive-order-date').value;

    if (!inclusiveName || !count || !date) {
        alert('Please fill all inputs.');
        return;
    }

    var inclusiveOrder = {
        Id: inclusiveOrderId,
        InclusiveName: inclusiveName,
        Count: parseInt(count),
        Date: date
    };

    fetch('/sale/client/EditInclusiveOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(inclusiveOrder)
    })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('edit-inclusive-order-content').innerHTML = data
        });




}




