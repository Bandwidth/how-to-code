//NodeJS

var Bandwidth = require("node-bandwidth");

var client = new Bandwidth({
    userId    : "YOUR_USER_ID", // <-- note, this is not the same as the username you used to login to the portal
    apiToken  : "YOUR_API_TOKEN",
    apiSecret : "YOUR_API_SECRET"
});

client.Call.create({
    from: "+12525089000",
    to: "+15035555555",
    callbackUrl: "http://requestb.in/10sze251"
})
.then(function (id) {
    console.log(id);
})