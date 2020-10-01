let contextPrincipalName;
let feedback;
let reflection;
let question;
let totalcount = 0;

$(document).ready(function () {
    $(".spinner").show();
    $('.close-container').hide();
    $('.check-in').mouseover(() => {
        $('.custom-emoji-card').css('display', 'block');
    });
    $('.check-in').mouseout(() => {
        $('.custom-emoji-card').css('display', 'none');
    });
    $('.parent-hover').mouseover(() => {
        $('.custom-emoji-card').css('display', 'block');
    });
    $('.parent-hover').mouseout(() => {
        $('.custom-emoji-card').css('display', 'none');
    });
    microsoftTeams.initialize();
    function closeTaskModule() {
        let closeTaskInfo = {
            action: "closeFirstTaskModule"
        };
        microsoftTeams.tasks.submitTask(closeTaskInfo);
        return true;
    }
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../../../CSS/openReflections.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../../../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../../../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        contextPrincipalName = context.userPrincipalName;

        var feedbackvalue = $("#feedbackId").val();
        var color = "";
        for (i = 1; i <= 5; i++) {
            if (i.toString() === feedbackvalue) {
                $("#selectedimage").attr("src", "/images/Default_" + i + ".png");
                $(".select-img").removeClass("active");
                $("#img" + i).addClass("active");
                $(".check-in").hide();
                $(".divider").show();
                $(".remove").show();
                if (i === 1)
                    $(".emoji-selected").css("background-color", "#E4F4EB");
                else if (i === 2)
                    $(".emoji-selected").css("background-color", "#E9FCE9");
                else if (i === 3)
                    $(".emoji-selected").css("background-color", "#FFF7CC");
                else if (i === 4)
                    $(".emoji-selected").css("background-color", "#FFECE4");
                else if (i === 5)
                    $(".emoji-selected").css("background-color", "#FEE6E3");
            }
            $(document).on("click", "#img" + i, function (event) {
                imgid = event.currentTarget.id.split('g')[1];
                if (imgid === "1")
                    color = "#E4F4EB";
                else if (imgid === "2")
                    color = "#E9FCE9";
                else if (imgid === "3")
                    color = "#FFF7CC";
                else if (imgid === "4")
                    color = "#FFECE4";
                else if (imgid === "5")
                    color = "#FEE6E3";
                $(".emoji-selected").css("background-color", color);
                $(".selected-img").attr("src", "/Images/Default_" + imgid + ".png");
                $(".selected-img").show();
                $(".select-img").removeClass("active");
                $("#img" + imgid).addClass("active");
                $(".check-in").hide();
                $(".divider").show();
                $(".remove").show();
                $.ajax({
                    type: 'POST',
                    url: '/api/SaveUserFeedback',
                    headers: {
                        "Content-Type": "application/json"
                    },
                    data: JSON.stringify({
                        "feedbackId": parseInt(imgid), "reflectionId": $("#reflectionid").val(), "emailId": contextPrincipalName, "type": "", "messageId": "", action: "SaveUserFeedback", UserName: userName
                    }),
                    success: function (data) {
                        if (data === "true") {
                            GetReflections();
                        }
                    }
                });
            });
        }

        if (feedbackvalue === "0") {
            $.ajax({
                type: 'POST',
                url: '/api/GetUserFeedback',
                headers: {
                    "Content-Type": "application/json"
                },
                data: JSON.stringify({
                    "feedbackId": 0, "reflectionId": $("#reflectionid").val(), "emailId": contextPrincipalName, "type": "", "messageId": "", action: "SaveUserFeedback", UserName: userName
                }),
                success: function (data) {
                    if (data !== null && data !== 0) {
                        $("#selectedimage").attr("src", "/images/Default_" + data + ".png");
                        $(".select-img").removeClass("active");
                        $("#img" + data).addClass("active");
                        $(".check-in").hide();
                        if (data === 1)
                            $(".emoji-selected").css("background-color", "#E4F4EB");
                        else if (data === 2)
                            $(".emoji-selected").css("background-color", "#E9FCE9");
                        else if (data === 3)
                            $(".emoji-selected").css("background-color", "#FFF7CC");
                        else if (data === 4)
                            $(".emoji-selected").css("background-color", "#FFECE4");
                        else if (data === 5)
                            $(".emoji-selected").css("background-color", "#FEE6E3");
                    }
                    else {
                        $(".select-img").removeClass("active");
                        $(".selected-img").hide();
                        $(".check-in").show();
                        $(".divider").hide();
                        $(".remove").hide();
                        $(".emoji-selected").css("background-color", "#F4F4F4");
                    }
                }
            });
        }

        $(".remove").click(function () {
            $(".emoji-selected").css("background-color", "#F4F4F4");
            $(".selected-img").hide();
            $(".check-in").show();
            $(".remove").hide();
            $(".divider").hide();
            $(".select-img").removeClass("active");
            $.ajax({
                type: 'POST',
                url: '/api/SaveUserFeedback',
                headers: {
                    "Content-Type": "application/json"
                },
                data: JSON.stringify({
                    "feedbackId": 0, "reflectionId": $("#reflectionid").val(), "emailId": contextPrincipalName, "type": "", "messageId": "", action: "SaveUserFeedback", UserName: ""
                }),
                success: function (data) {
                    if (data === "true") {
                        GetReflections();
                    }
                }
            });
        });
        GetReflections();
    });
});

