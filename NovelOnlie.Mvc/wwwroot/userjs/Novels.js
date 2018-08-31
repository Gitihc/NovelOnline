layui.config({
    base: "/js/"
}).use(['form', 'vue', 'layer', 'upload', 'element', 'jquery', 'openauth', 'utils'], function () {
    var form = layui.form,
        layer = layui.layer,
        element = layui.element,
        upload = layui.upload,
        $ = layui.jquery;


    var openauth = layui.openauth;

    $("#menus").loadMenus("NovelList");

    //#region "编辑对话框"
    var editDlg = function () {
        var vm = new Vue({
            el: "#formEdit"
        });
        var update = false;  //是否为更新
        var show = function (data) {
            var title = update ? "编辑模块" : "添加模块";
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
            var url = "/NovelManager/Update";

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
                update = true;
                show(data);
            }
        };
    }();
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
            var url2 = "/NovelManager/SearchWebsite";
            var url3 = "/NovelManager/UploadLocalfile";
            //提交数据
            form.on('submit(submint_link)',
                function (data) {
                    $.post(url1,
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
            form.on('submit(submint_website)',
                function (data) {
                    $.post(url2,
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
            var uploadInst = upload.render({
                elem: '#localfile'
                , url: url3
                , accept: 'file' //普通文件
                , exts: 'txt' //只允许上传压缩文件
                , before: function (obj) {
                    ////预读本地文件示例，不支持ie8
                    //obj.preview(function (index, file, result) {
                    //    $('#demo1').attr('src', result); //图片链接（base64）
                    //});
                }
                , done: function (res) {
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
        }
        return {
            add: function () { //弹出编辑框
                show();
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
            openauth.del("/NovelManager/DeleteNovel",
                rows.map(function (r) { return r.Id; }),
                function () {
                    reloadGridOfTable();
                });
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

//#region "Novel表格"
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
        idField: 'Id',
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
                field: 'Name', title: '名称', width: 320, align: "left", halign: "center", align: "center", hidden: false, formatter: function (v, r, i) {
                    return '<a href="/ChapterManager/ChapterView?id=' + r.Id + '&fromtyp=' + r.FromType + '" target="_blank">' + r.Name + '</a>';
                }
            }
            , { field: 'OriginLink', title: '源始地址', width: 180, align: "center", halign: "center" }
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

//#endregion