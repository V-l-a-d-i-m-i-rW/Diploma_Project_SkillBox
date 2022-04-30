
namespace SkillProfi_WPFClient.Interfaces
{
    public interface IDialogService
    {
        void ShowErrorMessage(string message);   // показ сообщения c ошибкой
        bool SwowQuestionMessage(string message); //показ сообщения с вопросом усли ответ пользователя "Да" возращает true
        void ShowMessage(string message);   // показ сообщения

        string FilePath { get;}   // путь к выбранному файлу
        bool OpenFileDialog(string filter);  // открытие файла
        //bool SaveFileDialog();  // сохранение файла
    }
}
