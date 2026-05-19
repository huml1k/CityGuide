/**
 * CityGuide API client — все вызовы к бэкенду через API Gateway (по умолчанию :8080).
 */
(function (global) {
    const API_BASE_URL = global.CITYGUIDE_API_URL || 'http://localhost:8080';

    const STORAGE_KEYS = {
        accessToken: 'accessToken',
        refreshToken: 'refreshToken',
        userId: 'userId',
        email: 'email',
        role: 'role',
    };

    function getToken() {
        return localStorage.getItem(STORAGE_KEYS.accessToken);
    }

    function saveAuth(data) {
        if (!data) return;
        if (data.accessToken) localStorage.setItem(STORAGE_KEYS.accessToken, data.accessToken);
        if (data.refreshToken) localStorage.setItem(STORAGE_KEYS.refreshToken, data.refreshToken);
        if (data.userId) localStorage.setItem(STORAGE_KEYS.userId, data.userId);
        if (data.email) localStorage.setItem(STORAGE_KEYS.email, data.email);
        if (data.role) localStorage.setItem(STORAGE_KEYS.role, data.role);
    }

    function clearAuth() {
        Object.values(STORAGE_KEYS).forEach((key) => localStorage.removeItem(key));
    }

    function isLoggedIn() {
        return Boolean(getToken());
    }

    function getCurrentUserId() {
        return localStorage.getItem(STORAGE_KEYS.userId);
    }

    function getCurrentRole() {
        return localStorage.getItem(STORAGE_KEYS.role);
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

    async function apiRequest(endpoint, options = {}) {
        const token = getToken();
        const headers = { ...(options.headers || {}) };

        const isFormData = options.body instanceof FormData;
        if (!isFormData && !headers['Content-Type'] && options.body) {
            headers['Content-Type'] = 'application/json';
        }

        if (token) {
            headers.Authorization = `Bearer ${token}`;
        }

        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            headers,
        });

        if (response.status === 401) {
            const hadToken = Boolean(token);
            clearAuth();
            if (hadToken && !options.skipAuthRedirect) {
                const loginPath = options.loginPath || 'pages/login.html';
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

    async function authRequest(endpoint, body) {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });

        if (response.status === 204) {
            return null;
        }

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            throw new Error(data.message || data.error || `Ошибка ${response.status}`);
        }

        return data;
    }

    // ——— Auth ———
    async function login(email, password) {
        const data = await authRequest('/api/auth/login', { email, password });
        saveAuth(data);
        return data;
    }

    async function register(email, password) {
        return authRequest('/api/auth/register', { email, password });
    }

    async function logout() {
        const refreshToken = localStorage.getItem(STORAGE_KEYS.refreshToken);
        try {
            if (refreshToken) {
                await authRequest('/api/auth/logout', { refreshToken });
            }
        } finally {
            clearAuth();
        }
    }

    async function getAuthMe() {
        return apiRequest('/api/auth/me', { skipAuthRedirect: true });
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
        return apiRequest(`/api/Favorites/routes/${routeId}/check`, { skipAuthRedirect: true });
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

    global.CityGuideApi = {
        API_BASE_URL,
        STORAGE_KEYS,
        getToken,
        saveAuth,
        clearAuth,
        isLoggedIn,
        getCurrentUserId,
        getCurrentRole,
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
})(window);
