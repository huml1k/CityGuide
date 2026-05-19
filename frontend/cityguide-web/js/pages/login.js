(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    document.getElementById('loginForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;
        const errorEl = document.getElementById('formError');
        const submitBtn = e.target.querySelector('button[type="submit"]');

        errorEl?.classList.add('hidden');
        submitBtn.disabled = true;

        try {
            const data = await api.login(email, password);
            const role = (data.role || '').toLowerCase();
            if (role === 'admin') {
                window.location.href = 'admin-moderation.html';
            } else if (role === 'creator') {
                window.location.href = 'creator-profile.html';
            } else {
                window.location.href = '../index.html';
            }
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
})();
