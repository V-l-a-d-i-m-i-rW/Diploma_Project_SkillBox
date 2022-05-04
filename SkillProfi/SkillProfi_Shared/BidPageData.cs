using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{
    /// <summary>
    /// настраиваемые данные страницы заявки
    /// </summary>
    public class BidPageData : INotifyPropertyChanged
    {
        private byte[] image;
        /// <summary>
        /// идентификатор данных для страницы заявки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// заголовок на рисунке  для страницы заявки
        /// </summary>
        public string HeaderImage { get; set; }

        /// <summary>
        /// заголовок на кнопке  для страницы заявки
        /// </summary>
        public string HeaderButton { get; set; }

        /// <summary>
        /// заголовок формы для страницы заявки
        /// </summary>
        public string HeaderForm { get; set; }

        /// <summary>
        /// изображение для страницы заявки в байтах
        /// </summary>
        public byte[] Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        /// <summary>
        /// изображение для страницы заявки
        /// </summary>
        [NotMapped]
        public IFormFile ImageFormFile { get; set; }

        ///// <summary>
        ///// заголовок на рисунке  для страницы заявки
        ///// </summary>
        //[JsonIgnore]
        //[NotMapped]
        //public string HeaderImageNotHtml { get => SkillProfiHelper.GetStringWithOutHtml(HeaderImage); }
        #region реализация интерфеса INotifyPropertyChanged
        /// <summary>
        /// событие PropertyChangedEventHandler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// метод изменения свойства
        /// </summary>
        /// <param name="propertyName">имя свойства</param>
        public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
