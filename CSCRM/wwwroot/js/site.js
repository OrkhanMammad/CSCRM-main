
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

    fetch('/manage/hotel/AddNewHotel', {
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
        fetch(`/manage/hotel/DeleteHotel?hotelId=${hotelId}`, {
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

    fetch('/manage/hotel/EditHotel', {
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
        fetch(`/manage/company/DeleteCompany?companyId=${companyId}`, {
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
    const address = document.getElementById('add-company-address-input').value;
    const phoneNumber = document.getElementById('add-company-phone-input').value;
    const email = document.getElementById('add-company-Email-input').value;

    if (!name) {
        alert('Name field is required.');
        return;
    }

    fetch('/manage/company/addnewcompany', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
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
    const address = document.getElementById('company-edit-address-input').value;
    const email = document.getElementById('company-edit-email-input').value;
    const phone = document.getElementById('company-edit-phone-input').value;

    if (!name) {
        alert('Name field cannot be empty.');
        return;
    }

    fetch('/manage/company/EditCompany', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: Id,
            Name: name,
            Address: address,
            Email: email,
            Phone:phone
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
    
    fetch('/manage/tour/AddNewTour', {
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
        fetch(`/manage/Tour/DeleteTour?tourId=${tourId}`, {
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






function addCarType() {
    const name = document.getElementById('add-carTypeName-input').value;
    const capacity = document.getElementById('add-carTypeCapacity-input').value;

    if (!name || !capacity) {
        alert('All fields are required.');
        return;
    }

    

    // Burada eklenti için gerekli API isteği yapılabilir, fetch kullanılabilir.
    fetch('/manage/car/AddNewCar', {
         method: 'POST',
         headers: {
             'Content-Type': 'application/json'
         },
         body: JSON.stringify({
             Name: name,
             Capacity: parseInt(capacity)
         })
     })
        .then(res => {
            return res.text();
        })
        .then(data => {

            document.getElementById('cartypes-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}

