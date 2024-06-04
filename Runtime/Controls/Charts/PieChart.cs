using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class PieChart : Chart
    {
        #region TRAITS & FACTORY

        [Preserve]
        public new class UxmlFactory : UxmlFactory<PieChart, UxmlTraits>
        {
            public override string uxmlName => nameof(PieChart);

            public override string uxmlNamespace => "Leaframe.Charts";
        }

        [Preserve]
        public new class UxmlTraits : Chart.UxmlTraits
        {

        }

        #endregion

        private const string PieChartClassname = "pie-chart";

        public PieChart()
        {
            DataSet = new List<ChartDataSet>()
            {
                new ChartDataSet(new()
                {
                    new(240, "Primary", new Color32(17, 29, 111, 255)),
                    new(175, "Secondary", new Color32(0xFF, 0xA3, 0x78, 255)),
                    new(123, "Success", new Color32(0x4B, 0xCC, 0x76, 255)),
                    new(89, "Error", new Color32(0xCC, 0x3B, 0x37, 255)),
                    new(70, "Info", new Color32(0x21, 0x96, 0xFF,  255)),
                    new(37, "Warning", new Color32(0xF2, 0x8F, 0x16,  255)),
                })
            };

            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var painter = context.painter2D;
            painter.strokeColor = Color.white;
            painter.fillColor = Color.white;

            float radius = Mathf.Min(contentRect.width, contentRect.height) / 2;

            float angle = 0.0f;
            float anglePct = 0.0f;
            int k = 0;
            double sum = DataSet[0].Sum(data => data.Value);
            foreach (var data in DataSet[0])
            {
                anglePct += 360.0f * (float)(data.Value / sum);

                painter.fillColor = data.Color;
                painter.BeginPath();
                painter.MoveTo(new Vector2(radius, radius));
                painter.Arc(new Vector2(radius, radius), radius, angle, anglePct);
                painter.Fill();

                angle = anglePct;
                k++;
            }
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
            //throw new System.NotImplementedException();
        }
    }
}
