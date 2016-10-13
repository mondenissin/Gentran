function notif_success(title,message) {
    $.Notification.autoHideNotify('success', 'top center', title, message);
}
function notif_error(title, message) {
    $.Notification.notify('error', 'top center', title, message);
}
function notif_warning(title, message) {
    $.Notification.notify('warning', 'top center', title, message);
}
function notif_info(title, message) {
    $.Notification.notify('info', 'top center', title, message);
}