var id, type, menuType, layer;
layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'jquery', 'utils', 'table'], function () {
    layer = (top == undefined || top.layer === undefined) ? layui.layer : top.layer,
        $ = layui.jquery;
    var table = layui.table;
    id = $.getUrlParam("id");      //待分配的id
    type = $.getUrlParam("type");  //待分配的类型
    menuType = $.getUrlParam("menuType");  //待分配菜单的类型

    //菜单列表
    var menucon = {};  //table的参数，如搜索key，点击tree的id

    var mainList = function (options) {
        if (options != undefined) {
            $.extend(menucon, options);
        }
        table.reload('mainList', {
            url: '/ModuleManager/LoadMenus',
            where: menucon
            , done: function (res, curr, count) {
                //如果是异步请求数据方式，res即为你接口返回的信息。
                //如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度
                var url = "/ModuleManager/LoadMenusForUser";
                if (type.indexOf("Role") != -1) {
                    url = "/ModuleManager/LoadMenusForRole";
                }

                $.ajax(url, {
                    async: false
                    , data: {
                        firstId: id
                        , moduleId: options.moduleId
                    }
                    , dataType: "json"
                    , success: function (roles) {

                        //循环所有数据，找出对应关系，设置checkbox选中状态
                        for (var i = 0; i < res.data.length; i++) {
                            for (var j = 0; j < roles.length; j++) {
                                if (res.data[i].Id != roles[j].Id) continue;

                                //这里才是真正的有效勾选
                                res.data[i]["LAY_CHECKED"] = true;
                                //找到对应数据改变勾选样式，呈现出选中效果
                                var index = res.data[i]['LAY_TABLE_INDEX'];
                                $('.layui-table-fixed-l tr[data-index=' + index + '] input[type="checkbox"]').prop('checked', true);
                                $('.layui-table-fixed-l tr[data-index=' + index + '] input[type="checkbox"]').next().addClass('layui-form-checked');
                            }

                        }

                        //如果构成全选
                        var checkStatus = table.checkStatus('mainList');
                        if (checkStatus.isAll) {
                            $('.layui-table-header th[data-field="0"] input[type="checkbox"]').prop('checked', true);
                            $('.layui-table-header th[data-field="0"] input[type="checkbox"]').next().addClass('layui-form-checked');
                        }
                    }
                });




            }
        });
    }


    //分配及取消分配
    table.on('checkbox(list)', function (obj) {
        console.log(obj.checked); //当前是否选中状态
        console.log(obj.data); //选中行的相关数据
        console.log(obj.type); //如果触发的是全选，则为：all，如果触发的是单选，则为：one

        var url = "/RelevanceManager/Assign";
        if (!obj.checked) {
            url = "/RelevanceManager/UnAssign";
        }
        $.post(url, { type: menuType, firstId: id, secIds: [obj.data.Id] }
            , function (data) {
                layer.msg(data.Message);
            }
            , "json");
    });
    //监听页面主按钮操作 end
})

$(function () {
    initTree();
    initTable();
});

//#region "模块树"
function initTree() {
    var tree = $("#tree");
    id = getUrlParam("id");      //待分配的id
    tree.tree({
        url: '/UserSession/GetModulesComboTree'
        , lines: true
        , checkbox: true
        , cascadeCheck: false
        , onCheck: function (node, checked) {
            if (isModulesLoading) return;
            var url = "/RelevanceManager/Assign";
            if (!checked) {
                url = "/RelevanceManager/UnAssign";
            }
            $.post(url, { type: type, firstId: id, secIds: [node.id] }
                , function (data) {
                    layer.msg(data.Message);
                }
                , "json");
        }
        , onSelect: function (node) {
            reloadGridOfTable();
        }
        , loadFilter: function (data, parent) {
            //var root = { id: '', text: '根节点', children: data };
            //var filterData = [];
            //filterData.push(root);
            //return filterData;
            return data;
        }
        , onLoadSuccess: function (node, data) {
            if (data.length > 0) {
                //找到第一个元素
                var n = $(this).tree('find', data[0].id);
                //调用选中事件
                $(this).tree('select', n.target);
            }
            loadUserModules();
        }
    });
}
//用户已受权模块
var isModulesLoading = false;
function loadUserModules() {
    var url = "/ModuleManager/LoadForUser";
    //if (type.indexOf("Role") != -1) {
    //    url = "/ModuleManager/LoadForRole";
    //}
    $.getJSON(url, { firstId: id }
        , function (data) {
            isModulesLoading = true;
            $.each(data,
                function (i) {
                    var that = this;
                    var objTree = $("#tree");
                    var node = objTree.tree('find', that.Id);
                    objTree.tree('check', node.target);
                });
            isModulesLoading = false;
        });
}
//#endregion

//#region "菜单表格"
var grdEditIndexOfTable;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)
var gridOfTable;
var initTable = function () {
    gridOfTable = $("#table");
    gridOfTable.datagrid({
        url: '/ModuleManager/GetMenus',     //获取数据地址
        queryParams: {},
        border: false,
        fit: false,
        fitColumns: false,
        striped: true,
        nowrap: false,
        idField:'Id',
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
            , { field: 'Sort', title: '排序', width: 80, align: "center", halign: "center" }
        ]],
        onCheck: function (rowIndex, rowData) {
            assignMenu(rowData, true);
        }
        , onUncheck: function (rowIndex, rowData) {
            assignMenu(rowData, false);
        }
        , onClickCell: function (rowIndex, field, value) {
            //var dg = $(this);
            //if (grdEditIndexOfTable != undefined) {
            //    $("span.textbox.textbox-focused").find("input").each(function () {
            //        $(this).blur();
            //    });
            //    endEditOfTable();
            //    return
            //}
            //grdEditIndexOfTable = rowIndex
            //dg.datagrid('editCell', { index: rowIndex, field: field });

            ////回车结束编辑
            //bindPressEnterEndEditOfTable();
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
            loadUserMenu();
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
    var node = $("#tree").tree("getSelected");
    if (node) {
        gridOfTable.datagrid("load", { id: node.id });
    }
}

function assignMenu(rowData, checked) {
    var url = "/RelevanceManager/Assign";
    if (!checked) {
        url = "/RelevanceManager/UnAssign";
    }
    $.post(url, {
        type: menuType, firstId: id, secIds: [rowData.Id]
    }
        , function (data) {
            layer.msg(data.Message);
        }
        , "json");
}

function loadUserMenu() {
    var url = "/ModuleManager/LoadMenusForUser";
    //if (type.indexOf("Role") != -1) {
    //    url = "/ModuleManager/LoadMenusForRole";
    //}
    var objTree = $("#tree");
    var node = objTree.tree("getSelected");
    if (node) {
        $.ajax(url, {
            async: false
            , data: {
                firstId: id
                , moduleId: node.id
            }
            , dataType: "json"
            , success: function (menus) {
                isModulesLoading = true;
                debugger
                $.each(menus
                    , function (i) {
                        var that = this;
                        var index = gridOfTable.datagrid("getRowIndex",that.Id);
                        gridOfTable.datagrid("checkRow", index);
                    })
                isModulesLoading = false;
            }
        });
    }
}
//#endregion

//#region "getUrlParam"
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
//#endregion

