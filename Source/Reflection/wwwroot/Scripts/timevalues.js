let timearray = [];
let timestring = '';
let optiondata = '';
$(document).ready(function () {
    let presentdate = new Date();
    let minutes = presentdate.getMinutes();
    if (minutes < 30) {
        presentdate.setMinutes(30);
    }
    else {
        presentdate.setHours(presentdate.getHours() + 1);
        presentdate.setMinutes(0);
    }
    let nextdate = new Date(presentdate.toISOString());
    nextdate.setDate(nextdate.getDate() + 1);
    while (presentdate < nextdate) {
        timestring = timestring + (presentdate.getHours() === 0 ? '00' : presentdate.getHours());
        timestring = timestring + ":";
        timestring = timestring + (presentdate.getMinutes() === 0 ? '00' : presentdate.getMinutes());
        timearray.push(timestring);
        presentdate.setMinutes(presentdate.getMinutes() + 30);
        timestring = '';
    }
    timearray.forEach(x => {
        let time = '';
        if (x.split(':')[0] >= 12) {
            let timearray = x.split(':');
            if(timearray[0]>12)
            timearray[0] = timearray[0] - 12;
            time = timearray.join(':') + ' PM';
        }
        else {
            if (x.split(':')[0] === '00') {
                let timearray = x.split(':');
                timearray[0] = 12;
                time = timearray.join(':') + ' AM';
            }
            else
            time = x + ' AM';
        }
        optiondata = optiondata + '<option value="' + time + '" id="'+time+'">' + time + '</option>';
    });
    $("#exectime").append(optiondata);
});
