using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Charts
{
    [UxmlElement(libraryPath = "Leaframe/Charts")]
    public partial class PieChart : Chart
    {
        private const string PieChartClassname = "pie-chart";

        private static CustomStyleProperty<Color> _borderColorProperty =
            new("--pie-chart-border-color");

        private static CustomStyleProperty<int> _borderWidthProperty =
            new("--pie-chart-border-width");

        private static CustomStyleProperty<int> _borderInnerWidthProperty =
            new("--pie-chart-inner-border-width");

        private Color _borderColor;
        private int _borderWidth;
        private int _borderInnerWidth;

        public float Radius => Mathf.Min(contentRect.width - _borderWidth,
                                   contentRect.height - _borderWidth)
                               / 2;

        public event Action<ChartData> OnChartDataHovered;

        public PieChart()
        {
            AddToClassList(PieChartClassname);

            DataSet = new List<ChartDataSet>()
            {
                new ChartDataSet(new()
                {
                    new(240, "Primary", new Color32(17, 29, 111, 255)),
                    new(175, "Secondary", new Color32(0xFF, 0xA3, 0x78, 255)),
                    new(123, "Success", new Color32(0x4B, 0xCC, 0x76, 255)),
                    new(89, "Error", new Color32(0xCC, 0x3B, 0x37, 255)),
                    new(70, "Info", new Color32(0x21, 0x96, 0xFF, 255)),
                    new(37, "Warning", new Color32(0xF2, 0x8F, 0x16, 255)),
                })
            };

            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseOutEvent>(OnMouseExit);

            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
        {
            evt.customStyle.TryGetValue(_borderColorProperty, out _borderColor);
            evt.customStyle.TryGetValue(_borderWidthProperty, out _borderWidth);
            evt.customStyle.TryGetValue(_borderInnerWidthProperty, out _borderInnerWidth);
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var painter = context.painter2D;
            painter.strokeColor = _borderColor;
            painter.lineCap = LineCap.Round;
            painter.fillColor = Color.white;

            // float padding = (float)_borderWidth / 2;
            float radius = Radius;
            Vector2 center = contentRect.center;

            // drawing pie parts fills first
            float angle = 0.0f;
            float anglePct = 0.0f;
            double sum = DataSet[0].Sum(data => data.Value);
            foreach (var data in DataSet[0])
            {
                anglePct += 360.0f * (float) (data.Value / sum);

                painter.fillColor = data.Color;
                painter.BeginPath();
                painter.MoveTo(center);
                painter.Arc(center, radius, angle, anglePct);
                painter.Fill();
                painter.ClosePath();

                angle = anglePct;
            }

            if (_borderInnerWidth >= 0)
            {
                painter.lineWidth = _borderInnerWidth;
                angle = 0.0f;
                anglePct = 0.0f;
                foreach (var data in DataSet[0])
                {
                    anglePct += 360.0f * (float) (data.Value / sum);

                    painter.fillColor = data.Color;
                    painter.BeginPath();
                    painter.MoveTo(center);
                    painter.Arc(center, radius, angle, anglePct);
                    painter.Stroke();
                    painter.ClosePath();

                    angle = anglePct;
                }
            }

            if (_borderWidth < 0) return;

            painter.lineWidth = _borderWidth;
            painter.BeginPath();
            painter.MoveTo(center + Vector2.right * radius);
            painter.Arc(center, radius, 0, 360);
            painter.Stroke();
            painter.ClosePath();
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
            //throw new System.NotImplementedException();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            float radius = Radius;
            Vector2 center = contentRect.center;

            if (Vector2.Distance(evt.localMousePosition, center) > radius)
            {
                OnChartDataHovered?.Invoke(null);
                return;
            }

            float angle = 0.0f;
            float anglePct = 0.0f;
            double sum = DataSet[0].Sum(data => data.Value);
            foreach (var data in DataSet[0])
            {
                float dataAngle = 360.0f * (float) (data.Value / sum);
                anglePct += dataAngle;

                Vector2 mousePosition = Quaternion.Euler(0, 0, -angle) * (evt.localMousePosition - contentRect.center);

                float mouseAngle = -Vector2.SignedAngle(mousePosition, Vector2.right);
                if (mouseAngle < 0) mouseAngle = 360 - mouseAngle * -1;
                if (mouseAngle < dataAngle)
                {
                    OnChartDataHovered?.Invoke(data);
                    return;
                }

                angle = anglePct;
            }
        }

        private void OnMouseExit(MouseOutEvent evt)
        {
            OnChartDataHovered?.Invoke(null);
        }
    }
}