namespace WorldCup.Domain
{
    public class TeamOutcome
    {
        public Team Team { get; set; }

        /// <summary>
        /// End result
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// At half time
        /// </summary>
        public int? ScoreI { get; set; }

        /// <summary>
        /// During Extra Time
        /// </summary>
        public int? ScoreET { get; set; }

        public Goal[] Goals { get; set; }
    }
}
