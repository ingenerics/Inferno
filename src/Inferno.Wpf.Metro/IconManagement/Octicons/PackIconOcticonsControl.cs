using System.Windows;

namespace Inferno
{
    /// <summary>
    /// Icons from GitHub Octicons - <see><cref>https://octicons.github.com</cref></see>
    /// </summary>
    public class PackIconOcticonsControl : PackIconControl<PackIconOcticonsKind>
    {
        static PackIconOcticonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconOcticonsControl), new FrameworkPropertyMetadata(typeof(PackIconOcticonsControl)));
        }

        public PackIconOcticonsControl() : base(PackIconOcticonsDataFactory.Create)
        {
        }
    }
}