using System;

namespace SlimeNull.UnityFramework
{
    public class DataBinding
    {
        public DataBinding(string propertyName, Delegate viewPropertySetter)
        {
            PropertyName = propertyName;
            ViewPropertySetter = viewPropertySetter;
        }

        public string PropertyName { get; }
        public Delegate ViewPropertySetter { get; }
    }
}
