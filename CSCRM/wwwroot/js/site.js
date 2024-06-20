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

function deleteHotel() {
    
    const confirmDelete = confirm('Are you sure you want to delete this hotel?');
    if (confirmDelete) {
        fetch('/manage/hotel/EditHotelAsync')
            .then(res => {
                return res.text();
            })
            .then(data => {
                console.log(data)
                document.getElementById('hotels-page-content').innerHTML = data

                /*$('hotels-page-content').html(data)*/
            });
    }
}
