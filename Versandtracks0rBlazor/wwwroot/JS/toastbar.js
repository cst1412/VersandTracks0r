function showToastBar(timeout){
    var x = document.getElementById("toastbar");

    // Add the "show" class to DIV
    x.className = "show";

    if(!timeout || timeout === ""){
        timeout = 3000;
    }
  
    // After 3 seconds, remove the show class from DIV
    setTimeout(() => { x.className = x.className.replace("show", ""); }, timeout);
}