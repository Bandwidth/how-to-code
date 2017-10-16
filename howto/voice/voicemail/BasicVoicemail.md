{% method %}

# How to Record a Voicemail

To record a voicemail, first create a call with a callback URL. When the call is rejected or the call state changes, turn the call recording on. To learn more about call states or creating a call, visit: http://dev.bandwidth.com/ap-docs/methods/calls/getCallsCallId.html

### Turn call recording on

```http
POST https://api.catapult.inetwork.com/v1/users/{userId}/calls/{callId} HTTP/1.1
Content-Type: application/json; charset=utf-8
User-Agent: BandwidthAPI/v1
{
    "recordingEnabled":"true"
}
```

### Response

```http
HTTP/1.1 200 OK
```

### Retreiving recording information

The recordings can either be retrieved through get list recording or get recoring info. See the full documentation here for the recording responses: http://dev.bandwidth.com/ap-docs/methods/recordings/recordings.html

{% sample lang="js" %}

### Turn call recording on on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```js
client.Call.enableRecording("callId").then(function (res) {});
```

### Turn call recording off on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```js
client.Call.disableRecording("callId").then(function (res) {});
```

{% sample lang="csharp" %}

### Turn call recording on on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```csharp
await client.Call.TurnCallRecordingAsync("callID", true);
```

### Turn call recording off on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```csharp
await client.Call.TurnCallRecordingAsync("callID", false);
```

{% sample lang="ruby" %}

### Turn call recording on on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```ruby
call.recording_on()
```

### Turn call recording off on a [call](http://dev.bandwidth.com/ap-docs/methods/calls/postCallsCallId.html)
```ruby
call.recording_off()
```

{% sample lang="python" %}

### Turn call recording on on a [call](http://dev.bandwidth.com/python-bandwidth/calls.html#update-call)
```python
Client.enable_call_recording('call_id')
```

### Turn call recording off on a [call](http://dev.bandwidth.com/python-bandwidth/calls.html#update-call)
```python
Client.disable_call_recording('call_id')
```

{% endmethod %}



