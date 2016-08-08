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

//Promise
client.Bridge.speakSentence("bridgeID", "Hello From Bandwidth")
.then(function (response) {
	console.log(response);
});

//Callback
client.Bridge.speakSentence("bridgeID", "Hello From Bandwidth",
	function (err, response) {
		if(err) {
			console.log(err);
		}
		else {
			console.log(numbers);
		}
	});