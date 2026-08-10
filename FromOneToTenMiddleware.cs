namespace RequestProcessingPipeline
{
    public static class FromOneToTenExtensions
    {
        public static IApplicationBuilder UseFromOneToTen(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FromOneToTenMiddleware>();
        }
    }

    // middleware обробляє числа від 1 до 10 та частину логіки для більших чисел
    public class FromOneToTenMiddleware
    {
        private readonly RequestDelegate _next;

        public FromOneToTenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"];

            // назви чисел від 1 до 9
            string[] ones =
            {
                "один", "два", "три", "чотири", "п'ять",
                "шість", "сім", "вісім", "дев'ять"
            };

            if (int.TryParse(token, out int number))
            {
                number = Math.Abs(number);

                if (number == 10)
                {
                    // спеціальний випадок для десяти
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Ваше число — десять");
                    return;
                }

                if (number >= 1 && number <= 9)
                {
                    // числа від 1 до 9 — видаємо відповідь відразу
                    string result = $"Ваше число — {ones[number - 1]}";
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync(result);
                    return;
                }

                if (number > 20)
                {
                    // для чисел >20 зберігаємо останню цифру для наступного middleware
                    context.Session.SetString("number", ones[number % 10 - 1]);
                }
            }
            else if (!string.IsNullOrEmpty(token))
            {
                // параметр передано, але він не є цілим числом
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Некоректний параметр!");
                return;
            }

            // передаємо управління далі по конвеєру
            await _next(context);
        }
    }
}

// доречі, число 0 видає помилку 404, бо жоден з middleware не дав відповідь —
// всі просто передали управління далі (await _next(context)),
// а в кінці конвеєра нічого немає, тому ASP.NET Core повертає стандартний 404

// ПРАКТИКА:
// спробуйте додати обробку числа 0, будь-яким способом на ваш вибір