using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TaskManager.Model;
using TaskManager.Model.Lib;
using TaskManager.Model.Lib.Types;

namespace TaskManager.ViewModel
{
    public class TaskEditContext : BaseViewModel
    {
        public TaskEditContext(TodoTask task)
        {
            // Копируем данные из модели в контекст
            Title = task.Title;
            Description = task.Description;
            Priority = task.Priority;
            DueDate = task.DueDate;

            // Запускаем начальную валидацию
            ValidateProperty(nameof(Title));
            ValidateProperty(nameof(DueDate));
        }

        #region Свойства

        private string _title = string.Empty;
        private string? _description;
        private TodoPriority _priority;
        private DateTime? _dueDate;
        private bool? _dialogResult;

        // Свойство для связи с Behavior в XAML (закрывает окно)
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "От 3 до 100 символов")]
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Приоритет
        /// </summary>
        public TodoPriority Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        /// <summary>
        /// Срок выполнения
        /// </summary>
        [CustomValidation(typeof(TaskEditContext), nameof(ValidateDate))]
        public DateTime? DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        #endregion
        #region Команды
        // Команда сохранения
        private ICommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(() =>
        {
            if (!HasErrors) DialogResult = true;
        }, _ => !HasErrors);

        // Метод для переноса правок обратно в модель (вызывается из MainViewModel)
        public void ApplyChanges(TodoTask task)
        {
            task.Title = Title;
            task.Description = Description;
            task.Priority = Priority;
            task.DueDate = DueDate;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Валидация (дата не может быть в прошлом)
        /// </summary>
        /// <param name="date">Проверяемая дата</param>
        /// <param name="context">прописан для вызова через CustomValidation, передавать не нужно</param>
        /// <returns></returns>
        public static ValidationResult? ValidateDate(DateTime? date, ValidationContext context)
        {
            if (date.HasValue && date.Value.Date < DateTime.Today)
                return new ValidationResult("Срок не может быть в прошлом");

            return ValidationResult.Success;
        }
        #endregion
    }
}