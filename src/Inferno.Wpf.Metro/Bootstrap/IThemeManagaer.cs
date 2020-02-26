using System.Collections.Generic;
using System.Windows;

namespace Inferno
{
    public interface IThemeManager
    {
        List<ResourceDictionary> GetThemeResources();

        void ChangeAppStyle(MahAppsTheme theme, MahAppsAccent accent);
    }
}
