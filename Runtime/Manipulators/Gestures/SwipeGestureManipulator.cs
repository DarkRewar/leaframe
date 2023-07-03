using System;
using Leaframe.Runtime.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Manipulators.Gestures
{
    public class SwipeGestureManipulator : Manipulator
    {
        private Vector2 _initialPosition = default;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            _initialPosition = evt.position;
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (_initialPosition.Equals(default)) return;
            
            var dir = ((Vector2)evt.position - _initialPosition).normalized;
            _initialPosition = default;
            using (var swipeEvent = SwipeEvent.GetPooled())
            {
                swipeEvent.target = target;
                if (dir.x < -0.5f) swipeEvent.Direction |= SwipeDirection.Left;
                if (dir.x > 0.5f) swipeEvent.Direction |= SwipeDirection.Right;
                if (dir.y < -0.5f) swipeEvent.Direction |= SwipeDirection.Up;
                if (dir.y > 0.5f) swipeEvent.Direction |= SwipeDirection.Down;
                Debug.Log(dir);
                target.SendEvent(swipeEvent);
            }
        }
    }
}
