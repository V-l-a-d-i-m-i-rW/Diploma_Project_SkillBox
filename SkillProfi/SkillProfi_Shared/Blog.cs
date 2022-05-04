using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{
    public class Blog : INotifyPropertyChanged
    {
        private byte[] image;
        /// <summary>
        /// идентификатор блога
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// заголовок блога
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// детальное описание блога
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// изображение для блога массив байт
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
        /// дата публикации
        /// </summary>
        public DateTime Date  { get; set; }

        /// <summary>
        /// изображение для блога
        /// </summary>
        [NotMapped]
        public IFormFile ImageFormFile { get; set; }

        ///// <summary>
        ///// детальное описание блога без html
        ///// </summary>
        //[NotMapped]
        //[JsonIgnore]
        //public string DescriptionNotHtml { get => SkillProfiHelper.GetStringWithOutHtml(Description); }

        public Blog()
        {
            Date = DateTime.Now;
        }

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
