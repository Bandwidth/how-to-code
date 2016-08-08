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

client.Bridge.list()
.then(function (response) {
	console.log(response.bridges);
	if(response.hasNextPage) {
		return response.getNextPage();
	}
	else {
		return {bridges: []};
	}
})
.then(function(response) {
	console.log(response.bridges);
});