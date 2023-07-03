using System.Collections.Generic;
using System.Linq;
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
            public override string uxmlName => "LineChart";

            public override string uxmlNamespace => "Leaframe.Charts";
        }

        [Preserve]
        public new class UxmlTraits : Chart.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _curveLine = new()
            {
                name = "curve-line"
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not LineChart lineChart) return;

                lineChart.CurveLine = _curveLine.GetValueFromBag(bag, cc);
            }
        }

        #endregion

        public bool CurveLine { get; private set; } = false;

        private const string LineChartClassname = "line-chart";

        public LineChart()
        {
            AddToClassList(LineChartClassname);

            DataSet = new List<ChartDataSet>()
            {
                new ChartDataSet(new List<ChartData>()
                {
                    new ChartData(100),
                    new ChartData(1000),
                    new ChartData(1250),
                    new ChartData(678),
                    new ChartData(50),
                    new ChartData(80),
                    new ChartData(234),
                })
            };
            
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext obj)
        {
            var painter = obj.painter2D;
            DrawAxes(painter);
            DrawLines(painter);
        }

        private void DrawAxes(Painter2D painter)
        {
            painter.strokeColor = Color.black;
            painter.lineWidth = 5;
            var rect = this.contentRect;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.Stroke();
        }

        private void DrawLines(Painter2D painter)
        {
            var rect = this.contentRect;
            
            Vector2 GetPoint(int i, ChartDataSet dataSet, double maxValue)
            {
                return new Vector2(
                    Mathf.Lerp(rect.xMin, rect.xMax, ((float)i) / (dataSet.Count - 1)),
                    Mathf.Lerp(rect.yMax, rect.yMin, (float)(dataSet[i].Value / maxValue)));
            }
            
            foreach (var dataSet in DataSet)
            {
                painter.strokeColor = dataSet.Color;
                painter.fillColor = dataSet.Color;
                painter.lineCap = LineCap.Round;
                painter.lineWidth = 5;
                
                painter.BeginPath();
                var minValue = dataSet.Min(x => x.Value);
                var maxValue = dataSet.Max(x => x.Value);
                painter.MoveTo(new Vector2(rect.xMin, Mathf.Lerp(rect.yMax, rect.yMin, (float)(dataSet[0].Value/maxValue))));
                for (int i = 1; i < dataSet.Count; i++)
                {
                    if (CurveLine)
                    {
                        // painter.BezierCurveTo(
                        //     GetPoint(i - 1, dataSet, maxValue),
                        //     GetPoint(i == dataSet.Count - 1 ? i : i + 1, dataSet, maxValue),
                        //     GetPoint(i, dataSet, maxValue));
                        var p1 = GetPoint(i - 1, dataSet, maxValue);
                        var p3 = GetPoint(i, dataSet, maxValue);
                        var p2 = p3;
                        painter.QuadraticCurveTo(p3 - new Vector2(50, 0), p3);
                    }
                    else
                    {
                        painter.LineTo(GetPoint(i, dataSet, maxValue));
                    }
                }
                painter.Stroke();

                for (int i = 0; i < dataSet.Count; i++)
                {
                    painter.BeginPath();
                    painter.Arc(GetPoint(i, dataSet, maxValue), 10, 0, 360);
                    painter.Fill();
                }
            }
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
            //throw new System.NotImplementedException();
        }
    }
}