var divEle = document.getElementById("selectedTxt");
function getSelectedOption(self) {
    var x = document.getElementById('questions-list').value;
    divEle.textContent= x;
}
