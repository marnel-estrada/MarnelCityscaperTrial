using System;
using System.Collections.Generic;

using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ContributionsHandler : SignalHandlerComponent {
        [SerializeField]
        private PrefabManager pool;

        [SerializeField]
        private TMP_Text headerLabel;

        [SerializeField]
        private RectTransform contributionsGridRoot;

        private readonly IdGenerator idGenerator = new IdGenerator();

        private readonly Dictionary<string, ContributionEntryRow> rowMap =
            new Dictionary<string, ContributionEntryRow>();

        private Option<ContributionSet> currentContributionSet;

        protected override void Awake() {
            base.Awake();

            Assertion.NotNull(this.pool);
            Assertion.NotNull(this.headerLabel);
            Assertion.NotNull(this.contributionsGridRoot);

            AddSignalListener(GameSignals.OPEN_CONTRIBUTIONS_PANEL, OnOpen);
            
            GameSignals.ADD_NEW_CONTRIBUTION.AddListener(AddContribution);
        }

        private void OnOpen(ISignalParameters parameters) {
            Option<string> objectId = parameters.GetParameter<string>(Params.OBJECT_ID);
            Assertion.IsSome(objectId);
            objectId.Match(new SetHeaderLabelMatcher(this.headerLabel));

            this.currentContributionSet = parameters.GetParameter<ContributionSet>(Params.CONTRIBUTION_SET);
            Assertion.IsSome(this.currentContributionSet);
            RegenerateContributionRows(this.currentContributionSet.ValueOrError());
        }
        
        private readonly struct SetHeaderLabelMatcher : IOptionMatcher<string> {
            private readonly TMP_Text headerLabel;

            public SetHeaderLabelMatcher(TMP_Text headerLabel) {
                this.headerLabel = headerLabel;
            }

            public void OnSome(string objectId) {
                this.headerLabel.text = $"Contributions: {objectId}";
            }

            public void OnNone() {
            }
        }

        private void RegenerateContributionRows(ContributionSet contributionSet) {
            // Clear existing ones first
            foreach (KeyValuePair<string,ContributionEntryRow> entry in this.rowMap) {
                entry.Value.Recycle();
            }
            this.rowMap.Clear();

            IEnumerable<Contribution> contributions = contributionSet.Contributions;
            foreach (Contribution contribution in contributions) {
                GenerateRow(contribution);
            }
        }

        private const string PROJECT = "MarnelCityscaperTrial";
        private const string AUTHOR = "CurrentLoggedInAuthor";

        private void AddContribution(AddNewContribution param) {
            Assertion.IsSome(this.currentContributionSet);
            ContributionSet contributionSet = this.currentContributionSet.ValueOrError();
            Vector3 selectedObjectPosition = contributionSet.transform.position;

            string id = this.idGenerator.Generate().ToString();
            Contribution contribution = new Contribution(id, DateTime.Now, PROJECT, AUTHOR);
            contribution.Title = param.title;
            contribution.ContributionContent = param.content;
            contribution.ContributionType = param.contributionType;
            contribution.Latitude = selectedObjectPosition.x;
            contribution.Longitude = selectedObjectPosition.z;
            contribution.Status = Status.NOT_YET_WORKED_ON;
            contributionSet.Add(contribution);

            GenerateRow(contribution);
        }

        private const string ROW_PREFAB = "ContributionEntryRow";

        private void GenerateRow(Contribution contribution) {
            GameObject go = this.pool.Request(ROW_PREFAB);
            Transform rowTransform = go.transform;
            rowTransform.SetParent(this.contributionsGridRoot);
            rowTransform.localScale = Vector3.one;
            rowTransform.SetAsFirstSibling();

            ContributionEntryRow row = go.GetRequiredComponent<ContributionEntryRow>();
            row.Init(contribution.Title, contribution.ContributionContent);
            
            // Manage
            this.rowMap[contribution.ID] = row;
        }
    }
}