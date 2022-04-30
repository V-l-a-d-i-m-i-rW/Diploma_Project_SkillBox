using System;
using System.ComponentModel;

namespace SkillProfi_WPFClient.Models
{
    public class BidFilter:INotifyPropertyChanged
    {
        #region Поля
        private bool isChecked;
        private DateTime from;
        private DateTime up_to;
        #endregion

        #region свойства
        /// <summary>
        /// название фильтра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// фильтр выбран
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set 
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        /// <summary>
        /// дата для фильтра "С"
        /// </summary>
        public DateTime DateFrom
        {
            get => from;
            set
            {
                from = value;
                OnPropertyChanged(nameof(DateFrom));
            }
        }

        /// <summary>
        /// дата для фильтра "ПО"
        /// </summary>
        public DateTime DateUpTo
        {
            get => up_to;
            set
            {
                up_to = value;
                OnPropertyChanged(nameof(DateUpTo));
            }
        }
        #endregion

        public BidFilter()
        {
            isChecked = false;
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
