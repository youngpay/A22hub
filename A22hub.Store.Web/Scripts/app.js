(function (app) {
    var self = app;

    // ajax 交互模型
    self.ajaxData = function (obj) {
        this.Status = obj.Status;
        this.Message = obj.Message;
        this.Data = obj.Data;
    };

    // 弹窗
    self.alert = function (content, title) {
        if (!content) content = '操作发生未知错误';
        if (!title) title = '提示'
        modalViewModel.content(content);
        modalViewModel.title(title);
        $('#myModal').modal('show');
    }

    // 对话框
    self.dialog = function (callback, content, title) {
        if (typeof (callback) !== 'function') {
            throw new TypeError('callback');
        }
        if (!content) content = '是否确定？';
        if (!title) title = '提示'
        dialogViewModel.content(content);
        dialogViewModel.title(title);
        dialogViewModel.onYes = callback;
        $('#myDialog').modal('show');
    }

    self.window = function () {
        var self = this;
        var win = $('#myWindow');
        var defaultWidth = win.width();
        var onhidef = null;

        win.on('hidden', function () {
            if (self.onhidef) {
                self.onhidef();
                self.onhidef = null;
            }
            self.setContent('');
            self.setTitle('New Window');
            self.hideLoading();
            self.setWidth(defaultWidth);
            win.find('iframe').remove();
        });

        self.dom = function () {
            return win.find('#myWindow-content')[0];
        }

        self.setTitle = function (title) {
            win.find('h3').text(title);
        }

        self.setContent = function (content) {
            win.find('#myWindow-content').html(content);
        }

        self.setFooter = function (content) {
            win.find('.modal-footer').html(content);
        }

        self.setWidth = function (width) {
            win.width(width);
        }
        self.setHeight = function (height) {
            win.height(height);
        }

        self.getWidth = function () {
            return win.width();
        }
        self.getHeight = function () {
            return win.height();
        }

        self.show = function () {
            win.modal('show');
        }

        self.showLoading = function () {
            win.find('#myWindow-loading').show();
        }
        self.hideLoading = function () {
            win.find('#myWindow-loading').hide();
        }

        self.hide = function () {
            win.modal('hide');
        }

        self.toggle = function () {
            win.modal('toggle');
        }

        self.onhide = function (func) {
            self.onhidef = func;
        }
    }

    // 私有 alert 视图模型
    var modalViewModel = {
        title: ko.observable(),
        content: ko.observable()
    }
    ko.applyBindings(modalViewModel, $('#myModal')[0]);

    // 私有 dialog 视图模型
    var dialogViewModel = {
        title: ko.observable(),
        content: ko.observable(),
        pressYes: ko.observable(false),
        clickYes: function () {
            this.pressYes(true);
        },
        onYes: function () {
        }
    }
    ko.applyBindings(dialogViewModel, $('#myDialog')[0]);
    $('#myDialog').on('hidden', function () {
        if (dialogViewModel.pressYes()) {
            dialogViewModel.pressYes(false);
            if (dialogViewModel.onYes) {
                dialogViewModel.onYes();
            }
        }
    });

    // 退出
    self.logout = function () {
        self.dialog(function () {
            location.href = "/Home/Logout";
        }, '确定要退出？');
    }

    // 登录
    self.onlogin = function () {
        var form = loginForm;
        var btn = $(form.submit);
        btn.button('loading');
        var username = form.username.value;
        var password = form.password.value;
        var remember = form.remember.value;
        var returnurl = form.returnurl.value;
        if (username.length === 0) {
            btn.button('reset');
            app.alert('请输入用户名');
        } else if (password.length === 0) {
            btn.button('reset');
            app.alert('请输入密码');
        } else {
            $.post('/Home/Login', 'username=' + username + '&password=' + password + '&remember=' + remember + '&returnurl=' + returnurl,
                function (data) {
                    var rtn = new app.ajaxData(data);
                    if (rtn.Status == 0) {
                        location.href = rtn.Data;
                    } else {
                        btn.button('reset');
                        app.alert(rtn.Message);
                    }
                });
        }
        return false;
    }

    // 记住我
    self.onrememberme = function (e) {
        loginForm.remember.value = loginForm.remember.value == '0' ? '1' : '0';
    }

    self.checkFilePreview = function (fe) {
        fe = fe.toLowerCase();
        return !(fe != null && fe != ".txt" && fe != ".ini" && fe != ".inc" && fe != ".html" &&
        fe != ".cs" && fe != ".java" && fe != ".py" && fe != ".doc" && fe != ".docx" &&
        fe != ".xls" && fe != "xlsx" && fe != ".jpg" && fe != ".jpeg" && fe != ".gif" &&
        fe != ".png" && fe != ".ico" && fe != ".htm" && fe != ".js" && fe != ".css" &&
        fe != ".php" && fe != ".asp" && fe != ".vb" && fe != ".bat" && fe != ".cmd" &&
        fe != ".md" && fe != ".markdown" && fe != ".config" && fe != ".xaml" &&
        fe != ".settings" && fe != ".xml" && fe != ".ps1" && fe != ".h" && fe != ".cpp" &&
        fe != ".m" && fe != ".c" && fe != ".pdf");
    }
})(window.app = window.app || {});

document.body.addEventListener('dragover', function (event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = "none";
}, false);