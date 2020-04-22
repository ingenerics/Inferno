using WorldCup.Domain;

namespace WorldCup.ViewModels.Detail
{
    public class GoalsWrapper
    {
        public GoalsWrapper(Match match, Goal[] goals)
        {
            Match = match;
            Goals = goals;
        }

        public Match Match { get; set; }
        public Goal[] Goals { get; set; }
    }
}
