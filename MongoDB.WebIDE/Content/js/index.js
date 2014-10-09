$(document).ready(function () {
    var htmlHeight = $(window).outerHeight();
    var navHeight = $('.navbar').outerHeight();
    var menutopHeight = $('.site-left-top').outerHeight();
    $('.site-left').height(htmlHeight - navHeight - 30);
    $('.site-left').next('div').height(htmlHeight - navHeight - 30);
    $('#menu').css({ 'height': htmlHeight - navHeight - menutopHeight - 30, 'overflow-y': 'auto' });
    $('a[action]').css('cursor', 'pointer');

    var $main = $("#mainFrame");
    var nodeClick = function (event, treeId, treeNode) {
        var type = treeNode.Type;
        var id = treeNode.ID;
        var url = "";
        if (type == 1 || type == 2 || type == 3) {
            url = "/DBAdmin/ShowInfo?id=" + id + "&type=" + type;
            $main.prop("src", url);
        } else if (type == 6) {
            url = "/DBAdmin/ShowData?id=" + id;
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
            if (rst.Success) {
                var zNodes = rst.Result;
                zTreeObj = $.fn.zTree.init($t, setting, zNodes);
            } else {
                alert(rst.Message);
            }
        },
        error: function () {
            alert('请求发生异常，请重试');
        }
    });

    $('a[action]').click(function () {
        var action = $(this).attr('action');
        if (action == 'expand' && zTreeObj) {
            zTreeObj.expandAll(true);
        } else if (action == 'collapse' && zTreeObj) {
            zTreeObj.expandAll(false);
        } else if (action == 'refresh') {
            window.top.location.reload();
        }
    });
});