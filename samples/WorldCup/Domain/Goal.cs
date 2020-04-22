namespace WorldCup.Domain
{
    public class Goal
    {
        public string Name { get; set; }
        public int Minute { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public bool? OwnGoal { get; set; }
        public bool? Penalty { get; set; }
    }
}
