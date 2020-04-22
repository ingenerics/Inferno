using Inferno;
using Inferno.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using WorldCup.Domain;
using WorldCup.Repo;
using WorldCup.ViewModels.Overview;

namespace WorldCup.ViewModels.Detail
{
    public class GoalsDetailViewModel : Screen
    {
        private readonly Dictionary<CupYear, IRepo<int, Match>> _matchesByYear;
        private readonly IHaveCupYear _hasCupYear;
        private readonly IHaveOverviewItem _overview;

        public GoalsDetailViewModel(Dictionary<CupYear, IRepo<int, Match>> matchesByYear, IHaveCupYear hasCupYear, IHaveOverviewItem overview)
        {
            _matchesByYear = matchesByYear;
            _hasCupYear = hasCupYear;
            _overview = overview;

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(
                        x => x._hasCupYear.SelectedYear, 
                        x => x._overview.SelectedItem, 
                        UpdateDetail)
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        public IBindableCollection<GoalsWrapper> Items { get; private set; }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Items = new BindableCollection<GoalsWrapper>();

            return Task.CompletedTask;
        }

        private Unit UpdateDetail(CupYear cupYear, IOverviewItem overviewItem)
        {
            Items.Clear();

            if (overviewItem != null)
            {
                IList<GoalsWrapper> items;

                switch (overviewItem)
                {
                    case MatchOverviewItem matchItem:
                        items = new List<GoalsWrapper> { 
                            new GoalsWrapper(
                                matchItem.Match, 
                                matchItem.Match.OutcomeTeam1.Goals.Concat(matchItem.Match.OutcomeTeam2.Goals)
                                    .OrderBy(goal => goal.Minute)
                                    .ToArray()) };
                        break;

                    case TeamOverviewItem teamItem:
                        items =
                            _matchesByYear[cupYear].GetAll()
                                .SelectMany(match => new[] { (Match : match, Outcome : match.OutcomeTeam1), (Match: match, Outcome: match.OutcomeTeam2) })
                                .Where(pair => pair.Outcome.Team.Code == teamItem.Team.Code && pair.Outcome.Goals.Any())
                                .Select(pair => new GoalsWrapper(pair.Match, pair.Outcome.Goals))
                                .ToList();
                        break;

                    default:
                        throw new NotImplementedException($"{nameof(IOverviewItem)} {overviewItem.GetType()}");
                }

                Items.AddRange(items);
            }

            return Unit.Default;
        }
    }
}
