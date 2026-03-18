using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TaskManager.Model.Lib
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // После любого изменения проверяем валидацию
            ValidateProperty(propertyName);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private readonly Dictionary<string, List<string>> _errors = [];
        public bool HasErrors => _errors.Count != 0;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string? propertyName)
            => _errors.GetValueOrDefault(propertyName ?? string.Empty) ?? Enumerable.Empty<string>();

        protected void ValidateProperty(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || propertyName == nameof(HasErrors)) return;

            var context = new ValidationContext(this) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            var propertyInfo = GetType().GetProperty(propertyName);
            if (propertyInfo == null) return;
            Validator.TryValidateProperty(propertyInfo.GetValue(this), context, results);
            _errors.Remove(propertyName);

            if (results.Count != 0)
                _errors[propertyName] = [.. results.Select(r => r.ErrorMessage ?? "Error")];

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors)); // Оповещаем UI, что статус валидности изменился
        }
    }   
}
