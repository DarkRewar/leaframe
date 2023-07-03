using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Controls.Charts
{
    public class ChartDataSet : List<ChartData>
    {
        public Color Color;

        public ChartDataSet(List<ChartData> chartDataList)
            : this(chartDataList, Random.ColorHSV()){}

        public ChartDataSet(List<ChartData> chartDataList, Color color)
        {
            Color = color;
            this.AddRange(chartDataList);
        }
    }
    
    public struct ChartData
    {
        public readonly double Value;
        public readonly string Id;

        public ChartData(double value)
        {
            Value = value;
            Id = string.Empty;
        }
    }
    
    public abstract class Chart : VisualElement
    {
        protected const string ChartClassname = "chart";

        private List<ChartDataSet> _dataSet;

        public List<ChartDataSet> DataSet
        {
            get => _dataSet;
            set
            {
                _dataSet = value;
                OnDataSetChanged(_dataSet);
            }
        }

        protected Chart()
        {
            AddToClassList(ChartClassname);
        }

        protected abstract void OnDataSetChanged(List<ChartDataSet> dataSet);
    }
}
