
//#region "getUrlParam"
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
//#endregion

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

    //$("#menus").loadMenus("WebsiteList");

    //var searchHtml = '<div style="display:inline-block;float: left;margin-right: 10px;">';
    //searchHtml += '<input type = "text" id = "key" name = "key" lay - verify="key" autocomplete = "off" placeholder = "请输名称或作者" class= "layui-input" style = "width:290px;float:left;" />';
    //searchHtml += '<button class="layui-btn" data-type="btnSearch" lay-filter="btnSearch">搜索</button>';
    //searchHtml += '</div>';
    //$("#menus").append(searchHtml);

    var active = {
        btnAddInMyNovel: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择要加入的行，且同时只能选择一行");
                return;
            }
            var Id = rows[0].Id;
            var url = '/WebsiteNovel/AddWebsiteNovelInMyNovel';
            $.post(url, { websiteId: websiteId, id: Id }, function (data) {
                var result = $.parseJSON(data);
                if (result.Code == 200) {
                    layer.msg("已添加到书架！");
                } else {
                    layer.msg(data.Message);
                }
            });
        }
        , btnReomve: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择要删除的行！");
                return;
            }
            var delUrl = '/WebsiteNovel/RemoveWebsiteNovel?websiteid=' + websiteId;
            openauth.del(delUrl,
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfTable();
                });
        }
        , btnRefresh: function () {
            reloadGridOfTable();
        }
        , btnSearch: function () {
            var keyValue = $("#key").val();
            //console.log("btnSearch");
            gridOfTable.datagrid("load", {
                websiteId: websiteId,
                key: keyValue
            })
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
});

var websiteId;
$(function () {
    websiteId = getUrlParam("id");
    initTable();
});

var grdEditIndexOfTable;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)
var gridOfTable;
var initTable = function () {
    gridOfTable = $("#table");
    gridOfTable.datagrid({
        url: '/WebsiteNovel/GetWebsiteNovelList',     //获取数据地址
        queryParams: { websiteId: websiteId },
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
            , { field: 'Name', title: '名称', width: 320, align: "center", halign: "center", hidden: false }
            , { field: 'Author', title: '作者', width: 180, align: "center", halign: "center" }
            , {
                field: 'OriginLink', title: '源始地址', width: 180, align: "center", halign: "center", formatter: function (v, r, i) {
                    return '<a href="' + v + '" target="_blank" style="color:blue;">源始地址</a>';
                }
            }
            , {
                field: 'State', title: '状态', width: 180, align: "center", halign: "center", formatter: function (v, r, i) {
                    //0-未获取，1-获取中，2-已获取,3-获取失败
                    var txtState = "";
                    switch (v) {
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
                            txtState = "获取失败";
                            break;
                    };
                    return txtState;
                }
                , hidden: true
            }
            , { field: 'CreateDate', title: '创建时间', width: 180, align: "center", halign: "center" }
            , {
                field: '_operate', title: '操作', width: 180, align: "center", halign: "center", hidden: true, formatter: function (v, r, i) {
                    var del = '<i title="删除" class="fa fa-trash-o" style="color: #386;margin-left: 10px;" onclick="addInMyNovel(' + i + ')"></i>';
                    var addToMyBook = '<i title="addToMyBook" class="fa fa-mail-forward" style="color: #386;margin-left: 10px;" onclick="removeNovel(' + i + ')" ></i>';
                    return addToMyBook + del;
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


