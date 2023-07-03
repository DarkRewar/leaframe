using System;
using UnityEngine.UIElements;

namespace Leaframe.Runtime.Events
{
    [Flags]
    public enum SwipeDirection
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
    }
    
    public class SwipeEvent : EventBase<SwipeEvent>
    {
        public SwipeDirection Direction { get; internal set; } = 0;

        protected override void Init()
        {
            base.Init();
            Direction = 0;
        }
    }
}