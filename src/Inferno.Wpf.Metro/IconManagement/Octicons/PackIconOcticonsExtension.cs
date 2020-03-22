using System.Windows.Markup;

namespace Inferno
{
    [MarkupExtensionReturnType(typeof(PackIconOcticonsControl))]
    public class OcticonsExtension : PackIcon<PackIconOcticonsControl, PackIconOcticonsKind>
    {
        public OcticonsExtension()
        {
        }

        public OcticonsExtension(PackIconOcticonsKind kind) : base(kind)
        {
        }
    }
}
