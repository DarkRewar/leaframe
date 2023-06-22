using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Leaframe.Controls.Components
{
    public class Alert : TextElement
    {
        #region FACTORY & TRAITS

        public new class UxmlFactory : UxmlFactory<Alert, UxmlTraits>
        {
            public override string uxmlName => nameof(Alert);

            public override string uxmlNamespace => "Leaframe.Components";
        }

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            protected readonly UxmlEnumAttributeDescription<AlertType> _type = new()
            {
                name = "type",
                defaultValue = AlertType.Info
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not Alert alert) return;

                alert.Type = _type.GetValueFromBag(bag, cc);
            }
        }

        #endregion

        public enum AlertType
        {
            Success,
            Warning,
            Info,
            Error
        }

        private AlertType _type;

        public AlertType Type
        {
            get => _type;
            set
            {
                RemoveFromClassList(_type.ToString().ToLower());
                AddToClassList(value.ToString().ToLower());
                _type = value;
            }
        }

        protected Label _label;

        public const string AlertClassname = "alert";
        public const string AlertCloseClassname = "alert-close";

        public Alert() : this(AlertType.Info){}

        public Alert(AlertType alertType)
        {
            AddToClassList(AlertClassname);
            Type = alertType;

            //_label = new Label(text);

            var closeButton = new Button(OnCloseClicked);
            closeButton.AddToClassList(AlertCloseClassname);
            //Add(closeButton);
        }

        private void OnCloseClicked()
        {
            this.RemoveFromHierarchy();
        }

        public Alert(AlertType alertType, string label, Action onClicked) : this(alertType)
        {
            text = label;
            RegisterCallback<ClickEvent>(_ => onClicked?.Invoke());
        }

        private static Alert Create(AlertType type, string label, Action onClicked)
        {
            UIDocument document = GameObject.FindAnyObjectByType<UIDocument>();
            if (document)
                return Create(type, label, onClicked, document.rootVisualElement);
            
            Debug.LogWarning("No UIDocument found to place the Alert in.");
                
            return new Alert(type, label, onClicked);
        }

        private static Alert Create(AlertType type, string label, Action onClicked, [NotNull] VisualElement container)
        {
            Alert alert = new(type, label, onClicked);
            if (container == null) 
                throw new NullReferenceException($"[Leaframe] Container can't be null for {nameof(Alert)}.");
            container.Add(alert);
            return alert;
        }

        public static Alert Success(string label, Action onClicked) => Create(AlertType.Success, label, onClicked);
        public static Alert Error(string label, Action onClicked) => Create(AlertType.Error, label, onClicked);
        public static Alert Info(string label, Action onClicked) => Create(AlertType.Info, label, onClicked);
        public static Alert Warning(string label, Action onClicked) => Create(AlertType.Warning, label, onClicked);

        public static Alert Success(string label, Action onClicked, [NotNull] VisualElement container) => 
            Create(AlertType.Success, label, onClicked);
        public static Alert Error(string label, Action onClicked, [NotNull] VisualElement container) => 
            Create(AlertType.Error, label, onClicked);
        public static Alert Info(string label, Action onClicked, [NotNull] VisualElement container) => 
            Create(AlertType.Info, label, onClicked);
        public static Alert Warning(string label, Action onClicked, [NotNull] VisualElement container) => 
            Create(AlertType.Warning, label, onClicked);
    }
}
