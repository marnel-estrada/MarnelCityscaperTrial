namespace Game {
    /// <summary>
    /// Struct enum for the different Status values
    /// </summary>
    public readonly struct Status {
        public static readonly Status NOT_YET_WORKED_ON = new Status(0, "Not yet worked on");
        public static readonly Status CATEGORIZED = new Status(1, "Categorized");
        public static readonly Status IN_PROGRESS = new Status(2, "In progress");
        public static readonly Status FINISHED = new Status(3, "Finished");

        public static readonly Status[] ALL = {
            NOT_YET_WORKED_ON,
            CATEGORIZED,
            IN_PROGRESS,
            FINISHED
        };
        
        public readonly byte id;
        public readonly string label;

        public Status(byte id, string label) {
            this.id = id;
            this.label = label;
        }
    }
}