let contextPrincipalName;

$(document).ready(function () {
    $(".loader").show();
    microsoftTeams.initialize();
    microsoftTeams.getContext(function (context) {
        if (context.theme === "default") {
            let head = document.getElementsByTagName("head")[0], // reference to document.head for appending/ removing link nodes
                link = document.createElement("link"); // create the link node
            link.setAttribute("href", "../CSS/view2.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else if (context.theme === "dark") {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        } else {
            let head = document.getElementsByTagName("head")[0],
                link = document.createElement("link");
            link.setAttribute("href", "../CSS/openReflections-dark.css");
            link.setAttribute("rel", "stylesheet");
            link.setAttribute("type", "text/css");
            head.appendChild(link);
        }
        contextPrincipalName = context.userPrincipalName;
    });
    GetReflections();
});

function GetReflections() {
    $.ajax({
        type: "GET",
        url: "/api/GetReflections/" + $("#reflectionid").val(),
        success: function (data) {
            $(".loader").hide();
            $(".modal-mb-sc2").show();
            $(".sc2br-blk").show();
            if (data) {
                data = JSON.parse(data);
            }
            if (data && data.feedback && data.reflection && data.question) {
                let feedback = JSON.parse(data.feedback);
                let reflection = JSON.parse(data.reflection);
                let question = JSON.parse(data.question);
                $("#createdby").text(reflection.CreatedBy);
                $("#questiontitle").text(question.Question);
                $("#privacy").text(reflection.Privacy);
                let blockdata = "";
                let peopledata = "";
                let color = "white";
                let totalcount = 0;
                let datacount = 0;
                let width = 0;
                let descriptio = "";
                let chatUrl = "https://teams.microsoft.com/l/chat/0/0?users=";
                Object.keys(JSON.parse(data.feedback)).forEach((x) => {
                    totalcount = totalcount + feedback[x].length;
                });
                for (i = 1; i <= 5; i++) {
                    if (i === $("#feedbackId").val()) {
                    if (Object.keys(feedback).indexOf(i.toString()) !== -1) {
                        datacount = feedback[i].length;
                        description =
                            reflection.Privacy === "anonymous"
                                ? ""
                                : feedback[i].map((x) => x.FullName).join(",");
                        width = (datacount * 100 / totalcount).toFixed(0);
                    } else {
                        datacount = 0;
                        width = 0;
                        description = "";
                    }
                    if (i === 1) {
                        color = "green";
                        img = "Default_1.png";
                    } else if (i === 2) {
                        color = "light-green";
                        img = "Default_2.png";
                    } else if (i === 3) {
                        color = "orng";
                        img = "Default_3.png";
                    } else if (i === 4) {
                        color = "red";
                        img = "Default_4.png";
                    } else if (i === 5) {
                        color = "dark-red";
                        img = "Default_5.png";
                    }
                    blockdata =
                            blockdata + 
                    '<div class="media"><img src="../../Images/' +
                        img +
                        '" class="align-self-start smil" alt="smile1"><div class="media-body cb-smile"><div class="progress custom-pr"><div class="progress-bar bg-' +
                        color +
                        '" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:' +
                        width.toString() +
                    '%"></div></div><div class="cnt-box box1">' + width + '% ('+datacount+')</div></div>';

                        if (description) {
                            feedback[i].forEach((names, index) => {
                                peopledata =
                                    peopledata + '<tr> <td class="text-left"><div class="media"><img class="align-self-center avatar" src="../../Images/default_avatar_default_theme.png" alt="image" width="40" heigth="40"> <div class="media-body ml-3 mt-1 names">' +
                                    names.FullName + '</div> </div></td><td class="text-right"></td></tr >';
                            });
                        }
                  }
                }
                $("#feedbackblock").html(blockdata);
                $("#peopledata").html(peopledata);

            } else {
                alert("no data");
            }

        }
    });
}
function GetChatConfig(userId) {
    return userId === contextPrincipalName ? "none" : "all";
}