function Checkin() {
    if ($(".emoji-selected-img img").attr('src') === '') {
        $(".check-in").show();
        $(".selected-img").hide();
        $(".select-img").removeClass("active");
    } else {
        $(".check-in").hide();
        $(".selected-img").show();
    }
}

function GetReflections() {
    $.ajax({
        type: "GET",
        url: "/api/GetReflections/" + $("#reflectionid").val(),
        success: function (data) {
            $(".spinner").hide();
            $(".modal-mb-sc2").show();
            $(".sc2br-blk").show();
            if (data) {
                data = JSON.parse(data);
            }
            if (data && data.feedback && data.reflection && data.question) {
                feedback = JSON.parse(data.feedback);
                reflection = JSON.parse(data.reflection);
                question = JSON.parse(data.question);
                $("#createdby").text(reflection.CreatedBy);
                $("#questiontitle").text(question.Question);
                $("#privacy").text(reflection.Privacy);
                let blockdata = "";
                let color = "white";
                let totalcount = 0;
                let datacount = 0;
                let width = 0;
                let description = "";
                Object.keys(JSON.parse(data.feedback)).forEach((x) => {
                    totalcount = totalcount + feedback[x].length;
                });
                for (i = 1; i <= 5; i++) {
                    if (Object.keys(feedback).indexOf(i.toString()) !== -1) {
                        datacount = feedback[i].length;
                        description =
                            reflection.Privacy === "Anonymous – Names not displayed on reflections"
                                ? ""
                                : feedback[i].map((x) => x.FullName).join(",");
                        if (reflection.Privacy === "Creator only – Names displayed to the creator only") {
                            description =
                                userName === reflection.CreatedBy
                                ? description
                                    : "";
                        }
                        width = (datacount * 100 / totalcount).toFixed(0);
                    } else {
                        datacount = 0;
                        width = 0;
                        description = "";
                    }
                    if (i === 1) {
                        color = "green";
                        img = "Default_1.png";
                        altText = "Thrilled";
                    } else if (i === 2) {
                        color = "light-green";
                        img = "Default_2.png";
                        altText = "Happy";
                    } else if (i === 3) {
                        color = "orng";
                        img = "Default_3.png";
                        altText = "Calm";
                    } else if (i === 4) {
                        color = "red";
                        img = "Default_4.png";
                        altText = "Sad";
                    } else if (i === 5) {
                        color = "dark-red";
                        img = "Default_5.png";
                        altText = "Troubled";
                    }
                    blockdata =
                        blockdata +
                    '<div  class="media"><img src="../../../Images/' +
                        img +
                    '" class="align-self-start smils" alt="' + altText + '"><div class="media-body cb-smile2"><div class="progress custom-pr"><div class="progress-bar bg-' +
                        color +
                        '" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:' +
                        width.toString() +
                        '%"></div></div>';

                    if (description) {
                        let userNames = feedback[i].map((x) => x.FullName).join(", ");
                        blockdata = blockdata + '<div class="description-data" onclick=openDetailReflection(' + i + ',"' + reflection.ReflectionID + '")>'
                            blockdata =
                                blockdata +
                                '<span class="smile-desc" id="' +
                                data.FeedbackID +
                                '">' +
                            userNames +'</span> ';
                        blockdata = blockdata + '</div>';
                    }
                    else if (reflection.Privacy !== "Anonymous – Names not displayed on reflections" && (reflection.Privacy === "Creator only – Names displayed to the creator only" && userName === reflection.CreatedBy)) {
                        blockdata = blockdata + '<div class="no-reflections">No Reflections</div>';
                    }

                    blockdata =
                        blockdata +
                        '</div><div class="cnt-box">' +
                        width +
                        '%<span class="cnt">(' +
                        datacount +
                        ")</span></div ></div >";
                }
                $("#reviewblock").html(blockdata);
                $("#detaiilfeedbackblock").hide();
                $('.close-container').hide();
                return true;

            } else {
                alert("no data");
            }
        }
    });
}

