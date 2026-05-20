(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    const routeId = utils.getQueryParam('id');

    async function loadRoute() {
        if (!routeId) {
            utils.showToast('Маршрут не указан', 'error');
            window.location.href = 'creator-profile.html';
            return;
        }

        try {
            const routes = await api.getMyRoutes();
            const route = (routes || []).find((r) => String(r.id) === String(routeId));

            if (!route) {
                throw new Error('Маршрут не найден или недоступен для редактирования');
            }

            const statusKey = (route.status || '').toLowerCase();
            if (statusKey === 'approved') {
                utils.showToast('Опубликованный маршрут нельзя редактировать', 'info');
                window.location.href = `route-detail.html?id=${routeId}`;
                return;
            }

            document.getElementById('title').value = route.title || '';
            document.getElementById('description').value = route.description || '';
            document.getElementById('duration').value = route.durationMinutes || 60;
            document.getElementById('mapUrl').value = route.googleMapsUrl || '';
        } catch (err) {
            utils.showToast(err.message, 'error');
            setTimeout(() => {
                window.location.href = 'creator-profile.html';
            }, 1500);
        }
    }

    document.getElementById('editRouteForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (!(await utils.requireCreator('login.html'))) return;

        const submitBtn = e.target.querySelector('button[type="submit"]');
        const errorEl = document.getElementById('formError');

        submitBtn.disabled = true;
        utils.hideFormMessage(errorEl);

        try {
            const durationMinutes = utils.parseDurationMinutes(
                document.getElementById('duration').value
            );

            await api.updateRoute(routeId, {
                title: document.getElementById('title').value.trim(),
                description: document.getElementById('description').value.trim(),
                durationMinutes,
                googleMapsUrl: document.getElementById('mapUrl').value.trim(),
            });

            utils.showToast('Маршрут обновлён', 'success');
            window.location.href = 'creator-profile.html';
        } catch (err) {
            utils.showFormMessage(errorEl, err.message, 'error');
        } finally {
            submitBtn.disabled = false;
        }
    });

    document.addEventListener('DOMContentLoaded', async () => {
        if (!(await utils.requireCreator('login.html'))) return;
        await utils.initPageNav();
        await loadRoute();
    });
})();
