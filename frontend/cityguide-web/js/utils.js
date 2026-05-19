/**
 * Общие утилиты UI и форматирования данных.
 */
(function (global) {
    function formatDuration(minutes) {
        if (!minutes && minutes !== 0) return '—';
        if (minutes < 60) return `${minutes} мин`;
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        if (mins === 0) return `${hours} ч`;
        return `${hours} ч ${mins} мин`;
    }

    function formatDate(isoString) {
        if (!isoString) return '';
        try {
            return new Date(isoString).toLocaleDateString('ru-RU', {
                day: 'numeric',
                month: 'long',
                year: 'numeric',
            });
        } catch {
            return isoString;
        }
    }

    function formatRelativeTime(isoString) {
        if (!isoString) return '';
        const date = new Date(isoString);
        const diffMs = Date.now() - date.getTime();
        const diffMin = Math.floor(diffMs / 60000);
        if (diffMin < 1) return 'только что';
        if (diffMin < 60) return `${diffMin} мин назад`;
        const diffHours = Math.floor(diffMin / 60);
        if (diffHours < 24) return `${diffHours} ч назад`;
        const diffDays = Math.floor(diffHours / 24);
        if (diffDays === 1) return 'вчера';
        if (diffDays < 7) return `${diffDays} дн назад`;
        return formatDate(isoString);
    }

    function getQueryParam(name) {
        return new URLSearchParams(window.location.search).get(name);
    }

    function getRoutePlaceholder(title) {
        const text = encodeURIComponent((title || 'Маршрут').slice(0, 24));
        return `https://placehold.co/600x400/2563eb/ffffff?text=${text}`;
    }

    function showAlert(message, type = 'error') {
        const prefix = type === 'success' ? '✅ ' : type === 'info' ? 'ℹ️ ' : '⚠️ ';
        alert(prefix + message);
    }

    function showInlineError(container, message) {
        if (!container) {
            showAlert(message);
            return;
        }
        container.innerHTML = `
            <div class="col-span-full text-center py-16">
                <p class="text-red-600 text-lg mb-2">Не удалось загрузить данные</p>
                <p class="text-gray-500 mb-4">${escapeHtml(message)}</p>
                <button type="button" onclick="location.reload()" class="text-blue-600 hover:underline">Повторить</button>
            </div>`;
    }

    function showLoading(container, text = 'Загрузка...') {
        if (!container) return;
        container.innerHTML = `
            <div class="col-span-full text-center py-16 text-gray-500">
                <p class="text-lg">${escapeHtml(text)}</p>
            </div>`;
    }

    function escapeHtml(str) {
        if (str == null) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    function parseDurationMinutes(value) {
        if (typeof value === 'number' && !Number.isNaN(value)) return value;
        const str = String(value || '').trim();
        const num = parseInt(str, 10);
        if (!Number.isNaN(num) && String(num) === str) return num;
        const hoursMatch = str.match(/(\d+)\s*ч/);
        const minsMatch = str.match(/(\d+)\s*мин/);
        let total = 0;
        if (hoursMatch) total += parseInt(hoursMatch[1], 10) * 60;
        if (minsMatch) total += parseInt(minsMatch[1], 10);
        if (total > 0) return total;
        return 60;
    }

    function updateAuthNav(navEl) {
        if (!navEl) return;
        const api = global.CityGuideApi;
        if (!api) return;

        if (api.isLoggedIn()) {
            const role = (api.getCurrentRole() || '').toLowerCase();
            let profileHref = 'pages/profile.html';
            if (role === 'admin') profileHref = 'pages/admin-moderation.html';
            else if (role === 'creator') profileHref = 'pages/creator-profile.html';

            navEl.innerHTML = `
                <a href="${profileHref}" class="hover:underline">Профиль</a>
                <a href="pages/favorites.html" class="hover:underline">Избранное</a>
                <button type="button" id="logoutBtn"
                    class="bg-white text-blue-600 px-5 py-2 rounded-lg font-medium hover:bg-gray-100 transition">
                    Выйти
                </button>`;
            const btn = document.getElementById('logoutBtn');
            if (btn) {
                btn.addEventListener('click', async () => {
                    await api.logout();
                    window.location.href = 'index.html';
                });
            }
        } else {
            navEl.innerHTML = `
                <a href="pages/login.html"
                   class="bg-white text-blue-600 px-5 py-2 rounded-lg font-medium hover:bg-gray-100 transition">
                    Войти
                </a>`;
        }
    }

    function requireAuth(loginPath = 'login.html') {
        if (!global.CityGuideApi?.isLoggedIn()) {
            window.location.href = loginPath;
            return false;
        }
        return true;
    }

    global.CityGuideUtils = {
        formatDuration,
        formatDate,
        formatRelativeTime,
        getQueryParam,
        getRoutePlaceholder,
        showAlert,
        showInlineError,
        showLoading,
        escapeHtml,
        parseDurationMinutes,
        updateAuthNav,
        requireAuth,
    };
})(window);
