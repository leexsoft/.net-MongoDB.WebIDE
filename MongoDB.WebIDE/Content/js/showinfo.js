$(document).ready(function () {
    var setting = {
        view: {
            dblClickExpand: false,
            showLine: true,
            selectedMulti: false
        },
        data: {
            key: {
                name: 'Name'
            },
            simpleData: {
                enable: true,
                idKey: 'ID',
                pIdKey: 'PID',
                rootPId: ''
            }
        }
    };
    var $t = $('#infotree');

    $('.accordion-body').on('show', function () {
        $(this).parent().find('i').removeClass('icon-chevron-down');
        $(this).parent().find('i').addClass('icon-chevron-up');
    }).on('hide', function () {
        $(this).parent().find('i').removeClass('icon-chevron-up');
        $(this).parent().find('i').addClass('icon-chevron-down');
    });

    //统计信息
    $('#collapseOne').on('show', function () {
        var treeObj = $.fn.zTree.getZTreeObj('infotree');
        if (!treeObj) {
            var id = $('#guid').val();
            var type = $('#type').val();
            $.ajax({
                url: '/Home/GetShowInfo/',
                type: 'POST',
                cache: false,
                data: { id: id, type: type },
                dataType: 'json',
                success: function (rst) {
                    var zNodes = rst;
                    t = $.fn.zTree.init($t, setting, zNodes);
                },
                error: function () {
                    alert('请求发生异常，请重试');
                }
            });
        }
    });

    //profile
    $('#collapseTwo').on('show', function () {
        var id = $('#guid').val();
        var type = $('#type').val();
        $.ajax({
            url: '/Home/GetPrfileInfo/',
            type: 'POST',
            cache: false,
            data: { id: id, type: type },
            dataType: 'json',
            success: function (rst) {
                $(this).find(':radio').val(rst.Status);
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });
});
