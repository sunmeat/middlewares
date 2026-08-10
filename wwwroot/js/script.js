const cities = ['Odesa', 'Kharkiv', 'Uzhhorod', 'Dnipro', 'Kyiv', 'Lviv', 'Mykolaiv', 'Zaporizhzhia', 'Chernivtsi', 'Poltava'];

const weatherGrid = document.getElementById('weather-grid');
const cityInput = document.getElementById('cityInput');
const searchBtn = document.getElementById('searchBtn');

const cityTranslations = {
    'Odesa': 'Одеса',
    'Kharkiv': 'Харків',
    'Uzhhorod': 'Ужгород',
    'Dnipro': 'Дніпро',
    'Kyiv': 'Київ',
    'Lviv': 'Львів',
    'Mykolaiv': 'Миколаїв',
    'Zaporizhzhia': 'Запоріжжя',
    'Chernivtsi': 'Чернівці',
    'Poltava': 'Полтава'
};

async function getCoordinates(city) {
    const res = await fetch(`https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(city)}&count=1&language=uk&format=json`);
    const data = await res.json();
    if (!data.results || data.results.length === 0) throw new Error('Місто не знайдено');
    return data.results[0];
}

async function loadWeather(cityName) {
    const card = document.createElement('div');
    card.className = 'card loading';

    const displayName = cityTranslations[cityName] || cityName;
    card.innerHTML = `
        <div class="city-name">${displayName}</div>
        <div>Завантаження...</div>
    `;
    weatherGrid.appendChild(card);

    try {
        const location = await getCoordinates(cityName);
        const weatherRes = await fetch(
            `https://api.open-meteo.com/v1/forecast?` +
            `latitude=${location.latitude}&longitude=${location.longitude}` +
            `&current=temperature_2m,apparent_temperature,relative_humidity_2m,wind_speed_10m,weather_code` +
            `&timezone=auto`
        );

        const data = await weatherRes.json();
        const current = data.current;

        const weatherDesc = getWeatherDescription(current.weather_code);

        card.classList.remove('loading');
        card.innerHTML = `
            <div class="city-name">${displayName}</div>
            <div class="temp">${Math.round(current.temperature_2m)}°C</div>
            <div class="description">${weatherDesc}</div>
            <div class="details">
                Відчувається як ${Math.round(current.apparent_temperature)}°C<br>
                Вологість: ${current.relative_humidity_2m}%<br>
                Вітер: ${Math.round(current.wind_speed_10m)} км/год
            </div>
        `;
    } catch (err) {
        console.error(err);
        card.classList.remove('loading');
        card.innerHTML = `
            <div class="city-name">${displayName}</div>
            <div style="color:#ff6b6b; margin-top: 15px;">
                Не вдалося завантажити :(<br>
                <small>Перевірте назву міста</small>
            </div>
        `;
    }
}

function getWeatherDescription(code) {
    const descriptions = {
        0: "Ясно",
        1: "Переважно ясно",
        2: "Частково хмарно",
        3: "Хмарно",
        45: "Туман",
        48: "Туман з памороззю",
        51: "Легка мряка",
        61: "Невеликий дощ",
        63: "Дощ",
        65: "Сильний дощ",
        71: "Невеликий сніг",
        73: "Сніг",
        75: "Сильний сніг",
        80: "Невеликі зливи",
        81: "Зливи",
        82: "Сильні зливи",
        95: "Гроза",
        96: "Гроза з градом",
        99: "Сильна гроза з градом"
    };
    return descriptions[code] || "Хмарно";
}

function loadDefaultCities() {
    weatherGrid.innerHTML = '';
    cities.forEach(city => loadWeather(city));
}

searchBtn.addEventListener('click', () => {
    const city = cityInput.value.trim();
    if (city) {
        weatherGrid.innerHTML = '';
        loadWeather(city);
        cityInput.value = '';
    }
});

cityInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') searchBtn.click();
});

window.addEventListener('load', loadDefaultCities);