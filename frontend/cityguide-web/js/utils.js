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

    const TOAST_DURATION_MS = 4500;

    function ensureToastRoot() {
        let root = document.getElementById('cg-toast-root');
        if (!root) {
            root = document.createElement('div');
            root.id = 'cg-toast-root';
            root.className =
                'fixed top-4 right-4 z-[9999] flex flex-col gap-2 max-w-sm w-full pointer-events-none';
            root.setAttribute('aria-live', 'polite');
            document.body.appendChild(root);
        }
        return root;
    }

    function showToast(message, type = 'error') {
        if (!message) return;

        const root = ensureToastRoot();
        const styles = {
            error: 'bg-red-50 text-red-800 border-red-200',
            success: 'bg-green-50 text-green-800 border-green-200',
            info: 'bg-blue-50 text-blue-800 border-blue-200',
        };
        const toast = document.createElement('div');
        toast.className = `pointer-events-auto px-4 py-3 rounded-xl border shadow-lg text-sm ${styles[type] || styles.error}`;
        toast.textContent = message;

        const close = () => {
            toast.classList.add('opacity-0', 'translate-x-2', 'transition-all', 'duration-200');
            setTimeout(() => toast.remove(), 200);
        };

        toast.addEventListener('click', close);
        root.appendChild(toast);
        setTimeout(close, TOAST_DURATION_MS);
    }

    /** @deprecated Используйте showToast */
    function showAlert(message, type = 'error') {
        showToast(message, type);
    }

    function showFormMessage(element, message, type = 'error') {
        if (!element) {
            showToast(message, type);
            return;
        }
        element.textContent = message;
        element.classList.remove('hidden', 'text-red-600', 'text-green-600', 'bg-red-50', 'bg-green-50', 'text-red-700');
        if (type === 'success') {
            element.classList.add('text-green-600');
        } else {
            element.classList.add('text-red-700', 'bg-red-50');
        }
        element.classList.remove('hidden');
    }

    function hideFormMessage(element) {
        if (!element) return;
        element.textContent = '';
        element.classList.add('hidden');
    }

    function showInlineError(container, message) {
        if (!container) {
            showToast(message, 'error');
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

    function isCreatorRole(role) {
        return (role || '').toLowerCase() === 'creator';
    }

    function isAdminRole(role) {
        return (role || '').toLowerCase() === 'admin';
    }

    /** Страница в подпапке pages/ (по пути скриптов или URL). */
    function isInPagesSubfolder() {
        const script = document.querySelector('script[src*="js/api.js"]');
        if (script) {
            const src = (script.getAttribute('src') || '').replace(/\\/g, '/');
            if (src.startsWith('../')) return true;
            if (/^(\.\/)?js\/api\.js/i.test(src)) return false;
        }
        return /\/pages(\/|$)/i.test(window.location.pathname || '');
    }

    /** Префикс до корня приложения (cityguide-web): '' или '../'. */
    function getAppBaseHref() {
        const script = document.querySelector('script[src*="js/api.js"]');
        if (script) {
            const src = (script.getAttribute('src') || '').replace(/\\/g, '/');
            if (src.startsWith('../')) return '../';
            if (src.startsWith('./')) return './';
            const match = src.match(/^(.*\/)js\/api\.js/i);
            if (match) return match[1];
        }
        return isInPagesSubfolder() ? '../' : '';
    }

    function getPagesBasePath() {
        return isInPagesSubfolder() ? '' : 'pages/';
    }

    /** Путь к главной относительно текущей страницы */
    function getHomeHref() {
        return `${getAppBaseHref()}index.html`;
    }

    function navigateToHome() {
        window.location.href = new URL(getHomeHref(), window.location.href).href;
    }

    function formatRouteStatus(status) {
        const key = (status || '').toLowerCase();
        if (key === 'approved') return 'Опубликован';
        if (key === 'rejected') return 'Отклонён';
        if (key === 'pendingmoderation') return 'На модерации';
        return status || '—';
    }

    function routeStatusBadgeClass(status) {
        const key = (status || '').toLowerCase();
        if (key === 'approved') return 'bg-green-100 text-green-800';
        if (key === 'rejected') return 'bg-red-100 text-red-800';
        return 'bg-amber-100 text-amber-800';
    }

    async function updateAuthNav(navEl) {
        if (!navEl) return;
        const api = global.CityGuideApi;
        if (!api) return;

        await api.initAuth();

        const base = getPagesBasePath();
        const homeHref = getHomeHref();

        if (api.isLoggedIn()) {
            const role = api.getCurrentRole() || '';
            let profileHref = `${base}profile.html`;
            if (isAdminRole(role)) profileHref = `${base}admin-moderation.html`;
            else if (isCreatorRole(role)) profileHref = `${base}creator-profile.html`;

            const creatorLink = isCreatorRole(role)
                ? `<a href="${base}create-route.html" class="hover:underline font-medium">Создать маршрут</a>`
                : '';

            navEl.innerHTML = `
                <a href="${homeHref}" class="hover:underline">Главная</a>
                ${creatorLink}
                <a href="${profileHref}" class="hover:underline">Профиль</a>
                <a href="${base}favorites.html" class="hover:underline">Избранное</a>
                <button type="button" id="logoutBtn"
                    class="bg-white text-blue-600 px-5 py-2 rounded-lg font-medium hover:bg-gray-100 transition">
                    Выйти
                </button>`;
            const btn = document.getElementById('logoutBtn');
            if (btn) {
                btn.addEventListener('click', async () => {
                    await api.logout();
                    navigateToHome();
                });
            }
        } else {
            navEl.innerHTML = `
                <a href="${base}login.html"
                   class="bg-white text-blue-600 px-5 py-2 rounded-lg font-medium hover:bg-gray-100 transition">
                    Войти
                </a>`;
        }
    }

    async function initPageNav(navId = 'mainNav') {
        await updateAuthNav(document.getElementById(navId));
    }

    async function requireCreator(loginPath = 'login.html') {
        if (!(await requireAuth(loginPath))) return false;
        const api = global.CityGuideApi;
        if (!isCreatorRole(api.getCurrentRole())) {
            showToast('Доступно только авторам маршрутов', 'error');
            const base = getPagesBasePath();
            window.location.href = `${base}profile.html`;
            return false;
        }
        return true;
    }

    async function requireAuth(loginPath = 'login.html') {
        const api = global.CityGuideApi;
        if (!api) {
            window.location.href = loginPath;
            return false;
        }
        const ok = await api.ensureAuthenticated();
        if (!ok) {
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
        showToast,
        showAlert,
        showFormMessage,
        hideFormMessage,
        showInlineError,
        showLoading,
        escapeHtml,
        parseDurationMinutes,
        isCreatorRole,
        isAdminRole,
        isInPagesSubfolder,
        getAppBaseHref,
        getPagesBasePath,
        getHomeHref,
        navigateToHome,
        formatRouteStatus,
        routeStatusBadgeClass,
        updateAuthNav,
        initPageNav,
        requireAuth,
        requireCreator,
    };
})(window);
