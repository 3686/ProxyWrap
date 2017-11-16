# ProxyWrap
Rewrite calls and record their success or fails, a bit like a proxy.

Calls that match the regular expression are re-written and sent to the destination `real.url`. 

Hostname, port and schema are changed on the request.

This lets you rewrite HTTP requests to HTTPS endpoints, so becomes useful if your load-balancer rewrites your SSL certificates for you.

## History

| URL | Description |
| --- | ----------- |
| /__admin/history | Historical items as a json object |

```javascript
[{
  "When": "2017-11-16T22:40:22.7161453+00:00",
  "ProxyResult": {
    "ProxyStatus": 1,
    "HttpStatusCode": 404,
    "OriginalUri": "http://localhost:50330/fish",
    "ProxiedUri": "https://google.com:443/fish",
    "Elapsed": "00:00:00.0240724",
    "Elipsed": "00:00:00.0240724"
  }
}, {
  "When": "2017-11-16T22:40:21.8624395+00:00",
  "ProxyResult": {
    "ProxyStatus": 1,
    "HttpStatusCode": 404,
    "OriginalUri": "http://localhost:50330/fish",
    "ProxiedUri": "https://google.com:443/fish",
    "Elapsed": "00:00:00.3676115",
    "Elipsed": "00:00:00.3676115"
  }
}, {
  "When": "2017-11-16T22:40:21.8579782+00:00",
  "ProxyResult": {
    "ProxyStatus": 1,
    "HttpStatusCode": 404,
    "OriginalUri": "http://localhost:50330/api/values",
    "ProxiedUri": "https://google.com:443/api/values",
    "Elapsed": "00:00:00.3674058",
    "Elipsed": "00:00:00.3674058"
  }
}]
```

## Configuration

Configuration comes from `appSettings.json`

```javascript
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },
  "real.url": "https://google.com",
  "match" : ".+" 
}
```
| Property | Description                                                     |
| -------- | --------------------------------------------------------------- |
| real.url | The service you are wrapping. Can be any URL                    |
| match    | A regular expression that matches the URLs you want to replace  |
|          | `__admin/history` is always reserved for this history json blob |
