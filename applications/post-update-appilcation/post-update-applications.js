//Download the node sdk from ap.bandwidth.com/docs/helper-libraries/node-js
//API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
var userId = 'u-userID';  //{user_id}
var token = 't-token'; //{token}
var secret = 'secret'; //{secret}

var Bandwidth = require('node-bandwidth');

var client = new Bandwidth({
	userId: userId,
	apiToken: token,
	apiSecret: secret
});

// Promise
client.Application.get('a-j4f2j6vjh45wsrhzmqz53mq')
.then(function (response) {
	console.log(response);
});

// Callback
client.Application.get('a-zuwwctyxth6ju4dcfzzrbea',
	function (err, response) {
		if (err) {
			console.log(err);
		}
		else {
			console.log(response);
		}
});