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

    var $tServer = $('#servertree');
    var ztreeServer = $.fn.zTree.init($tServer, setting, zServerNodes);

    var $tData = $('#infotree');
    var ztreeData = $.fn.zTree.init($tData, setting, zDataNodes);
});
