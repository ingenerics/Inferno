using System.Collections.Generic;
using WorldCup.Domain;

namespace WorldCup.Repo
{
    public interface IWorldCupRepo
    {
        Dictionary<CupYear, IRepo<string, Team>> TeamsByYear { get; }
        Dictionary<CupYear, IRepo<int, Match>> MatchesByYear { get; }
        Dictionary<CupYear, IRepo<string, Group>> GroupsByYear { get; }
        Dictionary<CupYear, IRepo<string, Standing>> StandingsByYear { get; }
    }
}
