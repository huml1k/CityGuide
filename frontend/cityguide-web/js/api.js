/**
 * CityGuide API client — авторизация с access/refresh и claims из JWT.
 */
(function (global) {
    const API_BASE_URL = global.CITYGUIDE_API_URL || 'http://localhost:8080';

    const ROLE_CLAIM =
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    const STORAGE_KEYS = {
        accessToken: 'accessToken',
        refreshToken: 'refreshToken',
        accessTokenExpiresAtUtc: 'accessTokenExpiresAtUtc',
        refreshTokenExpiresAtUtc: 'refreshTokenExpiresAtUtc',
        userId: 'userId',
        email: 'email',
        role: 'role',
    };

    /** За сколько секунд до exp обновлять access token */
    const REFRESH_BUFFER_SEC = 60;

    let refreshInFlight = null;

    function pick(obj, ...keys) {
        for (const key of keys) {
            const val = obj?.[key];
            if (val !== undefined && val !== null && val !== '') {
                return val;
            }
        }
        return null;
    }

    function normalizeAuthResponse(data) {
        if (!data || typeof data !== 'object') return null;
        return {
            userId: pick(data, 'userId', 'UserId'),
            email: pick(data, 'email', 'Email'),
            role: pick(data, 'role', 'Role'),
            accessToken: pick(data, 'accessToken', 'AccessToken'),
            accessTokenExpiresAtUtc: pick(
                data,
                'accessTokenExpiresAtUtc',
                'AccessTokenExpiresAtUtc'
            ),
            refreshToken: pick(data, 'refreshToken', 'RefreshToken'),
            refreshTokenExpiresAtUtc: pick(
                data,
                'refreshTokenExpiresAtUtc',
                'RefreshTokenExpiresAtUtc'
            ),
        };
    }

    function parseJwtPayload(token) {
        if (!token || typeof token !== 'string') return null;
        const parts = token.split('.');
        if (parts.length !== 3) return null;
        try {
            const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
            const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
            return JSON.parse(atob(padded));
        } catch {
            return null;
        }
    }

    function getClaimFromPayload(payload, ...keys) {
        if (!payload) return null;
        for (const key of keys) {
            if (payload[key] != null && payload[key] !== '') {
                return String(payload[key]);
            }
        }
        return null;
    }

    function applyClaimsFromToken(token) {
        const payload = parseJwtPayload(token);
        if (!payload) return;

        const userId = getClaimFromPayload(payload, 'sub');
        const email = getClaimFromPayload(payload, 'email');
        const role = getClaimFromPayload(payload, 'role', ROLE_CLAIM);

        if (userId) localStorage.setItem(STORAGE_KEYS.userId, userId);
        if (email) localStorage.setItem(STORAGE_KEYS.email, email);
        if (role) localStorage.setItem(STORAGE_KEYS.role, role);

        if (payload.exp) {
            localStorage.setItem(
                STORAGE_KEYS.accessTokenExpiresAtUtc,
                new Date(payload.exp * 1000).toISOString()
            );
        }
    }

    function getToken() {
        return localStorage.getItem(STORAGE_KEYS.accessToken);
    }

    function getRefreshToken() {
        return localStorage.getItem(STORAGE_KEYS.refreshToken);
    }

    function getTokenExpiryIso(key) {
        return localStorage.getItem(key);
    }

    function isExpired(isoString, bufferSec = 0) {
        if (!isoString) return true;
        const exp = new Date(isoString).getTime();
        if (Number.isNaN(exp)) return true;
        return Date.now() >= exp - bufferSec * 1000;
    }

    function isAccessTokenExpired() {
        const token = getToken();
        if (!token) return true;
        const stored = getTokenExpiryIso(STORAGE_KEYS.accessTokenExpiresAtUtc);
        if (stored) return isExpired(stored, REFRESH_BUFFER_SEC);
        const payload = parseJwtPayload(token);
        if (payload?.exp) {
            return Date.now() >= payload.exp * 1000 - REFRESH_BUFFER_SEC * 1000;
        }
        return false;
    }

    function isRefreshTokenExpired() {
        return isExpired(
            getTokenExpiryIso(STORAGE_KEYS.refreshTokenExpiresAtUtc),
            0
        );
    }

    function saveAuth(data) {
        const auth = normalizeAuthResponse(data);
        if (!auth) return auth;

        if (auth.accessToken) {
            localStorage.setItem(STORAGE_KEYS.accessToken, auth.accessToken);
            applyClaimsFromToken(auth.accessToken);
        }
        if (auth.refreshToken) {
            localStorage.setItem(STORAGE_KEYS.refreshToken, auth.refreshToken);
        }
        if (auth.accessTokenExpiresAtUtc) {
            localStorage.setItem(
                STORAGE_KEYS.accessTokenExpiresAtUtc,
                auth.accessTokenExpiresAtUtc
            );
        }
        if (auth.refreshTokenExpiresAtUtc) {
            localStorage.setItem(
                STORAGE_KEYS.refreshTokenExpiresAtUtc,
                auth.refreshTokenExpiresAtUtc
            );
        }
        if (auth.userId) localStorage.setItem(STORAGE_KEYS.userId, String(auth.userId));
        if (auth.email) localStorage.setItem(STORAGE_KEYS.email, auth.email);
        if (auth.role) localStorage.setItem(STORAGE_KEYS.role, auth.role);

        return auth;
    }

    function clearAuth() {
        Object.values(STORAGE_KEYS).forEach((key) => localStorage.removeItem(key));
    }

    function isLoggedIn() {
        const refresh = getRefreshToken();
        if (refresh && !isRefreshTokenExpired()) return true;
        const access = getToken();
        return Boolean(access && !isAccessTokenExpired());
    }

    function getCurrentUserId() {
        return localStorage.getItem(STORAGE_KEYS.userId);
    }

    function getCurrentRole() {
        return localStorage.getItem(STORAGE_KEYS.role);
    }

    function getClaims() {
        const token = getToken();
        const payload = parseJwtPayload(token);
        if (!payload) {
            return {
                userId: getCurrentUserId(),
                email: localStorage.getItem(STORAGE_KEYS.email),
                role: getCurrentRole(),
            };
        }
        return {
            userId: getClaimFromPayload(payload, 'sub') || getCurrentUserId(),
            email: getClaimFromPayload(payload, 'email') || localStorage.getItem(STORAGE_KEYS.email),
            role:
                getClaimFromPayload(payload, 'role', ROLE_CLAIM) || getCurrentRole(),
            exp: payload.exp,
            jti: payload.jti,
        };
    }

    function getMyRouteIds() {
        try {
            return JSON.parse(localStorage.getItem('myRouteIds') || '[]');
        } catch {
            return [];
        }
    }

    function addMyRouteId(routeId) {
        const ids = getMyRouteIds();
        if (!ids.includes(routeId)) {
            ids.push(routeId);
            localStorage.setItem('myRouteIds', JSON.stringify(ids));
        }
    }

    function removeMyRouteId(routeId) {
        const ids = getMyRouteIds().filter((id) => id !== routeId);
        localStorage.setItem('myRouteIds', JSON.stringify(ids));
    }

    async function parseErrorResponse(response) {
        const text = await response.text();
        if (!text) {
            return `Ошибка ${response.status}: ${response.statusText}`;
        }
        try {
            const json = JSON.parse(text);
            return json.message || json.error || json.title || text;
        } catch {
            return text;
        }
    }

    async function authRequest(endpoint, body, method = 'POST') {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });

        if (response.status === 204) {
            return null;
        }

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            throw new Error(
                data.message || data.error || `Ошибка ${response.status}`
            );
        }

        return data;
    }

    async function refreshAccessToken() {
        if (refreshInFlight) {
            return refreshInFlight;
        }

        const refreshToken = getRefreshToken();
        if (!refreshToken || isRefreshTokenExpired()) {
            throw new Error('Refresh token отсутствует или истёк');
        }

        refreshInFlight = (async () => {
            try {
                const data = await authRequest('/api/auth/refresh', { refreshToken });
                return saveAuth(data);
            } finally {
                refreshInFlight = null;
            }
        })();

        return refreshInFlight;
    }

    /**
     * Продлевает сессию: refresh при истёкшем access, если есть refresh.
     * @returns {boolean} можно ли отправлять авторизованные запросы
     */
    async function ensureAuthenticated() {
        const access = getToken();
        const refresh = getRefreshToken();

        if (!access && !refresh) {
            return false;
        }

        if (access && !isAccessTokenExpired()) {
            return true;
        }

        if (!refresh || isRefreshTokenExpired()) {
            if (!access) clearAuth();
            return false;
        }

        try {
            await refreshAccessToken();
            return Boolean(getToken());
        } catch {
            clearAuth();
            return false;
        }
    }

    async function apiRequest(endpoint, options = {}) {
        const skipRefresh = options.skipTokenRefresh === true;
        const isAuthEndpoint =
            endpoint.startsWith('/api/auth/login') ||
            endpoint.startsWith('/api/auth/register') ||
            endpoint.startsWith('/api/auth/refresh');

        if (!skipRefresh && !isAuthEndpoint) {
            await ensureAuthenticated();
        }

        const buildHeaders = () => {
            const headers = { ...(options.headers || {}) };
            const isFormData = options.body instanceof FormData;
            if (!isFormData && !headers['Content-Type'] && options.body) {
                headers['Content-Type'] = 'application/json';
            }
            const token = getToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }
            return headers;
        };

        const doFetch = () =>
            fetch(`${API_BASE_URL}${endpoint}`, {
                method: options.method || 'GET',
                body: options.body,
                credentials: options.credentials,
                signal: options.signal,
                headers: buildHeaders(),
            });

        let response = await doFetch();

        if (
            response.status === 401 &&
            !isAuthEndpoint &&
            getRefreshToken() &&
            !isRefreshTokenExpired()
        ) {
            try {
                await refreshAccessToken();
                response = await doFetch();
            } catch {
                /* refresh не удался — обработаем 401 ниже */
            }
        }

        if (response.status === 401) {
            const hadSession = Boolean(getToken() || getRefreshToken());
            clearAuth();
            if (hadSession && !options.skipAuthRedirect) {
                const loginPath = options.loginPath || getLoginPath();
                window.location.href = loginPath;
            }
            throw new Error('Требуется авторизация');
        }

        if (response.status === 204) {
            return null;
        }

        if (!response.ok) {
            throw new Error(await parseErrorResponse(response));
        }

        const contentType = response.headers.get('content-type') || '';
        if (contentType.includes('application/json')) {
            return response.json();
        }

        return response.text();
    }

    function getLoginPath() {
        const path = window.location.pathname || '';
        if (path.includes('/pages/')) {
            return 'login.html';
        }
        return 'pages/login.html';
    }

    // ——— Auth ———
    async function login(email, password) {
        const data = await authRequest('/api/auth/login', { email, password });
        return saveAuth(data);
    }

    async function register(email, password) {
        const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
        });

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            throw new Error(
                data.message || data.error || `Ошибка ${response.status}`
            );
        }

        return saveAuth(data);
    }

    async function logout() {
        const refreshToken = getRefreshToken();
        try {
            if (refreshToken) {
                await authRequest('/api/auth/logout', { refreshToken });
            }
        } finally {
            clearAuth();
        }
    }

    async function getAuthMe() {
        return apiRequest('/api/auth/me', {
            skipAuthRedirect: true,
            skipTokenRefresh: false,
        });
    }

    // ——— User profile ———
    async function getMyProfile() {
        return apiRequest('/api/UserProfiles/me');
    }

    async function getProfileById(userId) {
        return apiRequest(`/api/UserProfiles/${userId}`, { skipAuthRedirect: true });
    }

    async function updateMyProfile(payload) {
        return apiRequest('/api/UserProfiles/me', {
            method: 'PUT',
            body: JSON.stringify(payload),
        });
    }

    // ——— Routes ———
    async function getRoutes() {
        return apiRequest('/api/routes');
    }

    async function searchRoutes(search) {
        const params = new URLSearchParams({ search: search || '' });
        return apiRequest(`/api/routes/search?${params}`);
    }

    async function getRouteById(id) {
        return apiRequest(`/api/routes/${id}`, { skipAuthRedirect: true });
    }

    async function createRoute(routeData) {
        return apiRequest('/api/routes', {
            method: 'POST',
            body: JSON.stringify(routeData),
        });
    }

    async function updateRoute(id, routeData) {
        return apiRequest(`/api/routes/${id}`, {
            method: 'PUT',
            body: JSON.stringify(routeData),
        });
    }

    async function deleteRoute(id) {
        return apiRequest(`/api/routes/${id}`, { method: 'DELETE' });
    }

    // ——— Moderation (internal) ———
    async function getPendingRoutes() {
        return apiRequest('/internal/routes/pending', { skipAuthRedirect: true });
    }

    async function approveRoute(routeId) {
        return apiRequest(`/internal/routes/${routeId}/approve`, { method: 'POST' });
    }

    async function rejectRoute(routeId) {
        return apiRequest(`/internal/routes/${routeId}/reject`, { method: 'POST' });
    }

    // ——— Tags ———
    async function getTags() {
        return apiRequest('/api/tags', { skipAuthRedirect: true });
    }

    // ——— Files ———
    function getImageUrl(imageId) {
        return `${API_BASE_URL}/api/files/images/${imageId}`;
    }

    async function uploadRouteImage(routeId, file, isCover, orderIndex) {
        const form = new FormData();
        form.append('RouteId', routeId);
        form.append('File', file);
        form.append('IsCover', String(isCover));
        form.append('OrderIndex', String(orderIndex));
        return apiRequest('/api/files/images', { method: 'POST', body: form });
    }

    async function uploadRouteAudio(routeId, file) {
        const form = new FormData();
        form.append('RouteId', routeId);
        form.append('File', file);
        return apiRequest('/api/files/audio', { method: 'POST', body: form });
    }

    async function getAudioMeta(audioId) {
        return apiRequest(`/api/files/audio/${audioId}`, { skipAuthRedirect: true });
    }

    // ——— Favorites ———
    async function getFavoriteRouteIds() {
        return apiRequest('/api/Favorites/routes');
    }

    async function addFavorite(routeId) {
        return apiRequest(`/api/Favorites/routes/${routeId}`, { method: 'POST' });
    }

    async function removeFavorite(routeId) {
        return apiRequest(`/api/Favorites/routes/${routeId}`, { method: 'DELETE' });
    }

    async function checkFavorite(routeId) {
        return apiRequest(`/api/Favorites/routes/${routeId}/check`, {
            skipAuthRedirect: true,
        });
    }

    async function batchCheckFavorites(routeIds) {
        return apiRequest('/api/Favorites/routes/batch-check', {
            method: 'POST',
            body: JSON.stringify(routeIds),
        });
    }

    // ——— Notifications ———
    async function getUnreadNotifications() {
        return apiRequest('/api/notifications/unread');
    }

    async function markNotificationRead(id) {
        return apiRequest(`/api/notifications/${id}/read`, { method: 'POST' });
    }

    async function markAllNotificationsRead() {
        return apiRequest('/api/notifications/read-all', { method: 'POST' });
    }

    /** Вызов при загрузке приложения — тихий refresh без редиректа */
    let initPromise = null;
    function initAuth() {
        if (!initPromise) {
            initPromise = ensureAuthenticated().catch(() => false);
        }
        return initPromise;
    }

    global.CityGuideApi = {
        API_BASE_URL,
        STORAGE_KEYS,
        ROLE_CLAIM,
        getToken,
        getRefreshToken,
        getClaims,
        saveAuth,
        clearAuth,
        isLoggedIn,
        getCurrentUserId,
        getCurrentRole,
        ensureAuthenticated,
        refreshAccessToken,
        initAuth,
        getMyRouteIds,
        addMyRouteId,
        removeMyRouteId,
        login,
        register,
        logout,
        getAuthMe,
        getMyProfile,
        getProfileById,
        updateMyProfile,
        getRoutes,
        searchRoutes,
        getRouteById,
        createRoute,
        updateRoute,
        deleteRoute,
        getPendingRoutes,
        approveRoute,
        rejectRoute,
        getTags,
        getImageUrl,
        uploadRouteImage,
        uploadRouteAudio,
        getAudioMeta,
        getFavoriteRouteIds,
        addFavorite,
        removeFavorite,
        checkFavorite,
        batchCheckFavorites,
        getUnreadNotifications,
        markNotificationRead,
        markAllNotificationsRead,
    };

    initAuth();
})(window);
