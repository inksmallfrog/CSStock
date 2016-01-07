using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace StockInfo
{
    public abstract class StockBase : IStock
    {
        public abstract void UpdateFiveMinsLine(Define.TTdxDllShareData data);
        public abstract void InitMinsData(Define.TTdxDllShareData data);
        public abstract void InitDayData(Define.TTdxDllShareData data);

        public abstract List<StockNode> DayLine
        {
            get;
        }

        public abstract List<StockNode> FiveMinsLine
        {
            get;
        }

        protected  string stkCode;
        public string StkCode{
            get{
                return stkCode;
            }
        }

        protected string stkName;
        public string StkName{
            get{
                return stkName;
            }
        }

        protected int market;

        protected int lastDealTime;
        public int LastDealTime{
            get{
                return lastDealTime;
            }
        }

        protected bool fiveMinsInit = false;
        public bool FiveMinsInit
        {
            get
            {
                return fiveMinsInit;
            }
        }

        protected bool dayInit = false;
        public bool DayInit
        {
            get
            {
                return dayInit;
            }
        }

        protected uint connection;

        protected StockNode currentMinsNode = new StockNode();
        public StockNode CurrentMinsNode
        {
            get
            {
                return currentMinsNode;
            }
        }
        protected StockNode currentDayNode = new StockNode();
        public StockNode CurrentDayNode
        {
            get
            {
                return currentDayNode;
            }
        }
       
        protected uint dealtMinsData = 0;

        public StockBase(IntPtr handle, string _stkCode, string _stkName)
        {
            stkCode = _stkCode;
            if ("000001" == stkCode || stkCode.StartsWith("6"))
            {
                market = 1;
            }
            else
            {
                market = 0;
            }
            stkName = _stkName;
            lastDealTime = 0;

            connection = R_Open(handle, null);
            Connect();
            InitCurrentNode();
            InitFiveMinsLine();
            InitDayLine();
        }
        ~StockBase()
        {
            R_DisConnect(connection);
            R_Close(connection);
        }

        public void Connect()
        {
            if (!R_Connect(connection, "218.18.103.38", 7709))
            {
                Console.WriteLine("连接失败！");
                Connect();
            }
        }

        protected void InitCurrentNode()
        {
            LoadPK();
        }
        public abstract void UpdateCurrentDayNode(Define.TTdxDllShareData data);
        protected void InitFiveMinsLine()
        {
            LoadMins();
        }

        protected void InitDayLine()
        {
            LoadDayLine();
        }

        public void LoadPK()
        {
            string marketStr = (market == 0) ? "SZ" : "SH";
            R_GetPK(connection, StkCode + marketStr, StkName);
        }

        public void LoadTestPK()
        {
            string marketStr = (market == 0) ? "SZ" : "SH";
            R_GetTestRealPK(connection, StkCode + marketStr, StkName, lastDealTime);
        }

        public void LoadDayLine()
        {
            R_GetKDays(connection, StkCode, market, 0, 580);
        }

        public void LoadMins()
        {
            R_GetMins(connection, StkCode, market, 0);
        }

        [DllImport("RSRStock.dll", EntryPoint = "R_Open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern uint R_Open(IntPtr Handle, string RegKey);

        [DllImport("RSRStock.dll", EntryPoint = "R_Close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_Close(uint TDXManager);

        [DllImport("RSRStock.dll", EntryPoint = "R_Connect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern bool R_Connect(uint TDXManager, string ServerAddr, int port);

        [DllImport("RSRStock.dll", EntryPoint = "R_DisConnect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_DisConnect(uint TDXManager);

        [DllImport("RSRStock.dll", EntryPoint = "R_InitMarketData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_InitMarketData(uint TDXManager, int Market);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetPK", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_GetPK(uint TDXManager, string StkCode, string StkName);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetTestRealPK", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_GetTestRealPK(uint TDXManager, string StkCode, string StkName, int Time);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetKDays", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_GetKDays(uint TDXManager, string StkCode, int market, int startcount, int count);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetDeals", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_GetDeals(uint TDXManager, string StkCode, int market, int startcount, int count);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetMins", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern void R_GetMins(uint TDXManager, string StkCode, int market, int start);

        [DllImport("RSRStock.dll", EntryPoint = "R_GetMarket", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int R_GetMarket(string StkCode, string StkName);
    }
}
