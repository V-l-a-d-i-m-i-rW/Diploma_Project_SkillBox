using System;
using System.Windows.Input;

namespace SkillProfi_WPFClient.Classes
{
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// делегат выполнение команды
        /// </summary>
        private readonly Action<object> _execute;
        /// <summary>
        /// делегат может ли выполнятся команда
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
           _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
