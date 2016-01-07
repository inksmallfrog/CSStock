using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using ImageOfStock;
using StockInfo;
using System.Drawing.Drawing2D;
using FormulaEditor;

namespace StockMonitor.Forms
{
    public partial class StockForm : BaseForm
    {
        IStock stock;
        string lineType;
        System.Timers.Timer updatePkTimer = null;
        System.Timers.Timer updateFiveMinsTimer = null;

        public StockForm(string stkCode, string stkName)
        {
            InitializeComponent();
            InitMainMenu();
            CandleGraph();
            stock = new StockDependsOnMemory(this.Handle, stkCode, stkName);
            stock.LoadPK();
            stock.LoadMins();
            lineType = "日线";
        }

        private ToolStripMenuItem tsmiNew = null;

        private ToolStripMenuItem tsmiClose = null;

        private ToolStripMenuItem tsmiDayLine = null;

        private ToolStripMenuItem tsmiFiveMinsLine = null;

        private ToolStripMenuItem tsmiFormulaEditor = null;

        public void InitMainMenu()
        {
            ToolStripDropDownButton tsddbFile = new ToolStripDropDownButton("操作(&F)");
            this.menuBar.Items.Add(tsddbFile);
            tsmiNew = new ToolStripMenuItem("新窗口(&N)");
            tsddbFile.DropDownItems.Add(tsmiNew);
            tsmiNew.Click += new EventHandler(tsmiNew_Click);
            tsmiClose = new ToolStripMenuItem("关闭(&E)");
            tsddbFile.DropDownItems.Add(tsmiClose);
            tsmiClose.Click += new EventHandler(tsmiClose_Click);

            ToolStripDropDownButton tsddbGragh = new ToolStripDropDownButton("趋势图(&G)");
            this.menuBar.Items.Add(tsddbGragh);
            tsmiDayLine = new ToolStripMenuItem("日K线(&D)");
            tsddbGragh.DropDownItems.Add(tsmiDayLine);
            tsmiDayLine.Click += new EventHandler(tsmiToDayLine);
            tsmiFiveMinsLine = new ToolStripMenuItem("5分钟线(&M)");
            tsddbGragh.DropDownItems.Add(tsmiFiveMinsLine);
            tsmiFiveMinsLine.Click += new EventHandler(tsmiToFiveMinsLine);

            ToolStripDropDownButton tsddbTools = new ToolStripDropDownButton("工具(&T)");
            this.menuBar.Items.Add(tsddbTools);
            tsmiFormulaEditor = new ToolStripMenuItem("公式编辑器(&D)");
            tsddbTools.DropDownItems.Add(tsmiFormulaEditor);
            tsmiFormulaEditor.Click += new EventHandler(tsmiOpenFormulaEditor);
        }

        private void tsmiOpenFormulaEditor(object sender, EventArgs e)
        {
            FormulaEdit form = new FormulaEdit(stock);
            form.Show();
        }

        private void tsmiToDayLine(object sender, EventArgs e)
        {
            lineType = "日线";
            CandleGraph();
            UpdateDataToGraph(stock.DayLine);
            this.chartGraph1.Refresh();
        }

