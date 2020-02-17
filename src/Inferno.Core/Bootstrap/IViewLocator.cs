using System;
using System.Windows;

namespace Inferno.Core
{
    public interface IViewLocator
    {
        UIElement LocateForModel(object model, object context);
        UIElement LocateForModelType(Type modelType, object context);
    }
}
