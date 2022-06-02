namespace Game {
    /// <summary>
    /// Struct enum for the different contribution types
    /// </summary>
    public readonly struct ContributionType {
        public static readonly ContributionType SUGGESTION = new ContributionType("Suggestion");
        public static readonly ContributionType OPINION = new ContributionType("Opinion");
        public static readonly ContributionType CRITICISM = new ContributionType("Criticism");

        public static readonly ContributionType[] ALL = {
            SUGGESTION, OPINION, CRITICISM
        };
        
        public readonly string id;

        public ContributionType(string id) {
            this.id = id;
        }
    }
}