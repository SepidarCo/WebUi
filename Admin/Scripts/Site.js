function prepareCkEditor() {
    ClassicEditor.create(document.querySelector('#Description'), {})
        .then(editor => {
            window.editor = editor;
        })
        .catch(err => {
            console.error(err.stack);
        });
}

function bindSubmitModal(afterSubmitModel, afterShowModal = null) {
    $("button[data-role=submit-partial]").click(function (e) {
        e.preventDefault();
        var frm = $(this).closest("form");
        var btn = $(this);
        btn.addClass("loading");
        btn.attr("disabled", "disabled");
        $.ajax({
            async: true,
            url: frm.attr("action"),
            data: frm.serialize(),
            type: "POST",
            success: function (receivedData) {
                btn.removeClass("loading");
                btn.removeAttr("disabled");
                frm.parent().html(receivedData);

                if (afterShowModal !== null) {
                    afterShowModal();
                }

                bindSubmitModal(afterSubmitModel, afterShowModal);

                if ($(".validation-summary-errors").length === 0) {
                    afterSubmitModel(receivedData);
                }
            },
            error: function (err) {
                btn.removeAttr("disabled");
                btn.removeClass("loading");
                alert(err.responseText);
            }
        });
    });
}

function onClickShowModal(btn, targetUrl, targetData, afterSubmitModel, afterShowModal = null) {
    btn.attr("disabled", "disabled");
    btn.addClass("loading");
    $.ajax({
        async: true,
        url: targetUrl,
        data: targetData,
        type: "GET",
        success: function (receivedData) {
            btn.removeAttr("disabled");
            btn.removeClass("loading");
            $("#modal-place").html(receivedData);
            $("#modal-place").modal("show");

            if (afterShowModal !== null) {
                afterShowModal();
            }

            bindSubmitModal(afterSubmitModel, afterShowModal);
        },
        error: function (err) {
            btn.removeAttr("disabled");
            btn.removeClass("loading");
            alert(err.responseText);
        }
    });
}

function UploadFile(url, data, afterUpload) {
    $.ajax({
        async: true,
        url: url,
        data: data,
        type: 'POST',
        contentType: false,
        processData: false,
        success: function (response) {
            afterUpload(response);
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}

function openModalWithFile(btn, url, data, afterSubmitModel, afterShowModal = null) {
    btn.attr("disabled", "disabled");
    btn.addClass("loading");

    $.ajax({
        async: true,
        url: url,
        data: data,
        type: "GET",
        success: function (response) {
            $("#modal-place").html(response);
            $("#modal-place").modal("show");

            btn.removeAttr("disabled");
            btn.removeClass("loading");

            postModalWithFile(afterSubmitModel, afterShowModal);

            if (afterShowModal !== null) {
                afterShowModal();
            }
        },
        error: function (error) {
            btn.removeAttr("disabled");
            btn.removeClass("loading");
            alert(error.responseText);
        }
    });
}

function postModalWithFile(afterSubmitModel, afterShowModal = null) {
    $("button[data-role=submit-partial]").click(function (e) {
        e.preventDefault();

        var frm = $(this).closest("form");

        var data = new FormData();

        var filesTag = $("input[data-role=post-file]");
        $.each(filesTag, function (index) {
            var fileTag = $(filesTag[index]);

            var files = fileTag.get(0).files;

            if (files.length === 0 || files[0].length === 0)
                return;

            var tagName = fileTag.attr("name");
            var file = files[0];

            data.append(tagName, file);
        });

        var fieldsTag = $("input[data-role=post-field],select[data-role=post-field],textarea[data-role=post-field]");
        $.each(fieldsTag, function (index) {
            var fieldTag = $(fieldsTag[index]);

            var fieldName = fieldTag.attr("name");
            var fieldValue = fieldTag.val();

            var fieldType = fieldTag.attr("type");
            if (fieldType === "checkbox") {
                if (fieldTag.is(":checked") === false) {
                    fieldValue = false;
                }
            }
                        
            //console.log(`${fieldName}:${fieldValue} => ${fieldType}`);
            data.append(fieldName, fieldValue);
        });

        $.ajax({
            async: true,
            url: frm.attr("action"),
            data: data,
            type: "POST",
            contentType: false,
            processData: false,
            success: function (response) {
                frm.parent().html(response);

                postModalWithFile(afterSubmitModel, afterShowModal);

                if (afterShowModal !== null)
                    afterShowModal();

                if ($(".validation-summary-errors").length === 0) {
                    afterSubmitModel(response);
                }

            },
            error: function (error) {
                alert(error.responseText);
            }
        });

    });
}

$(".image-container").click(function (e) {
    e.preventDefault();
    var imgSrc = $($(this).children("img")[0]).attr("src");
    $("#image-modal-content").attr("src", imgSrc);
    $("#image-modal").modal("show");
});

$("li.active").removeClass("active");
var parentLi = $("#ActiveLink").data("parent");
$(`#${parentLi}`).addClass("active");
var currentLi = $("#ActiveLink").data("current");
$(`#${currentLi}`).addClass("active");
