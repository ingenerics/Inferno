using System;
using System.Windows;

namespace Inferno
{
    public interface IViewLocator
    {
        UIElement LocateForModel(object model, object context, bool isDialog);
        UIElement LocateForModelType(Type modelType, object context);
        UIElement LocateForDialogModelType(Type modelType, object context);
    }
}
