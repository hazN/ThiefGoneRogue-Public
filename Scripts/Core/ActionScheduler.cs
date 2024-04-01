using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        public void StartAction (IAction action) 
        {
            // Check if action is already running
            if (currentAction == action) return;

            // Cancel current action if possible
            if (currentAction != null) 
            {
               currentAction.Cancel();
            }

            // Set new action
            currentAction = action;
        }
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}