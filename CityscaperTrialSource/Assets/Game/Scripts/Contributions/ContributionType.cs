using System;

using Common;

namespace Game {
    /// <summary>
    /// Struct enum for the different contribution types
    /// </summary>
    public readonly struct ContributionType {
        public static readonly ContributionType SUGGESTION = new ContributionType("Suggestion", 0);
        public static readonly ContributionType OPINION = new ContributionType("Opinion", 1);
        public static readonly ContributionType CRITICISM = new ContributionType("Criticism", 2);

        public static readonly ContributionType[] ALL = {
            SUGGESTION, 
            OPINION, 
            CRITICISM
        };

        public static ContributionType ConvertFromIndex(int index) {
            return ALL[index];
        }

        public static ContributionType ConvertFromId(string id) {
            for (int i = 0; i < ALL.Length; ++i) {
                if (ALL[i].id.EqualsFast(id)) {
                    return ALL[i];
                }
            }

            throw new Exception($"Can;t convert '{id}' to ContributionType");
        }
        
        public readonly string id;
        public readonly int index;

        public ContributionType(string id, int index) {
            this.id = id;
            this.index = index;
        }
    }
}