function addHotel() {
    const name = document.getElementById('add-hotel-name-input').value;
    const singlePrice = document.getElementById('add-hotel-snglprc-input').value;
    const doublePrice = document.getElementById('add-hotel-dblprc-input').value;
    const triplePrice = document.getElementById('add-hotel-trplprc-input').value;
    const contactName = document.getElementById('add-hotel-contactName-input').value;
    const contactPhone = document.getElementById('add-hotel-contactPhone-input').value;
    console.log(name);
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
            ContactPhone: contactPhone
        })
    })
        .then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                alert('Error adding hotel.');
            }
        });
}
