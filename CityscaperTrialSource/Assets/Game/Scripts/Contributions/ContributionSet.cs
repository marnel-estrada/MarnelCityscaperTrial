using System.Collections.Generic;

using UnityEngine;

namespace Game {
    /// <summary>
    /// Handles the set of contributions of a particular object.
    /// </summary>
    public class ContributionSet : MonoBehaviour {
        private readonly List<Contribution> contributions = new List<Contribution>();

        public void Add(Contribution contribution) {
            this.contributions.Add(contribution);
        }
    }
}