## LiveReload Internet Explorer Extension

### About LiveReload

LiveReload is a collection of server-side and browser-side components that make browser refresh the page or reload the styling every time files on the server change. This is a handy little trick that is very useful for web development. See [LiveReload's site](http://livereload.com/) for details on the original product.

### About LiveReload Internet Explorer Extension

LiveReload Internet Explorer Extension plays two important roles:

Original LiveReload server and browser-side components communicate over WebSockets - a "HTML5" technology that is not present in IE older than v.10. LiveReload Internet Explorer Extension injects [needed components](https://github.com/gimite/web-socket-js) into IE's browser environment that allow pre-v.10 IE to talk over WebSockets.

Even with the working implementation of WebSocket the "LiveReload experience" would not be complete if the browser does not reconnect to the server after each reload. LiveReload Internet Explorer Extension detects cases where it thinks the page is coming from a development server and automatically tries to connect this page to a LiveReload server.

### How LiveReload Internet Explorer Extension works

The LiveRealod Internet Explorer Extension detects if the page is coming from a development server is the following way:

- The page must be coming from "http" or "https" protocols.
- If the hostname is 'localhost' it's a development server.
- If the hostname starts from 'test.', it's a development server.
- If the page is coming from a port that is higher than 1024, it's a development server.

If "development server" is inferred per above rules, LRIEE will execute the following scripts within the DOM of the page:
- if LiveReload and WebSocket and SWFObject are not present, set up SWFObject
- if LiveReload and WebSocket is not present, set up WebSocket Flash-based emulator.
- at this stage if WebSocket Flash-based emulation initializer detects a need to download WebSocket emulator SWF component, it does so.
- if LiveReload is not present, append a script tag to page's body that loads LiveReload from the server per [LiveReload server spec](http://feedback.livereload.com/knowledgebase/articles/86174-livereload-protocol)

The net result of these steps is such that on IE versions lacking WebSocket implementation a Flash-based WebSocket emulator is loaded from a compatible server. On IE 10, which supports WebSockets natively, no loading of Flash and no special compatible server is needed.

### LiveReload Internet Explorer Extension requirements

LiveReload Internet Explorer Extension is a Browser Helper Object (BHO) written in C# and relies on [Microsoft .Net Framework version **4.0**](http://www.microsoft.com/net/download/) to be installed on all computers where this browser extension is to be installed. Luckily .Net v4.0 is [available for versions of Windows from XP and up](http://msdn.microsoft.com/en-US/library/8z6watww(v=vs.100).aspx) and should effectively cover the range of Windows versions a modern, reasonable developer would want to test for.

LiveReload Internet Explorer Extension on pre-v10 IE also relies on special server to help it get WebSocket working:

If WebSocket is not already otherwise patched into the page, WebSocket emulator's JavaScript layer that LiveReload Internet Explorer Extension injects will need to download websocket_emulator.swf from the compatible server.

Establishing a WebSocket connection from an instance of Flash to a domain that is different from the one of the page requires [Flash-specific secret handshake with the server to which the socket connects](http://www.lightsphere.com/dev/articles/flash_socket_policy.html). This "secret handshake" is a "policy" TCP request to the WebSocket's port.

Because of the need for the WebSocket emulator SWF download and for "Flash secret handshake", all present (as of Oct 2012) LiveReload servers would **NOT** support pre-v10 IEs.

(Partly because of this, ) a new, alternative, cross-platform, [LiveReload-protocol-supporting development automation server is now available - **PFKaplr**](https://github.com/dvdotsenko/pfkaplr). This personal web development automation server supports the Flash secret handshake and the additional serving of the WebSocket emulation to old IEs.

### Getting, Installing LiveReload Internet Explorer Extension

#### .Net 4.0

Insure that you have Microsoft .Net Framework 4.0 (or 4.5) installed. .Net v4.0 comes in two varieties - "Client" and "Full". Both are supported.

LiveReload Internet Explorer Extension works on both, 64 and 32 versions of Internet Explorer. However, you need to insure that the version of .Net you install includes the 64bit components. The super-light version of .Net 4.0 "Client" comes only in 32bit variety. Insure that you have "Client" that is both, 32 and 64 bit, or, just go for "Full", which includes both sets of libraries.

#### Choose and install the extension

Look at the [Downloads](https://github.com/dvdotsenko/livereload_ie_extension/downloads) section and download the installer fitting your system. The "64bit" installer includes extensions for both, the 64 and 32 bit versions of IE. However, if used on pure 32bit Windows, that installer may not work properly. So, for pure 32bit Windows choose "32bit" installer.

#### Get compatible LiveReload-supporting server

Get the [**PFKaplr** - a LiveReload-protocol-supporting development automation server](https://github.com/dvdotsenko/pfkaplr) that supports the Flash secret handshake and the additional serving of the WebSocket emulation to old IEs.
