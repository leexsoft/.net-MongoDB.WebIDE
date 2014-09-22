$(document).ready(function () {
    var $main = $("#mainFrame");

    var nodeClick = function (event, treeId, treeNode) {
        var type = treeNode.Type;
        var id = treeNode.ID;
        var pid = treeNode.PID;
        var url = "";
        if (type == 1 || type == 2 || type == 3) {
            url = "/DBAdmin/ShowInfo?id=" + id + "&type=" + type;
            $main.prop("src", url);
        } else if (type == 6) {
            url = "/DBAdmin/ShowData?id=" + pid;
            $main.prop("src", url);
        } else if (type == 7) {
             url = "/DBAdmin/ShowIndex?id=" + id;
            $main.prop("src", url);
        }
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
