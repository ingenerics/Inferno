using Inferno;
using Inferno.Core;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using WorldCup.Domain;
using WorldCup.Repo;

namespace WorldCup.ViewModels.Overview
{
    public class OverviewViewModel : Screen, IHaveOverviewItem
    {
        private const int DeltaOffsetX = 4;
        private const int DeltaOffsetY = 2;
        private readonly Dictionary<CupYear, IRepo<int, Match>> _matchesByYear;

        public OverviewViewModel(Dictionary<CupYear, IRepo<int, Match>> matchesByYear, IHaveCupYear hasCupYear)
        {
            _matchesByYear = matchesByYear;

            this.WhenInitialized(disposables =>
            {
                hasCupYear
                    .WhenAnyValue(x => x.SelectedYear, UpdateOverview)
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Items = new BindableCollection<IOverviewItem>();

            return Task.CompletedTask;
        }

        public IBindableCollection<IOverviewItem> Items { get; private set; }

        private IOverviewItem _selectedItem;
        public IOverviewItem SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        private Unit UpdateOverview(CupYear year)
        {
            Items.Clear();

            var matchRepo = _matchesByYear[year];

            var firstSemi = matchRepo.Get(61);
            UpdateMatch(firstSemi, 0, 0);
            var secondSemi = matchRepo.Get(62);
            UpdateMatch(secondSemi, 0, 2);
            var final = matchRepo.Get(64);
            UpdateMatch(final, 1, 1);

            UpdateWinner(final);

            return Unit.Default;
        }

        private void UpdateMatch(Match match, int offsetX, int offsetY)
        {
            Items.Add(new TeamOverviewItem(match.Team1, 0 + DeltaOffsetX * offsetX, 0 + DeltaOffsetY * offsetY));
            Items.Add(new LineOverviewItem(1 + DeltaOffsetX * offsetX, 0 + DeltaOffsetY * offsetY));
            
            Items.Add(new MatchOverviewItem(match, 2 + DeltaOffsetX * offsetX, 1 + DeltaOffsetY * offsetY));
            Items.Add(new LineOverviewItem(3 + DeltaOffsetX * offsetX, 1 + DeltaOffsetY * offsetY));

            Items.Add(new TeamOverviewItem(match.Team2, 0 + DeltaOffsetX * offsetX, 2 + DeltaOffsetY * offsetY));
            Items.Add(new LineOverviewItem(1 + DeltaOffsetX * offsetX, 2 + DeltaOffsetY * offsetY));
        }

        private void UpdateWinner(Match final)
        {
            var winner = new TeamOverviewItem(final.Score1 > final.Score2 ? final.Team1 : final.Team2, DeltaOffsetX * 2, 1 + DeltaOffsetY * 1);
            Items.Add(winner);
        }
    }
}
