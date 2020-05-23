

(function () {

    const overrider = () => {
        const swagger = window.ui;
        if (!swagger) {
            console.error('Swagger wasn\'t found');
        }

        ensureAuthorization(swagger);
        reloadSchemaOnAuth(swagger);
    }

    // a hacky way to append authorization header - we are basically intercepting 
    // all requests, if no authorization was attached while user did authorized himself,
    // append token to request
    const ensureAuthorization = (swagger) => {

        // helpers to get authorization state
        const getAuthorization = () => swagger.auth()._root.entries.find(e => e[0] === 'authorized');
        const isAuthorized = () => { const auth = getAuthorization(); return auth && !!auth[1].size };

        // retrieve bearer token from authorization
        const getBearer = () => {
            const auth = getAuthorization();
            const def = auth[1]._root.entries.find(e => e[0] === 'BearerDefinition');
            if (!def) return undefined;

            const token = def[1]._root.entries.find(e => e[0] === 'value');
            if (!token) return undefined;

            return token[1];
        }

        // override fetch function of Swagger to make sure
        // that on every request of the client is authorized append auth-header
        const fetch = swagger.fn.fetch;
        swagger.fn.fetch = (req) => {
            if (!req.headers.Authorization && isAuthorized()) {
                const bearer = getBearer();
                if (bearer) {
                    req.headers.Authorization = bearer;
                }
            }
            return fetch(req);
        }

    };

    // makes that once user triggers performs authorization,
    // the schema will be reloaded from backend url
    const reloadSchemaOnAuth = (swagger) => {
        const getCurrentUrl = () => {
            const spec = swagger.getState()._root.entries.find(e => e[0] === 'spec');
            if (!spec) return undefined;

            const url = spec[1]._root.entries.find(e => e[0] === 'url');
            if (!url) return undefined;

            return url[1];
        }
        const reload = () => {
            const url = getCurrentUrl();
            if (url) {
                swagger.specActions.download(url);
            }
        };

        const handler = (caller, args) => {
            const result = caller(args);
            if (result.then) {
                result.then(() => reload())
            } else {
                reload();
            }
            return result;
        }

        const auth = swagger.authActions.authorize;
        swagger.authActions.authorize = (args) => handler(auth, args);
        const logout = swagger.authActions.logout;
        swagger.authActions.logout = (args) => handler(logout, args);
    };

    // append to event right after SwaggerUIBundle initialized
    window.addEventListener('load', () => setTimeout(overrider, 0), false);
}());


