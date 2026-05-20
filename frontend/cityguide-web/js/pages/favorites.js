(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    function createFavoriteCard(route) {
        const tag = route.tags?.[0] || 'Маршрут';
        const imageUrl = route.images?.length
            ? api.getImageUrl(route.images.find((i) => i.isCover)?.id || route.images[0].id)
            : utils.getRoutePlaceholder(route.title);

        const card = document.createElement('div');
        card.className = 'route-card cursor-pointer bg-white rounded-2xl overflow-hidden shadow';
        card.innerHTML = `
            <a href="route-detail.html?id=${route.id}" class="block">
                <img src="${imageUrl}" alt="${utils.escapeHtml(route.title)}" class="w-full h-52 object-cover">
                <div class="p-5">
                    <h3 class="font-semibold text-lg leading-tight mb-3">${utils.escapeHtml(route.title)}</h3>
                    <div class="flex items-center gap-4 text-sm text-gray-600 mb-4">
                        <span>⏱ ${utils.formatDuration(route.durationMinutes)}</span>
                        <span class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">${utils.escapeHtml(tag)}</span>
                    </div>
                    <div class="flex items-center justify-between">
                        <span class="text-gray-600">❤️ ${route.favoritesCount ?? 0}</span>
                        <span class="text-blue-600 font-medium text-sm">Подробнее →</span>
                    </div>
                </div>
            </a>`;
        return card;
    }

    async function loadFavorites() {
        if (!(await utils.requireAuth('login.html'))) return;

        const container = document.getElementById('favoritesContainer');
        const emptyState = document.getElementById('emptyState');
        const countEl = document.getElementById('favoritesCount');

        utils.showLoading(container);

        try {
            const ids = await api.getFavoriteRouteIds();
            if (!ids?.length) {
                container.innerHTML = '';
                emptyState.classList.remove('hidden');
                countEl.textContent = '0 маршрутов';
                return;
            }

            emptyState.classList.add('hidden');
            countEl.textContent = `${ids.length} маршрут${ids.length === 1 ? '' : ids.length < 5 ? 'а' : 'ов'}`;

            container.innerHTML = '';
            const routes = await Promise.all(
                ids.map((id) => api.getRouteById(id).catch(() => null))
            );

            const valid = routes.filter(Boolean);
            if (!valid.length) {
                emptyState.classList.remove('hidden');
                return;
            }

            valid.forEach((route) => container.appendChild(createFavoriteCard(route)));
        } catch (err) {
            utils.showInlineError(container, err.message);
        }
    }

    document.addEventListener('DOMContentLoaded', async () => {
        await utils.initPageNav();
        await loadFavorites();
    });
})();
