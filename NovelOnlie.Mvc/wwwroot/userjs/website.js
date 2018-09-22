var element;
layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'upload', 'element', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        upload = layui.upload,
        $ = layui.jquery;

    element = layui.element;
    var openauth = layui.openauth;

    $("#menus").loadMenus("WebsiteList");

    //#region "添加对话框"
    var addDlg = function () {
        var update = false;  //是否为更新
        var show = function (data) {
            var title = "添加";
            layer.open({
                title: title,
                area: ["500px", "230px"],
                type: 1,
                content: $('#divAdd'),
                success: function () {

                },
                end: function () {
                }
            });
            var url2 = "/NovelManager/SearchWebsite";

            form.on('submit(submint_website)',
                function (data) {
                    $.post(url2,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                layer.closeAll();
                                reloadGridOfTable();
                            }
                        },
                        "json");
                    return false;
                });
        }
        return {
            add: function () { //弹出编辑框
                show();
            }
        };
    }();
    //#endregion

    //#region "编辑对话框"
    var editDlg = function () {
        var vm = new Vue({
            el: "#formEdit"
        });
        var show = function (data) {
            var title = "编辑Website";
            layer.open({
                title: title,
                area: ["500px", "150px"],
                type: 1,
                content: $('#divEdit'),
                success: function () {
                    vm.$set('$data', data);
                },
                end: function () {

                }
            });
            var url = "/WebsiteManager/ModifyWebsite";

            //提交数据
            form.on('submit(formSubmit)',
                function (data) {
                    $.post(url,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                layer.closeAll();
                                reloadGridOfTable();
                            } else {
                                return layer.msg('编辑失败');
                            }
                        },
                        "json");
                    return false;
                });
        }
        return {
            update: function (data) { //弹出编辑框
                show(data);
            }
        };
    }();
    //#endregion

    var active = {
        btnAdd: function () {
            addDlg.add();
        }
        , btnEdit: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择编辑的行，且同时只能编辑一行");
                return;
            }
            editDlg.update(rows[0]);
        }
        , btnDel: function () {
            var rows = gridOfTable.datagrid("getChecked");
            openauth.del("/WebsiteManager/DeleteWebsite",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfTable();
                });
        }
        , btnReload: function () {
            reloadGridOfTable();
        }
        , btnStart: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择操作的行，且同时只能选择一行");
                return;
            }
            var row = rows[0];
            var state = row.State;
            if ($.inArray(state, [1, 2]) != -1) {
                layer.msg("当前状态无法执行此操作！");
                return;
            }
            var Id = row.Id;
            $.post('/WebsiteManager/SwitchWebsite', { websiteId: Id, state: 1 }, function (data) {
                if (data.Code == 200) {
                    reloadGridOfTable();
                } else {
                    layer.msg("操作失败！");
                }
            }, 'json');
        }
        , btnPause: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择操作的行，且同时只能选择一行");
                return;
            }
            var row = rows[0];
            var state = row.State;
            if (state != 1) {
                layer.msg("当前状态无法执行此操作！");
                return;
            }
            var Id = row.Id;
            $.post('/WebsiteManager/SwitchWebsite', { websiteId: Id, state: 3 }, function (data) {
                if (data.Code == 200) {
                    reloadGridOfTable();
                } else {
                    layer.msg("操作失败！");
                }
            }, 'json');
        }
        , btnStop: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择操作的行，且同时只能选择一行");
                return;
            }
            var row = rows[0];
            var state = row.State;
            if (state != 1) {
                layer.msg("当前状态无法执行此操作！");
                return;
            }
            var Id = row.Id;
            $.post('/WebsiteManager/SwitchWebsite', { websiteId: Id, state: 4 }, function (data) {
                if (data.Code == 200) {
                    reloadGridOfTable();
                } else {
                    layer.msg("操作失败！");
                }
            }, 'json');
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
});

$(function () {
    initTable();
});

var grdEditIndexOfTable;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)

