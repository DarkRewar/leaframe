﻿namespace Leaframe.Manipulators
{
    public class EvenChildManipulator : NthChildManipulator
    {
        protected override string ChildUssClassname => "even-child";

        public EvenChildManipulator() : base("even"){}
    }
}
