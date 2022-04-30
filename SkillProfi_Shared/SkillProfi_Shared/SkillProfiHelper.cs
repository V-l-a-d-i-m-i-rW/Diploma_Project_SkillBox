using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillProfi_Shared
{
    public static class SkillProfiHelper
    {
        /// <summary>
        /// Получить русскоязычное представление  BidStatusEnum
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string BidStatusGetName(BidStatusEnum status)
        {
            return status switch
            {
                BidStatusEnum.received => "Получена",
                BidStatusEnum.at_work => "В работе",
                BidStatusEnum.completed => "Выполнена",
                BidStatusEnum.rejected => "Отклонена",
                BidStatusEnum.cancelled => "Отменена",
                _ => "",
            };
        }
        /// <summary>
        /// по BidStatusEnum получить класс для раскраски ячееек таблицы
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string BidStatusGetTableClass(BidStatusEnum status)
        {
            return status switch
            {
                //BidStatusEnum.received => "",
                BidStatusEnum.at_work => "table-primary",
                BidStatusEnum.completed => "table-success",
                BidStatusEnum.rejected => "table-danger",
                BidStatusEnum.cancelled => "table-warning",
                _ => "",
            };
        }

        /// <summary>
        /// перевод изображения из IFormFile в byte[]
        /// </summary>
        public static byte[] GetImageToImageByte(IFormFile file)
        {
            if (file != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }
                // установка массива байтов
                return imageData;
            }
            return null;
        }

        /// <summary>
        /// убирает все html теги из строки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringWithOutHtml(string htmlString) => 
            Regex.Replace(Regex.Replace(htmlString, @"<[^>]+>|&nbsp;|&ndash;|&mdash;|&lsquo;|&rsquo;|&sbquo;|&ldquo;|&rdquo;|&bdquo;|&laquo;|&raquo;", "").Trim(), @"\s{2,}", " ");
        
        /// <summary>
        /// путь к файлу относительно запущенного файла
        /// </summary>
        /// <returns>полный путь</returns>
        public static string GetLocalFilePath(string filename)
        {
            string retVal = filename;
            if (!string.IsNullOrEmpty(filename.Trim()))
            {
                try
                {
                    retVal = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd('\\') + filename;
                    FileInfo fi = new (retVal);
                    if (!Directory.Exists(fi.DirectoryName)) Directory.CreateDirectory(fi.DirectoryName);
                }
                catch
                {
                    retVal = filename;
                }
            }
            return retVal;
        }

        /// <summary>
        /// рассчет MD5 хеш суммы строки
        /// </summary>
        /// <param name="str">cnhjrf c rjnjhjq nht,etncz gjkexbnm </param>
        /// <returns></returns>
        public static string CalculateMD5Checksum(string str)//
        {
            if (!string.IsNullOrEmpty(str?.Trim()))
            {
                var md5 = new MD5CryptoServiceProvider();
                byte[] strArr = Encoding.UTF8.GetBytes(str);
                return BitConverter.ToString(md5.ComputeHash(strArr)).Replace("-", string.Empty).ToUpper();
            }
            return string.Empty;
        }

        /// <summary>
        /// проверка введенной строки на соответвие регулярному выражению
        /// </summary>
        /// <param name="strPhoneNumber">проверяемая строка</param>
        /// <param name="matchNumberPattern">регулярное выражение Regex на которое проверяем строку, если отсутвует проверяем номер телефона</param>
        /// <returns>соответсвие строки регулярному выражению</returns>
        public static bool CheckStringByRegexPattern(string strPhoneNumber, string matchNumberPattern = @"^(\s*)?(\+)?([- _():=+]?\d[- _():=+]?){3,14}(\s*)?$")
        {
            matchNumberPattern = string.IsNullOrEmpty(matchNumberPattern?.Trim()) ? @"^(\s*)?(\+)?([- _():=+]?\d[- _():=+]?){3,14}(\s*)?$" : matchNumberPattern;//@"^(([0-9|\+[0-9])[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";// @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            return !string.IsNullOrEmpty(strPhoneNumber) && Regex.IsMatch(strPhoneNumber, matchNumberPattern);
        }
    }
}
