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

    var $t = $('#explaintree');

    $('#btnExplain').click(function () {
        var id = $('#hdId').val();
        var field = $('#selField option:selected').text();
        var val = $('#txtFind').val();

        if (val != '') {
            $.ajax({
                url: '/DBAdmin/Explain/',
                type: 'POST',
                cache: false,
                data: { id: id, key: field, val: val },
                dataType: 'json',
                success: function (rst) {
                    var zNodes = rst;                    
                    var ztreeObj = $.fn.zTree.init($t, setting, zNodes);
                },
                error: function () {
                    alert('请求发生异常，请重试');
                }
            });
        } else {
            alert('请输入要查找对象的值!');
        }
    });
});
