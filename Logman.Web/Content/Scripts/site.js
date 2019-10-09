var JavaScripts = {
    LoadMoreEvents: function() {

        var queryStringElement = $("#hidQueryString");
        var currentPage = parseInt($("#hidCurrentPageNumber").val());
        var newPage = currentPage + 1;
        $("#hidCurrentPageNumber").val(newPage);
        if (queryStringElement != undefined) {
            var query = queryStringElement.val().toLowerCase().replace("pagenumber=" + currentPage, "pagenumber=" + newPage);
            if (query != "") {

                $("#imgProgress").removeClass("element-hide");
                $(".btn").each(function () {
                    $(this).addClass("element-disabled");
                });
                var request = $.ajax({
                    type: "GET",
                    url: query
                });
                request.done(function(data) {

                    if (data != null) {
                        var html = $.parseHTML(data);
                        var raws = html[1].children[0].children;
                        if (raws.length > 0) {
                            var tableElement = $("#tblEvents > tbody");
                            $("#tblEvents > tbody").html(tableElement.html() + html[1].children[0].innerHTML);

                        } else {
                            $("divSeeMoreEvents").addClass("element-hide");
                        }

                    }
                });

                request.fail(function(textStatus) {

                });

                request.always(function () {
                    $(".btn").each(function () {
                        $(this).removeClass("element-disabled");
                    });
                    $("#imgProgress").addClass("element-hide");
                   
                });
            }
        }
    },

    UpdateQueryString: function(key, value, url) {
        if (!url) url = window.location.href;
        var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi");

        if (re.test(url)) {
            if (typeof value !== 'undefined' && value !== null)
                return url.replace(re, '$1' + key + "=" + value + '$2$3');
            else {
                var hash = url.split('#');
                url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            }
        } else {
            if (typeof value !== 'undefined' && value !== null) {
                var separator = url.indexOf('?') !== -1 ? '&' : '?',
                    hash = url.split('#');
                url = hash[0] + separator + key + '=' + value;
                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
                return url;
            } else
                return url;
        }
    },
    RefreshEvents: function () {

        debugger;
        var eventTypeValue = parseInt($('input[name="rgEventTpe"]:checked').val());
        var timeOptionValue = parseInt($('input[name="rdSince"]:checked').val());
        var now = Date.now();
        var fromDateTime = new Date(now).getMinutes() - timeOptionValue;
        var keyword = $("#txtKeyword").val();
        keyword = Base64.encode(keyword);

        var tempDate = new Date(now);
        tempDate.setMinutes(fromDateTime);
        fromDateTime = tempDate;

        var queryString = $("#hidQueryString").val().toLowerCase().replace("eventLevel");
        queryString = JavaScripts.UpdateQueryString("fromDate", fromDateTime.toString("yyyy-MM-dd hh:mm:ss tt"), queryString);
        queryString = JavaScripts.UpdateQueryString("eventLevel", eventTypeValue, queryString);
        queryString = JavaScripts.UpdateQueryString("keywords", keyword, queryString);
        $("#hidQueryString").val(queryString);
        $("#tblEvents > tbody").html('');
        JavaScripts.LoadMoreEvents();
        return false;
    },
    
    DeleteAlert: function (e) {
        var appId = $(e).attr("data-appid");
        if (appId > 0) {
            if (!confirm('Are you sure you want to remove this alert?')) {
                return false;
            }


            var task = $.get("/Application/RemoveAlert?alertId=" + appId);
            var rowId = $(e).attr("data-row");
            var rowName = "row" + rowId;
            var row = $("#" + rowName);
            row.remove();
        }
        return false;
    }
};