var gridOfTable;
var initTable = function () {
    gridOfTable = $("#table");
    gridOfTable.datagrid({
        url: '/WebsiteManager/GetResourceList',     //获取数据地址
        queryParams: {},
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
                    return '<a href="###" style="color:blue;" onclick="opentWebsiteNovelList(' + i + '); return;">' + v + '</a>';
                }
            }
            , {
                field: 'OriginLink', title: '源始地址', width: 180, align: "center", halign: "center", formatter: function (v, r, i) {
                    return '<a href="' + v + '" target="_blank" style="color:blue;">' + "源地址" + '</a>';
                }
            }
            , {
                field: 'State', title: '状态', width: 100, align: "center", halign: "center", formatter: function (v, r, i) {
                    var txtState = "";
                    switch (v) {
                        case -1:
                            txtState = "获取失败！";
                            break;
                        case 0:
                            txtState = "未获取";
                            break;
                        case 1:
                            txtState = "获取中";
                            break;
                        case 2:
                            txtState = "已获取";
                            break;
                        case 3:
                            txtState = "暂停获取";
                            break;
                        case 4:
                            txtState = "停止获取";
                            break;
                        default:
                            txtState = "其它";
                            break;
                    }
                    return txtState;
                }
                , hidden: false
            }
            , { field: 'CreatorId', title: '创建人', width: 150, align: "center", halign: "center", hidden: true }
            , { field: 'CreateDate', title: '创建时间', width: 150, align: "center", halign: "center" }
        ]],
        onCheck: function (rowIndex, rowData) {
            setSwitchBtn();
        },
        onClickCell: function (rowIndex, field, value) {
            return;
            var dg = $(this);
            if (grdEditIndexOfTable != undefined) {
                $("span.textbox.textbox-focused").find("input").each(function () {
                    $(this).blur();
                });
                endEditOfTable();
                return;
            }
            grdEditIndexOfTable = rowIndex
            dg.datagrid('editCell', { index: rowIndex, field: field });

            //回车结束编辑
            bindPressEnterEndEditOfTable();
        },
        onClickRow: function (rowIndex, rowData) {
            setSwitchBtn();
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

function opentWebsiteNovelList(index) {
    var row = gridOfTable.datagrid("selectRow", index).datagrid("getSelected");
    if (row) {
        var title = '<cite>' + row.Name + '</cite><i class="layui-icon layui-unselect layui-tab-close" data-id="2">ဆ</i>';
        var id = row.Id;
        var content = "<iframe src='/WebsiteNovel/Index?id=" + id + "' data-id='" + id + "'></frame>";
        var tabFilter = 'bodyTab';
        if (window.top.tab.hasTab(row.Name) == -1) {
            window.top.element.tabAdd(tabFilter, {
                title: title
                , content: content //支持传入html
                , id: id
            });
            window.top.element.tabChange(tabFilter, row.Id);
        } else {
            window.top.element.tabChange(tabFilter, row.Id);
        }
    }
}

function reGetWebsite(index) {
    var rows = gridOfTable.datagrid("selectRow", index).datagrid("getChecked");
    if (!rows || rows.length != 1) {
        layer.msg("请选择编辑的行，且同时只能编辑一行");
        return;
    }
    var websiteId = rows[0].Id;
    $.post('/WebsiteManager/ReGetWebsite', { websiteId: websiteId }, function (data) {
        if (data.Code == 200) {
            layer.msg("正在重新获取，请稍后刷新！");
            reloadGridOfTable();
        } else {
            layer.msg("重新获取失败！");
        }
    }, 'json');
}

function setSwitchBtn() {
    var rows = gridOfTable.datagrid("getChecked");
    var selRow = gridOfTable.datagrid("getSelected");

    if (!rows || rows.length != 1) {
        if (!selRow) {
            return;
        }
    }
    var row = rows.length == 1 ? rows[0] : selRow;
    var btnStart = $("#btnStart");
    var btnPause = $("#btnPause");
    var btnStop = $("#btnStop");

    btnStart.addClass("layui-btn-disabled");
    btnPause.addClass("layui-btn-disabled");
    btnStop.addClass("layui-btn-disabled");
    switch (row.State) {
        case -1:
            btnStart.removeClass("layui-btn-disabled");
            break;
        case 0:
            btnStart.removeClass("layui-btn-disabled");
            break;
        case 1:
            btnPause.removeClass("layui-btn-disabled");
            btnStop.removeClass("layui-btn-disabled");
            break;
        case 2:

            break;
        case 3:
            btnStart.removeClass("layui-btn-disabled");
            btnStop.removeClass("layui-btn-disabled");
            break;
        case 4:
            btnStart.removeClass("layui-btn-disabled");
            break;
        default:
            break;
    }
}

