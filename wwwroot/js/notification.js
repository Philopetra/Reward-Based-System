const connection = new signalR.HubConnectionBuilder()
    .withUrl('Notification-Hub')
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveNotification", function (notification) {
    var NumberNot = document.getElementById("notificationNumber");
    var notificationNumber = parseInt(NumberNot.innerHTML);
    NumberNot.innerHTML = notificationNumber+1;

    const notificationItem = document.createElement("div");
    notificationItem.innerHTML = `<p onclick='handleNotificationClick(this)'id ='INotifyU${+fireBaseNotfication.notificationId}'>${fireBaseNotfication.messages} + "<br> <span class='ogm-day-time pt'>${fireBaseNotfication.dateTime}</span> </p>`;
    notificationItem.style.backgroundColor = "#D9D6FE";
    notificationItem.className += "ryt-row ryt-padding ryt-margin-bottom";
    document.getElementById("notificationContainer").prepend(notificationItem);
});

connection.start().then(function () {
    console.log("SignalR connection established.");
}).catch(function (err) {
    console.error("Error establishing SignalR connection: ", err.toString());
});
