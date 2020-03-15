using System;
using System.Windows.Markup;

namespace Inferno
{
    public interface IPackIcon
    {
        double? Width { get; set; }
        double? Height { get; set; }
    }

    public static class PackIconExtensions
    {
        public static PackIconControl<TKind> GetPackIcon<TPack, TKind>(this IPackIcon packIcon, TKind kind) where TPack : PackIconControl<TKind>, new()
        {
            var icon = new TPack {Kind = kind};

            if (packIcon.Width != null)
                icon.Width = packIcon.Width.Value;
            if (packIcon.Height != null)
                icon.Height = packIcon.Height.Value;

            return icon;
        }
    }

    [MarkupExtensionReturnType(typeof(PackIconBase))]
    public class PackIcon : MarkupExtension, IPackIcon
    {
        [ConstructorArgument("kind")]
        public Enum Kind { get; set; }

        public double? Width { get; set; }
        public double? Height { get; set; }

        public PackIcon()
        {
        }

        public PackIcon(Enum kind)
        {
            Kind = kind;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.Kind is PackIconOcticonsKind)
            {
                return this.GetPackIcon<PackIconOcticonsControl, PackIconOcticonsKind>((PackIconOcticonsKind) this.Kind);
            }
            return null;
        }
    }

    [MarkupExtensionReturnType(typeof(PackIconBase))]
    public class PackIcon<TPack, TKind> : MarkupExtension, IPackIcon where TPack : PackIconControl<TKind>, new()
    {
        [ConstructorArgument("kind")]
        public TKind Kind { get; set; }

        public double? Width { get; set; }
        public double? Height { get; set; }

        public PackIcon()
        {
        }

        public PackIcon(TKind kind)
        {
            Kind = kind;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<TPack, TKind>(this.Kind);
        }
    }
}