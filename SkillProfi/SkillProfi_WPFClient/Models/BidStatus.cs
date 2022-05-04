using SkillProfi_Shared;
using System.ComponentModel;

namespace SkillProfi_WPFClient.Models
{
    public class BidStatus : INotifyPropertyChanged
    {
        private BidStatusEnum status;
        private bool isChecked;
        public BidStatusEnum Status { get =>status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public bool IsChecked { get=>isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
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
