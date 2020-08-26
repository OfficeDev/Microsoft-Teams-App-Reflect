let questions = [];
let userobject = {};
let accesstoken = "";
let weeks = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
let postTaskInfo = "";
$(document).ready(function () {
    $(".spinner").show();
    $("#postSentMessage").hide();
    $(".js-example-basic-single").select2({
        minimumResultsForSearch: Infinity
    });
    $(".js-example-tags").select2({
        tags: true,
        maximumInputLength: 150
    });
    $("#privacytext").html($("#privacy").val());
    $("#usertext").html(" " + userName);
    let today = moment().format("YYYY-MM-DD");
    $("#execdate").val(today);
    $("#startdatedisplay").html(today);
    $("#custom number").html($("#number").val());
    $("#customtype").html($("#dwm").val() + "(s)");
    $(".select2-selection__arrow").remove();
    let monthval = "";
    for (i = 1; i <= 31; i++) {
        monthval = monthval + '<option value="' + i + '">' + i + "</option>";
    }
    $("#monthdate").html(monthval);
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/Index.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/Index-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        GetDefaultQuestions(context.userPrincipalName);
    });

    document.addEventListener('DOMNodeInserted', () => {
        const node = document.getElementsByClassName("select2-search__field")[0];
        if (node) {
            node.addEventListener('focusout', () => {
                if ($(".select2-search__field").val()) {
                    var divPreviewEle = document.getElementById("selectedTxt");
                    divPreviewEle.textContent = $(".select2-search__field").val();
                    var divEle = document.getElementById("select2-questions-container");
                    divEle.textContent = $(".select2-search__field").val();
                    $("#questions").val($(".select2-search__field").val());
                }
            });
        }
    });

    $(".date-ip")
        .on("change", function () {
            this.setAttribute(
                "data-date",
                moment(this.value, "YYYY-MM-DD").format(
                    this.getAttribute("data-date-format")
                )
            );
            let today = moment().format("YYYY-MM-DD");
            if ($("#execdate").val() !== today) {
                $("#sendnow").attr("disabled", "true");
                $("#exectime").select2({ minimumResultsForSearch: Infinity}).val("00:00 AM").trigger("change");
                $(".select2-selection__arrow").remove();
            } else {
                $("#sendnow").removeAttr("disabled");
                $("#exectime").select2({ minimumResultsForSearch: Infinity}).val("Send now").trigger("change");
                $(".select2-selection__arrow").remove();
            }
            $("#startdatedisplay").html($("#execdate").val());
        })
        .trigger("change");
        $("#privacytext").html($("#privacy").val());
});

function SendAdaptiveCard() {
    let list = document.querySelectorAll(".htmlEle");
    let htmObj = {};
    list.forEach((obj, i) => {
        htmObj[obj.getAttribute("data-attr")] = obj.value;
    });
    let index = questions.findIndex((x) => x.question === $("#questions").val());
    let questionid = null;
    if (index !== -1) {
        questionid = questions[index].questionID;
    }
    let rectype = "";
    if ($("#recurrence").val() === "Custom") {
        if ($("#dwm").val() === "month") {
            if ($("input[name='days-check']:checked").val() === "days") {
                rectype = "Day " + $("#monthdate").val()+" "+$("#finaldates").html();
            }
            if ($("input[name='days-check']:checked").val() === "weeks") {
                rectype = $("#weekseries").val() + " " + $("#weekday").val()+" "+ $("#finaldates").html();
            }
            
        }
        else rectype = $("#finaldates").html();
    }
    else rectype = $("#recurrence").val();

    let exectime = "";
    if ($("#exectime").val() !== "Send now") {
        if ((new Date().getTimezoneOffset() / 60).toString().split('.').length > 1) {
            timehours = parseInt($("#exectime").val().split(":")[0]) - parseInt(-1 * new Date().getTimezoneOffset() / 60);
            timeminutes = parseInt($("#exectime").val().split(":")[1].split(' ')[0]) - parseInt((new Date().getTimezoneOffset() / 60).toString().split('.')[1] * 6);

            if (timeminutes === -30) {
                timehours = timehours - 1;
                timeminutes = '30';
            }

            if ($("#exectime").val().split(":")[1].split(' ')[1] === "PM") {
                timehours = timehours + 12;
            }
        }
        else {
            timehours = parseInt($("#exectime").val().split(":")[0]) - parseInt(-1 * new Date().getTimezoneOffset() / 60);
            timeminutes = "00";
            if ($("#exectime").val().split(":")[1].split('')[1] === "PM") {
                timehours = timehours + 12;
            }
        }
        exectime = timehours + ":" + timeminutes;

    }
    else
        exectime = $("#exectime").val();

    let taskInfo = {
        question: $("#questions").val(),
        questionID: questionid,
        privacy: $("#privacy").val(),
        executionDate: $("#execdate").val(),
        executionTime: exectime,
        nextExecutionDate: combineDateAndTime($("#execdate").val(), $("#exectime").val()),
        postDate: "",
        isDefaultQuestion: false,
        recurssionType: $("#recurrence").val(),
        customRecurssionTypeValue: rectype,
        action: "sendAdaptiveCard"
    };
    taskInfo.card = "";
    taskInfo.height = "medium";
    taskInfo.width = "medium";
    if (!$("#questions").val()) {
        alert("Please select " + $(".question").text());
    } else if (!$(".date-ip").val()) {
        alert("Please select " + $("#date").text());
    } else {
        if (taskInfo.executionTime !== "Send now") {
            postTaskInfo = taskInfo;
            $('#initialPost').hide();
            $("#confirmationMessage").html("<div class='u-set'>You're all set! This post was sent and is scheduled for " + taskInfo.executionDate + " at " + $("#exectime").val() + " (" + taskInfo.recurssionType + ")" + "</div>");
            $('.close-container').hide();
            $('#postSentMessage').show();
        } else {
            $(".spinner").show();
            $('#initialPost').hide();
            microsoftTeams.tasks.submitTask(taskInfo);
        }
    }
    return true;
}

