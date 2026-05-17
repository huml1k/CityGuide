// Пример данных маршрутов
const routes = [
    {
        id: 1,
        title: "Исторический центр Москвы",
        duration: "4 часа",
        tag: "Пеший",
        likes: 124,
        image: "https://picsum.photos/id/1015/600/400",
        liked: false
    },
    {
        id: 2,
        title: "Парк Зарядье и Красная площадь",
        duration: "3 часа",
        tag: "Архитектура",
        likes: 89,
        image: "https://picsum.photos/id/133/600/400",
        liked: false
    },
    {
        id: 3,
        title: "Вдоль реки в Санкт-Петербурге",
        duration: "5 часов",
        tag: "Природа",
        likes: 156,
        image: "https://picsum.photos/id/201/600/400",
        liked: false
    }
];

// Функция создания одной карточки
function createRouteCard(route) {
    const card = document.createElement('div');
    card.className = 'route-card cursor-pointer';
    
    card.innerHTML = `
        <a href="pages/route-detail.html?id=${route.id}" class="block">
            <img src="${route.image}" alt="${route.title}" class="w-full">
            <div class="p-5">
                <div class="flex justify-between items-start mb-3">
                    <h3 class="font-semibold text-lg leading-tight">${route.title}</h3>
                </div>
                
                <div class="flex items-center gap-4 text-sm text-gray-600 mb-4">
                    <div class="flex items-center gap-1">
                        <span>⏱</span>
                        <span>${route.duration}</span>
                    </div>
                    <div class="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">
                        ${route.tag}
                    </div>
                </div>

                <div class="flex items-center justify-between">
                    <button onclick="event.stopImmediatePropagation(); toggleLike(${route.id}, this)" 
                            class="like-btn flex items-center gap-1 text-gray-600 hover:text-red-500 ${route.liked ? 'liked' : ''}">
                        <span class="text-xl">❤️</span>
                        <span class="likes-count">${route.likes}</span>
                    </button>
                    
                    <span class="text-blue-600 font-medium text-sm">Подробнее →</span>
                </div>
            </div>
        </a>
    `;
    return card;
}

// Рендер всех карточек
function renderRoutes() {
    const popularContainer = document.getElementById('popularRoutes');
    const allContainer = document.getElementById('allRoutes');

    // Очищаем контейнеры
    popularContainer.innerHTML = '';
    allContainer.innerHTML = '';

    routes.forEach(route => {
        const card = createRouteCard(route);
        
        // Первые 3 — в популярные
        if (route.id <= 3) {
            popularContainer.appendChild(card);
        }
        const cardClone = card.cloneNode(true);
        allContainer.appendChild(cardClone);
    });
}

// Функция лайка
function toggleLike(id, button) {
    const likesCount = button.querySelector('.likes-count');
    let count = parseInt(likesCount.textContent);

    if (button.classList.contains('liked')) {
        button.classList.remove('liked');
        likesCount.textContent = count - 1;
    } else {
        button.classList.add('liked');
        likesCount.textContent = count + 1;
        
        // Анимация
        button.style.transform = 'scale(1.3)';
        setTimeout(() => button.style.transform = 'scale(1)', 200);
    }
}

// Запуск при загрузке страницы
document.addEventListener('DOMContentLoaded', renderRoutes);