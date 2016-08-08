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


client.Account.get()
	.then(function(response) {
		console.log(response);
	});