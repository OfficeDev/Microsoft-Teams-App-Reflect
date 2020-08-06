/// <reference path="index.js" />
let blockdata = "";
let deleteid = "";
let editid = "";
let previouseditid = "";
let weeks = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
let currentrecurrsion = {};
$(document).ready(function () {
    let show = $("#backButton").val();
    $('.float-left').hide();
    if (show === "True") {
        $('.float-right').hide();
        $('.float-left').show();
    }
    $(".spinner").show();
    $("#deleteIcon").hide();
    $("#edit").hide();
    let today = moment().format("YYYY-MM-DD");
    $("#execdate").val(today);
    $("#startdatedisplay").html(today);
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
    $('[data-toggle="popover"]').popover();
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/ManageRecurringPosts.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/ManageRecurringPosts-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
    });
    getRecurssions();
});

$("tbody#tablebody").on("click", "td.date-day", function () {
    $('#week').toggle();
});

$('.delete-icon').click(function () {
    $('#deleteIcon').modal('show');
});

$('.edit-icon').click(function () {
    $('#edit').modal('show');
});

$(".date-ip").on("change", function () {
    this.setAttribute(
        "data-date",
        moment(this.value, "YYYY-MM-DD")
            .format(this.getAttribute("data-date-format"))
    );
}).trigger("change");

