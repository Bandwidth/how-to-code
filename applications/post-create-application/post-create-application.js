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
client.Application.create({
	name: 'SampleApp',
	incomingCallUrl: 'http://your-server.com/CallCallback',
	incomingMessageUrl: 'http://your-server.com/MsgCallback'
})
.then(function (response) {
	console.log(response);
});

// Callback
client.Application.create({
	name: 'SampleApp2',
	incomingCallUrl: 'http://your-server.com/CallCallback',
	incomingMessageUrl: 'http://your-server.com/MsgCallback'
}, function (err, response) {
	if (err) {
		console.log(err);
	}
	else {
		console.log(response);
	}
});