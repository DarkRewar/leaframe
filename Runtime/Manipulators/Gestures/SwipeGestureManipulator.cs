using UnityEngine.UIElements;

namespace Leaframe.Manipulators.Gestures
{
    public class SwipeGestureManipulator : Manipulator
    {
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeft);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeft);
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            throw new System.NotImplementedException();
        }

        private void OnDragLeft(DragLeaveEvent evt)
        {
            throw new System.NotImplementedException();
        }
    }
}
