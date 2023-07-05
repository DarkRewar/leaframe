using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public abstract class AxesChart : Chart
    {
        #region TRAITS

        public new class UxmlTraits : Chart.UxmlTraits
        {
            protected readonly UxmlBoolAttributeDescription _displayHorizontalLabels = new()
            {
                name = "display-horizontal-labels",
                defaultValue = true
            };
            
            protected readonly UxmlBoolAttributeDescription _displayVerticalLabels = new()
            {
                name = "display-vertical-labels",
                defaultValue = true
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not AxesChart axesChart) return;
                axesChart.DisplayHorizontalLabels = _displayHorizontalLabels.GetValueFromBag(bag, cc);
                axesChart.DisplayVerticalLabels = _displayVerticalLabels.GetValueFromBag(bag, cc);
            }
        }

        #endregion

        public bool DisplayHorizontalLabels { get; protected set; } = true;
        public bool DisplayVerticalLabels { get; protected set; } = true;

        public virtual Rect ChartRect
        {
            get
            {
                var content = contentRect;
                content.x += 100;
                content.y += 0;
                content.width -= 100;
                content.height -= 50;
                return content;
            }
        }
        
        protected const string AxesChartClassname = "axes-chart";
        protected const string ChartLabelClassname = "chart-label";

        public AxesChart()
        {
            AddToClassList(AxesChartClassname);
            
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Clear();

            var rect = ChartRect;
            (int minStep, int maxStep) = ComputeMinMaxSteps();
            
            int numberOfSteps = DetermineNumberOfSteps();
            for (int i = 0; i <= numberOfSteps; ++i)
            {
                var y = Mathf.Lerp(rect.yMax, rect.yMin, (float)i/numberOfSteps);
                var label = new Label($"{Mathf.Lerp(minStep, maxStep, (float)i / numberOfSteps):### ### ##0}");
                label.AddToClassList(ChartLabelClassname);
                Add(label);
                label.style.position = Position.Absolute;
                label.style.top = Mathf.RoundToInt(y - 20);
            }
        }

        protected (double minValue, double maxValue) ComputeMinMax()
        {
            return (_dataSet.Min(set => set.Min(data => data.Value)), 
                _dataSet.Max(set => set.Max(data => data.Value)));
        }

        protected (int minStep, int maxStep) ComputeMinMaxSteps()
        {
            (double minValue, double maxValue) = ComputeMinMax();
            var unit = Mathf.FloorToInt(Mathf.Log10((float)(maxValue-minValue + 1)));
            unit = unit == 0 ? 1 : unit-1;
            unit = (int)Math.Pow(10, unit);
            int minStep = unit * Mathf.FloorToInt((float)minValue / unit);
            int maxStep = unit * Mathf.CeilToInt((float)maxValue / unit);
            minStep = (int)minValue == minStep ? minStep - unit : minStep;
            maxStep = (int)maxValue == maxStep ? maxStep + unit : maxStep;
            return (minStep, maxStep);
        }

        private void OnGenerateVisualContent(MeshGenerationContext obj)
        {
            var painter = obj.painter2D;
            (double minValue, double maxValue) = ComputeMinMax();
            DrawSteps(painter, minValue, maxValue);
            DrawAxes(painter);
        }
        
        private void DrawAxes(Painter2D painter)
        {
            painter.strokeColor = Color.black;
            painter.lineWidth = 5;
            var rect = ChartRect;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.Stroke();
        }

        private void DrawSteps(Painter2D painter, double minValue, double maxValue)
        {
            var rect = ChartRect;

            painter.lineWidth = 2;
            painter.strokeColor = new Color(0, 0, 0, 0.2f);

            int numberOfSteps = DetermineNumberOfSteps();
            for (int i = 0; i <= numberOfSteps; ++i)
            {
                var y = Mathf.Lerp(rect.yMax, rect.yMin, (float)i/numberOfSteps);
                painter.BeginPath();
                painter.MoveTo(new Vector2(rect.xMin-20, y));
                painter.LineTo(new Vector2(rect.xMax, y));
                painter.Stroke();
            }

            for (int j = 0; j < _dataSet[0].Count; ++j)
            {
                var x = Mathf.Lerp(rect.xMin, rect.xMax, (float)j/(_dataSet[0].Count-1));
                painter.BeginPath();
                painter.MoveTo(new Vector2(x, rect.yMin));
                painter.LineTo(new Vector2(x, rect.yMax+20));
                painter.Stroke();
            }
        }

        protected int DetermineNumberOfSteps()
        {
            (double minValue, double maxValue) = ComputeMinMaxSteps();
            var total = maxValue - minValue;
            var unit = Mathf.FloorToInt(Mathf.Log10((float)(total + 1)));
            var div = total / (unit <= 1 ? 1 : unit - 1);

            for (int i = 8; i >= 2; --i)
            {
                if (div % i == 0) return i;
            }
            
            return 10;
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
        }
    }
}