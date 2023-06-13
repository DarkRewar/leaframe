namespace Leaframe.Manipulators.Children
{
    /// <summary>
    /// Allow <see cref="VisualElement"/> to be flagged as last child.
    /// Following https://developer.mozilla.org/fr/docs/Web/CSS/:last-child
    /// </summary>
    public class LastChildManipulator : ChildManipulator
    {
        protected override string ChildUssClassname => "last-child";

        protected override bool IsValidChild(int index, int count) => index == count - 1;
    }
}
