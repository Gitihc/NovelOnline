//#region "getUrlParam"
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
//#endregion
var novelId, chapterId, fromType;
$(function () {
    novelId = getUrlParam("id");
    chapterId = getUrlParam("chapterId");
    fromType = getUrlParam("fromtyp");
    getChapters();
    getContent();
    initWindowScroll();
    goTop();
    bindBtnEvent();
});

//#region "获取章节内容"
function getContent() {
    if (chapterId == undefined || chapterId.length == 0) {
        var nid = getCookie('curNovelId');
        var cid = getCookie('curChapterId');
        if (nid == novelId) {
            chapterId = cid;
        }
    }
    getChapterContent(novelId, chapterId);
}

function getChapterContent(novelId, chapterId) {
    $.ajax({
        url: "/ChapterManager/GetChapterContent",
        async: true,
        data: { novelId: novelId, chapterId: chapterId },
        success: function (data) {
            if (data) {
                var obj = $.parseJSON(data);
                if (obj) {
                    $("#title").html(base64decodeCN(obj.Title));
                    $("#content").html(base64decodeCN(obj.Content));
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(message);
        }
    })
}
//#endregion

//#region "获取目录"
function getChapters() {
    $.ajax({
        url: "/NovelManager/GetChapterList",
        data: { novelId: novelId},
        async: true,
        success: function (data) {
            if (data) {
                var obj = $.parseJSON(data);
                if (obj) {
                    bookChapter = obj;

                    var newCatalog = ''
                    for (var i = 0; i < bookChapter.length; i++) {
                        var chapter = bookChapter[i];
                        var cguid = chapter.Id;
                        var name = chapter.Name;
                        newCatalog += '<li><a href="##" novelId="' + novelId + '" chapterId="' + cguid + '" onclick="jumpChapter()">' + name + '</a> </li>'
                    }

                    //新目录
                    $("#CatalogContainer ul").html("");
                    $("#CatalogContainer ul").html(newCatalog);

                    clickChapterEvent();
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(message);
        }
    })
}
//#endregion
function initWindowScroll() {
    $(window).scroll(function () {
        var windowScrollTop = $(window).scrollTop();
        setGoTopWhenScroll(windowScrollTop);    //回到顶部
        setCatalogWhenScroll(windowScrollTop);  //章节目录
        setMenuBtnWhenScroll(windowScrollTop);  //左侧浮动菜单
    });
}
//#region "滚动条滚动时设置左侧浮动菜单、回到顶部、章节目录位置"
//回到顶部
function setGoTopWhenScroll(windowScrollTop) {
    if (windowScrollTop > 30) {
        $(".go-top").fadeIn()
    }
    else {
        $(".go-top").fadeOut()
    }
}
//章节目录
function setCatalogWhenScroll(windowScrollTop) {
    var obj = $(".panel-wrap");
    if (windowScrollTop >= 30) {
        obj.css("position", "fixed");
        obj.css("top", "0");
    } else if (windowScrollTop > 0) {
        obj.css("position", "absolute");
        obj.css("top", windowScrollTop);
    } else {
        obj.css("position", "absolute");
        obj.css("top", 30);
    }
}
//左侧浮动菜单
function setMenuBtnWhenScroll(windowScrollTop) {
    var obj = $(".fixed-menu");
    if (windowScrollTop >= 30) {
        obj.css("position", "fixed");
        obj.css("top", "0");
    } else if (windowScrollTop > 0) {
        obj.css("position", "absolute");
        obj.css("top", windowScrollTop);
    } else {
        obj.css("position", "absolute");
        obj.css("top", 30);
    }
}
//#endregion
//回到顶部      
function goTop() {
    $(".go-top").click(function (event) {
        $('html,body').animate({ scrollTop: 0 }, 100);
        return false;
    });
}

//绑定左侧浮动菜单事件
function bindBtnEvent() {
    $(".fixed-menu dl dd").bind("click", function () {

        var curTarget = event.currentTarget;
        if (curTarget.localName == "a") {
            var parent = $(curTarget).parent();
            var pindex = parent.index();
            curTarget = $(".fixed-menu dl dd").eq(pindex);
        }
        var curIndex = $(curTarget).index();

        $(curTarget).siblings().removeClass("act");
        $(".panel-wrap").css("display", "none");

        var panel = $(".panel-wrap").eq(curIndex);
        var cls = $(curTarget).attr('class');
        if (cls == "act") {
            $(curTarget).removeClass('act');
            $(panel).css("display", "none");
        } else {
            $(curTarget).addClass('act');
            $(panel).css("display", "block");
        }

        switch (curIndex) {
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }
    });

    bindSetupEvent();
}

//#region "设置绑定事件"
function bindSetupEvent() {
    $(".setting-list-wrap ul .theme-list>span").bind("click", function () {
        $(this).siblings().removeClass("act");
        $(this).addClass("act");
        var index = $(this).index();
        $("body").removeClass().addClass("theme-" + (index - 1));
    });

    $(".setting-list-wrap ul .font-family>span").bind("click", function () {
        $(this).siblings().removeClass("act");
        $(this).addClass("act");
        var index = $(this).index();
        $("#content").removeClass().addClass("row").addClass("font-family0" + index);
    })

    $(".font-size .prev,.font-size .next").bind("click", function () {
        var size = $(".font-size .lang").html();
        var cls = $(this).attr("class");
        if (size.length > 0 && cls.length > 0) {
            size = parseInt(size);
            if (cls == "prev") {
                size = size - 2;
            } else {
                size = size + 2;
            }
            if (size <= 12) {
                size = 12;
            }

            if (size >= 48) {
                size = 48;
            }
            $(".font-size .lang").html(size);
            $("#content p").css("font-size", size);
        }
    });

    clickCloseChapterMenu();
}
//#endregion

//#region "目录菜单绑定事件"
function clickChapterEvent() {
    if ($("#CatalogContainer ul li").length > 0) {
        $("#CatalogContainer ul li").bind("click", function () {
            $(this).siblings().removeClass("on");
            $(this).addClass("on");
        });
    }
}

function clickCloseChapterMenu() {
    $(".close-panel").bind("click", function () {
        var panel = $(this).parent();
        var index = $(panel).index();
        var menubtn = $(".fixed-menu dl dd").eq(index);
        $(menubtn).click();
        return
    });
}
//#endregion

//点击章节时，跳转到章节
function jumpChapter(e) {
    var that = event.currentTarget;
    var cid = $(that).attr("chapterId");
    var nid = $(that).attr("novelId");
    gotoChapter(cid, nid);
}
function gotoChapter(cgid, bgid) {
    cid = cgid;
    nid = bgid;
    setCookie("curNovelId", cid);
    setCookie("curChapterId", cid);
    var href = '/ChapterManager/ChapterView?id=' + nid + '&chapterId=' + cid + "&ft=" + fromType;
    window.top.location.href = href;
}

//#region "设置cookie"
function setCookie(key, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = key + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

function getCookie(key) {
    var arr, reg = new RegExp("(^| )" + key + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return "";
}

function delCookie(key) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(key);
    if (cval != null)
        document.cookie = key + "=" + cval + ";expires=" + exp.toGMTString();
}

//#endregion