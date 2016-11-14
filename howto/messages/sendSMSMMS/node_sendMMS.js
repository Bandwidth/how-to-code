// install sdk: npm install node-bandwidth

var Bandwidth = require("node-bandwidth");
var client = new Bandwidth({
    userId    : "YOUR_USER_ID", // <-- note, this is not the same as the username you used to login to the portal
    apiToken  : "YOUR_API_TOKEN",
    apiSecret : "YOUR_API_SECRET"
});
var message = {
    from: "+19195551212",  // <-- This must be a Bandwidth number on your account
    to: "+191955512142",
    text: "Test",
    media: ["https://s3.amazonaws.com/bwdemos/logo.png"]
};

//Use Promises
client.Message.send(message)
.then(function(message) {
    console.log("Message sent with ID " + message.id);
})
.catch(function(err) {
    console.log(err.message);
});

//Use callbacks
client.Message.send(message, function(err, message) {
    if (err) {
        console.log(err);
        return;
    }
    console.log("Message sent with ID " + message.id);
});