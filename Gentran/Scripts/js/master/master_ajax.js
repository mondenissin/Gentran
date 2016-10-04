function _Ajax(url, method, dataString, dataItems, callback) {
    var payload;
    var responseData = {};
    
    if ($.isArray(dataItems)) {
        payload = 'payload:' + JSON.stringify(dataItems);
    } else {
        payload = 'payload:[' + JSON.stringify(dataItems) + ']';
    }

    if (method == "GET") {
        $.get(MyApp.rootPath + url, function (data) {
            callback(data);
        });
    }
    else if (method == "POST") {
        $.ajax({
            url: url,
            type: method,
            data: '{' + dataString + ',' + payload + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                responseData = data;
            },
            error: function (jqXHR, exception) {
                callback(jqXHR.responseText);
            },
            complete: function () {
                callback(responseData);
            }
        });
    }
}