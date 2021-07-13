function validateForms() {
    'use strict';

    var forms = document.querySelectorAll('.custom-validation');

    function isToMiss(role, fieldName){
        if(role === 'Admin'){
            if(fieldName === 'Input.ManagerId' || fieldName === 'Input.DepartmentId')
                return true;
        }
        else if(role === 'Manager'){
            if(fieldName === 'Input.ManagerId')
                return true;
        }
        else if(role === 'Employee'){
            if(fieldName === 'Input.DepartmentId')
                return true;
        }
        return false;
    }

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            let passValidation;
            let globalValidation = true;
            const selectedRole = $('#role option:selected').text();

            const formArray = $(form).serializeArray();
            let i = -1;
            for (const input of formArray) {
                let messages = [];
                passValidation = true;
                const inputValue = input.value;
                const inputName = input.name;
                i += 1;

                if(isToMiss(selectedRole, inputName))
                    continue;

                if (inputValue === "") {
                    passValidation = false;
                    messages.push("Это поле обязательно");
                }
                if (inputName === "Input.Email") {
                    const re = /^[a-zA-Z0-9]+[.]?[a-z0-9]{3,20}@ung.uz$/;
                    if (!re.test(inputValue)) {
                        passValidation = false;
                    }
                    messages.push("Введите почту в правильном формате");
                }
                if (inputName === "Input.PhoneNumber") {
                    const re = /^\+998 \([\d]{2}\) [\d]{3}-[\d]{2}-[\d]{2}$/;
                    if (!re.test(inputValue)) {
                        passValidation = false;
                    }
                    messages.push("Введите номер телефона в правильном формате");
                }
                if (inputName === "Input.Password") {
                    const re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$/;
                    if (!re.test(inputValue)) {
                        passValidation = false;
                    }
                    messages.push("Пароль должен содержать минимум одну прописную и одну заглавную букву на латинице, одну цифру, один знак и быть длиной от 6 до 15 символов");
                }
                if (inputName === "Input.ConfirmPassword") {
                    if (inputValue !== formArray.find(inp => inp.name === "Input.Password").value) {
                        passValidation = false;
                    }
                    messages.push("Пароли должны совпадать");
                }

                if (form[i].classList !== undefined) {
                    if (!passValidation) {
                        form[i].classList.remove("is-valid");
                        form[i].classList.add("is-invalid");

                        if ($(form[i]).closest(".form-group").find('.invalid-feedback').html() !== undefined) {
                            let htmlMessages = "<ol>";
                            messages.forEach(msg => {
                                htmlMessages += ("<li>" + msg + "</li>");
                            });
                            htmlMessages += "</ol>";

                            const errMsgContainer = $(form[i]).closest(".form-group").find('.invalid-feedback');
                            errMsgContainer.html(htmlMessages);
                            errMsgContainer.css({"display": "block"});
                        }
                    } else {
                        form[i].classList.remove("is-invalid");
                        form[i].classList.add("is-valid");

                        const errMsgContainer = $(form[i]).closest(".form-group").find('.invalid-feedback');
                        errMsgContainer.html("");
                        errMsgContainer.css({"display": "none"});
                    }
                }

                if (!passValidation) {
                    globalValidation = false;
                }
            }

            console.log($(form).serializeArray());
            if(!globalValidation){
                event.preventDefault();
                console.log("Prevent submit");
                return false;
            } else{
                console.log("Submit");
                return true;
            }
        }, false);
    });
}