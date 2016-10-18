$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $('.link-user-imgprof').click(function () {
        $('#fuProfPic').trigger('click');
    });
});
var hosts = window.location.origin + '/Gentran/';
var host = window.location.origin + '/files/';

function previewFile() {
    var preview = $('#imgEditProf');
    var imageField = $('#imageBase64');
    var file = document.querySelector('input[type=file]').files[0];
    var reader = new FileReader();
    reader.addEventListener("load", function () {

        if (preview.length <= 0) {
            $('#imgEditProf').remove(); 
            $('.link-user-imgprof').text('');
            $('.link-user-imgprof').css('line-height', '');
            $('.link-user-imgprof').css('background-color', '');
            $('.link-user-imgprof').append('<img ID="imgEditProf" src="img/loading/load.gif" style="border:none;"/>');
            $('#imgEditProf').prop('src', reader.result);
        }
        preview.attr('src', reader.result);
        imageField.val(reader.result);
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function _payloadParser(arrayVAlue) {
    var array = [];
    if ($.isArray(arrayVAlue)) {
        array = arrayVAlue;
    } else {
        array.push(arrayVAlue);
    }
    return array;
}


function UserProfileValidate() {

    var Values = {};

    if ($('#txt_edituname').val() == "" || $('#txt_editfname').val() == "" || $('#txt_editlname').val() == "") {

        notif_warning('Warning!', 'Please fill up all fields!');
    }
    else if ($('#txt_editemail').val() !== "" && validateEmail($('#txt_editemail').val()) == false) {
        notif_warning('Warning!', 'Invalid email!');
    }
    else {
        var Items = {}
        Items.umid = $('#txt_edituserid').val().replace(/\'/gi, '\'\'');
        Items.username = $('#txt_edituname').val().replace(/\'/gi, '\'\'');
        Items.nickname = $('#txt_editnname').val().replace(/\'/gi, '\'\'');
        Items.firstname = $('#txt_editfname').val().replace(/\'/gi, '\'\'');
        Items.middlename = $('#txt_editmname').val().replace(/\'/gi, '\'\'');
        Items.lastname = $('#txt_editlname').val().replace(/\'/gi, '\'\'');
        Items.email = $('#txt_editemail').val().replace(/\'/gi, '\'\'');
        Items.image = "unchanged";
        Items.oldpassword = "unchanged";

        if ($('#imageBase64').val().length > 0) {
            Items.image = $('#imageBase64').val().replace(/\'/gi, '\'\'');
        }

        if (document.getElementById('chk_changepass').checked) {

            if ($('#txt_oldpass').val() == '' || $('#txt_newpass').val() == '' || $('#txt_confirmpass').val() == '') {
                notif_warning('Warning!', 'Please fill up all fields!');
            }
            else {
                Items.oldpassword = $('#txt_oldpass').val().replace(/\'/gi, '\'\'');
                if ($('#txt_newpass').val() == $('#txt_confirmpass').val()) {
                    Items.newpassword = $('#txt_confirmpass').val().replace(/\'/gi, '\'\'');

                    Values.valid = true;
                    Values.items = Items;
                    return Values;
                }
                else {
                    notif_warning('Warning!', 'Your passwords does not match!');
                }
            }
        }
        else {
            Values.valid = true;
            Values.items = Items;
            return Values;
        }

    }
    Values.valid = false;
    return Values;
}

function resetFields() {
    $('#chk_changepass').prop('checked', false);
    $('#div-changepass').css({'display':'none'});
    $('#txt_oldpass').val('');
    $('#txt_newpass').val('');
    $('#txt_confirmpass').val('');

    $('#txt_addnname').val('');
    $('#txt_addfname').val('');
    $('#txt_addmname').val('');
    $('#txt_addlname').val('');
    $('#txt_addemail').val('');
}