        private void tsmiToFiveMinsLine(object sender, EventArgs e)
        {
            lineType = "5分钟";
            CandleGraph();
            UpdateDataToGraph(stock.FiveMinsLine);
            this.chartGraph1.Refresh();
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNew_Click(object sender, EventArgs e)
        {
            new NewForm().Show();
        }

        public void CandleGraph()
        {
            this.chartGraph1.ResetNullGraph();
            this.chartGraph1.UseScrollAddSpeed = true;
            this.chartGraph1.SetXScaleField("日期");
            this.chartGraph1.CanDragSeries = true;
            this.chartGraph1.SetSrollStep(10, 10);
            this.chartGraph1.ShowLeftScale = true;
            this.chartGraph1.ShowRightScale = true;
            this.chartGraph1.LeftPixSpace = 85;
            this.chartGraph1.RightPixSpace = 85;
            //K线图+BS点
            mainPanelID = this.chartGraph1.AddChartPanel(40);
            string candleName = "K线图-1";
            this.chartGraph1.AddCandle(candleName, "OPEN", "HIGH", "LOW", "CLOSE", mainPanelID, true);
            this.chartGraph1.YMBuySellSignal(mainPanelID, candleName, "BUYEMA", "(CLOSE+HIGH+LOW)/3", "SELLEMA", "BUYEMA");
            this.chartGraph1.AddBollingerBands("MID", "UP", "DOWN", "CLOSE", 20, 2, mainPanelID);
            this.chartGraph1.SetYScaleField(mainPanelID, new string[] { "HIGH","LOW"});
            //成交量
            volumePanelID = this.chartGraph1.AddChartPanel(20);
            this.chartGraph1.AddHistogram("VOL", "", candleName, volumePanelID);
            this.chartGraph1.SetHistogramStyle("VOL", Color.Red, Color.SkyBlue, 1, false);
            this.chartGraph1.AddSimpleMovingAverage("VOL-MA1", "MA5", "VOL", 5, volumePanelID);
            this.chartGraph1.SetTrendLineStyle("VOL-MA1", Color.White, Color.White, 1, DashStyle.Solid);
            this.chartGraph1.AddSimpleMovingAverage("VOL-MA2", "MA10", "VOL",10, volumePanelID);
            this.chartGraph1.SetTrendLineStyle("VOL-MA2", Color.Yellow, Color.Yellow, 1, DashStyle.Solid);
            this.chartGraph1.AddSimpleMovingAverage("VOL-MA3", "MA20", "VOL", 20, volumePanelID);
            this.chartGraph1.SetTrendLineStyle("VOL-MA3", Color.FromArgb(255, 0, 255), Color.FromArgb(255, 0, 255), 1, DashStyle.Solid);
            this.chartGraph1.SetTick(volumePanelID, 1);
            this.chartGraph1.SetDigit(volumePanelID, 0);
            kdjPanelID = this.chartGraph1.AddChartPanel(20);
            this.chartGraph1.AddStochasticOscillator("K", "D", "J", 9, "CLOSE", "HIGH", "LOW", kdjPanelID);
            macdPanelID = this.chartGraph1.AddChartPanel(20);
            this.chartGraph1.AddMacd("MACD", "DIFF", "DEA", "CLOSE", 26, 12, 9, macdPanelID);
        }

        int mainPanelID = -1;
        int volumePanelID = -1;
        int kdjPanelID = -1;
        int macdPanelID = -1;

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomForm_Load(object sender, EventArgs e)
        {
            this.Text = "STOCK MONITOR";
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height);
            CandleGraph();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateProcessBar(object obj)
        {
            int[] values = obj as int[];
            int total = values[0];
            int current = values[1];
            int processValue = Convert.ToInt32((double)current / (double)total * 100);
            if (processValue > this.chartGraph1.ProcessBarValue)
            {
                this.chartGraph1.ProcessBarValue = processValue;
            }
            if (current == total - 1)
            {
                this.chartGraph1.ProcessBarValue = 100;
                this.chartGraph1.RefreshGraph();
            }
        }

        private delegate void UpdateProcessBarDelegate(object obj);

        /// <summary>
        /// 更新数据到图像
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateOneDataToGraph(object obj)
        {
            StockNode node = (StockNode)obj;
            string timeKey = node.time;
            int year = 1970;
            int month = 1;
            int day = 1;
            int hour = 0;
            int minute = 0;
            switch (lineType)
            {
                case "5分钟":
                case "15分钟":
                case "30分钟":
                case "60分钟":
                    year = Convert.ToInt32(timeKey.Substring(0, 4));
                    month = Convert.ToInt32(timeKey.Substring(4, 2));
                    day = Convert.ToInt32(timeKey.Substring(6, 2));
                    hour = Convert.ToInt32(timeKey.Substring(8, 2));
                    minute = Convert.ToInt32(timeKey.Substring(10, 2));
                    break;
                case "日线":
                case "周线":
                    year = Convert.ToInt32(timeKey.Substring(0, 4));
                    month = Convert.ToInt32(timeKey.Substring(4, 2));
                    day = Convert.ToInt32(timeKey.Substring(6, 2));
                    break;
                case "月线":
                    year = Convert.ToInt32(timeKey.Substring(0, 4));
                    month = Convert.ToInt32(timeKey.Substring(4, 2));
                    break;
            }
            DateTime dt = new DateTime(year, month, day, hour, minute, 0);
            this.chartGraph1.SetValue("OPEN", node.open, dt);
            this.chartGraph1.SetValue("HIGH", node.high, dt);
            this.chartGraph1.SetValue("LOW", node.low, dt);
            this.chartGraph1.SetValue("CLOSE", node.close, dt);
            this.chartGraph1.SetValue("VOL", node.volumn, dt);
            double ymValue = (Convert.ToDouble(node.close) + Convert.ToDouble(node.high) + Convert.ToDouble(node.low)) / 3;
            this.chartGraph1.SetValue("(CLOSE+HIGH+LOW)/3",ymValue,dt);
        }
        public void UpdateDataToGraph(object obj)
        {
            List<StockNode> list = obj as List<StockNode>;
            this.chartGraph1.SetTitle(mainPanelID, stock.StkName + "(" + stock.StkCode + ") " + lineType);
            this.chartGraph1.SetTitle(kdjPanelID, "KDJ(9,3,3)");
            this.chartGraph1.SetTitle(volumePanelID, "VOL(5,10,20)");
            this.chartGraph1.SetTitle(macdPanelID, "MACD(12,26,9)");
            switch (lineType)
            {
                case "5分钟":
                case "15分钟":
                case "30分钟":
                case "60分钟":
                    this.chartGraph1.SetIntervalType(mainPanelID, ChartGraph.IntervalType.Minute);
                    this.chartGraph1.SetIntervalType(volumePanelID, ChartGraph.IntervalType.Minute);
                    this.chartGraph1.SetIntervalType(kdjPanelID, ChartGraph.IntervalType.Minute);
                    break;
                case "日线":
                    this.chartGraph1.SetIntervalType(mainPanelID, ChartGraph.IntervalType.Day);
                    this.chartGraph1.SetIntervalType(volumePanelID, ChartGraph.IntervalType.Day);
                    this.chartGraph1.SetIntervalType(kdjPanelID, ChartGraph.IntervalType.Day);
                    break;
                case "周线":
                    this.chartGraph1.SetIntervalType(mainPanelID, ChartGraph.IntervalType.Week);
                    this.chartGraph1.SetIntervalType(volumePanelID, ChartGraph.IntervalType.Week);
                    this.chartGraph1.SetIntervalType(kdjPanelID, ChartGraph.IntervalType.Week);
                    break;
                case "月线":
                    this.chartGraph1.SetIntervalType(mainPanelID, ChartGraph.IntervalType.Month);
                    this.chartGraph1.SetIntervalType(volumePanelID, ChartGraph.IntervalType.Month);
                    this.chartGraph1.SetIntervalType(kdjPanelID, ChartGraph.IntervalType.Month);
                    break;
            }
            this.chartGraph1.RefreshGraph();
            for (int i = 0; i < list.Count; ++i)
            {
                string timeKey = list[i].time;
                int year = 1970;
                int month = 1;
                int day = 1;
                int hour = 0;
                int minute = 0;
                switch (lineType)
                {
                    case "5分钟":
                    case "15分钟":
                    case "30分钟":
                    case "60分钟":
                        year = Convert.ToInt32(timeKey.Substring(0, 4));
                        month = Convert.ToInt32(timeKey.Substring(4, 2));
                        day = Convert.ToInt32(timeKey.Substring(6, 2));
                        hour = Convert.ToInt32(timeKey.Substring(8, 2));
                        minute = Convert.ToInt32(timeKey.Substring(10, 2));
                        break;
                    case "日线":
                    case "周线":
                        year = Convert.ToInt32(timeKey.Substring(0, 4));
                        month = Convert.ToInt32(timeKey.Substring(4, 2));
                        day = Convert.ToInt32(timeKey.Substring(6, 2));
                        break;
                    case "月线":
                        year = Convert.ToInt32(timeKey.Substring(0, 4));
                        month = Convert.ToInt32(timeKey.Substring(4, 2));
                        break;
                }
                DateTime dt = new DateTime(year, month, day, hour, minute, 0);
                this.chartGraph1.SetValue("OPEN", list[i].open, dt);
                this.chartGraph1.SetValue("HIGH", list[i].high, dt);
                this.chartGraph1.SetValue("LOW", list[i].low, dt);
                this.chartGraph1.SetValue("CLOSE", list[i].close, dt);
                this.chartGraph1.SetValue("VOL", list[i].volumn, dt);
                double ymValue = (Convert.ToDouble(list[i].close) + Convert.ToDouble(list[i].high) + Convert.ToDouble(list[i].low)) / 3;
                this.chartGraph1.SetValue("(CLOSE+HIGH+LOW)/3", ymValue, dt);
                this.BeginInvoke(new UpdateProcessBarDelegate(UpdateProcessBar), new int[] { list.Count, i });
            }
            this.chartGraph1.Enabled = true;
        }

        private void UpdatePk(object sender, System.Timers.ElapsedEventArgs e)
        {
            stock.LoadTestPK();
        }
        private void UpdateFiveMinsLine(object sender, System.Timers.ElapsedEventArgs e)
        {
            stock.LoadMins();
            stock.LoadTestPK();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Define.WM_TDX_DEPACKDATA - 1)
            {
                if (4 == m.WParam.ToInt32())
                {
                }
            }
            if (m.Msg == Define.WM_TDX_DEPACKDATA)
            {
                if (m.WParam.ToInt32() == Define.TDX_MSG_GETPK)
                {
                    Define.TTdxDllShareData data = new Define.TTdxDllShareData();
                    data = (Define.TTdxDllShareData)m.GetLParam(data.GetType());
                    stock.UpdateCurrentDayNode(data);

                    if (lineType == "日线")
                    {
                        UpdateOneDataToGraph(stock.CurrentDayNode);
                        this.chartGraph1.RefreshGraph();
                    }

                    System.Timers.Timer updatePkTimer = new System.Timers.Timer();
                    updatePkTimer.Enabled = true;
                    updatePkTimer.Interval = 2000;
                    updatePkTimer.Start();
                    updatePkTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdatePk);
                }
                else if (m.WParam.ToInt32() == Define.TDX_MSG_TESTREALPK)
                {
                    Define.TTdxDllShareData data = new Define.TTdxDllShareData();
                    data = (Define.TTdxDllShareData)m.GetLParam(data.GetType());
                    stock.UpdateCurrentDayNode(data);

                    if (lineType == "日线")
                    {
                        UpdateOneDataToGraph(stock.CurrentDayNode);
                        this.chartGraph1.RefreshGraph();
                    }

                    if (updatePkTimer == null)
                    {
                        updatePkTimer = new System.Timers.Timer();
                        updatePkTimer.Enabled = true;
                        updatePkTimer.Interval = 2000;
                        updatePkTimer.Start();
                        updatePkTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdatePk);
                    }

                }
                else if (m.WParam.ToInt32() == Define.TDX_MSG_GET_K_DAY)
                {
                    Define.TTdxDllShareData data = new Define.TTdxDllShareData();
                    data = (Define.TTdxDllShareData)m.GetLParam(data.GetType());
                    if (!stock.DayInit)
                    {
                        stock.InitDayData(data);
                    }
                    
                    if (lineType == "日线")
                    {
                        UpdateDataToGraph(stock.DayLine);
                        this.chartGraph1.RefreshGraph();
                    }
                }
                else if (m.WParam.ToInt32() == Define.TDX_MSG_GET_MINS)
                {
                    Define.TTdxDllShareData data = new Define.TTdxDllShareData();
                    data = (Define.TTdxDllShareData)m.GetLParam(data.GetType());
                    if (!stock.FiveMinsInit)
                    {
                        stock.InitMinsData(data);
                        if (lineType == "5分钟")
                        {
                            UpdateDataToGraph(stock.FiveMinsLine);
                            this.chartGraph1.RefreshGraph();
                        }
                    }
                    else
                    {
                        stock.UpdateFiveMinsLine(data);
                        if (lineType == "5分钟")
                        {
                            UpdateOneDataToGraph(stock.CurrentMinsNode);
                            this.chartGraph1.RefreshGraph();
                        }
                    }

                    if (updateFiveMinsTimer == null)
                    {
                        updateFiveMinsTimer = new System.Timers.Timer();
                        updateFiveMinsTimer.Enabled = true;
                        updateFiveMinsTimer.Interval = 10000;
                        updateFiveMinsTimer.Start();
                        updateFiveMinsTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateFiveMinsLine);
                    }
                   
                }
            }
            base.WndProc(ref m);
        }
    }
}