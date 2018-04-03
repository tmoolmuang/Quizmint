function showhide(ddl) {
    var divTf = document.getElementById("truefalse");
    var divMul = document.getElementById("multiple");
    if (ddl.value == 2) {
        divTf.style.display = "block";
        divMul.style.display = "none";
    }
    else if (ddl.value == 1)
    {
        divTf.style.display = "none";
        divMul.style.display = "block";
    }
    else if (ddl.value == 3) {
        divTf.style.display = "none";
        divMul.style.display = "none";
    }
}