function GetChatConfig(userId) {
    return userId === contextPrincipalName ? "none" : "all";
}

function SendFeedbackCard() {
    $('.btn-send').css("cursor", "not-allowed");
    $('.openreflection').hide();
    $('.spinner').show();
    let taskInfo = {
        reflectionID: $("#reflectionid").val(),
        action: "postAdaptivecard"
    };
    microsoftTeams.tasks.submitTask(taskInfo);
}

function openDetailReflection(feedbackId, reflectionId) {
    $('.emoji-selected').hide();
    totalcount = 0;
    Object.keys(feedback).forEach((x) => {
        totalcount = totalcount + feedback[x].length;
    });
    let datacount = feedback[feedbackId]?feedback[feedbackId].length:0;
    let width = (datacount * 100 / totalcount).toFixed(0);
    if (feedbackId === 1) {
        color = "green";
        img = "Default_1.png";
    } else if (feedbackId === 2) {
        color = "light-green";
        img = "Default_2.png";
    } else if (feedbackId === 3) {
        color = "orng";
        img = "Default_3.png";
    } else if (feedbackId === 4) {
        color = "red";
        img = "Default_4.png";
    } else if (feedbackId === 5) {
        color = "dark-red";
        img = "Default_5.png";
    }
    let blockdata ='<div class="media pb-2"><img src="../../../Images/' +
        img +
        '" class="align-self-start smils" alt="smile2"><div class="media-body cb-smile2"><div class="progress custom-pr"><div class="progress-bar bg-' +
        color +
        '" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:' +
        width.toString() +
        '%"></div></div>';

    blockdata =
        blockdata +
        '</div><div class="cnt-box">' +
        width +
        '%<span class="cnt">(' +
        datacount +
        ")</span></div ></div >";

    let chatUrl = "https://teams.microsoft.com/l/chat/0/0?users=";
    let peopledata = "";
    if (feedback[feedbackId]) {
        feedback[feedbackId].forEach((names, index) => {
            peopledata =
                peopledata + '<tr> <td class="text-left"><div class="media"><img class="align-self-center avatar" src="../../../Images/default_avatar_default_theme.png" alt="image" width="40" heigth="40"> <div class="media-body ml-3 mt-1 names">' +
            names.FullName + '</div> </div></td><td class="text-right"><div class="start-chat" style = "pointer-events: ' + GetChatConfig(names.FeedbackGivenBy) + ';"  > <span class="chat-icon" onclick = "microsoftTeams.executeDeepLink(' + "'" + chatUrl + names.FeedbackGivenBy + "'" + ');" ></span ></div > </td></tr >';
        });
        $("#peopledata").html(peopledata);
    }
    $("#reviewblock").hide();
    $("#detaiilfeedbackblock").show();
    $("#feedbackblock").html(blockdata);
    $("#doneButton").hide();
}

function closeDetailedFeedback() {
    $('.emoji-selected').show();
    $("#detaiilfeedbackblock").hide();
    $("#reviewblock").show();
    $("#doneButton").show();
}