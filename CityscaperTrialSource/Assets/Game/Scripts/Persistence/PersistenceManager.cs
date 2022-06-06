using System;
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
                        SelectableObject selectableObject = contributionSet.GetRequiredComponent<SelectableObject>();

                        writer.WriteStartElement("ContributionSet");
                        writer.SafeWriteAttributeString("id", selectableObject.Id);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            Debug.Log("Saved to " + this.saveFilePath);
        }
    }
}