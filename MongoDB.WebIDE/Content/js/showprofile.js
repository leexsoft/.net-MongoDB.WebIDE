$(document).ready(function () {
    $(':radio').each(function () {
        $(this).click(function () {
            if ($(this).val() == 1) {
                $('#txtSlowms').prop('disabled', false);
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
});
