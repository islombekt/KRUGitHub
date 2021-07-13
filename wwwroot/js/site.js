$(function () {
  var PlaceHolderElement = $("#PlaceHolderHere");
  $(".addEmployeeButton").click(function (event) {
    var url = $(this).data("url");
    var decodedUrl = decodeURIComponent(url);
    $.get(decodedUrl).done(function (data) {
      PlaceHolderElement.html(data);
      PlaceHolderElement.find(".modal").modal("show");
    });
    decodeURIComponent(url);
  });
});

function reverseFormat(formatNumber) {
  let number = formatNumber.replace(/\s/g, "");
  number = number.replace(/,/, ".");

  return number;
}

function convertNumber(number) {
  return new Intl.NumberFormat("ru-RU").format(number);
}

function formatAmount(input) {
  const number = reverseFormat(input.val());
  const formattedNumber = convertNumber(number);
  input.val(formattedNumber);
}

function isNumberInput(inputVal) {
  const length = inputVal.length;
  return /\d/.test(inputVal[length - 1]);
}

function tableTruncateText() {
  const maxSize = 20;
  $("#example1 tbody td, #example2 tbody td").each(function () {
    const tableText = $(this).text().trim();
    if ($(this).attr("class") !== "file-link" && tableText.length > maxSize) {
      $(this).text(tableText.substring(0, maxSize) + "...");
    }
  });
}

$(document).ready(function () {
  $(".select2").select2();

  makeTablesAdaptive();

  const tableAmount = $("table .amount");
  tableAmount.each(function () {
    const cellVal = $(this).text().trim();
    $(this).text(convertNumber(cellVal));
  });

  const amountObj = $("#amount");
  if (amountObj && amountObj.length > 0) {
    formatAmount(amountObj);
    amountObj.on("input", function () {
      if (isNumberInput($(this).val())) formatAmount($(this));
    });
  }

  $("form").each(function () {
    $(this).attr("autocomplete", "off");
  });

  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  navbarSelection();
  validateForms();
  statusChanged();

  const mask = "+998 (99) 999-99-99";
  $("form.kru #phone").inputmask(mask);

  //Date range picker
  $("#reservationdate").datetimepicker({
    format: "L",
  });
  $("#finance_reports_start_date").datetimepicker({
    format: "L",
  });
  $("#finance_reports_end_date").datetimepicker({
    format: "L",
  });
  $("#tasks_start_date").datetimepicker({
    format: "L",
  });
  $("#tasks_end_date").datetimepicker({
    format: "L",
  });

  tableTruncateText();
});

const makeTablesAdaptive = () => {
  $("table.table").wrap("<div class='table-responsive'></div>");
};

