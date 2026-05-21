(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let pendingRoutes = [];

    function createModerationCard(route) {
        const tag = route.tags?.[0] || 'Маршрут';
        const imageUrl = api.getRouteCoverImageUrl(route) || utils.getRoutePlaceholder(route.title);

        const card = document.createElement('div');
        card.className = 'route-card bg-white rounded-3xl overflow-hidden shadow';
        card.dataset.routeId = route.id;
        const description = route.description
            ? utils.escapeHtml(route.description.slice(0, 120)) +
              (route.description.length > 120 ? '…' : '')
            : 'Без описания';

        card.innerHTML = `
            <div class="block">
                <img src="${imageUrl}" alt="${utils.escapeHtml(route.title)}" class="w-full h-52 object-cover">
                <div class="p-5">
                    <h3 class="font-semibold text-lg leading-tight mb-2">${utils.escapeHtml(route.title)}</h3>
                    <p class="text-gray-600 text-sm mb-3 line-clamp-2">${description}</p>
                    <div class="flex items-center gap-4 text-sm text-gray-600 mb-2">
                        <span>⏱ ${utils.formatDuration(route.durationMinutes)}</span>
                        <span class="px-3 py-1 bg-amber-100 text-amber-800 rounded-full text-xs font-medium">На модерации</span>
                        <span class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">${utils.escapeHtml(tag)}</span>
                    </div>
                </div>
            </div>
            <div class="border-t p-5 flex gap-3 moderation-actions">
                <button type="button" data-action="reject"
                    class="flex-1 bg-red-100 hover:bg-red-200 text-red-700 py-3 rounded-2xl font-medium transition">
                    Отклонить
                </button>
                <button type="button" data-action="approve"
                    class="flex-1 bg-green-600 hover:bg-green-700 text-white py-3 rounded-2xl font-medium transition">
                    Одобрить
                </button>
            </div>`;

        card.querySelector('[data-action="approve"]')?.addEventListener('click', () =>
            handleModeration(route.id, 'approve', card)
        );
        card.querySelector('[data-action="reject"]')?.addEventListener('click', () =>
            handleModeration(route.id, 'reject', card)
        );

        return card;
    }

    async function handleModeration(routeId, action, card) {
        const label = action === 'approve' ? 'одобрить' : 'отклонить';
        if (!confirm(`Вы уверены, что хотите ${label} этот маршрут?`)) return;

        const actionsEl = card.querySelector('.moderation-actions');
        try {
            if (action === 'approve') {
                await api.approveRoute(routeId);
            } else {
                await api.rejectRoute(routeId);
            }
            pendingRoutes = pendingRoutes.filter((r) => r.id !== routeId);
            document.getElementById('pendingCount').textContent = pendingRoutes.length;
            actionsEl.innerHTML = `
                <p class="flex-1 text-center py-3 font-semibold ${action === 'approve' ? 'text-green-600' : 'text-red-600'}">
                    ${action === 'approve' ? '✅ Одобрен' : '❌ Отклонён'}
                </p>`;
            card.style.opacity = '0.7';
        } catch (err) {
            utils.showToast(err.message, 'error');
        }
    }

    async function loadPending() {
        const container = document.getElementById('moderationContainer');
        utils.showLoading(container);

        try {
            pendingRoutes = await api.getPendingRoutes();
            if (!Array.isArray(pendingRoutes)) pendingRoutes = [];

            container.innerHTML = '';
            document.getElementById('pendingCount').textContent = pendingRoutes.length;

            if (!pendingRoutes.length) {
                container.innerHTML = `
                    <div class="col-span-3 text-center py-20">
                        <p class="text-2xl text-gray-400">Нет маршрутов на модерации ✅</p>
                    </div>`;
                return;
            }

            pendingRoutes.forEach((route) => container.appendChild(createModerationCard(route)));
        } catch (err) {
            utils.showInlineError(container, err.message);
        }
    }

    document.addEventListener('DOMContentLoaded', async () => {
        const apiClient = window.CityGuideApi;
        await apiClient.initAuth();
        if (!utils.isAdminRole(apiClient.getCurrentRole())) {
            if (!(await utils.requireAuth('login.html'))) return;
            utils.showToast('Доступ только для администратора', 'error');
            utils.navigateToHome();
            return;
        }
        await utils.initPageNav('mainNav');
        await loadPending();
    });
})();
