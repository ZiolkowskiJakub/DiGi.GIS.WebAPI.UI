/**
 * Session helpers for the GIS front end.
 *
 * The session token is not here, and cannot be: it lives in an HttpOnly cookie written by this
 * application's server, so every function below asks this application's own /user endpoints and lets the
 * server present the token to the authentication service. Nothing in this file can read, copy or leak the
 * credential, which is the point of keeping it out of the page.
 *
 * Grouped into one object rather than left as loose globals like gis-common.js: names such as logout() and
 * refresh() are too general to own at window scope.
 */
const digiUser = (function () {
    'use strict';

    function baseUrl() {
        const base = window.AppBaseUrl || '/';
        return base.endsWith('/') ? base.slice(0, -1) : base;
    }

    /**
     * Whether the header is currently showing a signed-in visitor.
     *
     * Read off the header the server rendered rather than guessed at in script - the page cannot see the
     * cookie that decides it.
     *
     * @returns {boolean} True when a session is being shown.
     */
    function isLoggedIn() {
        const toggle = document.getElementById('gis-user-toggle');
        return !!toggle && !toggle.hidden;
    }

    /**
     * Swaps the header back to its signed-out form.
     *
     * Both forms are rendered by the layout with one of them hidden, so this only changes which is visible
     * and never builds markup.
     */
    function signOut() {
        const login = document.getElementById('gis-user-login');
        const toggle = document.getElementById('gis-user-toggle');
        const menu = document.getElementById('gis-user-menu');

        if (menu) {
            menu.hidden = true;
        }

        if (toggle) {
            toggle.hidden = true;
            toggle.setAttribute('aria-expanded', 'false');
        }

        if (login) {
            login.hidden = false;
        }
    }

    /**
     * Exchanges credentials for a session.
     *
     * The property names are the wire contract of this application's login action - they must match
     * UserLoginParameter. Only success or failure comes back: which part of the credential was wrong is
     * deliberately not knowable here.
     *
     * @param {string} email - The e-mail address identifying the account.
     * @param {string} password - The password submitted for the account.
     * @returns {Promise<boolean>} True when the session was established.
     */
    async function signIn(email, password) {
        const response = await fetch(`${baseUrl()}/user/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Email: email, Password: password })
        });

        return response.ok;
    }

    /**
     * Carries the session forward by asking the server to exchange the presented token for a new one.
     *
     * @returns {Promise<boolean>} True when the session was renewed.
     */
    async function refresh() {
        const response = await fetch(`${baseUrl()}/user/session/refresh`, { method: 'POST' });
        return response.ok;
    }

    /**
     * Ends the session.
     *
     * The server clears the cookie and asks the authentication service to revoke the token; the header is
     * swapped back either way. If the request itself could not be made the cookie is still there and a
     * reload will show the session again - which is honest, and the token expires by itself within the hour.
     */
    async function logout() {
        try {
            await fetch(`${baseUrl()}/user/logout`, { method: 'POST' });
        } catch (error) {
            console.error('Error signing out:', error);
        }

        signOut();
    }

    /**
     * Performs a request that needs the session, retrying once through a refresh if the token is spent.
     *
     * One retry and no more. A token that has already expired cannot be refreshed - the service issues the
     * new one for the identity carried by the old - so after an hour away, or after the service restarts and
     * regenerates its signing key, both calls answer 401. Signing out is then the correct outcome, not a
     * fault worth retrying.
     *
     * @param {string} url - The URL to request.
     * @param {Object} [options] - Fetch options.
     * @returns {Promise<Response>} The response, after at most one refresh and retry.
     */
    async function fetchWithAuth(url, options) {
        const response = await fetch(url, options);
        if (response.status !== 401) {
            return response;
        }

        const refreshed = await refresh();
        if (!refreshed) {
            signOut();
            return response;
        }

        const response_Retry = await fetch(url, options);
        if (response_Retry.status === 401) {
            signOut();
        }

        return response_Retry;
    }

    /**
     * Reads the stored record of the signed-in visitor.
     *
     * A signed-out visitor is answered with 204 rather than an error, so absence and failure both come back
     * as null and neither is worth reporting on a page that is otherwise working.
     *
     * @returns {Promise<Object|null>} The user record, or null when there is none.
     */
    async function getUser() {
        let response;

        try {
            response = await fetchWithAuth(`${baseUrl()}/user/me`, { method: 'GET' });
        } catch (error) {
            console.error('Error reading the signed-in user:', error);
            return null;
        }

        if (response.status === 204 || !response.ok) {
            return null;
        }

        try {
            const data = await response.json();

            // A single DiGi object is serialized as a one element collection by some endpoints and on its own
            // by others, so both shapes are accepted rather than assuming which one this build answers with.
            return Array.isArray(data) ? (data[0] || null) : data;
        } catch (error) {
            return null;
        }
    }

    async function fillIdentity() {
        const user = await getUser();

        // No record means the cookie was there but the service would not accept it: the session is already
        // over and fetchWithAuth has swapped the header back, so there is nothing left to show.
        if (!user) {
            return;
        }

        const nameElement = document.getElementById('gis-user-name');
        const emailElement = document.getElementById('gis-user-email');

        const email = (user.Email || '').trim();
        const name = [user.FirstName, user.LastName].map(part => (part || '').trim()).filter(part => part.length > 0).join(' ');

        // Only the e-mail is required of a user record, so an account may carry no name at all. The e-mail then
        // moves up to the primary line and the secondary line is dropped, rather than repeating the same address
        // underneath itself. Whitespace-only names count as absent - a name of " " would otherwise blank the line.
        // textContent, never innerHTML: both values come from the user record in the database.
        if (nameElement) {
            nameElement.textContent = name || email;
        }

        if (emailElement) {
            emailElement.textContent = name ? email : '';
            emailElement.hidden = !name || !email;
        }
    }

    /**
     * Wires the header session control: the profile menu fills itself the first time it is opened, and the
     * log out item ends the session.
     *
     * The identity is fetched on first open rather than on load, so a signed-in visitor browsing the site
     * does not pay for a request per page to fill a menu they may never open.
     */
    function setupHeader() {
        const logoutLink = document.getElementById('gis-user-logout');
        if (logoutLink) {
            logoutLink.addEventListener('click', function (event) {
                event.preventDefault();
                logout();
            });
        }

        const toggle = document.getElementById('gis-user-toggle');
        if (!toggle) {
            return;
        }

        let filled = false;
        toggle.addEventListener('click', function () {
            if (filled) {
                return;
            }

            filled = true;
            fillIdentity();
        });
    }

    /**
     * Wires the sign-in form: submits the credentials, then either returns to the page the visitor came from
     * or shows the one failure message.
     *
     * @param {string} formId - The id of the sign-in form.
     */
    function setupLoginForm(formId) {
        const form = document.getElementById(formId);
        if (!form) {
            return;
        }

        const errorElement = document.getElementById('gis-login-error');
        const submitButton = document.getElementById('gis-login-submit');

        form.addEventListener('submit', async function (event) {
            event.preventDefault();

            const emailInput = form.elements['email'];
            const passwordInput = form.elements['password'];

            if (errorElement) {
                errorElement.hidden = true;
            }

            if (submitButton) {
                submitButton.disabled = true;
            }

            let succeeded = false;

            try {
                succeeded = await signIn(emailInput ? emailInput.value : '', passwordInput ? passwordInput.value : '');
            } catch (error) {
                console.error('Error signing in:', error);
            }

            if (submitButton) {
                submitButton.disabled = false;
            }

            if (succeeded) {
                window.location.assign(form.dataset.returnUrl || window.AppBaseUrl || '/');
                return;
            }

            // One message for every refusal. A wrong password, an unknown address and an account with no
            // stored credential are one answer to this page by design, and saying more would tell anyone
            // probing which addresses exist. The password is cleared, the address left to be corrected.
            if (passwordInput) {
                passwordInput.value = '';
            }

            if (errorElement) {
                errorElement.textContent = 'Sign in failed. Check your e-mail address and password.';
                errorElement.hidden = false;
            }
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupHeader);
    } else {
        setupHeader();
    }

    return {
        fetchWithAuth: fetchWithAuth,
        getUser: getUser,
        isLoggedIn: isLoggedIn,
        logout: logout,
        refresh: refresh,
        setupLoginForm: setupLoginForm,
        signIn: signIn
    };
})();