const path_sortable_cols = {
  Manager: {
    Employees: {
      Index: [2, 5],
      Index2: [2, 5, 6],
    },
    Tasks: {
      Index: [7, 8],
      Index2: [],
    },
    FileHistories: {
      Index: [2, 6],
      Index2: [],
    },
    Reports: {
      Index: [2, 8, 9, 11],
      Index2: [2, 8, 9, 11],
    },
    Plans: {
      Index: [2, 5, 8],
      Index2: [],
    },
    FinanceReports: {
      Index: [7, 9, 12],
      Index2: [],
    },
    Category1: {
      Index: [1],
      Index2: [],
    },
    Task_Type: {
      Index: [1],
      Index2: [],
    },
  },
  Employee: {
    Reports: {
      Index: [5, 7, 9],
      Index2: [],
    },
    Plans: {
      Index: [3, 6],
      Index2: [],
    },
    FileHistories: {
      Index: [2],
      Index2: [],
    },
    FinanceReports: {
      Index: [8, 10, 11],
      Index2: [],
    },
    Tasks: {
      Index: [8, 9],
      Index2: [8, 9],
    },
    Category1: {
      Index: [1],
      Index2: [],
    },
  },
};
const path_date = {
  Manager: {
    FinanceReports: {
      Index: [5, 6],
      Index2: [],
    },
    Reports: {
      Index: [5],
      Index2: [5],
    },
    Plans: {
      Index: [3, 4],
      Index2: [],
    },
    Tasks: {
      Index: [2, 3],
      Index2: [],
    },
    FileHistories: {
      Index: [3, 4],
      Index2: [],
    },
  },
  Employee: {
    Reports: {
      Index: [4],
      Index2: [],
    },
    Plans: {
      Index: [4, 5],
      Index2: [],
    },
    Tasks: {
      Index: [3, 4],
      Index2: [3, 4],
    },
    FinanceReports: {
      Index: [5, 6],
      Index2: [],
    },
    FileHistories: {
      Index: [3, 4],
      Index2: [],
    },
  },
};
const settings = {
  bInfo: true,
  scrollX: true,
  pagingType: "full_numbers",
  language: {
    sProcessing: "...",
    sLengthMenu: "_MENU_",
    sZeroRecords: "",
    sInfo: "_START_ - _END_",
    sInfoEmpty: "",
    sInfoFiltered: "",
    sInfoPostFix: "",
    sSearch:
      '<i style="font-size: 22px;" class="fas fa-search"></i>&nbsp;&nbsp;',
    sUrl: "",
    oAria: {
      sSortAscending: "",
      sSortDescending: "",
    },
    paginate: {
      first: '<i class="fas fa-angle-double-left"/>',
      last: '<i class="fas fa-angle-double-right"></i>',
      previous: '<i class="fas fa-angle-left"></i>',
      next: '<i class="fas fa-angle-right"></i>',
    },
  },
  order: [],
};
$(function () {
  const path_arr = window.location.href.split("/");
  const last_el = path_arr[path_arr.length - 1];

  let man_targets = [];
  let man_date_targets = [];
  if (last_el === "Index2" || last_el === "Index") {
    const pre_last_el = path_arr[path_arr.length - 2];
    man_targets = R.prop(
      last_el,
      R.prop(pre_last_el, R.prop("Manager", path_sortable_cols))
    );
    man_date_targets = R.prop(
      last_el,
      R.prop(pre_last_el, R.prop("Manager", path_date))
    );
  } else {
    man_targets = R.prop(
      "Index",
      R.prop(last_el, R.prop("Manager", path_sortable_cols))
    );
    man_date_targets = R.prop(
      "Index",
      R.prop(last_el, R.prop("Manager", path_date))
    );
  }

  $("#example1")
    .DataTable({
      ...settings,
      columnDefs: [
        {
          orderable: false,
          targets: man_targets,
        },
        {
          targets: man_date_targets,
          render: $.fn.dataTable.render.moment(
            "DD.MM.YYYY H:mm:ss",
            "DD.MM.YYYY"
          ),
          type: "DD.MM.YYYY",
        },
      ],
    })
    .buttons()
    .container()
    .appendTo("#example1_wrapper .col-md-6:eq(0)");

  let emp_targets = [];
  let emp_date_targets = [];
  if (last_el === "Index2" || last_el === "Index") {
    const pre_last_el = path_arr[path_arr.length - 2];
    emp_targets = R.prop(
      last_el,
      R.prop(pre_last_el, R.prop("Employee", path_sortable_cols))
    );
    emp_date_targets = R.prop(
      last_el,
      R.prop(pre_last_el, R.prop("Employee", path_date))
    );
  } else {
    emp_targets = R.prop(
      "Index",
      R.prop(last_el, R.prop("Employee", path_sortable_cols))
    );
    emp_date_targets = R.prop(
      "Index",
      R.prop(last_el, R.prop("Employee", path_date))
    );
  }
  $("#example2")
    .DataTable({
      ...settings,
      columnDefs: [
        {
          orderable: false,
          targets: emp_targets,
        },
        {
          targets: emp_date_targets,
          render: $.fn.dataTable.render.moment(
            "DD.MM.YYYY H:mm:ss",
            "DD.MM.YYYY"
          ),
          type: "DD.MM.YYYY",
        },
      ],
    })
    .buttons()
    .container()
    .appendTo("#example1_wrapper .col-md-6:eq(0)");
});

function navbarSelection() {
  const path_id = {
    FileHistories: "documents",
    Users: "workers_admin",
    Departments: "department",
    Addresses: "address",
    Reports: "report",
    Plans: "plan",
    Tasks: "tasks",
    Task_Type: "types",
    FinanceReports: "fin_employee",
    Category1: "categories",
    Employees: "employees",
    Objects: "address",
    Tests: "tests",
  };
  const path_arr = window.location.href.split("/");
  const arr_length = path_arr.length;
  let select_name;
  Object.keys(path_id).forEach(function (key) {
    if (
      key === path_arr[arr_length - 1] ||
      key === path_arr[arr_length - 2] ||
      key === path_arr[arr_length - 3]
    ) {
      select_name = key;
    }
  });
  $(`#${path_id[select_name]} > a`).css({ "background-color": "#494E53" });
}

