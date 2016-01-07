using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
/*using TDXGrobal;

namespace DataReader
{    
    public class StockInfo
    {
        uint connection;
        string stkCode;
        string stkName;

        int lastDealTime;

        DataNode currentData;
        List<DataNode> fiveMinsLine;
        List<DataNode> dayLine;

        public StockInfo(IntPtr Handle, string _stkCode, string _stkName)
        {
            connection = R_Open(Handle, null);
            stkCode = _stkCode;
            stkName = _stkName;
            Connect();
        }

        ~StockInfo()
        {
            R_Close(connection);
        }

        private void Connect()
        {
            if (!R_Connect(connection, "218.18.103.38", 7709))
            {
                Console.WriteLine("连接失败！");
                Connect();
            }
        }

        public string GetStkCode()
        {
            return stkCode;
        }

        public string GetStkName()
        {
            return stkName;
        }

        public float GetLastDealTime()
        {
            return lastDealTime;
        }

        public void WritePKToFile(Define.TTdxDllShareData data)
        {
            Define.TTDX_PKBASE pkData = new Define.TTDX_PKBASE();
            StringBuilder pkStr = new StringBuilder("");
            for (int pos = 0, i = 0; i < data.count; pos += Marshal.SizeOf(pkData), ++i)
            {
                pkData = (Define.TTDX_PKBASE)Utility.BytesToStuct(data.buf, pkData.GetType(), pos);

                pkStr.Append(DateTime.Now.ToString("MMddhhmm") + "\t" + pkData.Open.ToString() +
                               "\t" + pkData.High.ToString() + "\t" + pkData.Low.ToString() +
                               "\t" + pkData.Close.ToString() + "\t" + Convert.ToDecimal(Convert.ToDouble(pkData.Amount.ToString())) +
                               "\t" + pkData.Volume.ToString() + "\n");

                High = pkData.High;
                Open = pkData.Open;
                Low = pkData.Low;
                Close = pkData.Close;

                lastDealTime = (int)pkData.LastDealTime;
            }

            FileStream fs = null;
            string filePath = stkCode + "PK.txt";
            Encoding encoder = Encoding.UTF8;
            byte[] bytes = encoder.GetBytes(pkStr.ToString());
            try
            {
                fs = File.OpenWrite(filePath);
                fs.Position = fs.Length;
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件打开失败{0}", ex.ToString());
            }
            finally
            {
                fs.Close();
            }
        }

        public void WriteMinsToFile(Define.TTdxDllShareData data)
        {
            Define.TTDX_MIN minsData = new Define.TTDX_MIN();
            StringBuilder minsStr = new StringBuilder("");
            int initHour = 9;
            int initMin = 30;
            int min;

            for (int pos = 0, i = 0; i < data.count; pos += Marshal.SizeOf(minsData), ++i)
            {
                min = initMin + i;
                
                minsData = (Define.TTDX_MIN)Utility.BytesToStuct(data.buf, minsData.GetType(), pos);
                string time = string.Format("{0:D2}{1:D2}", min / 60 + initHour, min % 60);
                minsStr.Append(DateTime.Now.ToString("MMdd") + time + "\t" + Open.ToString() +
                               "\t" + High.ToString() + "\t" + Low.ToString() +
                               "\t" + minsData.Close.ToString() + "\t" + 0 +
                               "\t" + minsData.Volume.ToString() + "\n");

                if (11 == min / 60 + initHour && 30 == min % 60)
                {
                    initMin = -121;
                    initHour = 1;
                }
            }

            FileStream fs = null;
            string filePath = stkCode + "PK.txt";
            Encoding encoder = Encoding.UTF8;
            byte[] bytes = encoder.GetBytes(minsStr.ToString());
            try
            {
                fs = File.OpenWrite(filePath);
                fs.Position = fs.Length;
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件打开失败{0}", ex.ToString());
            }
            finally
            {
                fs.Close();
            }
        }

        public void WriteDayInfoToFile(Define.TTdxDllShareData data)
        {
            Define.TTDX_DAYInfo dayInfo = new Define.TTDX_DAYInfo();
            FileStream fs = null;

            StringBuilder dayInfoStr = new StringBuilder("");

            for (int pos = 0, i = 0; i < data.count; pos += Marshal.SizeOf(dayInfo), ++i)
            {
                dayInfo = (Define.TTDX_DAYInfo)Utility.BytesToStuct(data.buf, dayInfo.GetType(), pos);

                dayInfoStr.Append(dayInfo.DAY.ToString() + "\t" + dayInfo.Open.ToString() + 
                               "\t" + dayInfo.High.ToString() + "\t" + dayInfo.Low.ToString() + 
                               "\t" + dayInfo.Close.ToString() + "\t" + Convert.ToDecimal(Convert.ToDouble(dayInfo.Amount.ToString())) + 
                               "\t" + dayInfo.Volume.ToString() + "\n");
            }

            string filePath = stkCode + "KLine.txt";
            Encoding encoder = Encoding.UTF8;
            byte[] bytes = encoder.GetBytes(dayInfoStr.ToString());
            try
            {
                fs = File.OpenWrite(filePath);
                fs.Position = fs.Length;
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件打开失败{0}", ex.ToString());
            }
            finally
            {
                fs.Close();
            }
        }

        public void GetPK()
        {
            R_GetPK(connection, stkCode + "SH", stkName);
        }

        public void GetTestPK()
        {
            R_GetTestRealPK(connection, stkCode + "SH", stkName, lastDealTime);
        }

        public void GetDayInfo()
        {
            R_GetKDays(connection, stkCode, 1, 0, 580);
        }

        public void GetMins()
        {
            R_GetMins(connection, stkCode, 1, 0);
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
}*/