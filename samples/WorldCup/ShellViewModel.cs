using Inferno;
using Inferno.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WorldCup.Bootstrap;
using WorldCup.Domain;
using WorldCup.Repo;
using WorldCup.ViewModels.Detail;
using WorldCup.ViewModels.Dialog;
using WorldCup.ViewModels.Overview;

namespace WorldCup
{
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IHaveCupYear
    {
        private readonly IDialogManager _dialogManager;
        private readonly IWorldCupRepo _worldCupRepo;

        public ShellViewModel(IDialogManager dialogManager, IWorldCupRepo worldCupRepo, IItemsControlLogger logger)
        {
            _dialogManager = dialogManager;
            _worldCupRepo = worldCupRepo;
            Logger = logger;

            #region Dialog

            this.WhenInitialized(disposables =>
            {
                var selectedOverviewItemObservable =
                    this.WhenAnyValue(x => x.OverviewViewModel.SelectedItem)
                        .Log("Overview", LoggingEventType.Information, item =>
                            item is MatchOverviewItem matchItem ? $"Selected {matchItem.Match.Team1.Name} - {matchItem.Match.Team2.Name}" :
                            item is TeamOverviewItem teamItem ? $"Selected {teamItem.Team.Name}" : "No item selected");

                var canExpandItem =
                    selectedOverviewItemObservable
                        .Select(item => item != null);

                var executeExpand =
                    selectedOverviewItemObservable
                        .SelectMany(ExpandItem)
                        .Take(1);

                ExpandItemCommand = ReactiveCommand.CreateFromObservable(() => executeExpand, canExpandItem).DisposeWith(disposables);
            });

            #endregion Dialog
        }

        #region Title bar

        public Uri Icon { get; private set; }

        #endregion Title bar

        #region Shell

        public List<CupYear> Years { get; set; }

        private CupYear _selectedYear;
        public CupYear SelectedYear
        {
            get => _selectedYear;
            set => this.RaiseAndSetIfChanged(ref _selectedYear, value);
        }

        public ReactiveCommand<Unit, bool?> ExpandItemCommand { get; private set; }

        #endregion Shell

        #region Hosts

        public OverviewViewModel OverviewViewModel { get; private set; }
        public DetailConductor DetailViewModel { get; private set; }

        #endregion Hosts

        #region Dialog

        private async Task<bool?> ExpandItem(IOverviewItem overviewItem)
        {
            DialogViewModel<ButtonChoice> dialogViewModel;

            switch (overviewItem)
            {
                case MatchOverviewItem matchItem:
                    dialogViewModel = new DialogViewModel<ButtonChoice>("Goal makers", new MatchDialogViewModel(matchItem), new[] { ButtonChoice.Close });
                    break;

                case TeamOverviewItem teamItem:
                    dialogViewModel = new DialogViewModel<ButtonChoice>("Team standing", new TeamDialogViewModel(teamItem, _worldCupRepo.StandingsByYear[SelectedYear]), new[] { ButtonChoice.Close });
                    break;

                default:
                    throw new NotImplementedException($"{nameof(IOverviewItem)} {overviewItem.GetType()}");
            }

            var dialogResult = await _dialogManager.ShowDialog(dialogViewModel, DialogSettings);

            return dialogResult;
        }

        private static IDialogSettings DialogSettings => new DialogSettings
        {
            Width = 520,
            Height = 400,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        #endregion Dialog

        public IItemsControlLogger Logger { get; }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            #region Title bar

            Icon = new Uri("pack://application:,,,/Resources/Icon.ico", UriKind.RelativeOrAbsolute);

            DisplayName = nameof(WorldCup);

            #endregion Title bar

            #region Shell

            Years = Enum.GetValues(typeof(CupYear)).Cast<CupYear>().ToList();

            SelectedYear = Years.Last();

            #endregion Shell

            #region Hosts

            OverviewViewModel = new OverviewViewModel(_worldCupRepo.MatchesByYear, this);

            DetailViewModel = new DetailConductor(_worldCupRepo, this, OverviewViewModel);

            Items.AddRange(OverviewViewModel, DetailViewModel);

            #endregion Hosts

            return Task.CompletedTask;
        }
    }
}