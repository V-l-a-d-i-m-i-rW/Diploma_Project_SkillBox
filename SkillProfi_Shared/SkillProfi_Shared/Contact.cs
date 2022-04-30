using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{
    public class Contact : INotifyPropertyChanged
    {
        private byte[] image;
        /// <summary>
        /// идентификатор контакта
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// детальное описание ссылки контактов
        /// </summary>
        public string Address { get; set; }

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
        /// ссыли на контакты
        /// </summary>
        public ObservableCollection<ContactLink> Links { get; set; } = new();
        /// <summary>
        /// изображение для ссылки контактов
        /// </summary>
        [NotMapped]
        public IFormFile ImageFormFile { get; set; }

        ///// <summary>
        ///// детальное описание ссылки контактов без html
        ///// </summary>
        //[JsonIgnore]
        //[NotMapped]
        //public string AddressNotHtml { get => SkillProfiHelper.GetStringWithOutHtml(Address); }

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
