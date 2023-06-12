using UnityEngine.UIElements;

namespace Leaframe.Manipulators
{
    /// <summary>
    /// Allow <see cref="VisualElement"/> to be flagged as empty.
    /// Following https://developer.mozilla.org/fr/docs/Web/CSS/:empty
    /// </summary>
    public class EmptyManipulator : Manipulator
    {
        public const string Classname = "empty";
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            UpdateClass();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent _) => target.schedule.Execute(UpdateClass).ExecuteLater(10);

        private void UpdateClass()
        {
            if(target.childCount == 0) target.AddToClassList(Classname);
            else target.RemoveFromClassList(Classname);
        }
    }
}
