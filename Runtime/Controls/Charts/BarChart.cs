using System.Collections.Generic;
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

        protected VisualElement _barsContainer;

        private const string BarChartClassname = "bar-chart";
        private const string BarChartContainerClassname = "bar-chart-container";
        private const string BarChartEntryClassname = "bar-chart-entry";
        private const string BarChartEntryLabelClassname = "bar-chart-entry__label";
        private const string BarChartEntryValueClassname = "bar-chart-entry__value";

        public BarChart()
        {
            AddToClassList(BarChartClassname);

            _barsContainer = new VisualElement();
            _barsContainer.AddToClassList(BarChartContainerClassname);
            Add(_barsContainer);

            // This is a default dataset for UI Builder purpose.
            // Replace the DataSet value at runtime.
            DataSet = new List<ChartDataSet>()
            {
                new(new List<ChartData>()
                {
                    new(100, "January", Color),
                    new(1000, "February", Color),
                    new(1250, "March", Color),
                    new(678, "April", Color),
                    new(50, "May", Color),
                    new(80, "June", Color),
                    new(234, "July", Color),
                })
            };

            RegisterCallback<GeometryChangedEvent>(_ => DrawBars());
            DrawBars();

            //generateVisualContent += OnGenerateVisualContent;
            DisplayHorizontalLabels = false;
        }

        private void DrawBars()
        {
            _barsContainer.style.width = ChartRect.width;
            //_barsContainer.style.height = ChartRect.height - 30;
            _barsContainer.style.marginLeft = ChartRect.x;
            _barsContainer.style.paddingBottom = contentRect.height - ChartRect.height + AxeLineWidth / 2;

            _barsContainer.Clear();
            (var min, var max) = ComputeMinMaxSteps();

            foreach (var entry in DataSet[0])
            {
                var dataElement = new VisualElement();
                dataElement.AddToClassList(BarChartEntryClassname);
                dataElement.style.height = new StyleLength(Mathf.Lerp(0, ChartRect.height, (float)(entry.Value / max)));
                dataElement.style.backgroundColor = DataSet[0].GetColor(entry);
                _barsContainer.Add(dataElement);

                var dataLabel = new Label(entry.Id);
                dataLabel.AddToClassList(BarChartEntryLabelClassname);
                dataElement.Add(dataLabel);

                var dataValue = new Label(entry.Value.ToString());
                dataValue.AddToClassList(BarChartEntryValueClassname);
                dataElement.Add(dataValue);
            }
        }

        protected override void OnCursorPositionChanged(Vector2 cursorPosition)
        {
            base.OnCursorPositionChanged(cursorPosition);
            MarkDirtyRepaint();
        }
    }
}
