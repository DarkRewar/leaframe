using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class LineChart : AxesChart
    {
        #region TRAITS & FACTORY

        [Preserve]
        public new class UxmlFactory : UxmlFactory<LineChart, UxmlTraits>
        {
            public override string uxmlName => nameof(LineChart);

            public override string uxmlNamespace => "Leaframe.Charts";
        }

        [Preserve]
        public new class UxmlTraits : AxesChart.UxmlTraits
        {
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

        private const string LineChartClassname = "line-chart";

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
        }

        private void OnGenerateVisualContent(MeshGenerationContext obj)
        {
            var painter = obj.painter2D;
            DrawLines(painter);
        }

        private void DrawLines(Painter2D painter)
        {
            var rect = this.ChartRect;
            
            (int minStep, int maxStep) = ComputeMinMaxSteps();

            Vector2 GetPoint(int i, ChartDataSet dataSet)
            {
                var y = (dataSet[i].Value + Math.Abs(minStep)) / (maxStep + Math.Abs(minStep));
                return new Vector2(
                    Mathf.Lerp(rect.xMin, rect.xMax, ((float)i) / (dataSet.Count - 1)),
                    Mathf.Lerp(rect.yMax, rect.yMin, (float)y));
            }
            
            foreach (var dataSet in DataSet)
            {
                painter.strokeColor = dataSet.Color;
                painter.fillColor = dataSet.Color;
                painter.lineCap = LineCap.Round;
                painter.lineWidth = 5;
                
                // var minValue = dataSet.Min(x => x.Value);
                // var maxValue = dataSet.Max(x => x.Value);
                var previousPoint = GetPoint(0, dataSet);
                var point = previousPoint;
                
                painter.BeginPath();
                painter.MoveTo(point);
                
                for (int i = 1; i < dataSet.Count; i++)
                {
                    previousPoint = point;
                    point = GetPoint(i, dataSet);
                    
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
                            var subPrevious = GetPoint(i - 2 < 0 ? i - 1 : i-2, dataSet);
                            var previous = previousPoint;
                            var current = point;
                            var next = GetPoint(i == dataSet.Count - 1 ? i : i + 1, dataSet);

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
                        painter.Arc(GetPoint(i, dataSet), 5, 0, 360);
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