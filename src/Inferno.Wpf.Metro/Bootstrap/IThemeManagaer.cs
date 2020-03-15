namespace Inferno
{
    public interface IThemeManager : IResourceManager
    {
        void ChangeAppStyle(MahAppsTheme theme, MahAppsAccent accent);
    }
}
