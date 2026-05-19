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
        const names = availableTags.length
            ? availableTags.map((t) => t.name)
            : ['Пеший', 'Природа', 'Исторический', 'Архитектура', 'Гастрономия'];

        names.forEach((tag) => {
            const btn = document.createElement('button');
            btn.type = 'button';
            const isSelected = selectedTags.includes(tag);
            btn.className = `px-5 py-2 rounded-2xl text-sm font-medium transition ${isSelected ? 'bg-blue-600 text-white' : 'bg-gray-200 hover:bg-gray-300'}`;
            btn.textContent = tag;
            btn.onclick = () => toggleTag(tag, btn);
            container.appendChild(btn);
        });
    }

    function toggleTag(tag, btn) {
        if (selectedTags.includes(tag)) {
            selectedTags = selectedTags.filter((t) => t !== tag);
            btn.classList.remove('bg-blue-600', 'text-white');
            btn.classList.add('bg-gray-200');
        } else if (selectedTags.length < 3) {
            selectedTags.push(tag);
            btn.classList.add('bg-blue-600', 'text-white');
            btn.classList.remove('bg-gray-200');
        } else {
            utils.showAlert('Можно выбрать максимум 3 тега');
        }
    }

    document.getElementById('createRouteForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (!utils.requireAuth('login.html')) return;

        const creatorId = api.getCurrentUserId();
        if (!creatorId) {
            utils.showAlert('Не удалось определить пользователя. Войдите снова.');
            return;
        }

        if (selectedPhotos.length === 0) {
            utils.showAlert('Добавьте хотя бы одну фотографию');
            return;
        }

        const submitBtn = e.target.querySelector('button[type="submit"]');
        const errorEl = document.getElementById('formError');
        submitBtn.disabled = true;
        errorEl?.classList.add('hidden');

        try {
            const durationMinutes = utils.parseDurationMinutes(
                document.getElementById('duration').value
            );

            const created = await api.createRoute({
                creatorId,
                title: document.getElementById('title').value.trim(),
                description: document.getElementById('description').value.trim(),
                durationMinutes,
                googleMapsUrl: document.getElementById('mapUrl').value.trim(),
            });

            for (let i = 0; i < selectedPhotos.length; i++) {
                await api.uploadRouteImage(
                    created.id,
                    selectedPhotos[i].file,
                    i === 0,
                    i
                );
            }

            const audioFile = document.getElementById('audioInput').files[0];
            if (audioFile) {
                await api.uploadRouteAudio(created.id, audioFile);
            }

            api.addMyRouteId(created.id);

            utils.showAlert(
                'Маршрут создан и отправлен на модерацию. После одобрения он появится в каталоге.',
                'success'
            );
            window.location.href = 'creator-profile.html';
        } catch (err) {
            if (errorEl) {
                errorEl.textContent = err.message;
                errorEl.classList.remove('hidden');
            } else {
                utils.showAlert(err.message);
            }
        } finally {
            submitBtn.disabled = false;
        }
    });

    document.addEventListener('DOMContentLoaded', () => {
        if (!utils.requireAuth('login.html')) return;
        loadTagsFromApi();
    });
})();
