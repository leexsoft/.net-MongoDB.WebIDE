$(document).ready(function () {
    var $main = $("#mainFrame");

    var nodeClick = function (event, treeId, treeNode) {
        var id = treeNode.ID;
        var type = treeNode.Type;
        var url = "/Home/ShowInfo?id=" + id + "&type=" + type;
        $main.prop("src", url);
    };

    var setting = {
        view: {
            dblClickExpand: false,
            showLine: true,
            selectedMulti: false
        },
        data: {
            key: {
                name: "Name"
            },
            simpleData: {
                enable: true,
                idKey: "ID",
                pIdKey: "PID",
                rootPId: ""
            }
        },
        callback: {
            onClick: nodeClick
        }
    };

    var $t = $("#tree");
    $.ajax({
        url: '/Home/GetServerDetail/',
        type: 'POST',
        cache: false,
        dataType: 'json',
        success: function (rst) {
            var zNodes = rst;
            t = $.fn.zTree.init($t, setting, zNodes);
        },
        error: function () {
            alert('请求发生异常，请重试');
        }
    });
});
