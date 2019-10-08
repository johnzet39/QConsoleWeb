$.validator.unobtrusive.adapters.addBool("ipformat");

$.validator.addMethod("ipformat", function (value, element) {
    if (!value) {
        return false;
    }
    var bytes = value.split(".");
    if (bytes.length != 4) {
        return false;
    }
    for (i in bytes) {
        var entry = bytes[i];
        if (!entry) {
            return false;
        }
        var byte = parseInt(entry);
        if (isNaN(byte) || byte < 0 || byte > 255) {
            return false;
        }
    }
    return true;
});