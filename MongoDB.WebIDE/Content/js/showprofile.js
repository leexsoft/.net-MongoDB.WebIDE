$(document).ready(function () {
    $(':radio').each(function () {
        $(this).click(function () {
            if ($(this).val() == 1) {
                $('#txtSlowms').prop('disabled', false);
            } else {
                $('#txtSlowms').prop('disabled', true);
            }
        });
    });

    $('#btnSetProfile').click(function () {
        var id = $('#hdId').val();
        var level = $(':radio:checked').val();
        var slowms = $('#txtSlowms').val();

        if (level == 1 && slowms.length == 0) {
            alert('开启慢命令时，慢命令时间必填');
            return false;
        }

        $.ajax({
            url: '/DBAdmin/SetProfile/',
            type: 'POST',
            cache: false,
            data: { id: id, level: level, slowms: slowms },
            dataType: 'json',
            success: function (rst) {
                alert(rst.Message);
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });

    var jsonDateParser = function (value) {
        if (typeof value === 'string') {
            var pattern = /\/Date\((\d+)\)\//;
            var a = value.match(pattern);
            if (a) {
                var utcMilliseconds = parseInt(a[1], 10);
                return new Date(utcMilliseconds);
            }
        }
        return value;
    };

    $('#btnShow').click(function () {
        var id = $('#hdId').val();
        var limit = $('#txtLimit').val();

        if (limit.length == 0) {
            alert('查看条数必填');
            return false;
        }

        $.ajax({
            url: '/DBAdmin/GetProfileData/',
            type: 'POST',
            cache: false,
            data: { id: id, limit: limit },
            dataType: 'json',
            success: function (rst) {
                if (rst.Success) {
                    $('table tr:not(:first)').remove();
                    $('#collapseTwo>.accordion-inner').addClass('datatable');
                    var list = rst.Result;
                    var i = 0;
                    for (i = 0; i < list.length; i++) {
                        var datetime = jsonDateParser(list[i].Timestamp);
                        var html = '<tr>' +
                                   '<td>' + list[i].Client + '</td>' +
                                   '<td>' + list[i].Op + '</td>' +
                                   '<td>' + list[i].Namespace + '</td>' +
                                   '<td>' + list[i].Command + '</td>' +
                                   '<td>' + datetime.toLocaleDateString() + ' ' + datetime.toLocaleTimeString() + '</td>' +
                                   '<td>' + list[i].Duration + '</td>' +
                                   '<td>' + list[i].NumberToReturn + '</td>' +
                                   '<td>' + list[i].NumberScanned + '</td>' +
                                   '<td>' + list[i].NumberReturned + '</td>' +
                                   '<td>' + list[i].ResponseLength + '</td>' +
                                   '</tr>';
                        $('table').append(html);
                    }
                } else {
                    alert(rst.Message);
                }
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });
});
