using UnityEngine.UIElements;

namespace Leaframe.Manipulators.Children
{
    /// <summary>
    /// Base class to allow child selection inside a <see cref="VisualElement"/>.
    /// Must be inherited to 
    /// </summary>
    public abstract class ChildManipulator : Manipulator
    {
        protected abstract string ChildUssClassname { get; }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            target.schedule.Execute(RebuildChildren).ExecuteLater(10);
        }

        private void RebuildChildren()
        {
            var childCount = target.childCount;
            for (var index = 0; index < childCount; ++index)
            {
                var child = target[index];
                if(IsValidChild(index, childCount))
                    child.AddToClassList(ChildUssClassname);
                else
                    child.RemoveFromClassList(ChildUssClassname);
            }
        }

        protected abstract bool IsValidChild(int index, int count);
    }
}
