$(document).ready(function () {
    $('[action]').click(function () {
        if (window.confirm('确认需要删除此服务器吗？')) {
            var ip = $(this).attr('ip');
            var port = $(this).attr('port');
            $.ajax({
                url: '/Home/DeleteServer/',
                type: 'POST',
                cache: false,
                data: { ip: ip, port: port },
                dataType: 'json',
                success: function (rst) {
                    alert(rst.Message);
                    if (rst.Success) {
                        window.top.location.reload();
                    }
                },
                error: function () {
                    alert('请求发生异常，请重试');
                }
            });
        }
    });

    $('#btnSave').click(function () {
        var ip = $('#txtIP').val();
        var port = $('#txtPort').val();

        if (ip.length == 0) {
            alert('IP必填');
            return false;
        }
        if (port.length == 0) {
            alert('端口必填');
            return false;
        }

        $.ajax({
            url: '/Home/AddServer/',
            type: 'POST',
            cache: false,
            data: { ip: ip, port: port },
            dataType: 'json',
            success: function (rst) {
                alert(rst.Message);
                if (rst.Success) {
                    window.top.location.reload();
                }
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });
});
