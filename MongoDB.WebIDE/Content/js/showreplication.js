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

    $.fn.zTree.init($('#servertree'), setting, zServerNodes);
    $.fn.zTree.init($('#infotree'), setting, zDataNodes);
});
