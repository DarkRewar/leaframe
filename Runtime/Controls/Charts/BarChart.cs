using System;
using System.Collections.Generic;
using System.Linq;
using Leaframe.Runtime.Events;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Leaframe.Charts
{
    [UxmlElement(libraryPath = "Leaframe/Charts")]
    public partial class BarChart : AxesChart
    {
        [UxmlAttribute]
        public bool SortData = true;

        protected VisualElement _barsContainer;
        protected VisualElement _horizontalLabelsContainer;

        private List<BarChartColumn> _chartColumns;

        private const string BarChartClassname = "bar-chart";
        private const string BarChartContainerClassname = "bar-chart-container";
        private const string BarChartLabelContainerClassname = "bar-chart-label-container";
        private const string BarChartColumnClassname = "bar-chart-column";
        private const string BarChartEntryClassname = "bar-chart-entry";
        private const string BarChartEntryLabelClassname = "bar-chart-entry__label";
        private const string BarChartEntryValueClassname = "bar-chart-entry__value";
        private const string BarChartHorizontalLabelClassname = "bar-chart__horizontal-label";
        private const string BarChartHorizontalLabelHoverClassname = "bar-chart__horizontal-label--hover";

        public BarChart()
        {
            AddToClassList(BarChartClassname);

            _barsContainer = new VisualElement();
            _barsContainer.AddToClassList(BarChartContainerClassname);
            Add(_barsContainer);

            _horizontalLabelsContainer = new VisualElement();
            _horizontalLabelsContainer.AddToClassList(BarChartLabelContainerClassname);
            Add(_horizontalLabelsContainer);

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
            RegisterCallback<CustomStyleResolvedEvent>(_ => DrawBars());
            DrawBars();

            //generateVisualContent += OnGenerateVisualContent;
            DisplayHorizontalLabels = false;
        }

        protected override (double minValue, double maxValue) ComputeMinMax()
        {
            if (_chartColumns is not { Count: > 0 })
                return (0, 0);
            var min = _chartColumns.Min(col => col.NegativeSum);
            var max = _chartColumns.Max(col => col.PositiveSum);
            if (OverrideMaximumStepValue)
                max = Mathf.Min(MaximumStepValue, (float) max);
            return (min, max);
        }

        private void DrawBars()
        {
            _barsContainer.Clear();
            if (_dataSet == null) return;

            float axeMidSize = _axeLineWidth / 2f;
            var chartRect = ChartRect;
            // _barsContainer.style.width = chartRect.width;
            // _barsContainer.style.height = chartRect.height;
            _barsContainer.style.marginLeft = chartRect.x;

            var barContentHeight = chartRect.height;
            var barContentPaddingBottom = contentRect.height - chartRect.height;
            _barsContainer.style.paddingBottom = barContentPaddingBottom - axeMidSize;
            // _barsContainer.style.paddingBottom = contentRect.height - ChartRect.height + AxeLineWidth / 2;

            _chartColumns = ComputeBarData();
            (var min, var max) = ComputeMinMax();
            var minMaxDelta = Mathf.Abs((float) min) + Mathf.Abs((float) max);
            float zeroHeight = min < 0 ? barContentHeight * Mathf.Abs((float) min) / minMaxDelta : 0;

            int i = 0;
            foreach (BarChartColumn chartColumn in _chartColumns)
            {
                ChartDataSet chartDataSet = DataSet[chartColumn.DataSetIndex];
                VisualElement column = new VisualElement();
                column.name = $"BarChartColumn-{i}";
                column.dataSource = chartColumn;
                column.AddToClassList(BarChartColumnClassname);
                _barsContainer.Add(column);

                float marginBottom = zeroHeight + barContentHeight * chartColumn.NegativeSum / minMaxDelta;
                column.style.marginBottom = marginBottom;
                column.RegisterCallback<MouseEnterEvent, int>(SetHoveredLabel, i, TrickleDown.TrickleDown);
                column.RegisterCallback<MouseOutEvent, int>(ClearHoverLabel, i, TrickleDown.TrickleDown);
                i++;

                foreach (ChartData entry in chartColumn.ChartData)
                {
                    var dataElement = new VisualElement();
                    dataElement.AddToClassList(BarChartEntryClassname);

                    float heightRatio = Mathf.Abs((float) entry.Value) / minMaxDelta;
                    dataElement.style.height = new StyleLength(Mathf.Lerp(0, barContentHeight, heightRatio));
                    dataElement.style.backgroundColor = chartDataSet.GetColor(entry);

                    var dataValue = new Label(entry.Value.ToString());
                    dataValue.AddToClassList(BarChartEntryValueClassname);
                    dataElement.Add(dataValue);

                    column.Add(dataElement);
                }
            }

            using (var barChartUpdatedEvent = BarChartUpdatedEvent.GetPooled())
            {
                barChartUpdatedEvent.target = this;
                this.SendEvent(barChartUpdatedEvent);
            }
        }

        private void SetHoveredLabel(MouseEnterEvent _, int labelIndex)
        {
            _horizontalLabelsContainer[labelIndex].AddToClassList(BarChartHorizontalLabelHoverClassname);
        }

        private void ClearHoverLabel(MouseOutEvent _, int labelIndex)
        {
            _horizontalLabelsContainer[labelIndex].RemoveFromClassList(BarChartHorizontalLabelHoverClassname);
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
                        barChartColumn = new BarChartColumn(i);
                        barChartColumns.Add(barChartColumn);
                    }
                    else
                    {
                        barChartColumn = barChartColumns[chartDataIndex];
                    }

                    dataSet[chartDataIndex].Color = dataSet.GetColor(dataSet[chartDataIndex]);
                    barChartColumn.ChartData.Add(dataSet[chartDataIndex]);
                }
            }

            foreach (BarChartColumn barChartColumn in barChartColumns)
            {
                barChartColumn.UpdateValues(SortData);
            }

            return barChartColumns;
        }

        protected override void OnCursorPositionChanged(Vector2 cursorPosition)
        {
            base.OnCursorPositionChanged(cursorPosition);
            MarkDirtyRepaint();
        }

        protected override void CreateHorizontalLabels(Rect rect)
        {
            if (!DisplayHorizontalLabels) return;

            _horizontalLabelsContainer.Clear();
            _horizontalLabelsContainer.style.marginLeft = ChartRect.x;

            int labelCount = Labels.Count;
            Length flexBasis = new Length(100f / labelCount, LengthUnit.Percent);
            for (int i = 0; i < labelCount; ++i)
            {
                var label = new Label(Labels[i]);
                label.AddToClassList(ChartLabelClassname);
                label.AddToClassList(HorizontalChartLabelClassname);
                label.AddToClassList(BarChartHorizontalLabelClassname);
                label.style.flexBasis = flexBasis;
                _horizontalLabelsContainer.Add(label);
            }

            // if (!DisplayHorizontalLabels) return;
            //
            // int labelCount = Labels.Count;
            // for (int i = 0; i < labelCount; ++i)
            // {
            //     var x = Mathf.Lerp(rect.xMin, rect.xMax, (float) i / (labelCount - 1));
            //     var label = new Label(Labels[i]);
            //     label.AddToClassList(ChartLabelClassname);
            //     label.AddToClassList(HorizontalChartLabelClassname);
            //     // _labelsContainer.Add(label);
            //     _barsContainer[i].Add(label);
            //     label.style.position = Position.Absolute;
            //     label.style.top = rect.yMax + 25;
            //     label.style.left = x;
            //
            //     label.schedule.Execute(_ =>
            //     {
            //         var labelRect = label.localBound;
            //         if (labelRect.xMin < rect.xMin)
            //             label.style.left = rect.xMin;
            //         else if (labelRect.xMax > rect.xMax)
            //             label.style.left = x - label.contentRect.width;
            //         else
            //             label.style.left = x - label.contentRect.width / 2;
            //
            //         // label.style.transformOrigin = new TransformOrigin(new Length(100, LengthUnit.Percent), new Length(50, LengthUnit.Percent));
            //         // label.style.rotate = new Rotate(-45);
            //     });
            // }
        }

        protected override void OnDataSetChanged(List<ChartDataSet> dataSet)
        {
            base.OnDataSetChanged(dataSet);
            DrawBars();
        }

        public class BarChartColumn
        {
            public readonly List<ChartData> ChartData = new();

            public int DataSetIndex { get; } = 0;

            public float PositiveSum { get; private set; } = 0;

            public float NegativeSum { get; private set; } = 0;

            public float Delta => Mathf.Abs(PositiveSum) + Mathf.Abs(NegativeSum);

            public BarChartColumn(int dataSetIndex) => DataSetIndex = dataSetIndex;

            public void UpdateValues(bool sortData = true)
            {
                NegativeSum = 0;
                foreach (ChartData chartData in ChartData)
                {
                    if (chartData.Value < 0) NegativeSum += (float) chartData.Value;
                    if (chartData.Value > 0) PositiveSum += (float) chartData.Value;
                }

                if (sortData)
                    ChartData.Sort((x, y) => -x.Value.CompareTo(y.Value));
            }
        }
    }
}