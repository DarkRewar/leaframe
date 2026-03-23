using Leaframe.Manipulators.Children;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Layouts
{
    [UxmlElement(libraryPath = "Leaframe/Layouts")]
    public partial class Container : VisualElement
    {
        protected FirstChildManipulator _firstChildManipulator;
        protected LastChildManipulator _lastChildManipulator;
        protected EvenChildManipulator _evenChildManipulator;
        protected OddChildManipulator _oddChildManipulator;
        protected OnlyChildManipulator _onlyChildManipulator;
        protected EmptyManipulator _emptyManipulator;

        public Container()
        {
            this.AddManipulator(_firstChildManipulator = new FirstChildManipulator());
            this.AddManipulator(_lastChildManipulator = new LastChildManipulator());
            this.AddManipulator(_evenChildManipulator = new EvenChildManipulator());
            this.AddManipulator(_oddChildManipulator = new OddChildManipulator());
            this.AddManipulator(_onlyChildManipulator = new OnlyChildManipulator());
            this.AddManipulator(_emptyManipulator = new EmptyManipulator());
        }
    }
}