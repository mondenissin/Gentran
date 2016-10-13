$(document).ready(function () {
    setInterval(function () {
        connect();
    }, 5000);
});

function connect() {
    _Ajax("api/login", "GET", "" , "", function (data) {
        console.log(data);
        if (data !== 'Connected!') {
            $('#username').prop('disabled', true);
            $('#password').prop('disabled', true);
            $('#submitButt').prop('disabled', true);

            $('#lblErrorMsg').text(data);
            $('#lblErrorMsg').fadeIn();
        }
        else {
            $('#username').prop('disabled', false);
            $('#password').prop('disabled', false);
            $('#submitButt').prop('disabled', false);
            $('#lblErrorMsg').text('');
            $('#lblErrorMsg').fadeOut();
        }
    });
}

function submitLogin() {

    var userinfo = [];
    userinfo[0] = $('#username').val();
    userinfo[1] = $('#password').val();
    
    _Ajax("api/login/"+userinfo, "GET", "", "", function (data) {
        if (data.toLowerCase().includes("success")) {
            $('#lblErrorMsg').text('Login Success');
            $('#lblErrorMsg').fadeIn();
            setTimeout(function () {
                window.location.href = MyApp.rootPath + "Retrieve/Index";
            },1000);
        } else {
            console.log("Login Failed");
            $('#lblErrorMsg').text(data);
            $('#lblErrorMsg').fadeIn();
        }
    });

    return false;
}