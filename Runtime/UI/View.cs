using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeNull.UnityFramework
{
    public class View<TViewModel> : MonoBehaviour
        where TViewModel : ViewModel, new()
    {
        private readonly Type typeOfViewModel = typeof(TViewModel);
        private readonly Dictionary<string, PropertyInfo> _cachedViewModelProperties = new();
        private readonly Dictionary<string, List<DataBinding>> _dataBindings = new();

        public TViewModel ViewModel { get; } = new();


        protected PropertyInfo GetViewModelProperty(string propertyName)
        {
            if (!_cachedViewModelProperties.TryGetValue(propertyName, out var propertyInfo))
                _cachedViewModelProperties[propertyName] = propertyInfo = typeOfViewModel.GetProperty(propertyName);

            return propertyInfo;
        }

        protected T GetPropertyValueOfViewModel<T>(string propertyName)
        {
            if (GetViewModelProperty(propertyName).GetValue(ViewModel) is T value)
                return value;

            return default;
        }

        protected void SetPropertyValueOfViewModel<T>(string propertyName, T value)
        {
            var propertyInfo = GetViewModelProperty(propertyName);
            if (propertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
            {
                propertyInfo.SetValue(ViewModel, value);
            }
        }

        protected void Awake()
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public virtual void Init() { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public virtual void Close() { }

        public Action<T> Bind<T>(string propertyName, Action<T> viewModelToView)
        {
            var propertyInfo = GetViewModelProperty(propertyName);
            if (!propertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException($"Type '{typeof(T)}' is not assignable to property '{propertyInfo.Name}'");
            }

            if (!_dataBindings.TryGetValue(propertyName, out var dataBindingList))
                _dataBindings[propertyName] = dataBindingList = new();

            dataBindingList.Add(new DataBinding(propertyName, viewModelToView));

            return value => SetPropertyValueOfViewModel(propertyName, value);
        }

        public void BindText(Text text, string propertyName)
        {
            Bind<string>(propertyName, value => text.text = value);
        }

        public void BindText(TMP_Text text, string propertyName)
        {
            Bind<string>(propertyName, value => text.text = value);
        }

        public void BindInputField(InputField inputField, string propertyName)
        {
            var viewModelPropertySetter = Bind<string>(propertyName, value => inputField.text = value);
            inputField.onValueChanged.AddListener(value => viewModelPropertySetter.Invoke(value));    
        }

        public void BindInputField(TMP_InputField inputField, string propertyName)
        {
            var viewModelPropertySetter = Bind<string>(propertyName, value => inputField.text = value);
            inputField.onValueChanged.AddListener(value => viewModelPropertySetter.Invoke(value));
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_dataBindings.TryGetValue(e.PropertyName, out var dataBindings))
            {
                var propertyValue = GetViewModelProperty(e.PropertyName);
                foreach (var dataBinding in dataBindings)
                {
                    dataBinding.ViewPropertySetter.DynamicInvoke(propertyValue);
                }
            }
        }
    }
}
