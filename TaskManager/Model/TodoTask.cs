using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManager.Model.Lib;

namespace TaskManager.Model
{
    //Статусы задач
    public enum TodoStatus
    {
        [Description("Активна")]
        Active,
        [Description("Завершена")]
        Completed
    }

    //Приоритеты задач
    public enum TodoPriority
    {
        [Description("Низкий")]
        Low,
        [Description("Средний")]
        Medium,
        [Description("Высокий")]
        High
    }

    /// <summary>
    /// Задача
    /// </summary>
    public class TodoTask : BaseViewModel
    {
        public TodoTask()
        {
            Id = Guid.NewGuid();
            Status = TodoStatus.Active;
            Priority = TodoPriority.Medium;
            CreatedAt = DateTime.Now;
            DueDate = DateTime.Now.AddDays(1); //По умолчанию дедлайн - завтра
        }

        private Guid _id;
        private string _title = string.Empty;
        private string? _description;
        private TodoStatus _status;
        private TodoPriority _priority;
        private DateTime? _dueDate;
        private DateTime _createdAt;

        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Название задачи
        /// </summary>
        [Required(ErrorMessage = "Название задачи обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "От 3 до 100 символов")]
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Описание (опционально)
        /// </summary>
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Статус (Active/Completed)
        /// </summary>
        public TodoStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Приоритет (Low/Medium/High)
        /// </summary>
        public TodoPriority Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        /// <summary>
        /// Срок выполнения (опционально)
        /// </summary>
        public DateTime? DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        /// <summary>
        /// Проверка даты дедлайна и статуса
        /// </summary>
        public bool IsOverdue => Status == TodoStatus.Active
                         && DueDate.HasValue
                         && DueDate.Value.Date < DateTime.Today;
    }
}
