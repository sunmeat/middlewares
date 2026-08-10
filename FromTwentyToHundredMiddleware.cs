namespace RequestProcessingPipeline
{
    // розширювальний метод для підключення middleware у конвеєр обробки запитів
    // цей клас має бути у тій же області імен, що й middleware, щоб розширювальний метод був видимий
    // !!! зазвичай такі класи розміщують у окремих файлах, але для простоти хай буде тут
    // цей клас підключає middleware для обробки чисел від 20 до 100
    // саме він дозволяє використовувати метод UseFromTwentyToHundred у Program.cs
    // без нього довелось би писати app.UseMiddleware<...>() - нудно і громіздко
    public static class FromTwentyToHundredExtensions
    {
        // розширювальний метод для IApplicationBuilder, який підключає наш middleware у конвеєр
        // якщо підзабули, що таке розширювальний метод - гляньте https://gist.github.com/sunmeat/75d1693cb6e23e7979c8701b116718c1
        public static IApplicationBuilder UseFromTwentyToHundred(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FromTwentyToHundredMiddleware>();
        }
    }

    // middleware для обробки чисел від 20 до 100, запускається першим у конвеєрі
    public class FromTwentyToHundredMiddleware
    {
        private readonly RequestDelegate _next; // посилання на наступний middleware у конвеєрі

        // конструктор: отримує наступний компонент конвеєра, параметр цей передається автоматично асп нет кором
        public FromTwentyToHundredMiddleware(RequestDelegate next)
        {
            _next = next; // зберігаємо посилання на наступний middleware
        }

        // основний метод middleware, викликається при кожному HTTP-запиті
        public async Task InvokeAsync(HttpContext context)
        {
            // явно вказуємо, що відповідь у кодуванні UTF-8 і тип - звичайний текст
            context.Response.ContentType = "text/plain; charset=utf-8";

            // отримуємо значення параметра "number" з рядка запиту (наприклад, ?number=45)
            string? token = context.Request.Query["number"];

            // намагаємося перетворити рядок у ціле число
            if (!int.TryParse(token, out int number))
            {
                // якщо параметр не є числом - повертаємо повідомлення про помилку
                await context.Response.WriteAsync("Некоректний параметр!");
                return; // !!! завершуємо обробку запиту, наступний middleware не викликається !!!
            }

            // беремо модуль числа, щоб коректно обробляти від'ємні значення
            number = Math.Abs(number);

            // якщо число менше 20 - передаємо запит далі (наприклад, до middleware для 1–19)
            if (number < 20)
            {
                await _next(context); // !!! передаємо обробку наступному middleware !!!
                return; // коли ми тут, значить наступний middleware вже відпрацював, і ми просто завершуємо обробку
            }

            // якщо число більше 100 - є готова відповідь, запит не передається далі
            if (number > 100)
            {
                await context.Response.WriteAsync("Число більше ста, я поки що вмію рахувати лише до ста :)");
                return;
            }

            // спеціальний випадок: рівно 100
            if (number == 100)
            {
                await context.Response.WriteAsync("Ваше число - сто");
                return;
            }

            // масив назв десятків (індекс 0 - 20, індекс 1 - 30 тощо)
            string[] tens = { "двадцять", "тридцять", "сорок", "п'ятдесят", "шістдесят", "сімдесят", "вісімдесят", "дев'яносто" };

            // якщо число кратне 10 (20, 30, ..., 90)
            if (number % 10 == 0)
            {
                // виводимо тільки десятки, наприклад: "Ваше число - сорок"
                await context.Response.WriteAsync($"Ваше число - {tens[number / 10 - 2]}");
            }
            else
            {
                // для чисел типу 21, 35, 47 тощо - спочатку викликаємо наступний middleware
                await _next(context);
                // а після повернення з наступного middleware продовжуємо обробку тут:

                // отримуємо назву одиниць, яку має покласти третій middleware у сесію
                string? units = context.Session.GetString("number"); // !!! важливо запам'ятати назву ключа :)

                // формуємо повну назву: "двадцять п'ять", "п'ятдесят вісім" тощо
                string result = units is not null
                    ? tens[number / 10 - 2] + " " + units
                    : tens[number / 10 - 2];

                // виводимо остаточний результат
                await context.Response.WriteAsync("Ваше число - " + result);
            }
        }
    }
}