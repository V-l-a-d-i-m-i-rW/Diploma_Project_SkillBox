using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{    
    /// <summary>
     /// класс описания проекта 
     /// </summary>
    public class Project : INotifyPropertyChanged
    {
        private byte[] image;
        /// <summary>
        /// идентификатор проекта
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// заголовок проекта
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// детальное описание проекта
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// изображение для проекта массив байт
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
        /// изображение для блога
        /// </summary>
        [NotMapped]
        public IFormFile ImageFormFile { get; set; }

        ///// <summary>
        ///// детальное описание проекта без html
        ///// </summary>
        //[JsonIgnore]
        //[NotMapped]
        //public string DescriptionNotHtml { get => SkillProfiHelper.GetStringWithOutHtml(Description); }

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
