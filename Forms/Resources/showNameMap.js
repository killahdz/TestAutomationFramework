/*
 Switch the frame context to gsft_main before executing this script
*/

var nameMap = window.top.frames['gsft_main'].g_form.nameMap
for (var i = 0; i < nameMap.length; i++) {
    var input = jQuery(":input[name='" + nameMap[i].realName + "']");
    input.after(jQuery("<div style='background: #a8d38d;'>" + nameMap[i].prettyName + " : " + nameMap[i].realName + "</div>"))
}