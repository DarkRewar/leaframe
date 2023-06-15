using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Manipulators.Gestures
{
    public class SwipeGestureManipulator : Manipulator
    {
        private Vector2 _initialPosition;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(OnDragEntered);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.RegisterCallback<DragExitedEvent>(OnDragExited);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(OnDragEntered);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            target.UnregisterCallback<DragExitedEvent>(OnDragExited);
        }

        private void OnDragEntered(DragEnterEvent evt)
        {
            _initialPosition = evt.mousePosition;
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            throw new System.NotImplementedException();
        }

        private void OnDragExited(DragExitedEvent evt)
        {
            var dir = evt.mousePosition - _initialPosition;
            throw new NotImplementedException();
            //if(dir.magnitude > )
        }
    }
}
