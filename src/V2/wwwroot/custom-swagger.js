(function (swagger) {
    var router = swagger.authView.router;
    var routerLoad = router.load;

    // overide 'swaggerUi.load' method, which called when 
    // user switch Shema or adds authorization token, in order
    // to prepend 'Bearer ' in front of token itself
    router.load = function () {
        var keyAuth = router.api.clientAuthorizations.authz;
        if (keyAuth.api_key && keyAuth.api_key.value.indexOf('Bearer ') == -1) {
            keyAuth.api_key.value = 'Bearer ' + keyAuth.api_key.value;
        }
        routerLoad.call(swagger);
    };
})(window.swaggerUi);