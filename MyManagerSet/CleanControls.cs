using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityCheatMainManager.MyManagerSet
{
    internal class CleanControls
    {
        public void RemoveNeedSetControls(List<GameObject> currentPageControls)
        {
            foreach (GameObject control in currentPageControls)
            {
                if (control != null)
                {
                    UnityEngine.Object.Destroy(control);
                }
            }
            currentPageControls.Clear();
        }
    }
}
