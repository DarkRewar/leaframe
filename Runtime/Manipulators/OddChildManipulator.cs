namespace Leaframe.Manipulators
{
    public class OddChildManipulator : NthChildManipulator
    {
        protected override string ChildUssClassname => "odd-child";

        public OddChildManipulator() : base("odd"){}
    }
}
