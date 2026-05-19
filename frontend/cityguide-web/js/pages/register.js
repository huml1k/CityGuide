(function () {
    const api = window.CityGuideApi;
    const utils = window.CityGuideUtils;

    document.getElementById('registerForm')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;
        const errorEl = document.getElementById('formError');
        const submitBtn = e.target.querySelector('button[type="submit"]');

        if (password.length < 6) {
            const msg = 'Пароль должен быть не короче 6 символов';
            if (errorEl) {
                errorEl.textContent = msg;
                errorEl.classList.remove('hidden');
            } else {
                utils.showAlert(msg);
            }
            return;
        }

        errorEl?.classList.add('hidden');
        submitBtn.disabled = true;

        try {
            await api.register(email, password);
            utils.showAlert('Регистрация успешна! Войдите в аккаунт.', 'success');
            window.location.href = 'login.html';
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
