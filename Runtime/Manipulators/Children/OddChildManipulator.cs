namespace Leaframe.Manipulators.Children
{
    public class OddChildManipulator : NthChildManipulator
    {
        protected override string ChildUssClassname => "odd-child";

        public OddChildManipulator() : base("odd"){}
    }
}
