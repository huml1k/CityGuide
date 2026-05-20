(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let displayedCount = 5;

    function renderNotifications(notifications) {
        const container = document.getElementById('userNotifications');
        const loadMoreBtn = document.getElementById('loadMoreUser');
        const markAllBtn = document.getElementById('markAllReadBtn');
        container.innerHTML = '';

        if (!notifications?.length) {
            container.innerHTML = '<p class="text-gray-500">Нет новых уведомлений</p>';
            loadMoreBtn?.classList.add('hidden');
            markAllBtn?.classList.add('hidden');
            return;
        }

        markAllBtn?.classList.remove('hidden');

        const toShow = notifications.slice(0, displayedCount);
        toShow.forEach((n) => {
            const el = document.createElement('div');
            el.className = 'p-5 bg-gray-50 hover:bg-gray-100 rounded-2xl transition flex gap-4';
            el.innerHTML = `
                <span class="text-3xl">🛎</span>
                <div class="flex-1">
                    <p class="font-medium">${utils.escapeHtml(n.title || 'Уведомление')}</p>
                    <p class="text-gray-700">${utils.escapeHtml(n.message || '')}</p>
                    <p class="text-sm text-gray-500 mt-1">${utils.formatRelativeTime(n.createdAt)}</p>
                </div>`;
            container.appendChild(el);
        });

        loadMoreBtn?.classList.toggle('hidden', notifications.length <= displayedCount);
    }

    window.loadMoreUserNotifications = function () {
        displayedCount += 5;
        loadNotifications();
    };

    window.markAllUserNotificationsRead = async function () {
        try {
            await api.markAllNotificationsRead();
            displayedCount = 5;
            await loadNotifications();
            utils.showToast('Все уведомления прочитаны', 'success');
        } catch (err) {
            utils.showToast(err.message, 'error');
        }
    };

    async function loadNotifications() {
        try {
            const list = await api.getUnreadNotifications();
            renderNotifications(Array.isArray(list) ? list : []);
        } catch (err) {
            document.getElementById('userNotifications').innerHTML =
                `<p class="text-gray-500">Уведомления недоступны: ${utils.escapeHtml(err.message)}</p>`;
        }
    }

    async function loadProfile() {
        if (!(await utils.requireAuth('login.html'))) return;

        try {
            const profile = await api.getMyProfile();
            document.getElementById('userName').value = profile.fullName || '';
            document.getElementById('userEmail').textContent =
                api.getClaims().email || '—';
        } catch (err) {
            if (err.message.includes('авторизац')) return;
            utils.showToast('Профиль: ' + err.message, 'error');
        }
    }

    window.saveProfile = async function () {
        const fullName = document.getElementById('userName').value.trim();
        const statusEl = document.getElementById('saveStatus');
        try {
            await api.updateMyProfile({ fullName });
            utils.showFormMessage(statusEl, 'Сохранено', 'success');
        } catch (err) {
            utils.showFormMessage(statusEl, err.message, 'error');
        }
    };

    async function loadFavoritesCount() {
        try {
            const ids = await api.getFavoriteRouteIds();
            const count = ids?.length || 0;
            const el = document.getElementById('favoritesCountText');
            if (el) {
                el.textContent = `${count} сохранённых маршрут${count === 1 ? '' : count < 5 ? 'а' : 'ов'}`;
            }
        } catch {
            /* ignore */
        }
    }

    document.addEventListener('DOMContentLoaded', async () => {
        const apiClient = window.CityGuideApi;
        await apiClient.initAuth();
        const role = apiClient.getCurrentRole() || '';
        if (utils.isCreatorRole(role)) {
            window.location.href = 'creator-profile.html';
            return;
        }
        if (utils.isAdminRole(role)) {
            window.location.href = 'admin-moderation.html';
            return;
        }
        await utils.initPageNav();
        await loadProfile();
        await loadFavoritesCount();
        await loadNotifications();
    });
})();
