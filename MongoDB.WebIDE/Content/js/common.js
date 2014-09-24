/* File Created: 九月 24, 2014 */
$.toJSONString = function (o) {
    if (typeof (JSON) == 'object' && JSON.stringify) return JSON.stringify(o);

    var _escapeable = /["\\\x00-\x1f\x7f-\x9f]/g;
    var _meta = {
        '\b': '\\b',
        '\t': '\\t',
        '\n': '\\n',
        '\f': '\\f',
        '\r': '\\r',
        '"': '\\"',
        '\\': '\\\\'
    };
    var quoteString = function (string) {
        if (string.match(_escapeable)) {
            return '"' + string.replace(_escapeable, function (a) {
                var c = _meta[a];
                if (typeof c === 'string') return c;
                c = a.charCodeAt();
                return '\\u00' + Math.floor(c / 16).toString(16) + (c % 16).toString(16);
            }) + '"';
        }
        return '"' + string + '"';
    };

    var type = typeof (o);
    if (o === null) return "null";
    if (type == "undefined") return undefined;
    if (type == "number" || type == "boolean") return o + "";
    if (type == "string") return quoteString(o);

    if (type == 'object') {
        if (typeof o.toJSONString == "function") return $.toJSONString(o.toJSONString());
        if (o.constructor === Date) {
            var month = o.getUTCMonth() + 1;
            if (month < 10) month = '0' + month;

            var day = o.getUTCDate();
            if (day < 10) day = '0' + day;

            var year = o.getUTCFullYear();

            var hours = o.getUTCHours();
            if (hours < 10) hours = '0' + hours;

            var minutes = o.getUTCMinutes();
            if (minutes < 10) minutes = '0' + minutes;

            var seconds = o.getUTCSeconds();
            if (seconds < 10) seconds = '0' + seconds;

            var milli = o.getUTCMilliseconds();
            if (milli < 100) milli = '0' + milli;
            if (milli < 10) milli = '0' + milli;

            return '"' + year + '-' + month + '-' + day + 'T' + hours + ':' + minutes + ':' + seconds + '.' + milli + 'Z"';
        }

        if (o.constructor === Array) {
            var ret = [];
            for (var i = 0; i < o.length; i++)
                ret.push($.toJSONString(o[i]) || "null");

            return "[" + ret.join(",") + "]";
        }

        var pairs = [];
        for (var k in o) {
            var name;
            var type = typeof k;

            if (type == "number") name = '"' + k + '"';
            else if (type == "string") name = quoteString(k);
            else continue;  //skip non-string or number keys

            if (typeof o[k] == "function") continue;  //skip pairs where the value is a function.

            var val = $.toJSONString(o[k]);

            pairs.push(name + ":" + val);
        }

        return "{" + pairs.join(", ") + "}";
    }
};