function ValidateAddUser() {

    var Values = {};
    Values.valid = false;

    var userType = $('#ddl_addusertype').val();
    var nName = $('#txt_addnname').val().replace(/\'/gi, '\'\'');
    var fName = $('#txt_addfname').val().replace(/\'/gi, '\'\'');
    var mName = $('#txt_addmname').val().replace(/\'/gi, '\'\'');
    var lName = $('#txt_addlname').val().replace(/\'/gi, '\'\'');
    var eMail = $('#txt_addemail').val().replace(/\'/gi, '\'\'');

    if (userType == "" || userType == null || fName == "" || lName == "") {
        notif_warning('Warning!','Please fill all the required fields!');
    }
    else if (eMail != "" && validateEmail(eMail) == false) {
        notif_warning('Warning!','Invalid email!');
    }
    else {
        var Item = {};
        Item.type = userType;
        Item.nickname = nName;
        Item.firstname = fName;
        Item.middlename = mName;
        Item.lastname = lName;
        Item.email = eMail;

        Values.items = Item;
        Values.valid = true;
    }
    return Values;
}