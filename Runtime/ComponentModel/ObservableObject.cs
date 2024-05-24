using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SlimeNull.UnityFramework
{
    public class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        protected void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            var change = !Object.Equals(field, newValue);

            if (change && PropertyChanging is not null)
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));

            field = newValue;

            if (change && PropertyChanged is not null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }

}