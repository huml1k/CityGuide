(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    let selectedPhotos = [];
    const maxPhotos = 6;
    let selectedTags = [];
    let availableTags = [];

    document.getElementById('photoInput')?.addEventListener('change', (e) => {
        Array.from(e.target.files).forEach((file) => {
            if (selectedPhotos.length >= maxPhotos) return;
            const reader = new FileReader();
            reader.onload = (ev) => {
                selectedPhotos.push({ file, preview: ev.target.result });
                renderPhotoPreviews();
            };
            reader.readAsDataURL(file);
        });
        e.target.value = '';
    });

    window.removePhoto = function (index) {
        selectedPhotos.splice(index, 1);
        renderPhotoPreviews();
    };

    function renderPhotoPreviews() {
        const container = document.getElementById('photoPreview');
        container.innerHTML = '';
        selectedPhotos.forEach((photo, index) => {
            const wrap = document.createElement('div');
            wrap.className = 'relative';
            wrap.innerHTML = `
                <img src="${photo.preview}" class="w-full h-32 object-cover rounded-xl" alt="">
                <button type="button" onclick="removePhoto(${index})"
                    class="absolute top-2 right-2 bg-red-500 text-white w-6 h-6 rounded-full text-xs">✕</button>`;
            container.appendChild(wrap);
        });
    }

    async function loadTagsFromApi() {
        try {
            availableTags = await api.getTags();
        } catch {
            availableTags = [];
        }
        renderTags();
    }

    function renderTags() {
        const container = document.getElementById('tagsContainer');
        container.innerHTML = '';
        const tags = availableTags.length
            ? availableTags
            : [];

        tags.forEach((tag) => {
            const btn = document.createElement('button');
            btn.type = 'button';
            const isSelected = selectedTags.some(t => t.id === tag.id);
            btn.className = `px-5 py-2 rounded-2xl text-sm font-medium transition ${isSelected ? 'bg-blue-600 text-white' : 'bg-gray-200 hover:bg-gray-300'}`;
            btn.textContent = tag.name;
            btn.onclick = () => toggleTag(tag, btn);
            container.appendChild(btn);
        });
    }

    function toggleTag(tag, btn) {

        const exists = selectedTags.some(t => t.id === tag.id);

        if (exists) {

            selectedTags = selectedTags.filter(t => t.id !== tag.id);

            btn.classList.remove('bg-blue-600', 'text-white');
            btn.classList.add('bg-gray-200');

        } else if (selectedTags.length < 3) {

            selectedTags.push(tag);

            btn.classList.add('bg-blue-600', 'text-white');
            btn.classList.remove('bg-gray-200');

        } else {

            utils.showToast('Можно выбрать максимум 3 тега', 'info');
        }
    }

    function extractGoogleMapsUrl(input) {

        if (!input) return '';

        input = input.trim();

        // Если уже обычная ссылка
        if (input.startsWith('https://')) {
            return input;
        }

        // Если iframe
        const match = input.match(/src="([^"]+)"/);

        if (match && match[1]) {
            return match[1];
        }

        return '';
    }

    document.getElementById('createRouteForm')?.addEventListener('submit', async function onSubmit(e) {
        e.preventDefault();

        if (!(await utils.requireCreator('login.html'))) return;

        const submitBtn = e.target.querySelector('button[type="submit"]');
        const errorEl = document.getElementById('formError');
        const creatorId = api.getCurrentUserId();

        if (!creatorId) {
            utils.showFormMessage(errorEl, 'Не удалось определить пользователя. Войдите снова.', 'error');
            return;
        }

        if (selectedPhotos.length === 0) {
            utils.showFormMessage(errorEl, 'Добавьте хотя бы одну фотографию', 'error');
            return;
        }

        submitBtn.disabled = true;
        utils.hideFormMessage(errorEl);

        try {
            const durationMinutes = utils.parseDurationMinutes(
                document.getElementById('duration').value
            );

            const parsedMapUrl = extractGoogleMapsUrl(
                document.getElementById('mapUrl').value
            );

            const created = await api.createRoute({
                creatorId,
                title: document.getElementById('title').value.trim(),
                description: document.getElementById('description').value.trim(),
                durationMinutes,
                googleMapsUrl: parsedMapUrl,
                tagIds: selectedTags.map(t => t.id),
            });

            const routeId = created?.id ?? created?.Id;
            if (!routeId) {
                throw new Error('Сервер не вернул идентификатор маршрута');
            }

            for (let i = 0; i < selectedPhotos.length; i++) {
                await api.uploadRouteImage(
                    routeId,
                    selectedPhotos[i].file,
                    i === 0,
                    i
                );
            }

            const audioFile = document.getElementById('audioInput').files[0];
            if (audioFile) {
                await api.uploadRouteAudio(routeId, audioFile);
            }

            api.addMyRouteId(routeId);

            utils.showToast(
                'Маршрут создан и отправлен на модерацию',
                'success'
            );
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
        loadTagsFromApi();
    });
})();
