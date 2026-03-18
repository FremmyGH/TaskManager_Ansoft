using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Model;
using TaskManager.Model.Lib;
using TaskManager.Model.Lib.Types;
using TaskManager.View;

namespace TaskManager.ViewModel
{
    public class TaskManagerContext : BaseViewModel
    {
        public TaskManagerContext()
        {
            TasksView = CollectionViewSource.GetDefaultView(Tasks);
            TasksView.Filter = FilterPredicate;

            // Очищаем старое
            TasksView.SortDescriptions.Clear();
            // Новые задачи будут вверху списка
            TasksView.SortDescriptions.Add(new SortDescription(nameof(TodoTask.CreatedAt), ListSortDirection.Descending));
            // Дополнительно по приоритету (если даты совпадут)
            TasksView.SortDescriptions.Add(new SortDescription(nameof(TodoTask.Priority), ListSortDirection.Descending));
        }

        #region Свойства

        private readonly TodoDataService _dataService = new();
        private string _searchText = string.Empty;
        private string _selectedFilterStatus = "Все";

        public ObservableCollection<TodoTask> Tasks { get; } = [];

        /// <summary>
        /// Коллекция-обертка для фильтров и поисков
        /// </summary>
        public ICollectionView TasksView { get; }

        /// <summary>
        /// Поисковый текст
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) TasksView.Refresh(); }
        }

        /// <summary>
        /// Выбранный статус
        /// </summary>
        public string SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set { if (SetProperty(ref _selectedFilterStatus, value)) TasksView.Refresh(); }
        }

        private string _statusMessage = "Готов к работе";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        #endregion

        #region Команды
        // Команда создания
        private ICommand? _createCommand;
        public ICommand CreateCommand => _createCommand ??= new RelayCommand(() =>
        {
            var newTask = new TodoTask();
            if (OpenEditDialog(newTask))
            {
                Tasks.Add(newTask);
                _ = SaveDataAsync();
            }
        });

        // Редактирование задачи
        private ICommand? _editCommand;
        public ICommand EditCommand => _editCommand ??= new RelayCommand(obj =>
        {
            if (obj is TodoTask selectedTask)
            {
                if (OpenEditDialog(selectedTask))
                    _ = SaveDataAsync();
            }
        }, obj => obj is TodoTask); // Кнопка "Изменить" активна только если выделена задача

        //Удаление задачи
        private ICommand? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new RelayCommand(obj =>
        {
            if (obj is TodoTask task && MessageBox.Show($"Вы уверены?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Tasks.Remove(task);
                _ = SaveDataAsync();
            }
        }, obj => obj is TodoTask);


        //Загрузка данных из JSON
        private ICommand? _loadTask;
        public ICommand LoadTaskCommand => _loadTask ??= new AsyncRelayCommand(LoadTasksAsync);
        private async Task LoadTasksAsync()
        {
            var loaded = await _dataService.LoadTasksAsync();
            Tasks.Clear();
            foreach (var t in loaded) Tasks.Add(t);
            MessageBox.Show("Данные загружены из JSON.");
        }


        private bool OpenEditDialog(TodoTask task)
        {
            var editContext = new TaskEditContext(task);
            var dialog = new TaskEditWindow
            {
                DataContext = editContext, 
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                editContext.ApplyChanges(task);
                // Принудительно уведомляем таблицу, что данные внутри объекта могли измениться
                TasksView.Refresh();
                return true;
            }
            return false;
        }

        //Изменение статуса
        private ICommand? _toggleStatusCommand;
        public ICommand ToggleStatusCommand => _toggleStatusCommand ??= new RelayCommand(obj =>
        {
            if (obj is TodoTask task)
            {
                task.Status = task.Status == TodoStatus.Active
                    ? TodoStatus.Completed
                    : TodoStatus.Active;

                // Обновляем отображение (чтобы сработали фильтры, если они включены)
                TasksView.Refresh();
                _ = SaveDataAsync();
            }
        }, obj => obj is TodoTask);
        #endregion

        #region Методы
        /// <summary>
        /// Фильтр для показа задачи в таблице
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool FilterPredicate(object obj)
        {
            if (obj is not TodoTask task) return false;

            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                                 task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                 (task.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);

            bool matchesStatus = SelectedFilterStatus switch
            {
                "Активные" => task.Status == TodoStatus.Active,
                "Завершенные" => task.Status == TodoStatus.Completed,
                _ => true
            };

            return matchesSearch && matchesStatus;
        }

        /// <summary>
        /// Асинхронное сохранение данных
        /// </summary>
        private async Task SaveDataAsync()
        {
            try
            {
                StatusMessage = "Сохранение...";
                await _dataService.SaveTasksAsync(Tasks);
                StatusMessage = $"Данные сохранены в {DateTime.Now:HH:mm:ss}";

                await Task.Delay(3000);
                if (StatusMessage.StartsWith("Данные сохранены"))
                    StatusMessage = "Готов";
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка сохранения!";
                MessageBox.Show($"Не удалось сохранить JSON: {ex.Message}", "Ошибка");
            }
        }
        #endregion
    }
}

