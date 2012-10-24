;(function(beenhere){
	'use strict'
	window.LiveReload_Already_Tried_Loading = true
	//this appends a script tag to body with scr pointing to livereload.js
	if (!beenhere && !window.LiveReload && window.WebSocket) {

		var script = window.document.createElement('script')
		script.src = window.location.protocol + '//' + window.location.hostname + ':35729/livereload.js'
		window.document.body.appendChild(script)
	}
	if (window.console && window.console.log) {
		window.console.log("LiveReload Loading...")
	}
})(window.LiveReload_Already_Tried_Loading);
