function leftNavBar(strData) {
    var data;
    if (typeof (strData) == "string") {
        var data = JSON.parse(strData); //部分用户解析出来的是字符串，转换一下
    } else {
        data = strData;
    }
    var ulHtml = '<ul class="layui-nav layui-nav-tree layui-bg-cyan layui-inline" lay-filter="leftMenu">';
    for (var i = 0; i < data.length; i++) {
        var obj = data[i];
        var item = obj.Item;
        if (obj.Children && obj.Children.length > 0) {
            ulHtml += '<li class="layui-nav-item">';
            ulHtml += '<a href="javascript:;">';
            if (item.Icon && item.Icon.length > 0) {
                ulHtml += '<i class="fa ' + item.Icon + '" data-icon="' + item.Icon + '"></i>';
            }
            ulHtml += '<cite>' + item.Name + '</cite>';
            ulHtml += '</a>';
            ulHtml += '<dl class="layui-nav-child">';
            for (var j = 0; j < obj.Children.length; j++) {
                var childObj = obj.Children[j];
                var child = childObj.Item;
                ulHtml += '<dd><a href="javascript:;" data-url="' + child.Link + '">';
                if (child.Icon && child.Icon.length > 0) {
                    //ulHtml += '<i class="layui-icon ' + child.icon + '" data-icon="' + child.icon + '"></i>';
                    ulHtml += '<i class="fa ' + child.Icon + '" data-icon="' + child.Icon + '"></i>';
                }
                ulHtml += '<cite>' + child.Name + '</cite>';
                ulHtml += '</a>';
                ulHtml += ' </dd>';
            }
            ulHtml += "</dl>";
            ulHtml += ' </li>';

        } else {
            ulHtml += '<li class="layui-nav-item">';
            ulHtml += '<a href="javascript:;" data-url="' + item.Link + '" >';
            if (item.Icon && item.Icon.length > 0) {
                ulHtml += '<i class="fa ' + item.Icon + '" data-icon="' + item.Icon + '"></i>';
            }
            ulHtml += '<cite>' + item.Name + '</cite>';
            ulHtml += '</a>';
            ulHtml += ' </li>';
        }
    }
    ulHtml += ' </ul>';
    return ulHtml;
}