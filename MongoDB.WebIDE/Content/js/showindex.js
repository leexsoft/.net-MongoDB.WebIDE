$(document).ready(function () {
    $('#btnAddColumn').click(function () {
        var field = $('#txtField').val();
        var orderText = $('#selOrder option:selected').text();
        var orderVal = $('#selOrder option:selected').val();

        if (field != '') {
            $('#tblColumn').append('<tr><td>' + field + '</td><td value=' + orderVal + '>' + orderText + '</td><td><a action="del">删除</a></td></tr>');
        } else {
            alert('请输入要添加的列!');
        }
    })
});
