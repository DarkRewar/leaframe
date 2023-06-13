using System;
using System.Text.RegularExpressions;

namespace Leaframe.Manipulators.Children
{
    /// <summary>
    /// Allow <see cref="VisualElement"/> to be flagged as nth child.
    /// Following https://developer.mozilla.org/fr/docs/Web/CSS/:nth-child
    /// </summary>
    public class NthChildManipulator : ChildManipulator
    {
        private readonly static Regex _selectorRegex =  new(@"^([0-9]+)n([?=\+|\-][0-9]+)?$");
        
        protected override string ChildUssClassname { get; } = "nth-child";

        private readonly int _index;
        private readonly int _offset;
        private readonly int? _count;
        
        protected NthChildManipulator(int index)
        {
            _index = index - 1;
            _count = null;
            ChildUssClassname = string.Empty;
        }

        protected NthChildManipulator(int index, int count)
        {
            _index = index;
            _count = count;
            ChildUssClassname = string.Empty;
        }

        public NthChildManipulator(int index, int count, string classname)
        {
            _index = index;
            _count = count;
            ChildUssClassname = classname;
        }

        public NthChildManipulator(string selector)
        {
            switch (selector)
            {
                case "even":
                    (_index, _count) = (0, 2);
                    break;
                case "odd":
                    (_index, _count) = (1, 2);
                    break;
                default:
                    if (!_selectorRegex.IsMatch(selector))
                        throw new ArgumentException($"Selector {selector} is not recognized.");
                    
                    var matches = _selectorRegex.Matches(selector)[0];
                    if (int.TryParse(matches.Groups[1].Value, out int index))
                        (_index, _count) = (index - 1, index);
                    
                    if (matches.Groups.Count > 2 && int.TryParse(matches.Groups[2].Value, out int offset))
                        _offset = offset;
                    
                    break;
            }
        }

        public NthChildManipulator(string selector, string classname) : this(selector)
        {
            ChildUssClassname = classname;
        }

        protected override bool IsValidChild(int index, int count) =>
            _count.HasValue 
                ? (index % _count) + _offset == _index 
                : index == _index;
    }
}