function getRecurssions() {
    let email = $("#contextemail").val();
    $.ajax({
        type: 'GET',
        url: '/api/GetRecurssions/' + email,
        success: function (data) {
            $(".spinner").hide();
            $(".mf-manage").show();
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#questioncount").html("(" + recurssions.length + ")");
            daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            let sendpostat = "";
            let blockdata = "";
            let wholedata = "";
            recurssions.forEach(x => {
                let timehours = "";
                let timeminutes = "";
                blockdata = "";
                let mode = ' AM';
                if (x.ExecutionTime) {
                    if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
                        timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                        timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

                        if (timeminutes === 60) {
                            timehours = timehours + 1;
                            timeminutes = '00';
                        }

                        if (timehours > 11) {
                            mode = ' PM';
                        }
                        if (timehours > 12) {
                            timehours = timehours - 12;
                        }
                    }
                    else {
                        timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                        timeminutes = "00";
                        if (timehours > 11) {
                            mode = ' PM';
                        }
                        if (timehours > 12) {
                            timehours = timehours - 12;
                        }
                    }
                }
                if (x.RecurssionType === "Daily") {
                    sendpostat = "Every 1 Day at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Monthly") {
                    sendpostat = "Every Month " + new Date(x.ExecutionDate).getDate() + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Weekly") {
                    sendpostat = "Every Week " + weeks[new Date(x.ExecutionDate).getDay()] + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Every weekday") {
                    sendpostat = "Every Week Day " + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                }
                else if (x.RecurssionType === "Custom") {
                    sendpostat = (new DOMParser).parseFromString(x.CustomRecurssionTypeValue, "text/html").
                        documentElement.textContent + " at " + timehours + ":" + timeminutes + mode;
                }
                blockdata = blockdata + '<tr id="row1"><td class="hw-r-u">' + x.Question + '<div class="hru-desc">Created by: ' + x.CreatedBy + ' on ' + new Date(x.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td><td class="edit-icon" id="edit' + x.RefID + '"></td><td class="delete-icon" id="deleteIcon' + x.RefID + '" data-toggle="modal" data-target="#myalert"></td></tr>';
                wholedata = wholedata + blockdata;

                $(document).on("click", "#deleteIcon" + x.RefID, function (event) {
                    $("#deleteIcon").show();
                    $("#managetable").hide();
                    deleteid = event.currentTarget.id.split('on')[1];
                    let ques = recurssions.find(x => x.RefID === deleteid);
                    $("#tabledeletebodydetails").html('<tr id="row1"><td class="hw-r-u">' + ques.Question + '<div class="hru-desc">Created by: ' + ques.CreatedBy + ' on ' + new Date(ques.RefCreatedDate).toDateString() + '</div></td><td class="privacy-cl">' + x.Privacy + '</td> <td class="date-day">' + sendpostat + '</td></tr>');
                });

                $(document).on("click", "#edit" + x.RefID, function (event) {
                    editid = event.currentTarget.id.split('it')[1];
                    if (editid === previouseditid)
                        $("#edit").toggle();
                    else
                        $("#edit").show();
                    $(".day-select,.eve-week-start,.month-cal").hide();
                    let singledata = blockdata;
                    currentrecurrsion= recurssions.find(x => x.RefID === editid);
                    let timehours = "";
                    let timeminutes = "";
                    let mode = ' AM';
                    if (x.ExecutionTime) {
                        if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
                            timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                            timeminutes = parseInt(x.ExecutionTime.split(":")[1]) + parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

                            if (timeminutes === 60) {
                                timehours = timehours + 1;
                                timeminutes = '00';
                            }

                            if (timehours > 11) {
                                mode = ' PM';
                            }
                            if (timehours > 12) {
                                timehours = timehours - 12;
                            }
                        }
                        else {
                            timehours = parseInt(x.ExecutionTime.split(":")[0]) + parseInt(-1 * new Date().getTimezoneOffset() / 60);
                            timeminutes = "00";
                            if (timehours > 11) {
                                mode = ' PM';
                            }
                            if (timehours > 12) {
                                timehours = timehours - 12;
                            }
                        }
                    }
                    currentrecurrsion.time = timehours + ":" + timeminutes + mode;
                    if (x.RecurssionType === "Daily") {
                        sendpostat = "Every Day at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("day");
                        $("#customnumber").html("1");
                        $("#customtype").html("day");
                        $("#dwm").trigger("change");
                    }
                    else if (x.RecurssionType === "Monthly") {
                        sendpostat = "Every Month " + new Date(x.ExecutionDate).getDate() + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("month");
                        $("#customnumber").html("1");
                        $("#customtype").html("month");
                        $("#dwm").trigger("change");
                    }
                    else if (x.RecurssionType === "Weekly") {
                        sendpostat = "Every Week " + weeks[new Date(x.ExecutionDate).getDay()] + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $(".weekselect").removeClass("selectedweek");
                        $("#dwm").val("week");
                        $("#" + weeks[new Date(x.ExecutionDate).getDay()]).addClass("selectedweek");
                        $("#customnumber").html("1");
                        $("#customtype").html("week");
                        $("#dwm").trigger("change");
                    }
                    else if (x.RecurssionType === "Every weekday") {
                        sendpostat = "Every Week Day " + " at " + timehours + ":" + timeminutes + mode + " starting from " + new Date(x.ExecutionDate).toLocaleDateString();
                        $("#dwm").val("week");
                        $(".weekselect").addClass("selectedweek");
                        $("#Sunday").removeClass("selectedweek");
                        $("#Saturday").removeClass("selectedweek");
                        $("#customtype").html("week day");
                        $("#dwm").trigger("change");
                    }
                    else if (x.RecurssionType === "Custom") {
                        sendpostat = (new DOMParser).parseFromString(x.CustomRecurssionTypeValue, "text/html").
                            documentElement.textContent + " at " + timehours + ":" + timeminutes + mode;
                        div = document.createElement('div');
                        $(div).html(x.CustomRecurssionTypeValue);
                        var type = $(div).find("#customtype").html().split('(')[0];
                        $("#dwm").val(type);
                        $("#number").val($(div).find("#customnumber").html());
                        if (type === 'month') {
                            data = sendpostat.split(' ');
                            if (data[0] === "Day") {
                                $("input[name='days-check']:checked").val("days");
                                $("#monthdate").val(data[1]);
                            }
                            else {
                                $("input[name='days-check']:checked").val("weeks");
                                $("#weekseries").val(data[0]);
                                $("#weekday").val(data[1]);
                            }
                        }
                        if (type === 'week') {
                            data = sendpostat.split(' ');
                            weekarray = data[3].split(',');
                            weekarray.forEach(week => {
                                $("#" + week).addClass('selectedweek');
                            });
                        }
                        $("#dwm").trigger("change");
                    }
                    var postposition = $("#edit" + x.RefID).position().top + $("#edit" + x.RefID).height();
                    $(".post").css("top", postposition);
                    previouseditid = event.currentTarget.id.split('it')[1];
                });
            });
            $("#tablebody").html(wholedata);
            setTimeout(() => {
                recurssions.forEach(x => {
                    $(document).on("click", "#deleteIcon" + x.RefID, function (event) {
                        deleteid = event.currentTarget.id.split('on')[1];
                    });
                });
            }, 100);
            $("#deleteIcon").hide();
            $("#managetable").show();
        }
    });

}

function deleteRecurssion() {
    $("#deleteIcon").hide();
    $(".spinner").show();
    $.ajax({
        type: 'GET',
        url: '/api/DeleteReflection/' + deleteid,
        success: function (data) {
            if (data === "Deleted") {
                $("#tablebody").html("");
                getRecurssions();
            }

        }
    });
}



