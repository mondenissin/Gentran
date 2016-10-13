function notif_success(title,message) {
    $.Notification.autoHideNotify('success', 'bottom left', title, message);
}
function notif_error(title, message) {
    $.Notification.notify('error', 'bottom left', title, message);
}
function notif_warning(title, message) {
    $.Notification.notify('warning', 'bottom left', title, message);
}
function notif_info(title, message) {
    $.Notification.notify('info', 'bottom left', title, message);
}