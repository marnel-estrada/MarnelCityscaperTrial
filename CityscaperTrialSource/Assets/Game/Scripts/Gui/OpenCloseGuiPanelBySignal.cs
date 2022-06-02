using System;

using UnityEngine;

using Common;
using Common.Signal;

#nullable enable

namespace Game {
    public class OpenCloseGuiPanelBySignal : MonoBehaviour {
        [SerializeField]
        private GuiPanelHandler? panelHandler;

        // the signal for the panel to be opened
        [SerializeField]
        private string? openSignal;

        // the signal for the panel to be closed
        [SerializeField]
        private string? closeSignal;

        private void Awake() {
            Assertion.NotNull(this.panelHandler);
            Assertion.NotEmpty(this.openSignal);
            Assertion.NotEmpty(this.closeSignal);
        }

        private void Start() {
            if (this.panelHandler == null) {
                return;
            }

            // we do this in start so that GuiPanelHandler already ran its Awake()
            this.panelHandler.AddSignalListener(GameSignals.GetSignal(this.openSignal ?? throw new InvalidOperationException()), Open);
            this.panelHandler.AddSignalListener(GameSignals.GetSignal(this.closeSignal ?? throw new InvalidOperationException()), this.panelHandler.Close);
        }

        private void Open(ISignalParameters parameters) {
            if (this.panelHandler != null) {
                this.panelHandler.Open(parameters);
            }
        }
    }
}