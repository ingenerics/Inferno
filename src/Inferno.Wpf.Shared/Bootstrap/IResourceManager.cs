using System.Collections.Generic;
using System.Windows;

namespace Inferno
{
    public interface IResourceManager
    {
        List<ResourceDictionary> GetResources();
    }
}
