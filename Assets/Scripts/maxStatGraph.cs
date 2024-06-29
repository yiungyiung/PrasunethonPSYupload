using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class maxStatGraph : MonoBehaviour
    {
        public GameObject mainCamera;
        public GameObject nameit;
        readdata readDataScript;
        readdata selectedJoint;
        readdata jointTimestamps;
        readdata jointmaxmin;

        private (float x, float y)[] dataPoints;
        void Awake()
        {
            readDataScript = mainCamera.GetComponent<readdata>();
            AddData();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //UpdateDataPoints();
                AddData();
            }
        }

        //enable diable the graph connected to the button. 
        public void whenButtonClicked()
        {
            if (nameit.activeSelf == true)
            {
                nameit.SetActive(false);
            }
            else
            {
                nameit.SetActive(true);
                AddData();
            }
        }

        public void AddData()
        {
            List<int> tandata = readDataScript.jointmaxmin[readDataScript.selectedJoint];

            //data points list
            //List<int> tandata = readDataScript.MaxminValues;
            for (int i = 0; i < tandata.Count; i++)
            {
                tandata[i] = Mathf.Abs(tandata[i]);
            }

            //list of dates 
            List<string> tanXaxis = readDataScript.jointTimestamps[readDataScript.selectedJoint];

            //linechart settings
            var chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
            }
            chart.EnsureChartComponent<Title>().show = true;
            chart.EnsureChartComponent<Title>().text = "Max ROM Graph";

            chart.EnsureChartComponent<Tooltip>().show = true;
            chart.EnsureChartComponent<Legend>().show = false;

            var xAxis = chart.EnsureChartComponent<XAxis>();
            var yAxis = chart.EnsureChartComponent<YAxis>();
            xAxis.show = true;
            yAxis.show = true;
            xAxis.type = Axis.AxisType.Category;
            yAxis.type = Axis.AxisType.Value;

            xAxis.splitLine.show = false;
            yAxis.splitLine.show = false;

            xAxis.splitNumber = 10;
            xAxis.boundaryGap = true;

            chart.RemoveData();
            var lineSeries = chart.AddSerie<Line>("LineSeries");
            lineSeries.lineType = LineType.Smooth;
            lineSeries.lineStyle.color = Color.white;
            lineSeries.symbol.size = 15;
            var textColor = Color.white;
            xAxis.axisLabel.textStyle.color = textColor;
            yAxis.axisLabel.textStyle.color = textColor;
            //adding points
            for (int i = 0; i < tandata.Count; i++)
            {
                chart.AddXAxisData(tanXaxis[i]);
                chart.AddData("LineSeries", tandata[i]);
            }
        }
    }
}
