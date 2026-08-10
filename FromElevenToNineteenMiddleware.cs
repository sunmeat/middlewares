namespace RequestProcessingPipeline
{
    // розширення для підключення middleware (обробка чисел 11–19)
    public static class FromElevenToNineteenExtensions
    {
        public static IApplicationBuilder UseFromElevenToNineteen(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FromElevenToNineteenMiddleware>();
        }
    }

    // middleware повертає назву числа для діапазону 11–19
    public class FromElevenToNineteenMiddleware
    {
        private readonly RequestDelegate _next; // посилання на наступний middleware у конвеєрі

        public FromElevenToNineteenMiddleware(RequestDelegate next) // значення next передається автоматично
        {
            _next = next; // зберігаємо посилання на наступний middleware
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Query["number"];

            // назви чисел від 11 до 19 (індекс 0 = одинадцять)
            string[] numbers =
            {
                "одинадцять", "дванадцять", "тринадцять", "чотирнадцять",
                "п'ятнадцять", "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять"
            };

            if (int.TryParse(token, out int number))
            {
                number = Math.Abs(number);

                if (number >= 11 && number <= 19)
                {
                    // якщо число в межах 11–19 - формуємо готову відповідь
                    string result = $"Ваше число — {numbers[number - 11]}";
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync(result);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(token))
            {
                // параметр передано, але він не є валідним цілим числом
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Некоректний параметр!");
                return;
            }

            // якщо число поза діапазоном або параметр відсутній — передаємо далі
            await _next(context);
        }
    }
}