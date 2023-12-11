$(document).ready(function () {

    $("#EnterAccessCodeForm").submit(function (n) {
        var t;
        n.preventDefault();
        t = Ladda.create(document.querySelector("#btnenteraccesscode"));
        t.start();
        t.isLoading();
        t.setProgress(-1);
        var i = $("#EnterAccessCodeForm input[name=__RequestVerificationToken]").val(),
            r = $("#EnterAccessCode").val();
        GetOrPostAsync("POST", "/Security/AuthenticateAccessPassword/", r, i).then(() => {
                t.stop(), (window.location.href = "/Dashboard/Accounts/");
            })
            .catch((n) => {
                Notify(!1, n), t.stop();
            });
    });

});