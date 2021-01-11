$.modalOpen = function (options) {
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        btnclass: ['btn btn-primary', 'btn btn-danger'],
        callBack: null,
        type: 2,

    };
    options = $.extend(defaults, options);
    var _width = top.$(window).width() > parseInt(options.width.replace('px', '')) ? options.width : top.$(window).width() + 'px';
    var _height = top.$(window).height() > parseInt(options.height.replace('px', '')) ? options.height : top.$(window).height() + 'px';
    layer.open({
        id: options.id,
        type: options.type,
        shade: options.shade,
        shadeClose: options.shadeClose,
        title: options.title,
        fix: false,
        loseBtn: options.closeBtn,
        area: [_width, _height],
        zIndex: options.zIndex > layer.zIndex ? options.zIndex : layer.zIndex,
        // content: [options.url, 'no'],
        content: options.url,
        btn: options.btn,
        btnclass: options.btnclass,
        yes: function (index, layero) {
            options.callBack(index, layero, options.id)
        }, cancel: function () {
            return true;
        }
    });
};

$.modalClose = function () {
    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    var $IsdialogClose = parent.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
    var IsClose = $IsdialogClose.is(":checked");
    if ($IsdialogClose.length == 0) {
        IsClose = true;
    }
    if (IsClose) {
        parent.layer.close(index);
    } else {
        location.reload();
    }
};

//空对象显示处理
$.renderText = function (value) {
    return (value === "" || value === null) ? "-" : value;
};

//是否可用bool
$.renderEnabled = function (value) {
    return (value == 1) ? "是" : "否";
};

$.request = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURIComponent(r[2]);
    return "";
};

$.getQueryVariable = function (variable, url) {
    var query = url;
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (false);
};

$.reload = function (id) {
    table.reload(id); //数据刷新
}; 

(function ($) {
    $.fn.clickToggle = function (func1, func2) {
        var funcs = [func1, func2];
        this.data('toggleclicked', 0);
        this.click(function () {
            var data = $(this).data();
            var tc = data.toggleclicked;
            $.proxy(funcs[tc], this)();
            data.toggleclicked = (tc + 1) % 2;
        });
        return this;
    };
}(jQuery));