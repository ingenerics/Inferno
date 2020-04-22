using System.Collections.Generic;
using WorldCup.Domain;
using WorldCup.Resources;

namespace WorldCup.Repo
{
    public class WorldCupRepo : IWorldCupRepo
    {
        public WorldCupRepo()
        {
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            TeamsByYear = new Dictionary<CupYear, IRepo<string, Team>>
            {
                {CupYear._2014, RepoFactory.Build<Team, string>(ResourcePath.Teams_2014, t => t.Code)},
                {CupYear._2018, RepoFactory.Build<Team, string>(ResourcePath.Teams_2018, t => t.Code)},
            };
            MatchesByYear = new Dictionary<CupYear, IRepo<int, Match>>
            {
                {CupYear._2014, RepoFactory.Build<Match, int>(ResourcePath.WorldCup_2014, t => t.Num)},
                {CupYear._2018, RepoFactory.Build<Match, int>(ResourcePath.WorldCup_2018, t => t.Num)},
            };
            GroupsByYear = new Dictionary<CupYear, IRepo<string, Group>>
            {
                {CupYear._2014, RepoFactory.Build<Group, string>(ResourcePath.Groups_2014, t => t.Name)},
                {CupYear._2018, RepoFactory.Build<Group, string>(ResourcePath.Groups_2018, t => t.Name)},
            };
            StandingsByYear = new Dictionary<CupYear, IRepo<string, Standing>>
            {
                {CupYear._2014, RepoFactory.Build<Standing, string>(ResourcePath.Standings_2014, t => t.Team.Code)},
                {CupYear._2018, RepoFactory.Build<Standing, string>(ResourcePath.Standings_2018, t => t.Team.Code)},
            };
        }

        public Dictionary<CupYear, IRepo<string, Team>> TeamsByYear { get; private set; }
        public Dictionary<CupYear, IRepo<int, Match>> MatchesByYear { get; private set; }
        public Dictionary<CupYear, IRepo<string, Group>> GroupsByYear { get; private set; }
        public Dictionary<CupYear, IRepo<string, Standing>> StandingsByYear { get; private set; }
    }
}
