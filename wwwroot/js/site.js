$("#toggle-menu-btn").click(function(){
    toggleLogo();
});

$(document).ready(function(){
  $('[data-toggle="tooltip"]').tooltip();
});

function toggleLogo(){
    console.log("toggle");
    const logoImg = $("#ung-logo");
    const isAttrExist = typeof logoImg.attr("opened") !== typeof undefined && typeof logoImg.attr("opened") !== false;
    if(isAttrExist){
     logoImg.attr("src", "/images/only_logo.png");
     logoImg.removeAttr("opened");
    } else{
     logoImg.attr("src", "/images/Logotype-UzbekNefteGaz-.png");
     logoImg.attr("opened", "");
    }
 }

 $(function () {
  $("#example1").DataTable({
  "responsive": true, "lengthChange": false, "autoWidth": false,
  }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
});