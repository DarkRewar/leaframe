using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class BarChart : AxesChart
    {
        #region TRAITS & FACTORY

        [Preserve]
        public new class UxmlFactory : UxmlFactory<BarChart, UxmlTraits>
        {
            public override string uxmlName => nameof(BarChart);

            public override string uxmlNamespace => "Leaframe.Charts";
        }

        [Preserve]
        public new class UxmlTraits : AxesChart.UxmlTraits
        {

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not BarChart barChart) return;
            }
        }

        #endregion

        private const string BarChartClassname = "bar-chart";

        public BarChart()
        {
            AddToClassList(BarChartClassname);

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
                })
            };

            //generateVisualContent += OnGenerateVisualContent;
        }

        /*private void OnGenerateVisualContent(MeshGenerationContext obj)
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
                            var subPrevious = GetPoint(i - 2 < 0 ? i - 1 : i - 2, dataSet);
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
                    int dotRadius = 20;

                    for (int i = 0; i < dataSet.Count; i++)
                    {
                        var valuePoint = GetPoint(i, dataSet);
                        var radius = Vector2.Distance(CursorPosition, valuePoint) < dotRadius ? 10 : 5;
                        painter.BeginPath();
                        painter.Arc(valuePoint, radius, 0, 360);
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
            var d01 = Math.Sqrt(Math.Pow(current.x - previous.x, 2) + Math.Pow(current.y - next.y, 2));
            var d12 = Math.Sqrt(Math.Pow(next.x - current.x, 2) + Math.Pow(next.y - current.y, 2));
            var fa = tension * d01 / (d01 + d12);   // scaling factor for triangle Ta
            var fb = tension * d12 / (d01 + d12);   // ditto for Tb, simplifies to fb=t-fa
            var p1x = (float)(current.x - fa * (next.x - previous.x));    // x2-x0 is the width of triangle T
            var p1y = (float)(current.y - fa * (next.y - previous.y));    // y2-y0 is the height of T
            var p2x = (float)(current.x + fb * (next.x - previous.x));
            var p2y = (float)(current.y + fb * (next.y - previous.y));
            return (new(p1x, p1y), new(p2x, p2y));
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {

        }
*/
        protected override void OnCursorPositionChanged(Vector2 cursorPosition)
        {
            base.OnCursorPositionChanged(cursorPosition);
            MarkDirtyRepaint();
        }
    }
}