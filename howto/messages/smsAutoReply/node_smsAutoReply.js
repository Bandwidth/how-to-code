//NodeJS

//Lets require/import the HTTP module, don't need anything fancy here
var http = require('http');
var url = require('url');
var Bandwidth = require("node-bandwidth");

//Lets define a port we want to listen to
var PORT=8080;

//We need a function which handles requests and send response
function handleRequest(request, response){
	var requestUrl = url.parse(request.url, true);
	var messageDetails = requestUrl.query;
	//Make sure that the we only respond to queries on the '/bxml' path name
	if (requestUrl.pathname === '/bxml' && request.method === 'GET'){
		// Create new BXML response
		var autoReply = new Bandwidth.BXMLResponse();
		// Add sendmessage verb
		autoReply.sendMessage("Auto Response", {
			// swap the to & from
			from: messageDetails.to,
			to: messageDetails.from
		});
		// Create the BXML
		var bxml = autoReply.toString();
		// Set header
		response.writeHead(200, {'Content-Type': 'text/xml'});
		// Send BXML
		response.end(bxml);
	}
	// Anything other than bxml, don't process
	else {
		response.writeHead(404);
		response.end();
	}
}

//Create a server
var server = http.createServer(handleRequest);

//Lets start our server
server.listen(PORT, function(){
    //Callback triggered when server is successfully listening. Hurray!
    console.log("Server listening on: http://localhost:%s/bxml", PORT);

});