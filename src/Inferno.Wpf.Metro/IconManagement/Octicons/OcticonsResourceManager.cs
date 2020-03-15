using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Inferno.Wpf.Shared.IconManagement.Octicons
{
    public class OcticonsResourceManager : IResourceManager
    {
        private readonly List<ResourceDictionary> _resources;

        public OcticonsResourceManager()
        {
            _resources = CreateOcticonsResources();
        }

        #region IResourceManager

        public List<ResourceDictionary> GetResources() => _resources;

        #endregion IResourceManager

        private static List<ResourceDictionary> CreateOcticonsResources()
        {
            var resourceDictionary = new ResourceDictionary();

            var style = new Style(typeof(PackIconOcticonsControl));

            style.Setters.Add(new Setter { Property = FrameworkElement.HeightProperty, Value = 16.0 });
            style.Setters.Add(new Setter { Property = FrameworkElement.WidthProperty, Value = 16.0 });
            style.Setters.Add(new Setter { Property = Control.PaddingProperty, Value = new Thickness(0.0) });
            style.Setters.Add(new Setter { Property = FrameworkElement.FlowDirectionProperty, Value = FlowDirection.LeftToRight });
            style.Setters.Add(new Setter { Property = FrameworkElement.HorizontalAlignmentProperty, Value = HorizontalAlignment.Center });
            style.Setters.Add(new Setter { Property = FrameworkElement.VerticalAlignmentProperty, Value = VerticalAlignment.Center });
            style.Setters.Add(new Setter { Property = Control.IsTabStopProperty, Value = false });
            style.Setters.Add(new Setter { Property = UIElement.SnapsToDevicePixelsProperty, Value = false });
            style.Setters.Add(new Setter { Property = FrameworkElement.UseLayoutRoundingProperty, Value = false });
            style.Setters.Add(new Setter { Property = Control.TemplateProperty, Value = GetIconTemplate() });

            resourceDictionary.Add(typeof(PackIconOcticonsControl), style);

            return new List<ResourceDictionary> { resourceDictionary };
        }

        private static ControlTemplate GetIconTemplate()
        {
            var template = new ControlTemplate(typeof(PackIconOcticonsControl));

            var root = new FrameworkElementFactory(typeof(Grid));

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Control.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
            border.SetValue(Control.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
            border.SetValue(Control.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
            border.SetValue(Control.SnapsToDevicePixelsProperty, new TemplateBindingExtension(Control.SnapsToDevicePixelsProperty));
            root.AppendChild(border);

            var innerGrid = new FrameworkElementFactory(typeof(Grid));
            innerGrid.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));

            var path = new FrameworkElementFactory(typeof(Path));
            path.SetValue(Path.FillProperty, new TemplateBindingExtension(Control.ForegroundProperty));
            path.SetValue(Path.StretchProperty, Stretch.Uniform);
            path.SetBinding(Path.DataProperty, new Binding(nameof(PackIconOcticonsControl.Data)) { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),  Mode = BindingMode.OneWay });
            path.SetValue(UIElement.SnapsToDevicePixelsProperty, false);
            path.SetValue(FrameworkElement.UseLayoutRoundingProperty, false);

            var viewBox = new FrameworkElementFactory(typeof(Viewbox));
            viewBox.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));

            viewBox.AppendChild(path);
            innerGrid.AppendChild(viewBox);
            root.AppendChild(innerGrid);

            template.VisualTree = root;
            template.Seal();
            return template;
        }
    }
}