function saveRecurssion() {
    let rectype = "";
        if ($("#dwm").val() === "month") {
            if ($("input[name='days-check']:checked").val() === "days") {
                rectype = "Day " + $("#monthdate").val() + " " + $("#finaldates").html();
            }
            if ($("input[name='days-check']:checked").val() === "weeks") {
                rectype = $("#weekseries").val() + " " + $("#weekday").val() + " " + $("#finaldates").html();
            }

        }
    else rectype = $("#finaldates").html();
    currentrecurrsion.recurssionType = "Custom";
    nextexecutiondate = combineDateAndTime(currentrecurrsion.ExecutionDate, currentrecurrsion.time);
    $.ajax({
        type: 'POST',
        url: '/api/SaveRecurssionData',
        headers: {
            "Content-Type": "application/json"
        },
        data: JSON.stringify({
            "refID": editid, "recurssionType": "Custom", customRecurssionTypeValue: rectype, nextExecutionDate: nextexecutiondate
        }),
        success: function (data) {
            if (data === "true") {
                $("#tablebody").html("");
                $("#edit").hide();
                getRecurssions();
            }
        }
    });
}

function combineDateAndTime(date, time) {
        time = getTwentyFourHourTime(time);
        customvalue = $("#dwm").val();
    if (customvalue === "day" || customvalue === "month")
                return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
            else if (customvalue === "week") {
                if ($("#slectedweeks").html().split(',').length === 1) {
                    if (weeks[new Date(date).getDay()] === $("#slectedweeks").html())
                        return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
                    else {
                        let nextweekdate = nextWeekdayDate(date, weeks.indexOf($("#slectedweeks").html()));
                        nextweekdate.setHours(time.split(":")[0]);
                        nextweekdate.setMinutes(time.split(":")[1]);
                        return nextweekdate.toISOString();
                    }

                }
                else {
                    if ($("#slectedweeks").html().split(',').indexOf(weeks[new Date(date).getDay()])) {
                        return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
                    }
                }

    }
       
}

function nextWeekdayDate(date, day_in_week) {
    var ret = new Date(date || new Date());
    ret.setDate(ret.getDate() + (day_in_week - 1 - ret.getDay() + 7) % 7 + 1);
    return ret;
}

function getTwentyFourHourTime(time) {
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1].toLowerCase();

    if (AMPM === "pm" && hours < 12) hours = hours + 12;
    if (AMPM === "am" && hours === 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;

    return sHours + ':' + sMinutes;
}


function gotoIndex() {
    let linkInfo = {
        action: "reflection"
    };
    microsoftTeams.tasks.submitTask(linkInfo);
    return true;
}

$("#dwm").on("change", function () {
    if (this.value === "day") {
        $(".eve-day-start").show();
        $(".card").removeClass("week month");
        $(".card").addClass("day");
        $(".day-select,.eve-week-start,.month-cal").hide();
        $("#slectedweeks").html();
    } else if (this.value === "week") {
        $(".day-select,.eve-week-start").show();
        $(".card").removeClass("day month");
        $(".card").addClass("week");
        $(".eve-day-start,.eve-month-start,.month-cal").hide();
        let slectedweeks = [];
        let weekdays = $(".weekselect");
        weekdays.each((x) => {
            if ($(weekdays[x]).hasClass("selectedweek")) {
                slectedweeks.push($(weekdays[x]).attr("id"));
            }
        });
        $("#slectedweeks").html(slectedweeks.join(","));
    } else {
        $(".day-select,.eve-week-start,.eve-day-start,.day-select").hide();
        $(".month-cal,.eve-month-start").show();
        $(".card").removeClass("week day");
        $(".card").addClass("month");
        $("#slectedweeks").html();
    }
    $("#startdatedisplay").html($("#execdate").val());
    $("#customnumber").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
});

$("#number").on("change", function () {
    $("#customnumber").html($("#number").val());
});

$("#monthdate").on("keyup", function () {
    if (this.value > 31) {
        this.value = this.value[0];
    }
});

$(".weekselect").on("click", function () {
    if ($(this).hasClass("selectedweek")) {
        $(this).removeClass("selectedweek");
    } else {
        $(this).addClass("selectedweek");
    }
    let slectedweeks = [];
    let weekdays = $(".weekselect");
    weekdays.each((x) => {
        if ($(weekdays[x]).hasClass("selectedweek")) {
            slectedweeks.push($(weekdays[x]).attr("id"));
        }
    });
    $("#slectedweeks").html(slectedweeks.join(","));
});

$("#recurrance").on("change", function () {
    if (this.value === "Custom") {
        $(".custom-cal").show();
        $(".day-select,.eve-week-start,.month-cal").hide();
    } else {
        $(".custom-cal").hide();
    }
});

function cancelRecurssion() {
    $("#edit").hide();
    $("#managetable").show();
}

function cancel() {
    $("#deleteIcon").hide();
    $("#managetable").show();
}

function closeTaskModule() {
    $("#managetable").hide();
    $(".spinner").show();
    let closeTaskInfo = {
        action: "closeFirstTaskModule"
    };
    microsoftTeams.tasks.submitTask(closeTaskInfo);
    return true;
}