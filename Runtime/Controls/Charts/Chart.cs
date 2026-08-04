using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Charts
{
    [UxmlObject]
    public partial class ChartDataSet : ICollection<ChartData>, IEnumerable<ChartData>, IEnumerable, INotifyBindablePropertyChanged
    {
        [UxmlAttribute]
        public Color Color;

        private List<ChartData> _chartData = new List<ChartData>();

        [UxmlObjectReference]
        public List<ChartData> ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value;
                propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(nameof(ChartData)));
            }
        }

        public ChartDataSet() : this(new(), Chart.Color) { }

        public ChartDataSet(List<ChartData> chartDataList)
            : this(chartDataList, Chart.Color) { }

        public ChartDataSet(List<ChartData> chartDataList, Color color)
        {
            Color = color;
            _chartData.AddRange(chartDataList);
        }

        public ChartData this[int index]
        {
            get => _chartData[index];
            set => _chartData[index] = value;
        }

        internal Color GetColor(ChartData entry)
        {
            return entry.Color == default
                ? this.Color == default
                    ? Chart.Color
                    : Color
                : entry.Color;
        }

        public IEnumerator<ChartData> GetEnumerator() => _chartData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ChartData item) => _chartData.Add(item);

        public void Clear() => _chartData.Clear();

        public bool Contains(ChartData item) => _chartData.Contains(item);

        public void CopyTo(ChartData[] array, int arrayIndex) => _chartData.CopyTo(array, arrayIndex);

        public bool Remove(ChartData item) => _chartData.Remove(item);

        public int Count => _chartData.Count;
        public bool IsReadOnly => false;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
    }

    [UxmlObject]
    public partial class ChartData
    {
        [UxmlAttribute]
        public double Value;

        [UxmlAttribute]
        public string Id;

        [UxmlAttribute]
        public Color Color;

        public ChartData() { }

        public ChartData(double value) : this(value, string.Empty) { }

        public ChartData(double value, string id, Color color = default)
        {
            Value = value;
            Id = id;
            Color = color;
        }
    }

    [UxmlElement]
    public abstract partial class Chart : VisualElement
    {
        protected static readonly Color[] _availableColors = new[]
        {
            new Color(17f / 255, 29f / 255, 111f / 255),
            new Color(1, 163f / 255, 120f / 255),
            new Color(46f / 255, 204f / 255, 113f / 255),
            new Color(231f / 255, 76f / 255, 60f / 255),
            new Color(52f / 255, 152f / 255, 219f / 255),
            new Color(243f / 255, 156f / 255, 18f / 255),
        };

        protected static int _currentColorIndex = 0;

        public static Color Color =>
            _availableColors[(_currentColorIndex++) % (_availableColors.Length - 1)];

        private Vector2 _cursorPosition = default;

        public Vector2 CursorPosition
        {
            get => _cursorPosition;
            protected set
            {
                _cursorPosition = value;
                OnCursorPositionChanged(_cursorPosition);
            }
        }

        protected const string ChartClassname = "chart";

        protected List<ChartDataSet> _dataSet;

        [UxmlObjectReference]
        public List<ChartDataSet> DataSet
        {
            get => _dataSet;
            set
            {
                using (var chartDataEvent = ChangeEvent<List<ChartDataSet>>.GetPooled(_dataSet, value))
                {
                    chartDataEvent.target = this;
                    this.SendEvent(chartDataEvent);
                }

                _dataSet = value;
                if (Labels == default & _dataSet is { Count: > 0 })
                    Labels = _dataSet[0].Select(data => data.Id).ToList();
                OnDataSetChanged(_dataSet);
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        public List<string> Labels = default;

        protected Chart()
        {
            AddToClassList(ChartClassname);

            RegisterCallback<PointerMoveEvent>(OnPointerMoved);
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            CursorPosition = evt.localPosition;
        }

        private void OnPointerMoved(PointerMoveEvent evt)
        {
            CursorPosition = evt.localPosition;
        }

        protected virtual void OnCursorPositionChanged(Vector2 cursorPosition) { }

        protected abstract void OnDataSetChanged(List<ChartDataSet> dataSet);
    }
}