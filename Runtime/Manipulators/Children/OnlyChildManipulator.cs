namespace Leaframe.Manipulators.Children
{
    public class OnlyChildManipulator : ChildManipulator
    {
        protected override string ChildUssClassname => "only-child";

        protected override bool IsValidChild(int index, int count) => count == 1;
    }
}
