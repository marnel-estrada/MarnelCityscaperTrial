namespace Game {
    /// <summary>
    /// The data to pass when creating a new contribution
    /// </summary>
    public readonly struct AddNewContribution {
        public readonly string title;
        public readonly string content;
        public readonly ContributionType contributionType;

        public AddNewContribution(string title, string content, ContributionType contributionType) {
            this.title = title;
            this.content = content;
            this.contributionType = contributionType;
        }
    }
}