function validateForms() {
  "use strict";

  var forms = document.querySelectorAll(".custom-validation");

  function isToMiss(role, fieldName) {
    if (role === "Admin") {
      if (fieldName === "Input.ManagerId" || fieldName === "Input.DepartmentId")
        return true;
    } else if (role === "Manager") {
      if (fieldName === "Input.ManagerId") return true;
    } else if (role === "Employee") {
      if (fieldName === "Input.DepartmentId") return true;
    } else {
      if (
        fieldName === "ReportDescription" ||
        fieldName === "ReportComment" ||
        fieldName === "PlanDescription" ||
        fieldName === "FinComment" ||
        fieldName === "TaskName" ||
        fieldName === "SumLost" ||
        fieldName === "SumGain" ||
        fieldName === "Comment" ||
        fieldName === "Description" ||
        fieldName === "File" ||
        fieldName === "Name_C1" ||
        fieldName === "Name_C11" ||
        fieldName === "Name_C111" ||
        fieldName === "FileUrl" ||
        fieldName === "City" ||
        fieldName === "CheckPeriod" ||
        fieldName === ""
      )
        return true;
    }
    return false;
  }

  // Loop over them and prevent submission
  Array.prototype.slice.call(forms).forEach(function (form) {
    form.addEventListener(
      "submit",
      function (event) {
        let passValidation;
        let globalValidation = true;
        const selectedRole = $("#role option:selected").text();

        const formArray = $(form).serializeArray();

        let i = -1;
        for (const input of formArray) {
          let messages = [];
          passValidation = true;
          const inputValue = input.value;
          const inputName = input.name;
          i += 1;

          if (isToMiss(selectedRole, inputName)) {
            // form[i].classList.add("is-valid");
            continue;
          }

          if (inputValue === "") {
            passValidation = false;
            messages.push("Ушбу қатор тўлдирилиши шарт");
          }
          if (inputName === "Input.Email") {
            const re = /^[a-zA-Z0-9]+[.]?[a-z0-9]{3,20}@ung.uz$/;
            if (!re.test(inputValue)) {
              passValidation = false;
            }
            messages.push("Илтимос, E-mail ни тўғри форматда киритинг");
          }
          if (inputName === "Input.PhoneNumber") {
            const re = /^\+998 \([\d]{2}\) [\d]{3}-[\d]{2}-[\d]{2}$/;
            if (!re.test(inputValue)) {
              passValidation = false;
            }
            messages.push(
              "Илтимос, телефон рақамингизни тўғри форматда киритинг"
            );
          }
          if (
            inputName === "Input.Password" ||
            inputName === "Input.NewPassword"
          ) {
            const re =
              /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,15}$/;
            if (!re.test(inputValue)) {
              passValidation = false;
            }
            messages.push(
              "Парол камида битта катта ва битта лотин ҳарфидан иборат бўлиши керак, битта рақам, битта белги ва узунлиги 6 дан 15 гача бўлиши керак"
            );
          }
          if (inputName === "Input.ConfirmPassword") {
            let password = formArray.find(
              (inp) => inp.name === "Input.Password"
            );
            if (password !== undefined && inputValue !== password.value) {
              passValidation = false;
            }
            password = formArray.find(
              (inp) => inp.name === "Input.NewPassword"
            );
            if (password !== undefined && inputValue !== password.value) {
              passValidation = false;
            }
            messages.push("Пароллар бир хил бўлиши керак");
          }
          if (inputName === "ReportScore" || inputName === "amount") {
            const re = /^\d*\.?\d*$/;
            const reversedNumber = parseFloat(reverseFormat(inputValue));

            // const re = /^\d*\,?\d*$/;
            // const reversedNumber = inputValue.replace(" ", "");
            console.log("Reversed number");
            console.log(reversedNumber);
            console.log(typeof reversedNumber);

            if (!re.test(reversedNumber)) {
              passValidation = false;
            } else {
              $('input[name="' + inputName + '"]').val(reversedNumber);
            }
            messages.push("Майдонда фақат рақамлар бўлиши керак");
          }

          if (form[i].classList !== undefined) {
            if (!passValidation) {
              form[i].classList.remove("is-valid");
              form[i].classList.add("is-invalid");

              if (
                $(form[i])
                  .closest(".form-group")
                  .find(".invalid-feedback")
                  .html() !== undefined
              ) {
                let htmlMessages = "<ol>";
                messages.forEach((msg) => {
                  htmlMessages += "<li>" + msg + "</li>";
                });
                htmlMessages += "</ol>";

                const errMsgContainer = $(form[i])
                  .closest(".form-group")
                  .find(".invalid-feedback");
                errMsgContainer.html(htmlMessages);
                errMsgContainer.css({ display: "block" });
              }
            } else {
              form[i].classList.remove("is-invalid");
              form[i].classList.add("is-valid");

              const errMsgContainer = $(form[i])
                .closest(".form-group")
                .find(".invalid-feedback");
              errMsgContainer.html("");
              errMsgContainer.css({ display: "none" });
            }
          }

          if (!passValidation) {
            globalValidation = false;
          }
        }

        if (!globalValidation) {
          event.preventDefault();
          return false;
        } else {
          return true;
        }
      },
      false
    );
  });
}

function statusChanged() {
  const ids = ["parentTask", "parentAddress", "object"];
  const showFields = () => {
    ids.forEach((id) => {
      $(`.status-kru #${id}`).slideDown();
    });
  };
  const hideFields = () => {
    ids.forEach((id) => {
      $(`.status-kru #${id}`).hide();
    });
  };
  const removeVals = () => {
    ids.forEach((id) => {
      $(`.status-kru #${id}`).children("select").val("");
    });
  };
  $(".status-kru #address").each(function () {
    const selectedAddressArr = $(".status-kru #object select option").toArray();
    if (selectedAddressArr.length === 0) $(".status-kru #object").hide();
    else $(".status-kru #object").slideDown();
  });

  const showOrHideFields = (thisRef) => {
    const showValues = ["8", "7", "К"];
    const selectedChild = $(thisRef).children("option:selected").val();
    const isShown = showValues.some((val) => {
      if (val === selectedChild) {
        showFields();
        return true;
      }
    });
    if (!isShown) {
      hideFields();
      removeVals();
    }
  };
  showOrHideFields($(".status-kru #status")[0]);
  $(".status-kru #status").change(function () {
    showOrHideFields(this);
  });
}
