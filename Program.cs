namespace RequestProcessingPipeline
{
    public class Program
    {
        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddDistributedMemoryCache();

            // сесія - це набір даних, прив'язаних до конкретного користувача на певний час
            // сесія дозволяє зберігати інформацію між запитами від одного користувача
            // дуже спрощено, сесія - це словник, де можна зберігати пари ключ-значення, така собі глобальна змінна для користувача
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // час життя сесії, через 30 хвилин неактивності сесія буде видалена, і всі дані в ній втрачено
                options.Cookie.HttpOnly = true; // захищає cookie сесії від доступу зі сторони клієнтського коду (JavaScript)
                options.Cookie.IsEssential = true; // робить cookie сесії "необхідним" для роботи додатка, навіть якщо користувач відмовився від cookie
            }); // є варіант і без параметрів, але тоді сесія житиме 20 хвилин за замовчуванням 
            // builder.Services.AddSession();
            // докладніше про сесію: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-10#session-state


            // 3. будуємо додаток
            var app = builder.Build();

            // 4. підключаємо middleware у правильному порядку
            app.UseSession();                    // обов’язково перед тими, що читають/пишуть у сесію

            app.UseFromTwentyToHundred();        // 20–99
            app.UseFromElevenToNineteen();       // 11–19
            app.UseFromOneToTen();               // 1–10

            // 5. запускаємо
            app.Run();

            // 6. перевірка роботи:
            // https://aspnet.dev.localhost:7046/?number=21
        }
    }
}