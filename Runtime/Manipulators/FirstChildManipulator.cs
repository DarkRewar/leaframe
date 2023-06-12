using UnityEngine.UIElements;

namespace Leaframe.Manipulators
{
    /// <summary>
    /// Allow <see cref="VisualElement"/> to be flagged as first child.
    /// Following https://developer.mozilla.org/fr/docs/Web/CSS/:first-child
    /// </summary>
    public class FirstChildManipulator : ChildManipulator
    {
        protected override string ChildUssClassname => "first-child";

        protected override bool IsValidChild(int index, int _) => index == 0;
    }
}
