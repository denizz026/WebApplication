document.addEventListener('DOMContentLoaded', function () {
    const loadingMessage = document.getElementById('loadingMessage');
    const successMessage = document.getElementById('successMessage');
    const errorMessage = document.getElementById('errorMessage');
    const errorText = document.getElementById('errorText');

    // Token'ı URL'den al
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get('token');

    if (token) {
        // Token doğrulama işlemi
        fetch('/Auth/ActivateAccount', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(token)
        })
            .then(response => response.json())
            .then(data => {
                loadingMessage.classList.add('hidden');
                if (data.success) {
                    successMessage.classList.remove('hidden');
                } else {
                    errorMessage.classList.remove('hidden');
                    errorText.textContent = data.message || 'Tokenin süresi dolmuş veya kullanılmış. Lütfen daha sonra tekrar deneyin.';
                }
            })
            .catch(error => {
                console.error('Hata:', error);
                loadingMessage.classList.add('hidden');
                errorMessage.classList.remove('hidden');
                errorText.textContent = 'Bir hata oluştu. Lütfen daha sonra tekrar deneyin.';
            });
    } else {
        loadingMessage.classList.add('hidden');
        errorMessage.classList.remove('hidden');
        errorText.textContent = 'Token bulunamadı. Aktivasyon başarısız.';
    }
});