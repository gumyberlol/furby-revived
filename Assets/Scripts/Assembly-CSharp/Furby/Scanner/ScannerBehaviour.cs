using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Scanner
{
    public class ScannerBehaviour : GameEventReceiver
    {
        public override Type EventType
        {
            get { return typeof(ScannerEvents); }
        }

        private void Start()
        {
            // stub - no Furby scanning!! 😂
            // "yeah totally scanned!!"
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // stub - no debug panel needed!!
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // stub - nothing to dispose!!
        }

        protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
        {
            // stub - handle PlayWithoutFurby only!!
            ScannerEvents scannerEvents = (ScannerEvents)(object)enumValue;
            if (scannerEvents == ScannerEvents.PlayWithoutFurbyTemporarily)
            {
                GameEventRouter.SendEvent(PlayerFurbyCommand.TurnOnNoFurbyMode);
                GameEventRouter.SendEvent(PlayerFurbyEvent.StatusUpdated, null, FurbyGlobals.Player);
            }
        }

        public bool IsBusy()
        {
            return false; // never busy!! 😂
            // "yeah totally not scanning!!"
        }
    }
}