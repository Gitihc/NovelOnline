layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        $ = layui.jquery;
    var openauth = layui.openauth;
    var toplayer = (top == undefined || top.layer === undefined) ? layer : top.layer;  //顶层的LAYER

    $("#menus").loadMenus("Users");

    //#region "编辑模块对话框"
    var editDlg = function () {
        var vm = new Vue({
            el: "#formEdit"
        });
        var update = false;  //是否为更新
        var show = function (data) {
            var title = update ? "编辑用户" : "添加用户";
            layer.open({
                title: title,
                area: ["500px", "270px"],
                type: 1,
                content: $('#divEdit'),
                success: function () {
                    vm.$set('$data', data);
                },
                end: function () {

                }
            });
            var url = "/UserManager/AddOrUpdate";
            if (update) {
                url = "/UserManager/AddOrUpdate";
            }

            //提交数据
            form.on('submit(formSubmit)',
                function (data) {
                    $.post(url,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                reloadGridOfGrdUser();
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
    var active = {
        btnAdd: function () {
            editDlg.add();
        }
        , btnEdit: function () {
            var rows = gridOfGrdUser.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择编辑的行，且同时只能编辑一行");
                return;
            }
            editDlg.update(rows[0]);
            $("#cmbMdTree").combotree("setValue", rows[0].ParentId);
        }
        , btnDel: function () {
            var rows = gridOfGrdUser.datagrid("getChecked");
            openauth.del("/UserManager/Delete",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfGrdUser();
                });
        }
        , btnAccessModule: function () {
            var rows = gridOfGrdUser.datagrid("getChecked");
            if (rows.length != 1) {
                toplayer.msg("请选择要分配的用户");
                return;
            }

            var index = toplayer.open({
                title: "为用户【" + rows[0].Name + "】分配模块",
                type: 2,
                area: ['750px', '600px'],
                content: "/ModuleManager/Assign?type=UserModule&menuType=UserElement&id=" + rows[0].Id,
                success: function (layero, index) {

                }
            });
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
});

$(function () {
    initGrdUserr();
});

//#region "用户表"
var grdEditIndexOfGrdUser;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)
var gridOfGrdUser;
var initGrdUserr = function () {
    gridOfGrdUser = $("#grdUser");
    gridOfGrdUser.datagrid({
        url: '/UserManager/GetAllUser',     //获取数据地址
        queryParams: {},
        border: false,
        fit: false,
        fitColumns: false,
        striped: true,
        nowrap: false,
        rownumbers: true,        //行号
        singleSelect: true,
        checkOnSelect: false,
        selectOnCheck: false,
        columns: [[
            { field: 'chk', title: 'chk', width: 50, checkbox: true }
            , { field: 'Account', title: '帐号', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'Name', title: '昵称', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'Sex', title: '性别', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'CreateDate', title: '创建时间', width: 150, align: "center", halign: "center", hidden: false }
            , { field: 'CreatorId', title: '创建人', width: 150, align: "center", halign: "center", hidden: false }
        ]],
        onCheck: function (rowIndex, rowData) {

        },
        onClickCell: function (rowIndex, field, value) {
            //var dg = $(this);
            //if (grdEditIndexOfGrdUser != undefined) {
            //    $("span.textbox.textbox-focused").find("input").each(function () {
            //        $(this).blur();
            //    });
            //    endEditOfGrdUser();
            //    return
            //}
            //grdEditIndexOfGrdUser = rowIndex
            //dg.datagrid('editCell', { index: rowIndex, field: field });

            //回车结束编辑
            //bindPressEnterEndEditOfGrdUser();
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
                            reloadGridOfGrdUser();
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
function endEditOfGrdUser() {
    if (grdEditIndexOfGrdUser == undefined) {
        return true
    }
    if (gridOfGrdUser.datagrid('validateRow', grdEditIndexOfGrdUser)) {
        gridOfGrdUser.datagrid('endEdit', grdEditIndexOfGrdUser);
        grdEditIndexOfGrdUser = undefined;
        return true;
    } else {
        return false;
    }
}
function bindPressEnterEndEditOfGrdUser() {
    //回车结束编辑
    $('.datagrid-editable .textbox,.datagrid-editable .datagrid-editable-input,.datagrid-editable .textbox-text').bind('keyup', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            $("span.textbox.textbox-focused").find("input").each(function () {
                $(this).blur();
            });
            endEditOfGrdUser();
        }
    });
}
function reloadGridOfGrdUser() {
    gridOfGrdUser.datagrid("reload");
}

//#endregion

