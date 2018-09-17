//#region "getUrlParam"
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
//#endregion

layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'upload', 'element', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        element = layui.element,
        upload = layui.upload,
        $ = layui.jquery;

    var fromType = $.getUrlParam("fromType");
    var openauth = layui.openauth;

    var active = {
        btnGet: function () {
            if (fromType == 0) {
                layer.msg("本地上传文件，无需获取！");
                return;
            }
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择编辑的行，且同时只能编辑一行");
                return;
            }
            var id = rows[0].Id;
            $.post('/ChapterManager/GetChapterToLocal', { novelId: novelId, id: id }, function (data) {
                if (data.Code == 200) {
                    layer.msg("正在获取中，请稍后刷新！");
                    reloadGridOfTable();
                } else {
                    layer.msg(data.Message);
                }
            }, 'json');

        }
        , btnGetAll: function () {
            if (fromType == 0) {
                layer.msg("本地上传文件，无需获取！");
                return;
            }
            $.post('/ChapterManager/GetAllChapterToLocal', { novelId: novelId }, function (data) {
                if (data.Code == 200) {
                    layer.msg("正在获取中，请稍后刷新！");
                } else {
                    layer.msg(data.Message);
                }
            }, 'json');
        }
        , btnDel: function () {
            var rows = gridOfTable.datagrid("getChecked");
            openauth.del("/ChapterManager/DeleteNovel",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfTable();
                });
        }
        , btnRefresh: function () {
            reloadGridOfTable();
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
})

var novelId, fromType;
$(function () {
    novelId = getUrlParam("id");
    fromType = getUrlParam("fromType");
    initTable();
});

var grdEditIndexOfTable;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)

var gridOfTable;
var initTable = function () {
    gridOfTable = $("#table");
    gridOfTable.datagrid({
        url: '/ChapterManager/GetChapterList',     //获取数据地址
        queryParams: { novelId: novelId },
        border: false,
        fit: false,
        fitColumns: false,
        striped: true,
        nowrap: false,
        pagination: true,            //分页
        pageNumber: 1, //当前页
        pageSize: 25,                //当前页显示数量
        pageList: [15, 25, 35, 45],        //页显示数量
        rownumbers: true,        //行号
        singleSelect: true,
        checkOnSelect: false,
        selectOnCheck: false,
        columns: [[
            { field: 'chk', title: 'chk', checkbox: true }
            , {
                field: 'Name', title: '名称', width: 320, align: "left", halign: "center", hidden: false, formatter: function (v, r, i) {
                    return '<a href="/ChapterManager/ChapterView?id=' + r.NovelId + '&chapterId=' + r.Id + '&fromtyp=' + fromType + '" target="_blank">' + r.Name + '</a>';
                }
            }
            , {
                field: 'OriginLink', title: '源始地址', width: 180, align: "center", halign: "center", formatter: function (v, r, i) {
                    if (v) {
                        return '<a href="' + v + '" target="_blank" style="color:blue;">源始地址</a>';
                    }
                    return "无";
                }
            }
            , {
                field: 'State', title: '状态', width: 100, align: "center", halign: "center", formatter: function (v, r, i) {
                    //-1：失败，0：未开始，1：获取中，2：完成
                    var str = "";
                    switch (v) {
                        case -1:
                            str = "失败";
                            break;
                        case 0:
                            str = "未开始";
                            break;
                        case 1:
                            str = "获取中";
                            break;
                        case 2:
                            str = "完成";
                            break;
                        default:
                            str = "";
                            break;
                    }
                    return str;
                }
            }
        ]],
        onCheck: function (rowIndex, rowData) {

        },
        onClickCell: function (rowIndex, field, value) {
            return;
            var dg = $(this);
            if (grdEditIndexOfTable != undefined) {
                $("span.textbox.textbox-focused").find("input").each(function () {
                    $(this).blur();
                });
                endEditOfTable();
                return
            }
            grdEditIndexOfTable = rowIndex
            dg.datagrid('editCell', { index: rowIndex, field: field });

            //回车结束编辑
            bindPressEnterEndEditOfTable();
        },
        onClickRow: function (rowIndex, rowData) {

        },
        onAfterEdit: function (rowIndex, rowData, changes) {
            var data = JSON.stringify(changes);
            if (data != "{}" || rowData.ID == -1) {
                var jsonData = JSON.stringify(rowData);
                jsonData = base64encodeCN(jsonData);
                $.ajax({
                    type: "post",
                    url: "modifyUrl",
                    async: true,
                    data: {
                        Action: 10,
                        flag: 10,
                        jsonData: jsonData
                    },
                    success: function (data) {
                        if (data && data == "success") {
                            reloadGridOfTable();
                        } else {
                            msgAlert("提示", "保存数据失败！", "info");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        msgAlert("提示", "保存数据失败！", "info");
                    }
                })
            }
        },
        onLoadSuccess: function (data) {
            if (data.total == 0) {
                var body = $(this).data().datagrid.dc.body2;
                body.find('table tbody').append('<tr><td width="' + body.width() + '" style="height: 35px; text-align: center;"><h3>暂无数据</h3></td></tr>');
            }
        }
    })
};
function endEditOfTable() {
    if (grdEditIndexOfTable == undefined) {
        return true
    }
    if (gridOfTable.datagrid('validateRow', grdEditIndexOfTable)) {
        gridOfTable.datagrid('endEdit', grdEditIndexOfTable);
        grdEditIndexOfTable = undefined;
        return true;
    } else {
        return false;
    }
}
function bindPressEnterEndEditOfTable() {
    //回车结束编辑
    $('.datagrid-editable .textbox,.datagrid-editable .datagrid-editable-input,.datagrid-editable .textbox-text').bind('keyup', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            $("span.textbox.textbox-focused").find("input").each(function () {
                $(this).blur();
            });
            endEditOfTable();
        }
    });
}
function reloadGridOfTable() {
    gridOfTable.datagrid("reload");
}

