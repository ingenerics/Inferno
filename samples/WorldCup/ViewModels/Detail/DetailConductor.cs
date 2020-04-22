using Inferno;
using Inferno.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldCup.Repo;
using WorldCup.ViewModels.Overview;

namespace WorldCup.ViewModels.Detail
{
    public class DetailConductor : Conductor<IChild>.Collection.OneActive
    {
        private readonly IWorldCupRepo _worldCupRepo;
        private readonly IHaveCupYear _hasCupYear;
        private readonly IHaveOverviewItem _overview;

        public DetailConductor(IWorldCupRepo worldCupRepo, IHaveCupYear hasCupYear, IHaveOverviewItem overview)
        {
            _worldCupRepo = worldCupRepo;
            _hasCupYear = hasCupYear;
            _overview = overview;

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.SelectedDetail, SwitchDetail)
                    .Subscribe()
                    .DisposeWith(disposables);

                var canRefresh =
                    this.WhenAnyValue(x => x.ActiveItem)
                        .Select(item => item is EmptyDetailViewModel);

                var canCleanUp = canRefresh.Select(x => !x);

                CleanUpCommand = ReactiveCommand.CreateFromTask(CleanUp, canCleanUp).DisposeWith(disposables);

                RefreshCommand = ReactiveCommand.Create(() => SwitchDetail(SelectedDetail), canRefresh).DisposeWith(disposables);
            });
        }

        public ReactiveCommand<Unit, Unit> CleanUpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; private set; }

        public List<Detail> DetailOptions { get; set; }

        private Detail _selectedDetail;
        public Detail SelectedDetail
        {
            get => _selectedDetail;
            set => this.RaiseAndSetIfChanged(ref _selectedDetail, value);
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            DetailOptions = Enum.GetValues(typeof(Detail)).Cast<Detail>().ToList();
            SelectedDetail = DetailOptions.First();

            return Task.CompletedTask;
        }

        private Unit SwitchDetail(Detail detail)
        {
            // Check if the requested detail is already in the Items collection

            var detailViewModelType =
                detail == Detail.Goals ? typeof(GoalsDetailViewModel) :
                detail == Detail.Group ? typeof(GroupDetailViewModel) :
                throw new NotImplementedException($"{nameof(Detail)} {detail}");

            var detailViewModel = Items.FirstOrDefault(vm => vm.GetType() == detailViewModelType);

            if (detailViewModel != null)
            {
                ActiveItem = detailViewModel;
            }
            else // Create new DetailViewModel
            {
                switch (detail)
                {
                    case Detail.Goals:
                        ActiveItem = new GoalsDetailViewModel(_worldCupRepo.MatchesByYear, _hasCupYear, _overview);
                        break;

                    case Detail.Group:
                        ActiveItem = new GroupDetailViewModel(_worldCupRepo.GroupsByYear, _hasCupYear, _overview);
                        break;

                    default:
                        throw new NotImplementedException($"{nameof(Detail)} {detail}");
                }
            }

            return Unit.Default;
        }

        private async Task CleanUp()
        {
            foreach (var activatable in Items.OfType<IActivate>())
            {
                await activatable.DeactivateAsync(true, CancellationToken.None);
            }

            Items.Clear();

            ActiveItem = new EmptyDetailViewModel();
        }
    }
}