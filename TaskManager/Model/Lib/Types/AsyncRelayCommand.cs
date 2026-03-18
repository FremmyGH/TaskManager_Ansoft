using System.Windows.Input;

namespace TaskManager.Model.Lib.Types
{
    public class AsyncRelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null) : ICommand
    {
        private readonly Func<object?, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Predicate<object?>? _canExecute = canExecute;
        private bool _isExecuting; // Флаг, что задача в процессе

        public AsyncRelayCommand(Func<Task> execute, Predicate<object?>? canExecute = null)
            : this(_ => execute(), canExecute) { }

        public bool CanExecute(object? parameter)
        {
            // Кнопка неактивна, если задача уже выполняется ИЛИ пользовательский предикат вернул false
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            // Уведомляем UI, что состояние CanExecute изменилось (кнопка должна стать серой)
            CommandManager.InvalidateRequerySuggested();

            try
            {
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
