using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class LineChart : Chart
    {
        #region TRAITS & FACTORY

        [Preserve]
        public new class UxmlFactory : UxmlFactory<LineChart, UxmlTraits>
        {
            public override string uxmlName => nameof(LineChart);

            public override string uxmlNamespace => "Leaframe.Charts";
        }

        [Preserve]
        public new class UxmlTraits : Chart.UxmlTraits
        {
            private readonly UxmlIntAttributeDescription _numberOfSteps = new()
            {
                name = "number-of-steps",
            };
            
            private readonly UxmlBoolAttributeDescription _displayDots = new()
            {
                name = "display-dots",
                defaultValue = true
            };

            private readonly UxmlEnumAttributeDescription<SteppedLineType> _steppedLine = new()
            {
                name = "stepped-line"
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not LineChart lineChart) return;

                lineChart.DisplayDots = _displayDots.GetValueFromBag(bag, cc);
                lineChart.SteppedLine = _steppedLine.GetValueFromBag(bag, cc);
            }
        }

        #endregion

        public enum SteppedLineType
        {
            No,
            Before,
            After,
            Middle,
            Curve
        }

        public bool DisplayDots { get; private set; } = true;
        public SteppedLineType SteppedLine { get; private set; }

        public Rect ChartRect
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

        private const float _heightPadding = 10;

        private const string LineChartClassname = "line-chart";
        private const string ChartLabelClassname = "chart-label";

        public LineChart()
        {
            AddToClassList(LineChartClassname);

            // This is a default dataset for UI Builder purpose.
            // Replace the DataSet value at runtime.
            DataSet = new List<ChartDataSet>()
            {
                new(new List<ChartData>()
                {
                    new(100, "January"),
                    new(1000, "February"),
                    new(1250, "March"),
                    new(678, "April"),
                    new(50, "May"),
                    new(80, "June"),
                    new(234, "July"),
                }),
                new ChartDataSet(new List<ChartData>()
                {
                    new(0, "January"),
                    new(20, "February"),
                    new(20, "March"),
                    new(60, "April"),
                    new(60, "May"),
                    new(140, "June"),
                    new(105, "July"),
                })
            };
            
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Clear();

            var rect = ChartRect;
            (double minValue, double maxValue) = ComputeMinMax();
            double padding = 10; //percent
            double totalHeight = (maxValue - minValue) * 100f / (100-padding*2);
            float maxStep = (float)(maxValue + totalHeight / padding);
            float minStep = (float)(minValue - totalHeight / padding);
            
            int numberOfSteps = 6;
            for (int i = 0; i <= numberOfSteps; ++i)
            {
                var y = Mathf.Lerp(rect.yMax, rect.yMin, (float)i/numberOfSteps);
                var label = new Label($"{Mathf.Lerp(minStep, maxStep, (float)i / numberOfSteps):0}");
                label.AddToClassList(ChartLabelClassname);
                Add(label);
                label.style.position = Position.Absolute;
                label.style.top = Mathf.RoundToInt(y - 20);
            }
        }

        private void OnGenerateVisualContent(MeshGenerationContext obj)
        {
            var painter = obj.painter2D;
            (double minValue, double maxValue) = ComputeMinMax();
            DrawSteps(painter, minValue, maxValue);
            DrawAxes(painter);
            DrawLines(painter);
        }

        private (double minValue, double maxValue) ComputeMinMax()
        {
            return (_dataSet.Min(set => set.Min(data => data.Value)), 
                _dataSet.Max(set => set.Max(data => data.Value)));
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
            double padding = _heightPadding; //percent
            double totalHeight = (maxValue - minValue) * 100f / (100-padding*2);
            float maxStep = (float)(maxValue + totalHeight / padding);
            float minStep = (float)(minValue - totalHeight / padding);

            painter.lineWidth = 2;
            painter.strokeColor = new Color(0, 0, 0, 0.2f);
            
            int numberOfSteps = 6;
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

        private void DrawLines(Painter2D painter)
        {
            var rect = this.ChartRect;
            
            Vector2 GetPoint(int i, ChartDataSet dataSet, double maxValue)
            {
                return new Vector2(
                    Mathf.Lerp(rect.xMin, rect.xMax, ((float)i) / (dataSet.Count - 1)),
                    Mathf.Lerp(rect.yMax - (rect.yMax / _heightPadding), rect.yMin + (rect.yMax / _heightPadding), (float)(dataSet[i].Value / maxValue)));
            }
            
            foreach (var dataSet in DataSet)
            {
                painter.strokeColor = dataSet.Color;
                painter.fillColor = dataSet.Color;
                painter.lineCap = LineCap.Round;
                painter.lineWidth = 5;
                
                var minValue = dataSet.Min(x => x.Value);
                var maxValue = dataSet.Max(x => x.Value);
                var previousPoint = GetPoint(0, dataSet, maxValue);
                var point = previousPoint;
                
                painter.BeginPath();
                painter.MoveTo(point);
                
                for (int i = 1; i < dataSet.Count; i++)
                {
                    previousPoint = point;
                    point = GetPoint(i, dataSet, maxValue);
                    
                    switch (SteppedLine)
                    { 
                        case SteppedLineType.After:
                            painter.LineTo(new Vector2(previousPoint.x, point.y));
                            painter.LineTo(point);
                            break;
                        case SteppedLineType.Before:
                            painter.LineTo(new Vector2(point.x, previousPoint.y));
                            painter.LineTo(point);
                            break;
                        case SteppedLineType.Middle:
                            var middle = (point.x + previousPoint.x) / 2;
                            painter.LineTo(new Vector2(middle, previousPoint.y));
                            painter.LineTo(new Vector2(middle, point.y));
                            painter.LineTo(point);
                            break;
                        case SteppedLineType.Curve:
                            var subPrevious = GetPoint(i - 2 < 0 ? i - 1 : i-2, dataSet, maxValue);
                            var previous = previousPoint;
                            var current = point;
                            var next = GetPoint(i == dataSet.Count - 1 ? i : i + 1, dataSet, maxValue);

                            var controlPoints1 = GetControlPoints(subPrevious, previous, current);
                            var controlPoints2 = GetControlPoints(previousPoint, current, next);
                            
                            painter.BezierCurveTo(controlPoints1.p2, controlPoints2.p1, point);
                            break;
                        default:
                            painter.LineTo(point);
                            break;
                    }
                }
                painter.Stroke();

                if (DisplayDots)
                {
                    for (int i = 0; i < dataSet.Count; i++)
                    {
                        painter.BeginPath();
                        painter.Arc(GetPoint(i, dataSet, maxValue), 5, 0, 360);
                        painter.Fill();
                    }
                }
            }
        }

        /// <summary>
        /// Following the scaled innovation website about splines, this method
        /// implements a calc to retrieve control points from 3 points on a bezier spline. <br/>
        /// http://scaledinnovation.com/analytics/splines/aboutSplines.html
        /// </summary>
        /// <param name="previous">The previous point on the curve.</param>
        /// <param name="current">The current point on the curve.</param>
        /// <param name="next">The next point on the curve.</param>
        /// <param name="tension">The slope tension factor between 0 and 1. Default: 0.4f</param>
        /// <returns></returns>
        private (Vector2 p1, Vector2 p2) GetControlPoints(Vector2 previous, Vector2 current, Vector2 next, float tension = 0.4f)
        {
            var d01=Math.Sqrt(Math.Pow(current.x-previous.x,2)+Math.Pow(current.y-next.y,2));
            var d12=Math.Sqrt(Math.Pow(next.x-current.x,2)+Math.Pow(next.y-current.y,2));
            var fa=tension * d01/(d01+d12);   // scaling factor for triangle Ta
            var fb=tension * d12/(d01+d12);   // ditto for Tb, simplifies to fb=t-fa
            var p1x=(float)(current.x-fa*(next.x-previous.x));    // x2-x0 is the width of triangle T
            var p1y=(float)(current.y-fa*(next.y-previous.y));    // y2-y0 is the height of T
            var p2x=(float)(current.x+fb*(next.x-previous.x));
            var p2y=(float)(current.y+fb*(next.y-previous.y));  
            return (new(p1x,p1y), new(p2x,p2y));
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
            
        }
    }
}