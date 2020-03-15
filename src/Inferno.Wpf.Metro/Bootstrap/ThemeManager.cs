using System;
using System.Collections.Generic;
using System.Windows;

namespace Inferno
{
    public class ThemeManager : IThemeManager
    {
        private readonly List<ResourceDictionary> _themeResources;
        private readonly ResourceDictionary _accent;
        private readonly ResourceDictionary _theme;

        public ThemeManager(MahAppsTheme theme, MahAppsAccent accent, IResourceManager iconManager)
        {
            CurrentTheme = theme;
            CurrentAccent = accent;

            _theme = new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Accents/{CurrentTheme}.xaml") };
            _accent = new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Accents/{CurrentAccent}.xaml") };

            _themeResources = new List<ResourceDictionary>
            {
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml") },
                _accent,
                _theme,
            };

            _themeResources.AddRange(iconManager.GetResources());
        }

        public MahAppsTheme CurrentTheme { get; private set; }
        public MahAppsAccent CurrentAccent { get; private set; }

        public List<ResourceDictionary> GetResources() => _themeResources;

        public void ChangeAppStyle(MahAppsTheme theme, MahAppsAccent accent)
        {
            var appTheme = MahApps.Metro.ThemeManager.GetAppTheme(nameof(theme));
            var appAccent = MahApps.Metro.ThemeManager.GetAccent(nameof(accent));

            _theme.Source = appTheme.Resources.Source;
            _accent.Source = appAccent.Resources.Source;

            CurrentTheme = theme;
            CurrentAccent = accent;
        }
    }
}
