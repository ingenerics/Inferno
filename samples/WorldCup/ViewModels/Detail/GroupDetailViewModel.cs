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
    public class GroupDetailViewModel : Screen
    {
        private readonly Dictionary<CupYear, IRepo<string, Group>> _groupsByYear;
        private readonly IHaveCupYear _hasCupYear;
        private readonly IHaveOverviewItem _overview;

        public GroupDetailViewModel(Dictionary<CupYear, IRepo<string, Group>> groupsByYear, IHaveCupYear hasCupYear, IHaveOverviewItem overview)
        {
            _groupsByYear = groupsByYear;
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

        public IBindableCollection<Group> Items { get; private set; }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Items = new BindableCollection<Group>();

            return Task.CompletedTask;
        }

        private Unit UpdateDetail(CupYear cupYear, IOverviewItem overviewItem)
        {
            Items.Clear();

            if (overviewItem != null)
            {
                IList<Group> items;
                ISet<string> teamCodes;

                switch (overviewItem)
                {
                    case MatchOverviewItem matchItem:
                        teamCodes = new HashSet<string> { matchItem.Match.Team1.Code, matchItem.Match.Team2.Code };
                        break;

                    case TeamOverviewItem teamItem:
                        teamCodes = new HashSet<string> { teamItem.Team.Code };
                        break;

                    default:
                        throw new NotImplementedException($"{nameof(IOverviewItem)} {overviewItem.GetType()}");
                }

                items =
                    _groupsByYear[cupYear].GetAll()
                        .Where(group => group.Teams.Any(team => teamCodes.Contains(team.Code)))
                        .ToList();

                Items.AddRange(items);
            }

            return Unit.Default;
        }
    }
}
