
using Microsoft.Win32;
using SkillProfi_WPFClient.Interfaces;
using System.Windows;

namespace SkillProfi_WPFClient.Classes
{
    public class DefaultDialogService : IDialogService
    {
        /// <summary>
        /// вывод окна с ошибкой
        /// </summary>
        /// <param name="message"></param>
        void IDialogService.ShowErrorMessage(string message)
        {
            _ = MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// вывод окна с вопросом
        /// </summary>
        /// <param name="message"></param>
        bool IDialogService.SwowQuestionMessage(string message)
        {
            return MessageBoxResult.Yes == MessageBox.Show(message, "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        /// <summary>
        /// вывод окна с сообщением
        /// </summary>
        /// <param name="message"></param>
        void IDialogService.ShowMessage(string message)
        {
            _ = MessageBox.Show(message);
        }

        private string _filePath;
        /// <summary>
        /// путь к выбранному файлу
        /// </summary>
        public string FilePath { get => _filePath; }

        /// <summary>
        /// диалог открытия файла
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool OpenFileDialog(string filter = "")
        {
            OpenFileDialog ofd = new ();
            try
            {
                ofd.Filter = filter;
            }
            catch { }
            if (ofd.ShowDialog() == true)
            {
                _filePath = ofd.FileName;
                return true;
            }
            return false;
        }
    }

}
