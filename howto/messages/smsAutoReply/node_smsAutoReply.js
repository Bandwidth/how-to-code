//NodeJS

//Lets require/import the HTTP module, don't need anything fancy here
var express = require('express');
var Bandwidth = require("node-bandwidth");

var PORT = 8080;
var app = express();

app.get('/bxml', function (req, res) {
	var messageDetails = req.query;
	var autoReply = new Bandwidth.BXMLResponse();
	// Add sendmessage verb
	autoReply.sendMessage("Auto Response", {
		// swap the to & from
		from: messageDetails.to,
		to: messageDetails.from
	});
	// Create the BXML
	var bxml = autoReply.toString();
	res.set('Content-Type', 'text/xml');
	res.send(bxml);
});

app.listen(PORT, function () {
	console.log('Server listening on: http://localhost:%s/bxml', PORT);
});