using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Common;
using Common.Signal;

using UnityEngine;

namespace Game {
    public class PersistenceManager : SignalHandlerComponent {
        private string saveFilePath;
        
        protected override void Awake() {
            base.Awake();

            this.saveFilePath = Path.Combine(Application.persistentDataPath, "Contributions.xml");
            
            AddSignalListener(GameSignals.SAVE_THEN_CLOSE, SaveThenClose);
        }

        private void SaveThenClose(ISignalParameters parameters) {
            Save();
            Application.Quit();
        }
        
        private static readonly XmlWriterSettings WRITER_SETTINGS = new XmlWriterSettings() {
            Indent = true
        };

        private void Save() {
            // Delete existing one first
            if (File.Exists(this.saveFilePath)) {
                File.Delete(this.saveFilePath);
            }
            
            using (FileStream stream = new FileStream(this.saveFilePath, FileMode.Create, FileAccess.ReadWrite,
                       FileShare.ReadWrite, 1024, FileOptions.WriteThrough)) {
                using (XmlWriter writer = XmlWriter.Create(stream, WRITER_SETTINGS)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Root"); // Root
                    writer.WriteAttributeString("Timestamp", DateTime.Now.ToString("MMMM dd yyyy HH:mm:ss"));

                    // Traverse through all contribution sets
                    ContributionSet[] contributionSets = FindObjectsOfType<ContributionSet>();
                    for (int i = 0; i < contributionSets.Length; ++i) {
                        ContributionSet contributionSet = contributionSets[i];
                        Write(writer, contributionSet);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            Debug.Log("Saved to " + this.saveFilePath);
        }

        private readonly InstanceWriter contributionWriter = new InstanceWriter(typeof(Contribution)); 

        private void Write(XmlWriter writer, ContributionSet contributionSet) {
            SelectableObject selectableObject = contributionSet.GetRequiredComponent<SelectableObject>();

            writer.WriteStartElement("ContributionSet");
            writer.SafeWriteAttributeString("id", selectableObject.Id);
            
            // Write each contribution and its comment tree
            IEnumerable<Contribution> contributions = contributionSet.Contributions;
            foreach (Contribution contribution in contributions) {
                this.contributionWriter.Start(writer);
                this.contributionWriter.WriteProperties(writer, contribution);
                
                // Write child comments
                IEnumerable<CommentTreeNode> children = contribution.Children;
                foreach (CommentTreeNode childNode in children) {
                    Write(writer, childNode);
                }
                
                this.contributionWriter.End(writer);
            }
            
            writer.WriteEndElement();
        }

        private readonly InstanceWriter commentWriter = new InstanceWriter(typeof(Comment));

        private void Write(XmlWriter writer, CommentTreeNode commentNode) {
            // Allow comments only
            Comment comment = commentNode as Comment;
            if (comment == null) {
                // Not a comment
                return;
            }
            
            this.commentWriter.Start(writer);
            this.commentWriter.WriteProperties(writer, comment);
            
            // Write children
            IEnumerable<CommentTreeNode> children = comment.Children;
            foreach (CommentTreeNode childNode in children) {
                Write(writer, childNode);
            }
            
            this.commentWriter.End(writer);
        }
    }
}