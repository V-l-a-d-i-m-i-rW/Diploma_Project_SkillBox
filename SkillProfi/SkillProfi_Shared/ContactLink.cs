using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{
    /// <summary>
    /// класс ссылка контактов
    /// </summary>
    public class ContactLink : INotifyPropertyChanged
    {
        private byte[] image;
        /// <summary>
        /// идентификатор ссылки контактов
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// детальное описание ссылки контактов
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// изображение для ссылки контактов массив байт
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
        /// ключ требуется для каскадного удаления
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// изображение для ссылки контактов
        /// </summary>
        [NotMapped]
        public IFormFile ImageFormFile { get; set; }
        /// <summary>
        /// признак что ссылка уже сохранена 
        /// </summary>
        [NotMapped]
        public bool Saved { get; set; }

        public ContactLink()
        {
            Saved = false;
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
