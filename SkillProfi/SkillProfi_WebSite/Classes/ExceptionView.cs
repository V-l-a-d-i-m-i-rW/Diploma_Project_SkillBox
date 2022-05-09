using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SkillProfi_WebSite.Classes
{
    public static class ExceptionView
    {
        /// <summary>
        /// обработка исключения и формирование сообщения
        /// </summary>
        /// <param name="ex"></param>
        public static IActionResult View(Exception ex, Controller controller)
        {
            if (controller != null && ex != null)
            {
                string exception_string = ex switch
                {
                    InvalidOperationException => $"Ошибка URI.\n{ex.InnerException?.Message ?? ex.Message}",
                    System.Net.Http.HttpRequestException => $"Ошибка выполнения запроса.\n{ex.InnerException?.Message ?? ex.Message}",
                    TaskCanceledException => $"Отмена выполнения задачи.\n{ex.InnerException?.Message ?? ex.Message}",
                    Newtonsoft.Json.JsonSerializationException => $"Ошибка преобазования данных.\n{ex.InnerException?.Message ?? ex.Message}",
                    _ => throw ex,
                };
                return controller.View("Error", exception_string);
            }
            return null;
        }
    }
}
