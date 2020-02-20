using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Inferno
{
    public class AssemblySource : List<Assembly>
    {
        private readonly List<Type> _allTypes;
        private readonly List<Type> _viewTypes;

        public AssemblySource(params Assembly[] assemblies)
            : base(assemblies)
        {
            _allTypes =
                this
                    .SelectMany(a => a.ExportedTypes)
                    .ToList();

            _viewTypes =
                _allTypes
                    .Where(t => typeof(UIElement).IsAssignableFrom(t))
                    .ToList();
        }

        /// <summary>
        /// Finds a type which matches one of the elements in the sequence of names.
        /// </summary>
        public Type FindTypeByNames(IEnumerable<string> names)
            => FindByNames(names, _allTypes);

        /// <summary>
        /// Finds a view type (implementing IViewFor) which matches one of the elements in the sequence of names.
        /// </summary>
        public Type FindViewTypeByNames(IEnumerable<string> names)
            => FindByNames(names, _viewTypes);

        private Type FindByNames(IEnumerable<string> names, IEnumerable<Type> types)
        {
            if (names == null)
            {
                return null;
            }

            var type =
                names
                    .Join(types, n => n, t => t.FullName, (n, t) => t)
                    .FirstOrDefault();

            return type;
        }
    }
}