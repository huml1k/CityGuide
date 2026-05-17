// js/api.js
const API_BASE_URL = "http://localhost:8080";   // ← Измени порт, если у тебя другой

// Получение токена
function getToken() {
    return localStorage.getItem('accessToken');
}

// Основной запрос
async function apiRequest(endpoint, options = {}) {
    const token = getToken();
    
    const config = {
        headers: {
            'Content-Type': 'application/json',
        },
        ...options
    };

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

    if (response.status === 401) {
        alert("Сессия истекла. Войдите заново.");
        localStorage.removeItem('accessToken');
        window.location.href = 'pages/login.html';
        return null;
    }

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Ошибка запроса');
    }

    return response.json();
}

// ==================== AUTH ====================
export async function login(email, password) {
    const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
    });

    const data = await response.json();
    if (response.ok) {
        localStorage.setItem('accessToken', data.accessToken);
        // Если есть refreshToken — тоже сохрани
    }
    return data;
}

export async function register(userData) {
    const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
    });
    return response.json();
}

// ==================== ROUTES ====================
export async function getRoutes(params = '') {
    return apiRequest(`/api/routes${params}`);
}

export async function getRouteById(id) {
    return apiRequest(`/api/routes/${id}`);
}

export async function createRoute(routeData) {
    return apiRequest('/api/routes', {
        method: 'POST',
        body: JSON.stringify(routeData)
    });
}

// ==================== FAVORITES ====================
export async function addToFavorites(routeId) {
    return apiRequest(`/api/favorites/${routeId}`, { method: 'POST' });
}

export async function getFavorites() {
    return apiRequest('/api/favorites');
}