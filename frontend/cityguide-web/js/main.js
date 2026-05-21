/**
 * Главная страница — список и поиск маршрутов.
 */
(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let allRoutes = [];
    let favoriteIds = new Set();

    function createRouteCard(route, options = {}) {
        const { basePath = 'pages/' } = options;
        const tag = route.tags?.[0] || 'Маршрут';
        const imageUrl = api.getRouteCoverImageUrl(route) || utils.getRoutePlaceholder(route.title);
        const isFavorite = favoriteIds.has(route.id);

        const card = document.createElement('div');
        card.className = 'route-card cursor-pointer bg-white rounded-2xl overflow-hidden shadow hover:shadow-lg transition';
        card.innerHTML = `
            <a href="${basePath}route-detail.html?id=${route.id}" class="block">
                <img src="${imageUrl}" alt="${utils.escapeHtml(route.title)}" class="w-full h-52 object-cover">
                <div class="p-5">
                    <h3 class="font-semibold text-lg leading-tight mb-3">${utils.escapeHtml(route.title)}</h3>
                    <div class="flex items-center gap-4 text-sm text-gray-600 mb-4">
                        <div class="flex items-center gap-1">
                            <span>⏱</span>
                            <span>${utils.formatDuration(route.durationMinutes)}</span>
                        </div>
                        <div class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">
                            ${utils.escapeHtml(tag)}
                        </div>
                    </div>
                    <div class="flex items-center justify-between">
                        <button type="button" data-favorite-id="${route.id}"
                            class="like-btn flex items-center gap-1 text-gray-600 hover:text-red-500 ${isFavorite ? 'liked text-red-500' : ''}">
                            <span class="text-xl">❤️</span>
                            <span class="likes-count">${route.favoritesCount ?? 0}</span>
                        </button>
                        <span class="text-blue-600 font-medium text-sm">Подробнее →</span>
                    </div>
                </div>
            </a>`;

        const favBtn = card.querySelector('[data-favorite-id]');
        favBtn?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            toggleFavorite(route.id, favBtn);
        });

        return card;
    }

    async function toggleFavorite(routeId, button) {
        if (!api.isLoggedIn()) {
            window.location.href = 'pages/login.html';
            return;
        }

        const countEl = button.querySelector('.likes-count');
        const wasFavorite = favoriteIds.has(routeId);

        try {
            if (wasFavorite) {
                await api.removeFavorite(routeId);
                favoriteIds.delete(routeId);
                button.classList.remove('liked', 'text-red-500');
            } else {
                await api.addFavorite(routeId);
                favoriteIds.add(routeId);
                button.classList.add('liked', 'text-red-500');
            }
            const count = parseInt(countEl.textContent, 10) || 0;
            countEl.textContent = wasFavorite ? Math.max(0, count - 1) : count + 1;
        } catch (err) {
            utils.showToast(err.message, 'error');
        }
    }

    function renderRoutesList(routes, container) {
        container.innerHTML = '';
        if (!routes.length) {
            container.innerHTML = `
                <div class="col-span-full text-center py-12 text-gray-500">
                    Маршруты не найдены
                </div>`;
            return;
        }
        routes.forEach((route) => container.appendChild(createRouteCard(route)));
    }

    function applyTagFilter(routes) {
        const tagFilter = document.getElementById('tagFilter')?.value;
        if (!tagFilter) return routes;
        return routes.filter((r) =>
            r.tags?.some((t) => t.toLowerCase().includes(tagFilter.toLowerCase()))
        );
    }

    function renderAllSections() {
        const filtered = applyTagFilter(allRoutes);
        const popular = [...filtered]
            .sort((a, b) => (b.favoritesCount ?? 0) - (a.favoritesCount ?? 0))
            .slice(0, 3);

        const popularContainer = document.getElementById('popularRoutes');
        const allContainer = document.getElementById('allRoutes');

        renderRoutesList(popular, popularContainer);
        renderRoutesList(filtered, allContainer);
    }

    async function loadFavoriteIds() {
        if (!api.isLoggedIn()) return;
        try {
            const ids = await api.getFavoriteRouteIds();
            favoriteIds = new Set(ids || []);
        } catch {
            favoriteIds = new Set();
        }
    }

    async function loadTagsFilter() {
        const select = document.getElementById('tagFilter');
        if (!select) return;
        try {
            const tags = await api.getTags();
            tags.forEach((tag) => {
                const opt = document.createElement('option');
                opt.value = tag.name;
                opt.textContent = tag.name;
                select.appendChild(opt);
            });
        } catch {
            /* теги с бэка опциональны */
        }
    }

    async function loadRoutes() {
        const popularContainer = document.getElementById('popularRoutes');
        const allContainer = document.getElementById('allRoutes');
        utils.showLoading(popularContainer);
        utils.showLoading(allContainer);

        try {
            allRoutes = await api.getRoutes();
            if (!Array.isArray(allRoutes)) allRoutes = [];
            renderAllSections();
        } catch (err) {
            utils.showInlineError(popularContainer, err.message);
            utils.showInlineError(allContainer, err.message);
        }
    }

    async function searchRoutes() {
        const query = document.getElementById('searchInput')?.value?.trim() || '';
        const popularContainer = document.getElementById('popularRoutes');
        const allContainer = document.getElementById('allRoutes');
        utils.showLoading(allContainer);

        try {
            allRoutes = query ? await api.searchRoutes(query) : await api.getRoutes();
            if (!Array.isArray(allRoutes)) allRoutes = [];
            renderAllSections();
        } catch (err) {
            utils.showInlineError(allContainer, err.message);
        }
    }

    window.searchRoutes = searchRoutes;

    document.addEventListener('DOMContentLoaded', async () => {
        await api.initAuth();
        await utils.initPageNav();
        await loadTagsFilter();
        await loadFavoriteIds();
        await loadRoutes();

        document.getElementById('tagFilter')?.addEventListener('change', renderAllSections);
        document.getElementById('searchInput')?.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                searchRoutes();
            }
        });
    });
})();
