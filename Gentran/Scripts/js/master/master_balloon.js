function success(title, message) {
    $.Notification.autoHideNotify('success', 'bottom left', title, message);
}
function error(title, message) {
    $.Notification.notify('error', 'bottom left', title, message);
}
function warning(title, message) {
    $.Notification.notify('warning', 'bottom left', title, message);
}
function info(title, message) {
    $.Notification.notify('info', 'bottom left', title, message);
}