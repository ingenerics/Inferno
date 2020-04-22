namespace WorldCup.Domain
{
    public class Match
    {
        public int Num { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string City { get; set; }

        public TeamOutcome OutcomeTeam1 { get; set; }
        public Team Team1 => OutcomeTeam1.Team;
        public int Score1 => OutcomeTeam1.Score;

        public TeamOutcome OutcomeTeam2 { get; set; }
        public Team Team2 => OutcomeTeam2.Team;
        public int Score2 => OutcomeTeam2.Score;
    }
}
