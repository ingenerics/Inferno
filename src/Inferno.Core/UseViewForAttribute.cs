using System;

namespace Inferno.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseViewForAttribute : Attribute
    {
        public Type ViewModelType { get; }

        public UseViewForAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}
