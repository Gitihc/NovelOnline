

layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'upload', 'element', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        element = layui.element,
        upload = layui.upload,
        $ = layui.jquery;


    var openauth = layui.openauth;

    $("#menus").loadMenus("LocalNovelList");

    var searchHtml = '<div style="display:inline-block;float: left;margin-right: 10px;">';
    searchHtml += '<input type = "text" id = "key" name = "key" lay - verify="key" autocomplete = "off" placeholder = "请输名称" class= "layui-input" style = "width:290px;float:left;" />';
    searchHtml += '<button class="layui-btn" data-type="btnSearch" lay-filter="btnSearch">搜索</button>';
    searchHtml += '</div>';
    $("#menus").append(searchHtml);


    //#region "上传本地文件"
    var url3 = "/NovelManager/UploadLocalfile";
    var uploadInst = upload.render({
        elem: '#btnUpload'
        , url: url3
        , accept: 'file' //普通文件
        , exts: 'txt' //只允许上传压缩文件
        , before: function (obj) {
            ////预读本地文件示例，不支持ie8
            //obj.preview(function (index, file, result) {
            //    $('#demo1').attr('src', result); //图片链接（base64）
            //});
            $("#localfile").html("正在上传中...");
        }
        , done: function (res) {
            $("#localfile").html("上传本地文件");
            if (res.Code == 200) {
                layer.closeAll();
                reloadGridOfTable();
                return layer.msg('上传成功');
            } else {
                return layer.msg('上传失败');
            }
        }
        , error: function () {
            //演示失败状态，并实现重传
            var demoText = $('#showMsg');
            demoText.html('<span style="color: #FF5722;">上传失败</span> <a class="layui-btn layui-btn-xs demo-reload">重试</a>');
            demoText.find('.demo-reload').on('click', function () {
                uploadInst.upload();
            });
        }
    });
    //#endregion

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
            var url1 = "/NovelManager/GetNovelByLink";

            //提交数据
            form.on('submit(submint_link)',
                function (data) {
                    $.post(url1,
                        data.field,
                        function (data) {
                            layer.msg(data.Message);
                            if (data.Code == 200) {
                                layer.closeAll();
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

    var active = {
        btnSearch: function () {
            var keyValue = $("#key").val();
            //console.log("btnSearch");
            gridOfTable.datagrid("load", {
                key: keyValue
            })
        }
        , btnAdd: function () {
            addDlg.add();
        }
        , btnAddInMyNovel: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择要加入的行，且同时只能选择一行");
                return;
            }
            var Id = rows[0].Id;
            var url = '/NovelManager/AddLocalNovelInMyNovel';
            $.post(url, { novelId: Id }, function (data) {
                var result = $.parseJSON(data);
                if (result.Code == 200) {
                    layer.msg("已添加到书架！");
                } else {
                    layer.msg(data.Message);
                }
            });
        }
        , btnDown: function () {
            var rows = gridOfTable.datagrid("getChecked");
            if (!rows || rows.length != 1) {
                layer.msg("请选择要下载的数据，且同时只能选择一行");
                return;
            }
            var row = rows[0];
            if (row.State != 2) {
                layer.msg("书籍未获取完成，请获取到本地再试！");
                return;
            }
            var Id = row.Id;
            var form = $("<form>");
            form.attr('style', 'display:none');
            form.attr('target', '');
            form.attr('method', 'post');
            form.attr('action', '/NovelManager/DownNovel');

            var input1 = $('<input>');
            input1.attr('type', 'hidden');
            input1.attr('name', 'novelId');
            input1.attr('value', Id);
            form.append(input1);
            form.appendTo("body")
            form.submit();
            form.remove();
        }
        , btnDel: function () {
            var rows = gridOfTable.datagrid("getChecked");
            openauth.del("/NovelManager/DeleteNovel",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfTable();
                });
        }
        , btnReload: function () {
            reloadGridOfTable();
        }
    }
    $('.toolList .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
})

$(function () {
    initTable();
});

var grdEditIndexOfTable;		//当前编辑行(改名后不要删除，用于记录编辑行，全局变量)

var gridOfTable;
var initTable = function () {
    gridOfTable = $("#table");
    gridOfTable.datagrid({
        url: '/NovelManager/GetNovelList',     //获取数据地址
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
                    return '<a href="###" style="color:blue;" onclick="opentChapterList(' + i + '); return;">' + v + '</a>';
                }
            }
            , {
                field: 'OriginLink', title: '源始地址', width: 180, align: "center", halign: "center", formatter: function (v, r, i) {
                    return '<a href="' + v + '" target="_blank" style="color:blue;">源始地址</a>';
                }
            }
            , { field: 'FromType', title: '来源', width: 100, align: "center", halign: "center", formatter: function (v, r, i) { return v == 0 ? "本地" : "网络"; } }
            , {
                field: 'State', title: '状态', width: 100, align: "center", halign: "center", formatter: function (v, r, i) {
                    //状态（本地：0：处理中，1：完成，；网络：-2:失败,-1：部分未完成 ，1:获取中，2：完成）
                    if (r.FromType == 0) {
                        return v == 1 ? "完成" : "处理中";
                    } else if (r.FromType == 1) {
                        var str = "";
                        switch (v) {
                            case -2:
                                str = "失败";
                                break;
                            case -1:
                                str = "部分未完成";
                                break;
                            case 0:
                                str = "新建";
                                break;
                            case 1:
                                str = "获取中";
                                break;
                            case 2:
                                str = "完成";
                                break;
                            default:
                                str = "失败";
                                break;
                        }
                        return str;
                    }
                    return v;
                }
            }
            , { field: 'CreateDate', title: '创建时间', width: 150, align: "center", halign: "center" }
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

function opentChapterList(index) {
    var row = gridOfTable.datagrid("selectRow", index).datagrid("getSelected");
    if (row) {
        var title = '<cite>' + row.Name + '</cite><i class="layui-icon layui-unselect layui-tab-close" data-id="2">ဆ</i>';
        var id = row.Id;
        var fromType = row.FromType;
        var content = "<iframe src='/ChapterManager/ChapterList?id=" + id + "&fromType=" + fromType + "' data-id='" + id + "'></frame>";
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

