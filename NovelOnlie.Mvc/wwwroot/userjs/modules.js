layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        $ = layui.jquery;

    var openauth = layui.openauth;

    $("#menus").loadMenus("Modules");

    //#region "编辑模块对话框"
    var editDlg = function () {
        var vm = new Vue({
            el: "#formEdit"
        });
        var update = false;  //是否为更新
        var show = function (data) {
            var title = update ? "编辑模块" : "添加模块";
            layer.open({
                title: title,
                area: ["500px", "430px"],
                type: 1,
                content: $('#divEdit'),
                success: function () {
                    vm.$set('$data', data);
                },
                end: function () {

                }
            });
            var url = "/ModuleManager/Add";
            if (update) {
                url = "/ModuleManager/Update";
            }

            //提交数据
            form.on('submit(formSubmit)',
                function (data) {
                    $.post(url,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                reloadTree();
                                layer.closeAll();
                            }
                        },
                        "json");
                    return false;
                });
        }
        return {
            add: function () { //弹出添加
                update = false;
                show({
                    Id: "",
                    SortNo: 1,
                    IconName: '&#xe678;'
                });
            },
            update: function (data) { //弹出编辑框
                update = true;
                show(data);
            }
        };
    }();
    //#endregion

    //#region "添加菜单对话框"
    var meditDlg = function () {
        var vm = new Vue({
            el: "#mfromEdit"
        });
        var update = false;  //是否为更新
        var show = function (data) {
            var title = update ? "编辑菜单" : "添加菜单";
            layer.open({
                title: title,
                area: ["500px", "430px"],
                type: 1,
                content: $('#divMenuEdit'),
                success: function () {
                    vm.$set('$data', data);
                },
                end: function () { }
            });
            var url = "/moduleManager/AddMenu";
            if (update) {
                url = "/moduleManager/UpdateMenu";
            }
            //提交数据
            form.on('submit(mformSubmit)',
                function (data) {
                    $.post(url,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                reloadGridOfBtnGrid();
                                layer.closeAll();
                            }
                        },
                        "json");
                    return false;
                });
        }
        return {
            add: function (moduleId) { //弹出添加
                update = false;
                show({
                    Id: "",
                    ModuleId: moduleId,
                    Sort: 1
                });
            },
            update: function (data) { //弹出编辑框
                update = true;
                show(data);
            }
        };
    }();
    //#endregion

    var active = {
        btnAdd: function () {
            editDlg.add();
            $("#cmbMdTree").combotree("setValue", "");
        }
        , btnEdit: function () {
            var rows = gridOfTreeGrid.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择编辑的行，且同时只能编辑一行");
                return;
            }
            editDlg.update(rows[0]);
            $("#cmbMdTree").combotree("setValue", rows[0].ParentId);
        }
        , btnDel: function () {
            var rows = gridOfTreeGrid.datagrid("getChecked");
            openauth.del("/ModuleManager/Delete",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadTree();
                });
        }
        , btnAddMenu: function () {
            var rows = gridOfTreeGrid.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择一个要添加菜单的模块");
                return;
            }
            meditDlg.add(rows[0].Id);
        }
        , btnEditMenu: function () {
            var rows = gridOfBtnGrid.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择编辑的行，且同时只能编辑一行");
                return;
            }
            meditDlg.update(rows[0]);
        }
        , btnDelMenu: function () {
            var rows = gridOfBtnGrid.datagrid("getChecked");
            openauth.del("/ModuleManager/",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfBtnGrid();
                });
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
})

$(function () {
    initTree();
    initTreeGrid();
    initBtnGrid();
});

//#region "模块树"
function initTree() {
    var tree = $("#mdTree");
    tree.tree({
        url: '/UserSession/GetModulesComboTree'
        , lines: true
        , onSelect: function (node) {
            reloadGridOfTreeGrid();
        }
        , loadFilter: function (data, parent) {
            var root = { id: '', text: '根节点', children: data };
            var filterData = [];
            filterData.push(root);
            return filterData;
        }
        , onLoadSuccess: function (node, data) {
            if (data.length > 0) {
                //找到第一个元素
                var n = $(this).tree('find', data[0].id);
                //调用选中事件
                $(this).tree('select', n.target);
            }
        }
    });
}

function reloadTree() {
    var mdTree = $("#mdTree");
    mdTree.tree("reload");
    //var root = mdTree.tree("getRoot");
    //mdTree.tree("reload", root.target);
}
//#endregion

