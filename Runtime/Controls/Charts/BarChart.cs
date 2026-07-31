using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Charts
{
    [UxmlElement(libraryPath = "Leaframe/Charts")]
    public partial class BarChart : AxesChart
    {
        protected VisualElement _barsContainer;

        private const string BarChartClassname = "bar-chart";
        private const string BarChartContainerClassname = "bar-chart-container";
        private const string BarChartColumnClassname = "bar-chart-column";
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
            RegisterCallback<AttachToPanelEvent>(_ => DrawBars());
            DrawBars();

            //generateVisualContent += OnGenerateVisualContent;
            DisplayHorizontalLabels = false;
        }

        private void DrawBars()
        {
            float axeMidSize = AxeLineWidth / 2f;
            var chartRect = ChartRect;
            // _barsContainer.style.width = chartRect.width;
            // _barsContainer.style.height = chartRect.height;
            _barsContainer.style.marginLeft = chartRect.x;

            var barContentHeight = chartRect.height;
            var barContentPaddingBottom = contentRect.height - chartRect.height;
            _barsContainer.style.paddingBottom = barContentPaddingBottom;
            // _barsContainer.style.paddingBottom = contentRect.height - ChartRect.height + AxeLineWidth / 2;

            _barsContainer.Clear();
            (var min, var max) = ComputeMinMax();
            var minMaxDelta = Mathf.Abs((float) min) + Mathf.Abs((float) max);
            float zeroHeight = min < 0 ? barContentHeight * Mathf.Abs((float) min) / minMaxDelta : 0;

            var chartColumns = ComputeBarData();

            foreach (var chartColumn in chartColumns)
            {
                VisualElement column = new VisualElement();
                column.dataSource = chartColumn;
                column.AddToClassList(BarChartColumnClassname);
                _barsContainer.Add(column);

                float marginBottom = zeroHeight + barContentHeight * chartColumn.NegativeSum / minMaxDelta;
                column.style.marginBottom = marginBottom;

                foreach (ChartData entry in chartColumn.ChartData)
                {
                    var dataElement = new VisualElement();
                    dataElement.AddToClassList(BarChartEntryClassname);

                    float heightRatio = Mathf.Abs((float) entry.Value) / minMaxDelta;
                    dataElement.style.height = new StyleLength(Mathf.Lerp(0, barContentHeight, heightRatio));
                    // dataElement.style.marginBottom = new Length(zeroHeight, LengthUnit.Percent);
                    // dataElement.style.marginBottom = zeroHeight;
                    dataElement.style.backgroundColor = DataSet[0].GetColor(entry);
                    if (entry.Value < 0)
                    {
                        // dataElement.style.paddingTop = axeMidSize;
                        // dataElement.style.translate = new Translate(0, new Length(100, LengthUnit.Percent));
                    }
                    else
                    {
                        // dataElement.style.paddingBottom = axeMidSize;
                    }

                    var dataValue = new Label(entry.Value.ToString());
                    dataValue.AddToClassList(BarChartEntryValueClassname);
                    dataElement.Add(dataValue);

                    column.Add(dataElement);
                }
            }
        }

        private List<BarChartColumn> ComputeBarData()
        {
            List<BarChartColumn> barChartColumns = new List<BarChartColumn>();

            for (int i = 0; i < DataSet.Count; i++)
            {
                var dataSet = DataSet[i];
                for (int chartDataIndex = 0; chartDataIndex < dataSet.Count; chartDataIndex++)
                {
                    BarChartColumn barChartColumn;

                    if (barChartColumns.Count <= chartDataIndex)
                    {
                        barChartColumn = new BarChartColumn();
                        barChartColumns.Add(barChartColumn);
                    }
                    else
                    {
                        barChartColumn = barChartColumns[chartDataIndex];
                    }

                    barChartColumn.ChartData.Add(dataSet[chartDataIndex]);
                }
            }

            foreach (BarChartColumn barChartColumn in barChartColumns)
            {
                barChartColumn.UpdateValues();
            }

            return barChartColumns;
        }

        protected override void OnCursorPositionChanged(Vector2 cursorPosition)
        {
            base.OnCursorPositionChanged(cursorPosition);
            MarkDirtyRepaint();
        }

        public class BarChartColumn
        {
            // public VisualElement Column;
            public readonly List<ChartData> ChartData = new();

            public float MaxValue { get; private set; } = 0;
            public float MinValue { get; private set; } = 0;

            public float NegativeSum { get; private set; } = 0;

            public float Delta => Mathf.Abs(MaxValue) + Mathf.Abs(MinValue);

            public void UpdateValues()
            {
                NegativeSum = 0;
                foreach (ChartData chartData in ChartData)
                {
                    MaxValue = Mathf.Max(MaxValue, (float) chartData.Value);
                    MinValue = Mathf.Min(MaxValue, (float) chartData.Value);
                    if (chartData.Value < 0) NegativeSum += (float) chartData.Value;
                }

                ChartData.Sort((x, y) => -x.Value.CompareTo(y.Value));
            }
        }
    }
}