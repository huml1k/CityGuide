(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let displayedCount = 5;
    let myRoutes = [];

    async function loadProfile() {
        if (!(await utils.requireCreator('login.html'))) return;

        try {
            const profile = await api.getMyProfile();
            document.getElementById('creatorName').value = profile.fullName || '';
        } catch (err) {
            if (!err.message.includes('авторизац')) {
                utils.showToast(err.message, 'error');
            }
        }
    }

    window.saveCreatorProfile = async function () {
        const fullName = document.getElementById('creatorName').value.trim();
        const statusEl = document.getElementById('saveStatus');
        try {
            await api.updateMyProfile({ fullName });
            utils.showFormMessage(statusEl, 'Профиль сохранён', 'success');
        } catch (err) {
            utils.showFormMessage(statusEl, err.message, 'error');
        }
    };

    function renderMyRoutes(routes) {
        const container = document.getElementById('myRoutes');
        container.innerHTML = '';

        if (!routes.length) {
            container.innerHTML =
                '<p class="text-gray-500 col-span-2">У вас пока нет маршрутов. <a href="create-route.html" class="text-blue-600 underline">Создать первый</a></p>';
            return;
        }

        routes.forEach((route) => {
            const statusKey = (route.status || '').toLowerCase();
            const canEdit = statusKey !== 'approved';
            const el = document.createElement('div');
            el.className = 'bg-white border border-gray-200 rounded-2xl p-6 hover:shadow-md transition flex flex-col';
            el.innerHTML = `
                <div class="flex justify-between items-start gap-2 mb-2">
                    <h3 class="font-semibold text-lg">${utils.escapeHtml(route.title)}</h3>
                    <span class="text-xs px-3 py-1 rounded-full shrink-0 ${utils.routeStatusBadgeClass(route.status)}">
                        ${utils.escapeHtml(utils.formatRouteStatus(route.status))}
                    </span>
                </div>
                <p class="text-gray-500 text-sm mb-4">❤️ ${route.favoritesCount ?? 0} • ${utils.formatDuration(route.durationMinutes)}</p>
                <div class="mt-auto flex flex-col gap-2">
                    ${canEdit ? `<a href="edit-route.html?id=${route.id}"
                        class="w-full text-center bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700">Редактировать</a>` : ''}
                    ${statusKey === 'approved' ? `<a href="route-detail.html?id=${route.id}"
                        class="w-full text-center border border-gray-300 py-3 rounded-xl hover:bg-gray-50">Открыть</a>` : ''}
                    <button type="button" data-delete-id="${route.id}"
                        class="w-full bg-red-100 text-red-700 py-3 rounded-xl hover:bg-red-200">
                        Удалить
                    </button>
                </div>`;

            el.querySelector('[data-delete-id]')?.addEventListener('click', async () => {
                if (!confirm('Удалить маршрут?')) return;
                try {
                    await api.deleteRoute(route.id);
                    api.removeMyRouteId(route.id);
                    myRoutes = myRoutes.filter((r) => r.id !== route.id);
                    renderMyRoutes(myRoutes);
                } catch (err) {
                    utils.showToast(err.message, 'error');
                }
            });
            container.appendChild(el);
        });
    }

    async function loadMyRoutes() {
        const container = document.getElementById('myRoutes');
        utils.showLoading(container);

        try {
            myRoutes = await api.getMyRoutes();
            if (!Array.isArray(myRoutes)) myRoutes = [];

            myRoutes.forEach((r) => api.addMyRouteId(r.id));
            renderMyRoutes(myRoutes);
        } catch (err) {
            const fallbackIds = api.getMyRouteIds();
            if (fallbackIds.length) {
                myRoutes = await Promise.all(
                    fallbackIds.map((id) =>
                        api.getRouteById(id).catch(() => ({
                            id,
                            title: 'Маршрут',
                            favoritesCount: 0,
                            durationMinutes: 0,
                            status: 'pendingModeration',
                        }))
                    )
                );
                renderMyRoutes(myRoutes);
                return;
            }
            utils.showInlineError(container, err.message);
        }
    }

    function renderCreatorNotifications(notifications) {
        const container = document.getElementById('creatorNotifications');
        const loadMoreBtn = document.getElementById('loadMoreCreator');
        const markAllBtn = document.getElementById('markAllReadBtn');
        container.innerHTML = '';

        if (!notifications?.length) {
            container.innerHTML = '<p class="text-gray-500">Нет новых уведомлений</p>';
            loadMoreBtn?.classList.add('hidden');
            markAllBtn?.classList.add('hidden');
            return;
        }

        markAllBtn?.classList.remove('hidden');

        notifications.slice(0, displayedCount).forEach((n) => {
            const el = document.createElement('div');
            el.className = 'p-6 bg-gray-50 hover:bg-gray-100 rounded-2xl transition-all duration-200';
            el.innerHTML = `
                <div class="flex gap-4">
                    <span class="text-4xl flex-shrink-0">❤️</span>
                    <div class="flex-1">
                        <p class="font-medium">${utils.escapeHtml(n.title || 'Уведомление')}</p>
                        <p class="text-gray-700 mt-1">${utils.escapeHtml(n.message || '')}</p>
                        <p class="text-sm text-gray-500 mt-2">${utils.formatRelativeTime(n.createdAt)}</p>
                    </div>
                </div>`;
            container.appendChild(el);
        });

        loadMoreBtn?.classList.toggle('hidden', notifications.length <= displayedCount);
    }

    async function loadNotifications() {
        try {
            const list = await api.getUnreadNotifications();
            renderCreatorNotifications(Array.isArray(list) ? list : []);
        } catch {
            document.getElementById('creatorNotifications').innerHTML =
                '<p class="text-gray-500">Уведомления недоступны</p>';
        }
    }

    window.loadMoreCreatorNotifications = function () {
        displayedCount += 5;
        loadNotifications();
    };

    window.markAllCreatorNotificationsRead = async function () {
        try {
            await api.markAllNotificationsRead();
            displayedCount = 5;
            await loadNotifications();
            utils.showToast('Все уведомления прочитаны', 'success');
        } catch (err) {
            utils.showToast(err.message, 'error');
        }
    };

    document.addEventListener('DOMContentLoaded', async () => {
        await utils.initPageNav();
        await loadProfile();
        await loadMyRoutes();
        await loadNotifications();
    });
})();
