namespace WorldCup.Domain
{
    public class Standing
    {
        public Team Team { get; set; }
        public int Pos { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int Goals_For { get; set; }
        public int Goals_Against { get; set; }
        public int Pts { get; set; }
    }
}
