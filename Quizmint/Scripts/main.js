function showhide(ddl) {
    var divTf = document.getElementById("truefalse");
    if (ddl.value == 2) {
        divTf.style.display = "block";
    }
    else {
        divTf.style.display = "none";
    }
}