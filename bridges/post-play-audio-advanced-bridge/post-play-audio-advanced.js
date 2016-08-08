//Download the node sdk from ap.bandwidth.com/docs/helper-libraries/node-js
//API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
var userId = 'userId';  //{user_id}
var token = 'token'; //{token}
var secret = 'secret'; //{secret}

var Bandwidth = require('node-bandwidth');

var client = new Bandwidth({
	userId: userId,
	apiToken: token,
	apiSecret: secret
});

//Play Audio File on loop
var options = {
	fileUrl     : "http://myurl.com/file.mp3",
	loopEnabled : true
}
//Promise
client.Bridge.playAudioAdvanced("bridgeId", options)
.then(function (response) {
	console.log(response);
});

//Callback
client.Bridge.playAudioAdvanced("bridgeId", options, function (err, response) {
	if(err) {
		console.log(err);
	}
	else {
		console.log(response);
	}
});

//Speak sentence with options
var options = {
	sentence : "hola de Bandwidth",
	gender   : "male",
	locale   : "es",
	voice    : "Jorge"
}
//Promise
client.Bridge.playAudioAdvanced("bridgeId", options)
.then(function (response) {
	console.log(response);
});

//Callback
client.Bridge.playAudioAdvanced("bridgeId", options, function (err, response) {
	if(err) {
		console.log(err);
	}
	else {
		console.log(response);
	}
});