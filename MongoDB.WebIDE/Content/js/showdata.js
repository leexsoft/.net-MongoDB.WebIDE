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

    $('#chkLimit').click(function () {
        if ($(this).prop('checked')) {
            $('#txtLimit').prop('disabled', false);
        } else {
            $('#txtLimit').prop('disabled', true);
        }
    });

    $('#btnFind').click(function () {
        var id = $('#hdId').val();
        var find = $('#txtFind1').val();
        var sort = $('#txtSort1').val();
        var skip = parseInt($('#txtSkip').val(), 10) || 0;
        var limit = 0;
        if (!$('#chkLimit').prop('disabled')) {
            limit = parseInt($('#txtLimit').val(), 10) || 0;
        }

        $.ajax({
            url: '/DBAdmin/GetData/',
            type: 'POST',
            cache: false,
            data: { id: id, jsonfind: find, jsonsort: sort, skip: skip, limit: limit },
            dataType: 'json',
            success: function (rst) {
                $('table tr').not(':first').remove();
                $('#collapseOne>.accordion-inner').addClass('datatable');

                var list = $.parseJSON(rst);
                var i = 0;
                for (i = 0; i < list.length; i++) {
                    var html = '<tr>';
                    var item = list[i];
                    var j = 0;
                    for (j = 0; j < item.length; j++) {
                        html += '<td>'+item[j].Value+'</td>'
                    }
                    html += '</tr>';
                    $('table').append(html);
                }
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });

    var $t = $('#explaintree');
    $('#btnExplain').click(function () {
        var id = $('#hdId').val();
        var find = $('#txtFind2').val();
        var sort = $('#txtSort2').val();

        $.ajax({
            url: '/DBAdmin/Explain/',
            type: 'POST',
            cache: false,
            data: { id: id, jsonfind: find, jsonsort: sort },
            dataType: 'json',
            success: function (rst) {
                var zNodes = rst;
                var ztreeObj = $.fn.zTree.init($t, setting, zNodes);
            },
            error: function () {
                alert('请求发生异常，请重试');
            }
        });
    });
});
