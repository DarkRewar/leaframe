using Leaframe.Manipulators.Children;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Layouts
{
    public class Container : VisualElement
    {
        #region FACTORY & TRAITS

        public new class UxmlFactory : UxmlFactory<Container, UxmlTraits>
        {
            public override string uxmlName => nameof(Container);

            public override string uxmlNamespace => "Leaframe.Layouts";
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            
        }

        #endregion

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
