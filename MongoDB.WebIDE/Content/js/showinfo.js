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
    var ztreeObj = $.fn.zTree.init($t, setting, zNodes);
});