function done() {
    $('#postSentMessage').hide();
    $(".spinner").show();
    microsoftTeams.tasks.submitTask(postTaskInfo);
}

function combineDateAndTime(date, time) {
    if ($('#exectime').val() !== "Send now") {
        time = getTwentyFourHourTime(time);
        if ($("#recurrence").val() === "Weekly" || $("#recurrence").val() === "Monthly" || $("#recurrence").val() === "Daily" || $("#recurrence").val() === "Every Weekday" && new Date(date).getDay !== 0 && new Date(date).getDay !== 6)
            return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
        else if ($("#recurrence").val() === "Every Weekday") {
            if (new Date(date).getDay === 0)
                return new Date(moment(`${date} ${time}`).add(1, 'days').format('YYYY-MM-DD HH:mm')).toUTCString();
            if (new Date(date).getDay === 6)
                return new Date(moment(`${date} ${time}`).add(2, 'days').format('YYYY-MM-DD HH:mm')).toUTCString();
        }
        else if ($("#recurrence").val() === "Custom") {
            customvalue = $("#dwm").val();
            if (customvalue === "day")
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
        else {
            return new Date(moment(`${date} ${time}`, 'YYYY-MM-DD HH:mm').format()).toUTCString();
        }
    }
    else {
        return "";
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

function getSelectedOption(event) {
    $("#selectedTxt").html($("#questions").val());
    if ($("#questions").val().length === 0) {
        $("#selectedTxt").text("No reflection question entered");
        $(".feeling").addClass("feeling-noquestion");
    } else {
        $(".feeling").removeClass("feeling-noquestion");
    }
}

function setPrivacy() {
    $("#privacytext").html($("#privacy").val());
}

function GetDefaultQuestions(userPrincipleName) {
    let blockdata = "";
    $.ajax({
        type: "GET",
        url: "api/GetAllDefaultQuestions/" + userPrincipleName,
        success: function (data) {
            questions = data;
            let defaultquestions = data.filter(x => x.isDefaultFlag);
            let myquestions = data.filter(x => !x.isDefaultFlag);
            if (myquestions.length > 0) {
                myquestions.forEach((x) => {
                    blockdata =
                        blockdata +
                        ' <option class="default-opt" id="' +
                        x.questionID +
                        '" value="' +
                        x.question +
                        '" title="' +
                        x.question +
                        '">' +
                        x.question +
                        "</option>";
                });
            } else {
                blockdata = blockdata + '<option class="default-opt" disabled>' + 'No custom questions entered yet' + '</option>';
            }
            blockdata = blockdata + '<optgroup label="Potential questions">Potential questions</optgroup>';
            defaultquestions.forEach((x) => {
                blockdata =
                    blockdata +
                    ' <option class="default-opt" id="' +
                    x.questionID +
                    '" value="' +
                    x.question +
                    '" title="' +
                    x.question +
                    '">' +
                    x.question +
                    "</option>";
            });
            $(".spinner").hide();
            $(".mc-content").show();
            $("#questions").append(blockdata);
            $("#selectedTxt").html($("#questions").val());
            $(".select2-search__field").attr("maxlength", "150");
            GetRecurssionsCount(userPrincipleName);
        }
    });
}

function GetRecurssionsCount(userPrincipleName) {
    $.ajax({
        type: "GET",
        url: "api/GetRecurssions/" + userPrincipleName,
        success: function (data) {
            recurssions = JSON.parse(JSON.parse(data).recurssions);
            $("#recurssionscount").html("(" + recurssions.length + ")");
        }
    });
}

submitHandler = (err, result) => {
    console.log("Reached submithandler!");
};

function openTaskModule() {
    let linkInfo = {
        action: "ManageRecurringPosts"
    };
    microsoftTeams.tasks.submitTask(linkInfo);
    return true;
}

function closeTaskModule() {
    let closeTaskInfo = {
        action: "closeFirstTaskModule"
    };
    microsoftTeams.tasks.submitTask(closeTaskInfo);
    return true;
}

function addShowHideButton() {
    if ($("#questionsblock").hasClass("hidequestions")) {
        $("#questionsblock").removeClass("hidequestions");
        $("#questionsblock").addClass("showquestions");
    } else {
        $("#questionsblock").addClass("hidequestions");
        $("#questionsblock").removeClass("showquestions");
    }
}

$("#recurrence").on("change", function () {
    if (this.value === "Custom") {
        $(".custom-cal").show();
        $("#customdata").show();
        $("#customdata").html('<span class="cutomdatavalue">'+$("#finaldates").text()+'</span>');
        $(".day-select,.eve-week-start,.month-cal").hide();
    } else {
        $(".custom-cal").hide();
        $("#customdata").hide();
    }
});

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

$(document).click(function (e) {
    if ($(e.target).closest("#customrecurrencediv").length > 0 || $(e.target).closest("#customdata").length > 0) {
        return false;
    }
    $(".custom-cal").hide();
    if ($("#recurrence").val() === "Custom") {
        $("#customdata").html('<span class="cutomdatavalue">' + $("#finaldates").text() + '</span>')
    }
});

$("#customdata").click(function (e) {
    $(".custom-cal").show();
});
