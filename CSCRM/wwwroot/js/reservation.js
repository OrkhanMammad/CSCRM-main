
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

    fetch('/reservation/hotel/AddNewHotel', {
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
        fetch(`/reservation/hotel/DeleteHotel?hotelId=${hotelId}`, {
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

    fetch('/reservation/hotel/EditHotel', {
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
            document.getElementById('hotel-edit-page-content').innerHTML = data

            /*$('hotels-page-content').html(data)*/
        });
}

function addConfirmation(hotelOrderId)
{
    // Aynı sınıf ismine sahip tüm input elemanlarını seç
    const inputs = document.querySelectorAll('.add-confirmation-number');

    // Değerleri toplamak için bir liste oluştur, boş olanları dahil etme
    const values = Array.from(inputs)
        .map(input => input.value)
        .filter(value => value.trim() !== '');  // Boş değerleri filtrele

    // Göndermek istediğiniz ID değerini tanımlayın
     // ID değerini buraya koyun

    // Değerleri ve ID'yi sunucuya JSON formatında gönderme
    
        fetch('/reservation/reservation/EditReservation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ReservationId: hotelOrderId,          // ID değeri
                ConfirmationNumbers: values   // Input değerleri
            })
        })
            .then(res => {
                return res.text();
            })
            .then(data => {
                document.getElementById('reservation-edit-page-content').innerHTML = data

                /*$('hotels-page-content').html(data)*/
            });
    
    
    

}



    


