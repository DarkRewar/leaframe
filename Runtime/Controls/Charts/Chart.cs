using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class ChartDataSet : List<ChartData>
    {
        public Color Color;

        public ChartDataSet(List<ChartData> chartDataList)
            : this(chartDataList, Chart.Color){}

        public ChartDataSet(List<ChartData> chartDataList, Color color)
        {
            Color = color;
            this.AddRange(chartDataList);
        }
    }
    
    public struct ChartData
    {
        public readonly double Value;
        public readonly string Id;
        public readonly Color Color;

        public ChartData(double value): this(value, string.Empty){}
        
        public ChartData(double value, string id, Color color = default)
        {
            Value = value;
            Id = id;
            Color = color;
        }
    }
    
    public abstract class Chart : VisualElement
    {
        protected static readonly Color[] _availableColors = new[]
        {
            new Color(46f/255, 204f/255, 113f/255),
            new Color(231f/255, 76f/255, 60f/255),
            new Color(52f/255, 152f/255, 219f/255),
            new Color(243f/255, 156f/255, 18f/255),
        };

        protected static int _currentColorIndex = 0;

        public static Color Color => _availableColors[(_currentColorIndex++) % (_availableColors.Length - 1)];

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

        public virtual List<ChartDataSet> DataSet
        {
            get => _dataSet;
            set
            {
                _dataSet = value;
                if (Labels == default & _dataSet.Count > 0)
                    Labels = _dataSet[0].Select(data => data.Id).ToList();
                OnDataSetChanged(_dataSet);
            }
        }

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
        
        protected virtual void OnCursorPositionChanged(Vector2 cursorPosition){}

        protected abstract void OnDataSetChanged(List<ChartDataSet> dataSet);
    }
}
