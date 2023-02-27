// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// Write your JavaScript code.


function Reset() {
    event.stopPropagation()
    const Btns = document.getElementsByName('sortStele');
    for (const btn of Btns) {
        btn.checked = false;
    }
    Btns2 = document.getElementsByName('sortPret');
    for (const btn of Btns2) {
        btn.checked = false;
    }
 }



//drop down pt filtre
function myFunction() {

    document.getElementById("myDropdown").classList.toggle("show");
}


function AddToCart(id) {
    event.stopPropagation();
    $.ajax({
        type: "POST",
        url: window.location.origin + "/Carts/New/" + id
    }).fail(function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status === 401) {
            window.location.href = 'https://localhost:7275/Identity/Account/Login';
            console.log(401);
        }
    });
}