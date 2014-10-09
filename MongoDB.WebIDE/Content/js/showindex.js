$(document).ready(function () {
    $('[action]').click(function () {
        if (window.confirm('确认需要删除此索引吗？')) {
            var id = $('#hdId').val();
            var idx = $(this).attr('idx');
            $.ajax({
                url: '/DBAdmin/DeleteIndex/',
                type: 'POST',
                cache: false,
                data: { id: id, idx: idx },
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

    $('#btnAddColumn').click(function () {
        var field = $('#selField option:selected').text();
        var orderText = $('#selOrder option:selected').text();
        var orderVal = $('#selOrder option:selected').val();

        if (field != '') {
            $('#tblColumn').append('<tr><td>' + field + '</td><td value=' + orderVal + '>' + orderText + '</td><td action="del"><a>删除</a></td></tr>');
        } else {
            alert('请输入要添加的列!');
        }
    });

    $('#tblColumn').on('click', '[action]', function () {
        $(this).closest('tr').remove();
    });

    $('#btnCreate').click(function () {
        var $trs = $('#tblColumn tr').not(':first');
        if ($trs.length == 0) {
            alert('请选择要生成索引的列');
            return false;
        }

        var id = $('#hdId').val();
        var keys = [];
        $trs.each(function () {
            var $tds = $(this).find('td');
            var field = $tds.eq(0).text().split(' ')[0];
            var order = $tds.eq(1).attr('value');
            keys.push({ Field: field, Order: order });
        });

        var unique = $('#cbUnique').prop('checked');
        var background = $('#cbBackground').prop('checked');
        var dropDups = $('#cbDropDups').prop('checked');

        var para = {
            Keys: keys,
            Unique: unique,
            Backgroud: background,
            Dropdups: dropDups
        };

        $.ajax({
            url: '/DBAdmin/CreateIndex/',
            type: 'POST',
            cache: false,
            data: { id: id, data: $.toJSONString(para) },
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
