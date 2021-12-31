# cr-proxy
Custom Implementation of Reverse Proxy server

#### How it works:

Assuming you have mappings for devices like in [mappings.json](src/mappings.json). where the entity represented as below.

```
{ "From": "some_uint_id", "To": "some_host:port" }
```

Every id should be 4 bytes long. The header of the message starts at the 3-rd byte.

Proxy searches for this id in the specified map and if it exists there, forwards message to the target.

Alternatively, If noting was found and default endpoint (wildcard one) exists, the message is forwarded to the server listening on target endpoint.

Test coverage is provided only for main parts of assembly and does not covers all internal features, so there are some internal methods are covered implicitly.

The code provided as it is. There is no benchmarks or performance tests. 

#### LICENCE: MIT
