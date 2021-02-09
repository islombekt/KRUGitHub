$(document).ready(function () {

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    navbarSelection();
    validateForms();
    // $("form.kru").validate({
    //     rules: {
    //         "Input.FName": {
    //             required: true
    //         },
    //         "Input.LName": {
    //             required: true
    //         },
    //         "Input.SName": {
    //             required: true
    //         },
    //         "Input.PhoneNumber": {
    //             required: true
    //         },
    //         "Input.Position": {
    //             required: true
    //         },
    //         "Input.Email": {
    //             required: true
    //         },
    //         "Input.Password": {
    //             required: true
    //         },
    //
    //     },
    //     errorClass: "is-invalid",
    // });

    const mask = "+998 (99) 999-99-99";
    $('form.kru #phone').inputmask(mask, {removeMaskOnSubmit: true});
});

function toggleLogo() {
    const logoImg = $("#ung-logo");
    const isAttrExist = typeof logoImg.attr("opened") !== typeof undefined && typeof logoImg.attr("opened") !== false;
    if (isAttrExist) {
        logoImg.attr("src", "/images/only_logo.png");
        logoImg.removeAttr("opened");
    } else {
        logoImg.attr("src", "/images/Logotype-UzbekNefteGaz-.png");
        logoImg.attr("opened", "");
    }
}

$(function () {
    $("#example1").DataTable({
        "responsive": true, "lengthChange": false, "autoWidth": false,
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
});

function navbarSelection() {
    const path_id = {
        "FileHistories": "documents",
        "Users": "workers_admin",
        "Departments": "department",
        "Addresses": "address",
        "Reports": "report",
        "Plans": "plan",
        "Tasks": "tasks",
        "Task_Type": "types"
    };
    const path_arr = window.location.href.split("/");
    const arr_length = path_arr.length;
    let select_name;
    Object.keys(path_id).forEach(function (key) {
        if (key === path_arr[arr_length - 1] || key === path_arr[arr_length - 2]) {
            select_name = key;
        }
    });
    $(`#${path_id[select_name]} a`).css({'background-color': '#494E53'});
}

// $("form.kru").on("submit", function(){
//     console.log("click!");
//     console.log($(this));
//     $(this).each(function(){
//         const form = $(this);
//         console.log(form);
//         form.each(function(){
//             console.log($(this));
//         });
//     });
// });

function validateForms() {
    'use strict';

    var forms = document.querySelectorAll('.custom-validation');

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                let passValidation;
                for(const [key, val] of Object.entries(form)){
                    let messages = [];
                    passValidation = true;
                    const inputValue = val.value;
                    if(inputValue === ""){
                        passValidation = false;
                        messages.push("Это поле обязательно");
                    }
                    if(key === "6"){
                        const re = /^[\w-\.]+@([\w-]+\.)+[\w-]+$/;
                        if(!re.test(inputValue)){
                            passValidation = false;
                        }
                        messages.push("Введите почту в правильном формате");
                    }
                    if(key === "3"){
                        const re = /^\+998 \([\d]{2}\) [\d]{3}-[\d]{2}-[\d]{2}$/;
                        if(!re.test(inputValue)){
                            passValidation = false;
                        }
                        messages.push("Введите номер телефона в правильном формате");
                    }
                    if(key === "7"){
                        const re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$/;
                        if(!re.test(inputValue)){
                            passValidation = false;
                        }
                        messages.push("Ваш пароль должен содержать как минимум одну цифру, прописную и строчную букву, знак");
                    }
                    if(key === "8"){
                        if(inputValue !== form["7"].value) {
                            passValidation = false;
                        }
                        messages.push("Пароли должны совпадать");
                    }

                    if(val.classList !== undefined){
                        if(!passValidation){
                            val.classList.remove("is-valid");
                            val.classList.add("is-invalid");

                            if($(val).closest(".form-group").find('.invalid-feedback').html() !== undefined){
                                let htmlMessages = "<ol>";
                                messages.forEach(msg => {
                                    htmlMessages += ("<li>" + msg + "</li>");
                                });
                                htmlMessages += "</ol>";

                                const errMsgContainer = $(val).closest(".form-group").find('.invalid-feedback');
                                errMsgContainer.html(htmlMessages);
                                errMsgContainer.css({"display" : "block"});
                            }
                        }
                        else{
                            val.classList.remove("is-invalid");
                            val.classList.add("is-valid");

                            const errMsgContainer = $(val).closest(".form-group").find('.invalid-feedback');
                            errMsgContainer.html("");
                            errMsgContainer.css({"display" : "none"});
                        }
                    }
                }

                if(!passValidation){
                    console.log("Prevent submit");
                    event.preventDefault();
                    event.stopPropagation();
                }
            }, false);
        });
}