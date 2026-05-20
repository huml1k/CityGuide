(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let displayedCount = 5;

    async function loadProfile() {
        if (!(await utils.requireAuth('login.html'))) return;

        try {
            const profile = await api.getMyProfile();
            document.getElementById('creatorName').value = profile.fullName || '';
        } catch (err) {
            if (!err.message.includes('авторизац')) {
                utils.showToast(err.message, 'error');
            }
        }
    }

    async function loadMyRoutes() {
        const container = document.getElementById('myRoutes');
        utils.showLoading(container);

        try {
            const routeIds = api.getMyRouteIds();
            const routes = await Promise.all(
                routeIds.map((id) =>
                    api.getRouteById(id).catch(() => ({ id, title: 'Маршрут (на модерации)', favoritesCount: 0, durationMinutes: 0 }))
                )
            );

            container.innerHTML = '';
            if (!routes.length) {
                container.innerHTML = '<p class="text-gray-500 col-span-2">У вас пока нет маршрутов. <a href="create-route.html" class="text-blue-600 underline">Создать первый</a></p>';
                return;
            }

            routes.forEach((route) => {
                const el = document.createElement('div');
                el.className = 'bg-white border border-gray-200 rounded-2xl p-6 hover:shadow-md transition';
                el.innerHTML = `
                    <h3 class="font-semibold text-lg mb-2">${utils.escapeHtml(route.title)}</h3>
                    <p class="text-gray-500 text-sm mb-4">❤️ ${route.favoritesCount ?? 0} • ${utils.formatDuration(route.durationMinutes)}</p>
                    <button type="button" data-delete-id="${route.id}"
                        class="w-full bg-red-100 text-red-700 py-3 rounded-xl hover:bg-red-200">
                        Удалить
                    </button>`;
                el.querySelector('[data-delete-id]')?.addEventListener('click', async () => {
                    if (!confirm('Удалить маршрут?')) return;
                    try {
                        await api.deleteRoute(route.id);
                        api.removeMyRouteId(route.id);
                        el.remove();
                    } catch (err) {
                        utils.showToast(err.message, 'error');
                    }
                });
                container.appendChild(el);
            });
        } catch (err) {
            utils.showInlineError(container, err.message);
        }
    }

    function renderCreatorNotifications(notifications) {
        const container = document.getElementById('creatorNotifications');
        const loadMoreBtn = document.getElementById('loadMoreCreator');
        container.innerHTML = '';

        if (!notifications?.length) {
            container.innerHTML = '<p class="text-gray-500">Нет новых уведомлений</p>';
            loadMoreBtn?.classList.add('hidden');
            return;
        }

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

    document.addEventListener('DOMContentLoaded', async () => {
        await loadProfile();
        await loadMyRoutes();
        await loadNotifications();
    });
})();
