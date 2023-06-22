using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Layouts
{
    public class TabView : VisualElement
    {
        #region TRAITS & FACTORY

        [Preserve]
        public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits>
        {
            public override string uxmlName => nameof(TabView);

            public override string uxmlNamespace => "Leaframe.Layouts";
        }

        [Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _animated = new()
            {
                name = "animated",
                defaultValue = true
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not TabView tabView) return;

                tabView.Animated = _animated.GetValueFromBag(bag, cc);
            }
        }
        
        #endregion

        private bool _animated = true;

        public bool Animated
        {
            get => _animated;
            set
            {
                _animated = value;
                ApplyAnimationClasses();
            }
        }

        public IList<string> ButtonTexts
        {
            get => _tabContainer.Query<Label>().ToList().Select(child => child.text).ToList();
            set => Enumerable.Range(0, _tabContainer.childCount)
                .ToList()
                .ForEach(i => ((Label) _tabContainer[i]).text = value[i]);
        }

        public override VisualElement contentContainer => _tabSection;

        private VisualElement _tabContainer;
        private VisualElement _tabSection;

        private int _currentIndex = 0;

        private const string TabViewClassname = "tab-view";
        private const string TabContainerClassname = "tab-view-container";
        private const string TabSectionClassname = "tab-view-section";
        private const string TabContentClassname = "tab-view-content";
        private const string TabContentAnimatedClassname = "tab-view-content-animated";
        private const string TabButtonClassname = "tab-view-button";
        private const string ActiveClassname = "active";
        private const string ExitLeftClassname = "exit-left";
        private const string ExitRightClassname = "exit-right";

        public TabView()
        {
            AddToClassList(TabViewClassname);
            
            _tabContainer = new VisualElement();
            _tabContainer.AddToClassList(TabContainerClassname);
            hierarchy.Add(_tabContainer);

            _tabSection = new VisualElement();
            _tabSection.AddToClassList(TabSectionClassname);
            hierarchy.Add(_tabSection);
            
            RebuildTabs();
            RegisterCallback<AttachToPanelEvent>(_ => RebuildTabs());
            RegisterCallback<DetachFromPanelEvent>(_ => RebuildTabs());
        }

        public void RebuildTabs()
        {
            _tabContainer.Clear();
            
            for (int i = 0; i < _tabSection.childCount; ++i)
            {
                var child = _tabSection[i];
                child.AddToClassList(TabContentClassname);
                if(i == 0) child.AddToClassList(ActiveClassname);
                else child.AddToClassList(ExitRightClassname);
                
                string childName = string.IsNullOrEmpty(child.name) 
                    ? $"Header{i + 1}" 
                    : child.name;

                Label tabButton = new()
                {
                    name = $"{childName}NavButton"
                };
                tabButton.text = childName;
                tabButton.RegisterCallback<ClickEvent, int>(OnTabClicked, i);
                tabButton.AddToClassList(TabButtonClassname);
                if(i == 0) tabButton.AddToClassList(ActiveClassname);
                _tabContainer.Add(tabButton);
            }

            ApplyAnimationClasses();
        }

        private void OnTabClicked(ClickEvent evt, int index)
        {
            var navButton = evt.target as Label;
            if (navButton.ClassListContains(ActiveClassname)) return;

            ProcessAnimation(_currentIndex, index);

            _tabSection[_currentIndex].RemoveFromClassList(ActiveClassname);
            _tabContainer[_currentIndex].RemoveFromClassList(ActiveClassname);
            
            _tabSection[index].AddToClassList(ActiveClassname);
            _tabContainer[index].AddToClassList(ActiveClassname);
            
            _currentIndex = index;
        }

        private void ProcessAnimation(int previousIndex, int newIndex)
        {
            var exit = previousIndex < newIndex ? ExitLeftClassname : ExitRightClassname;
            for (int i = 0; i < _tabSection.childCount; ++i)
            {
                if (i < newIndex)
                {
                    _tabSection[i].RemoveFromClassList(ExitRightClassname);
                    _tabSection[i].AddToClassList(ExitLeftClassname);
                }
                else if (i > newIndex)
                {
                    _tabSection[i].RemoveFromClassList(ExitLeftClassname);
                    _tabSection[i].AddToClassList(ExitRightClassname);
                }
            }
            
            _tabSection[_currentIndex].AddToClassList(exit);
            _tabSection[newIndex].RemoveFromClassList(ExitLeftClassname);
            _tabSection[newIndex].RemoveFromClassList(ExitRightClassname);
        }

        private void ApplyAnimationClasses()
        {
            foreach(var child in _tabSection.Children())
            {
                if(_animated) child.AddToClassList(TabContentAnimatedClassname);
                else child.RemoveFromClassList(TabContentAnimatedClassname);
            }
        }
    }
}
