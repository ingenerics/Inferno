using Inferno.Core.Conventions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Inferno.Core;
using Inferno.Core.Logging;
using Inferno.DialogManagement.Views;

namespace Inferno
{
    public class ViewLocator : IViewLocator
    {
        protected readonly IDependencyResolver DependencyResolver;
        protected readonly AssemblySource AssemblySource;
        protected readonly IThemeManager ThemeManager;
        protected readonly ILogger Logger;

        private readonly List<string> _viewSuffixList;
        private readonly NameTransformer _nameTransformer;
        private readonly string _contextSeparator;
        private string _defaultSubNsViews;
        private string _defaultSubNsViewModels;
        private bool _useNameSuffixesInMappings;
        private string _nameFormat;
        private string _viewModelSuffix;
        private bool _includeViewSuffixInVmNames;

        public ViewLocator(IDependencyResolver dependencyResolver, AssemblySource assemblySource, IThemeManager themeManager, ILogger logger)
        {
            DependencyResolver = dependencyResolver;
            AssemblySource = assemblySource;
            ThemeManager = themeManager;
            Logger = logger;

            _viewSuffixList = new List<string>();
            _nameTransformer = new NameTransformer();
            _contextSeparator = ".";

            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        #region Conventions

        /// <summary>
        /// Specifies how type mappings are created, including default type mappings. Calling this method will
        /// clear all existing name transformation rules and create new default type mappings according to the
        /// configuration.
        /// </summary>
        /// <param name="config">An instance of TypeMappingConfiguration that provides the settings for configuration</param>
        public void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (String.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException("DefaultSubNamespaceForViews field cannot be blank.");
            }

            if (String.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException("DefaultSubNamespaceForViewModels field cannot be blank.");
            }

            if (String.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field cannot be blank.");
            }

            _nameTransformer.Clear();
            _viewSuffixList.Clear();

            _defaultSubNsViews = config.DefaultSubNamespaceForViews;
            _defaultSubNsViewModels = config.DefaultSubNamespaceForViewModels;
            _nameFormat = config.NameFormat;
            _useNameSuffixesInMappings = config.UseNameSuffixesInMappings;
            _viewModelSuffix = config.ViewModelSuffix;
            _viewSuffixList.AddRange(config.ViewSuffixList);
            _includeViewSuffixInVmNames = config.IncludeViewSuffixInViewModelNames;

            SetAllDefaults();
        }

        private void SetAllDefaults()
        {
            if (_useNameSuffixesInMappings)
            {
                //Add support for all view suffixes
                _viewSuffixList.ForEach(AddDefaultTypeMapping);
            }
            else
            {
                AddSubNamespaceMapping(_defaultSubNsViewModels, _defaultSubNsViews);
            }
        }

        /// <summary>
        /// Adds a default type mapping using the standard namespace mapping convention
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddDefaultTypeMapping(string viewSuffix = "View")
        {
            if (!_useNameSuffixesInMappings)
            {
                return;
            }

            //Check for <Namespace>.<BaseName><ViewSuffix> construct
            AddNamespaceMapping(String.Empty, String.Empty, viewSuffix);

            //Check for <Namespace>.ViewModels.<NameSpace>.<BaseName><ViewSuffix> construct
            AddSubNamespaceMapping(_defaultSubNsViewModels, _defaultSubNsViews, viewSuffix);
        }

