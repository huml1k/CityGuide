(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    const routeId = utils.getQueryParam('id');
    let isFavorite = false;

    async function loadCreatorName(creatorId) {
        const link = document.getElementById('creatorLink');
        try {
            const profile = await api.getProfileById(creatorId);
            link.textContent = profile.fullName || 'Автор';
            link.href = `creator-profile.html?userId=${creatorId}`;
        } catch {
            link.textContent = 'Автор';
            link.href = '#';
        }
    }

    async function loadAudio(audioFiles) {
        const player = document.getElementById('audioPlayer');
        const audioInfo = document.getElementById('audioInfo');
        if (!audioFiles?.length) {
            document.getElementById('audioSection')?.classList.add('hidden');
            return;
        }

        const first = audioFiles[0];
        try {
            const meta = await api.getAudioMeta(first.id);
            if (meta?.url) {
                player.src = meta.url;
                const mins = first.durationSeconds
                    ? Math.round(first.durationSeconds / 60)
                    : '—';
                audioInfo.textContent = `${first.originalFilename || 'Аудио'} • ~${mins} мин`;
            }
        } catch {
            audioInfo.textContent = 'Не удалось загрузить аудио';
        }
    }

    async function toggleLike() {
        if (!api.isLoggedIn()) {
            window.location.href = 'login.html';
            return;
        }

        const btn = document.getElementById('likeButton');
        const countEl = document.getElementById('likesCount');
        let count = parseInt(countEl.textContent, 10) || 0;

        try {
            if (isFavorite) {
                await api.removeFavorite(routeId);
                isFavorite = false;
                btn.classList.remove('liked');
                countEl.textContent = Math.max(0, count - 1);
            } else {
                await api.addFavorite(routeId);
                isFavorite = true;
                btn.classList.add('liked');
                countEl.textContent = count + 1;
            }
        } catch (err) {
            utils.showToast(err.message, 'error');
        }
    }

    window.toggleLike = toggleLike;

    async function loadRoute() {
        if (!routeId) {
            utils.showToast('Маршрут не указан', 'error');
            window.location.href = '../index.html';
            return;
        }

        const loadingEl = document.getElementById('pageLoading');
        loadingEl?.classList.remove('hidden');

        try {
            const route = await api.getRouteById(routeId);

            document.getElementById('routeTitle').textContent = route.title;
            document.getElementById('duration').textContent = utils.formatDuration(route.durationMinutes);
            document.getElementById('description').textContent = route.description || 'Описание отсутствует';
            document.getElementById('likesCount').textContent = route.favoritesCount ?? 0;

            await loadCreatorName(route.creatorId);

            const tagsContainer = document.getElementById('tagsContainer');
            tagsContainer.innerHTML = '';

            (route.tags || []).forEach((tag) => {
                const span = document.createElement('span');

                span.className =
                    'px-4 py-2 bg-blue-100 text-blue-700 rounded-full text-sm font-medium';

                span.textContent = tag.name;

                tagsContainer.appendChild(span);
            });

            const gallery = document.getElementById('photoGallery');
            gallery.innerHTML = '';
            const images = route.images || [];
            if (images.length) {
                images
                    .sort((a, b) => a.orderIndex - b.orderIndex)
                    .forEach((img) => {
                        const wrap = document.createElement('div');
                        wrap.className = 'rounded-2xl overflow-hidden shadow-md';
                        wrap.innerHTML = `<img src="${api.getImageUrl(img.id)}" alt="" class="w-full h-64 object-cover hover:scale-105 transition">`;
                        gallery.appendChild(wrap);
                    });
            } else {
                gallery.innerHTML = `<p class="text-gray-500 col-span-3">Фотографии пока не добавлены</p>`;
            }

            const mapFrame = document.getElementById('mapFrame');
            const mapLink = document.getElementById('mapLink');

            if (route.googleMapsUrl) {

                mapFrame.src = route.googleMapsUrl;
                mapFrame.classList.remove('hidden');

            } else {

                mapFrame.classList.add('hidden');
                mapLink.classList.add('hidden');
            }

            await loadAudio(route.audioFiles);

            if (api.isLoggedIn()) {
                try {
                    const check = await api.checkFavorite(routeId);
                    isFavorite = check.isFavorite;
                    if (isFavorite) {
                        document.getElementById('likeButton')?.classList.add('liked');
                    }
                } catch {
                    /* ignore */
                }
            }
        } catch (err) {
            utils.showToast(err.message, 'error');
            setTimeout(() => { window.location.href = '../index.html'; }, 1500);
        } finally {
            loadingEl?.classList.add('hidden');
        }
    }

    document.addEventListener('DOMContentLoaded', async () => {
        await utils.initPageNav();
        loadRoute();
    });
})();
