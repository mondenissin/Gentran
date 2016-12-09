function notif_success(title,message) {
    $.Notification.autoHideNotify('success', 'bottom left', title, message);
}
function notif_error(title, message) {
    $.Notification.autoHideNotify('error', 'bottom left', title, message);
}
function notif_warning(title, message) {
    $.Notification.autoHideNotify('warning', 'bottom left', title, message);
    $('.notifyjs-metro-warning .text-wrapper').css('color', 'rgba(115, 115, 115, 0.94)');
}
function notif_info(title, message) {
    $.Notification.notify('info', 'bottom left', title, message);
}