        /// <summary>
        /// This method registers a View suffix or synonym so that View Context resolution works properly.
        /// It is automatically called internally when calling AddNamespaceMapping(), AddDefaultTypeMapping(),
        /// or AddTypeMapping(). It should not need to be called explicitly unless a rule that handles synonyms
        /// is added directly through the NameTransformer.
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void RegisterViewSuffix(string viewSuffix)
        {
            if (_viewSuffixList.Count(s => s == viewSuffix) == 0)
            {
                _viewSuffixList.Add(viewSuffix);
            }
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetsRegEx">Array of RegEx replace values for target namespaces</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string[] nsTargetsRegEx, string viewSuffix = "View")
        {
            RegisterViewSuffix(viewSuffix);

            var replist = new List<string>();
            var repsuffix = _useNameSuffixesInMappings ? viewSuffix : String.Empty;
            const string basegrp = "${basename}";

            foreach (var t in nsTargetsRegEx)
            {
                replist.Add(t + String.Format(_nameFormat, basegrp, repsuffix));
            }

            var rxbase = RegExHelper.GetNameCaptureGroup("basename");
            var suffix = String.Empty;
            if (_useNameSuffixesInMappings)
            {
                suffix = _viewModelSuffix;
                if (!_viewModelSuffix.Contains(viewSuffix) && _includeViewSuffixInVmNames)
                {
                    suffix = viewSuffix + suffix;
                }
            }
            var rxsrcfilter = String.IsNullOrEmpty(nsSourceFilterRegEx)
                ? null
                : String.Concat(nsSourceFilterRegEx, String.Format(_nameFormat, RegExHelper.NameRegEx, suffix), "$");
            var rxsuffix = RegExHelper.GetCaptureGroup("suffix", suffix);

            _nameTransformer.AddRule(
                String.Concat(nsSourceReplaceRegEx, String.Format(_nameFormat, rxbase, rxsuffix), "$"),
                replist.ToArray(),
                rxsrcfilter
            );
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetRegEx">RegEx replace value for target namespace</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx, string viewSuffix = "View")
        {
            AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, new[] { nsTargetRegEx }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTargets">Namespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            //need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            //Start pattern search from beginning of string ("^")
            //unless original string was blank (i.e. special case to indicate "append target to source")
            if (!String.IsNullOrEmpty(nsSource))
            {
                nsencoded = "^" + nsencoded;
            }

            //Capture namespace as "origns" in case we need to use it in the output in the future
            var nsreplace = RegExHelper.GetCaptureGroup("origns", nsencoded);

            var nsTargetsRegEx = nsTargets.Select(t => t + ".").ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTarget">Namespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTargets">Subnamespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            //need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            string rxbeforetgt, rxaftersrc, rxaftertgt;
            var rxbeforesrc = rxbeforetgt = rxaftersrc = rxaftertgt = String.Empty;

            if (!String.IsNullOrEmpty(nsSource))
            {
                if (!nsSource.StartsWith("*"))
                {
                    rxbeforesrc = RegExHelper.GetNamespaceCaptureGroup("nsbefore");
                    rxbeforetgt = @"${nsbefore}";
                }

                if (!nsSource.EndsWith("*"))
                {
                    rxaftersrc = RegExHelper.GetNamespaceCaptureGroup("nsafter");
                    rxaftertgt = "${nsafter}";
                }
            }

            var rxmid = RegExHelper.GetCaptureGroup("subns", nsencoded);
            var nsreplace = String.Concat(rxbeforesrc, rxmid, rxaftersrc);

            var nsTargetsRegEx = nsTargets.Select(t => String.Concat(rxbeforetgt, t, ".", rxaftertgt)).ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTarget">Subnamespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddSubNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        #endregion Conventions

        /// <summary>
        ///   Locates the view for the specified model instance.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model instance and the context (or null) as parameters and receive a view instance.
        /// </remarks>
        public UIElement LocateForModel(object model, object context, bool isDialog)
        {
            HydrateViewModel(model);

            return isDialog ?
                LocateForDialogModelType(model.GetType(), context) :
                LocateForModelType(model.GetType(), context);
        }

        /// <summary>
        /// Used to hydrate the model.
        /// </summary>
        protected virtual object HydrateViewModel(object model) => model;

        /// <summary>
        ///   Locates the view for the specified model type.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model type and the context instance (or null) as parameters and receive a view instance.
        /// </remarks>
        public virtual UIElement LocateForModelType(Type modelType, object context)
        {
            var useViewForAttributes =
                modelType
                    .GetCustomAttributes(typeof(UseViewForAttribute), true)
                    .Cast<UseViewForAttribute>()
                    .ToList();

            Contract.Assert(useViewForAttributes.Count() <= 1, "There should not be more than one UseViewForAttribute on a view model");

            var attributedType = useViewForAttributes.FirstOrDefault()?.ViewModelType;
            var viewType = LocateTypeForModelType(attributedType ?? modelType, context);

            return viewType == null
                ? new TextBlock { Text = string.Format("Cannot find view for {0}.", modelType) }
                : GetOrCreateViewType(viewType);
        }

        /// <summary>
        ///   Locates the view for the specified model type.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model type and the context instance (or null) as parameters and receive a view instance.
        /// </remarks>
        public virtual UIElement LocateForDialogModelType(Type modelType, object context)
        {
            var dialogModelType = modelType.GetInterfaces()
                .FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IDialogViewModel<>));

            UIElement result;

            if (dialogModelType == null)
            {
                result = LocateForModelType(modelType, context);
            }
            else
            {
                var dialogViewType = typeof(DialogView<>);
                var choiceType = dialogModelType.GenericTypeArguments.Single();
                var viewType = dialogViewType.MakeGenericType(choiceType);

                result = GetOrCreateViewType(viewType);
            }

            return result;
        }

        /// <summary>
        /// Transforms a ViewModel type name into all of its possible View type names. Optionally accepts an instance
        /// of context object
        /// </summary>
        /// <returns>Enumeration of transformed names</returns>
        /// <remarks>Arguments:
        /// typeName = The name of the ViewModel type being resolved to its companion View.
        /// context = An instance of the context or null.
        /// </remarks>
        public IList<string> TransformName(string typeName, object context)
        {
            Func<string, string> getReplaceString;
            if (context == null)
            {
                getReplaceString = r => r;
                return _nameTransformer.Transform(typeName, getReplaceString).ToList();
            }

            var contextstr = _contextSeparator + context;
            string grpsuffix = String.Empty;
            if (_useNameSuffixesInMappings)
            {
                //Create RegEx for matching any of the synonyms registered
                var synonymregex = "(" + String.Join("|", _viewSuffixList.ToArray()) + ")";
                grpsuffix = RegExHelper.GetCaptureGroup("suffix", synonymregex);
            }

            const string grpbase = @"\${basename}";
            var patternregex = String.Format(_nameFormat, grpbase, grpsuffix) + "$";

            //Strip out any synonym by just using contents of base capture group with context string
            var replaceregex = "${basename}" + contextstr;

            //Strip out the synonym
            getReplaceString = r => Regex.Replace(r, patternregex, replaceregex);

            //Return only the names for the context
            return _nameTransformer.Transform(typeName, getReplaceString).Where(n => n.EndsWith(contextstr)).ToList();
        }

        /// <summary>
        ///   Locates the view type based on the specified model type.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model type, display location (or null) and the context instance (or null) as parameters and receive a view type.
        /// </remarks>
        protected virtual Type LocateTypeForModelType(Type modelType, object context)
        {
            var viewTypeName = modelType.FullName;

            viewTypeName = viewTypeName.Substring(
                0,
                viewTypeName.IndexOf('`') < 0
                    ? viewTypeName.Length
                    : viewTypeName.IndexOf('`')
                );

            var viewTypeList = TransformName(viewTypeName, context);
            var viewType = AssemblySource.FindViewTypeByNames(viewTypeList);

            if (viewType == null)
            {
                Logger.LogWarning(this, $"View not found. Searched: {string.Join(", ", viewTypeList.ToArray())}.");
            }

            return viewType;
        }

        /// <summary>
        ///   Retrieves the view from the IoC container or tries to create it if not found.
        /// </summary>
        /// <remarks>
        ///   Pass the type of view as a parameter and recieve an instance of the view.
        /// </remarks>
        protected virtual UIElement GetOrCreateViewType(Type viewType)
        {
            DependencyResolver.TryGetInstance(viewType, out var instance);

            var view = instance as UIElement;

            if (view == null)
            {
                if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
                    return new TextBlock { Text = $"Cannot create {viewType.FullName}."};

                view = (UIElement)System.Activator.CreateInstance(viewType);
            }

            if (view is Window window)
            {
                ThemeManager.GetResources().ForEach(window.Resources.MergedDictionaries.Add);
            }

            view = HydrateView(view);

            return view;
        }

        /// <summary>
        /// Used to hydrate the view.
        /// </summary>
        protected virtual UIElement HydrateView(UIElement view) => view;
    }
}
