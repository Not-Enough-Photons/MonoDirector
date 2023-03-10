using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class PhotoMode
    {
        public PhotoMode()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public PhotoMode instance;

        public void TakeSnapshot()
        {

        }

        private IEnumerator SnapshotRoutine()
        {
            Events.OnPreSnapshot?.Invoke();
            yield return new WaitForSeconds(5f);
        }
    }
}
