//NodeJS

var Bandwidth = require("node-bandwidth");

var client = new Bandwidth({
	userId    : "{{userId}}",
	apiToken  : "{{apiToken}}",
	apiSecret : "{{apiSecret}}"
});

// Need to use the id of the new application when ordering phone number
var appId;

//Parameters for creating new application
var appPayload = {
	name: "MyFirstApp", // Name
	incomingCallUrl: "http://example.com/calls", //Callback URL for phone calls
	incomingMessageUrl : "http://example.com/messages" //Callback URL for messages
};

//Create the application
client.Application.create(appPayload)
.then(function (response) {
	appId = response.id;
	console.log("Application ID: " + appId);
	return client.Application.get(appId);
})
.then(function (application) {
	console.log("Application Name: " + application.name);
	//Search for phone numbers in North Carolina
	return client.AvailableNumber.search("local", {state:"NC"});
})
.then(function (availableNumbers) {
	//Parameters for ordering the phone number
	var phoneNumberPayload = {
		//Be sure to set the applicationId to the id of application created
		applicationId : appId,
		number : availableNumbers[0].number,
		name : availableNumbers[0].nationalNumber
	}
	//Order the phone number
	return client.PhoneNumber.create(phoneNumberPayload);
})
.then(function (phoneNumber) {
	console.log("Phone Number ID: " + phoneNumber.id);
})
.catch(function (e) {
	console.error(e);
	throw e;
});