//#region "模块表格"
var grdEditIndexOfTreeGrid;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)
var gridOfTreeGrid;
var initTreeGrid = function () {
    gridOfTreeGrid = $("#treeGrid");
    gridOfTreeGrid.datagrid({
        url: '/UserSession/GetModulesTable',     //获取数据地址
        queryParams: {},
        border: true,
        fit: false,
        fitColumns: false,
        striped: true,
        nowrap: false,
        //pagination: true,            //分页
        //pageNumber: 1, //当前页
        //pageSize: 25,                //当前页显示数量
        //pageList: [15, 25, 35, 45],        //页显示数量
        rownumbers: true,        //行号
        singleSelect: true,
        checkOnSelect: true,
        selectOnCheck: true,
        columns: [[
            { field: 'chk', title: 'chk', width: 50, checkbox: true }
            , { field: 'Name', title: '模块名称', width: 150, align: "left", halign: "center", hidden: false }
            , { field: 'Code', title: '模块标识', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'ParentName', title: '父模块名称', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'CascadeId', title: '级联id', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'Link', title: 'Link', width: 150, align: "center", halign: "center", hidden: false }
            , {
                field: 'Icon', title: '图标', width: 60, align: "center", halign: "center", hidden: false, formatter: function (v, r, i) {
                    if (r.Icon.length > 0) {
                        return '<i class="fa ' + r.Icon + ' " ></i>';
                    }
                    return r.Icon;
                }
            }
            , { field: 'Sort', title: '排序号', width: 60, align: "center", halign: "center", hidden: false }
            , { field: 'CreateDate', title: '创建时间', width: 150, align: "center", halign: "center", hidden: false }
        ]],
        onCheck: function (rowIndex, rowData) {

        },
        onClickCell: function (rowIndex, field, value) {
            //var dg = $(this);
            //if (grdEditIndexOfTreeGrid != undefined) {
            //    $("span.textbox.textbox-focused").find("input").each(function () {
            //        $(this).blur();
            //    });
            //    endEditOfTreeGrid();
            //    return
            //}
            //grdEditIndexOfTreeGrid = rowIndex
            //dg.datagrid('editCell', { index: rowIndex, field: field });

            ////回车结束编辑
            //bindPressEnterEndEditOfTreeGrid();
        },
        onClickRow: function (rowIndex, rowData) {
            reloadGridOfBtnGrid();
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
                            reloadGridOfTreeGrid();
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
function endEditOfTreeGrid() {
    if (grdEditIndexOfTreeGrid == undefined) {
        return true
    }
    if (gridOfTreeGrid.datagrid('validateRow', grdEditIndexOfTreeGrid)) {
        gridOfTreeGrid.datagrid('endEdit', grdEditIndexOfTreeGrid);
        grdEditIndexOfTreeGrid = undefined;
        return true;
    } else {
        return false;
    }
}
function bindPressEnterEndEditOfTreeGrid() {
    //回车结束编辑
    $('.datagrid-editable .textbox,.datagrid-editable .datagrid-editable-input,.datagrid-editable .textbox-text').bind('keyup', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            $("span.textbox.textbox-focused").find("input").each(function () {
                $(this).blur();
            });
            endEditOfTreeGrid();
        }
    });
}
function reloadGridOfTreeGrid() {
    var node = $("#mdTree").tree("getSelected");
    if (node) {
        gridOfTreeGrid.datagrid("load", { id: node.id });
    }
}
//#endregion

//#region "模块菜单"
var grdEditIndexOfBtnGrid;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)
var gridOfBtnGrid;
var initBtnGrid = function () {
    gridOfBtnGrid = $("#btnGrid");
    gridOfBtnGrid.datagrid({
        url: '/ModuleManager/GetMenus',     //获取数据地址
        queryParams: {},
        border: true,
        fit: false,
        fitColumns: false,
        striped: true,
        nowrap: false,
        //pagination: true,            //分页
        //pageNumber: 1, //当前页
        //pageSize: 25,                //当前页显示数量
        //pageList: [15, 25, 35, 45],        //页显示数量
        rownumbers: true,        //行号
        singleSelect: true,
        checkOnSelect: false,
        selectOnCheck: false,
        columns: [[
            { field: 'chk', title: 'chk', checkbox: true }
            , {
                field: 'Name', title: '已有菜单', width: 180, align: "left", halign: "center", align: "center", hidden: false, formatter: function (v, r, i) {
                    var Class = r.Class ? r.Class : 'layui-btn-normal';
                    return '<button data-type="' + r.DomId + '" class="layui-btn layui-btn-xs ' + Class + '"><i class="fa ' + r.Icon + '"></i>' + r.Name + '</button>';
                }
            }
        ]],
        onCheck: function (rowIndex, rowData) {

        },
        onClickCell: function (rowIndex, field, value) {
            //var dg = $(this);
            //if (grdEditIndexOfBtnGrid != undefined) {
            //    $("span.textbox.textbox-focused").find("input").each(function () {
            //        $(this).blur();
            //    });
            //    endEditOfBtnGrid();
            //    return
            //}
            //grdEditIndexOfBtnGrid = rowIndex
            //dg.datagrid('editCell', { index: rowIndex, field: field });

            ////回车结束编辑
            //bindPressEnterEndEditOfBtnGrid();
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
                    url: "",
                    async: true,
                    data: {
                        Action: 10,
                        flag: 10,
                        jsonData: jsonData
                    },
                    success: function (data) {
                        if (data && data == "success") {
                            reloadGridOfBtnGrid();
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
function endEditOfBtnGrid() {
    if (grdEditIndexOfBtnGrid == undefined) {
        return true
    }
    if (gridOfBtnGrid.datagrid('validateRow', grdEditIndexOfBtnGrid)) {
        gridOfBtnGrid.datagrid('endEdit', grdEditIndexOfBtnGrid);
        grdEditIndexOfBtnGrid = undefined;
        return true;
    } else {
        return false;
    }
}
function bindPressEnterEndEditOfBtnGrid() {
    //回车结束编辑
    $('.datagrid-editable .textbox,.datagrid-editable .datagrid-editable-input,.datagrid-editable .textbox-text').bind('keyup', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            $("span.textbox.textbox-focused").find("input").each(function () {
                $(this).blur();
            });
            endEditOfBtnGrid();
        }
    });
}
function reloadGridOfBtnGrid() {
    var row = gridOfTreeGrid.datagrid("getSelected");
    if (row) {
        gridOfBtnGrid.datagrid("load", { id: row.Id });
    }
}
//#endregion



