using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace StockInfo
{
    public class StockDependsOnFile : StockBase
    {
        private string fiveMinsLineFileName;
        public override List<StockNode> FiveMinsLine
        {
            get
            {
                return GetFiveMinsLineFromFile();
            }
        }

        private string dayLineFileName;
        public override List<StockNode> DayLine
        {
            get
            {
                return GetDayLineFromFile();
            }
        }

        public StockDependsOnFile(IntPtr handle, string _stkCode, string _stkName) : 
            base(handle, _stkCode, _stkName)
        {
            dayLineFileName = stkCode + "DayLine.dat";
            fiveMinsLineFileName = stkCode + "FiveMinsLine.dat";
        }
        ~StockDependsOnFile()
        {
            File.Delete(dayLineFileName);
            File.Delete(fiveMinsLineFileName);
        }

        private List<StockNode> GetFiveMinsLineFromFile()
        {
            List<StockNode> fiveMinsLine = new List<StockNode>();
            using (FileStream fs = new FileStream(fiveMinsLineFileName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (sr.Peek() > -1)
                    {
                        string value = sr.ReadLine();
                        string[] records = value.ToString().Split('\t');
                        StockNode node = new StockNode();
                        node.time = records[0];
                        node.open = float.Parse(records[1]);
                        node.high = float.Parse(records[2]);
                        node.low = float.Parse(records[3]);
                        node.close = float.Parse(records[4]);
                        node.volumn = float.Parse(records[5]);
                        fiveMinsLine.Add(node);
                    }
                }
            }
            return fiveMinsLine;
        }

        private List<StockNode> GetDayLineFromFile()
        {
            List<StockNode> dayLine = new List<StockNode>();
            using (FileStream fs = new FileStream(dayLineFileName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (sr.Peek() > -1)
                    {
                        string value = sr.ReadLine();
                        string[] records = value.ToString().Split('\t');
                        StockNode node = new StockNode();
                        node.time = records[0];
                        node.open = float.Parse(records[1]);
                        node.high = float.Parse(records[2]);
                        node.low = float.Parse(records[3]);
                        node.close = float.Parse(records[4]);
                        node.volumn = float.Parse(records[5]);
                        dayLine.Add(node);
                    }
                }
            }
            return dayLine;
        }

        public override void UpdateFiveMinsLine(Define.TTdxDllShareData data)
        {
            if (data.count > dealtMinsData)
            {
                Define.TTDX_MIN minsData = new Define.TTDX_MIN();
                int pos = (data.count - 1) * Marshal.SizeOf(minsData);
                minsData = (Define.TTDX_MIN)Utility.BytesToStuct(data.buf, minsData.GetType(), pos);
                currentMinsNode.volumn += minsData.Volume;

                if (data.count % 5 == 0)
                {
                    float lastClose = currentMinsNode.close;
                    WriteToFiveMinsLine(currentMinsNode);
                    currentMinsNode = new StockNode();
                    currentMinsNode.time = DateTime.Now.ToString("yyyyMMddhhmm");
                    currentMinsNode.open = lastClose;
                    currentMinsNode.high = lastClose;
                    currentMinsNode.low = lastClose;
                    currentMinsNode.close = lastClose;

                    currentMinsNode.high = Math.Max(currentMinsNode.high, minsData.Close);
                    currentMinsNode.low = Math.Min(currentMinsNode.low, minsData.Close);
                    currentMinsNode.close = minsData.Close;
                }
                ++dealtMinsData;
            }
        }

        private void WriteToFiveMinsLine(StockNode node)
        {
            string str = node.time + "\t" + node.open + "\t" + node.high + "\t" + 
                         node.low + "\t" + node.close + "\t" + node.volumn + "\n";

            FileStream fs = null;
            Encoding encoder = Encoding.UTF8;
            byte[] bytes = encoder.GetBytes(str.ToString());
            try
            {
                fs = File.OpenWrite(fiveMinsLineFileName);
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

        public override void InitMinsData(Define.TTdxDllShareData data)
        {
            Define.TTDX_MIN minsData = new Define.TTDX_MIN();
            float lastClose = CurrentDayNode.open;
            int count = data.count - data.count % 5;

            int pos = 0;
            for (; dealtMinsData < count;)
            {
                StockNode node = new StockNode();
                node.open = lastClose;
                node.high = lastClose;
                node.low = lastClose;
                node.volumn = 0;
                if(dealtMinsData < 120){
                    node.time = string.Format(DateTime.Now.ToString("yyyyMMdd") + "{0:D2}{1:D2}", (dealtMinsData  + 30) / 60 + 9, (dealtMinsData + 30) % 60);
                }
                else
                {
                    node.time = string.Format(DateTime.Now.ToString("yyyyMMdd") + "{0:D2}{1:D2}", (dealtMinsData - 120) / 60 + 13, dealtMinsData % 60);
                }

                for (int i = 0; i < 5 && dealtMinsData < count; ++i)
                {
                    minsData = (Define.TTDX_MIN)Utility.BytesToStuct(data.buf, minsData.GetType(), pos);
                    if (4 == i)
                    {
                        node.close = minsData.Close;
                        lastClose = node.close;
                    }
                    node.high = Math.Max(node.high, minsData.Close);
                    node.low = Math.Min(node.low, minsData.Close);
                    node.volumn += minsData.Volume;

                    ++dealtMinsData;
                    pos += Marshal.SizeOf(minsData);
                }
                WriteToFiveMinsLine(node);
            }
            
            currentMinsNode.open = lastClose;
            currentMinsNode.high = lastClose;
            currentMinsNode.low = lastClose;
            currentMinsNode.close = lastClose;
            currentMinsNode.volumn = 0;
            if (dealtMinsData < 120)
            {
                currentMinsNode.time = string.Format(DateTime.Now.ToString("yyyyMMdd") + "{0:D2}{1:D2}", (dealtMinsData + 30) / 60 + 9, (dealtMinsData + 30) % 60);
            }
            else
            {
                currentMinsNode.time = string.Format(DateTime.Now.ToString("yyyyMMdd") + "{0:D2}{1:D2}", (dealtMinsData - 120) / 60 + 13, dealtMinsData % 60);
            }
            for (int i = 0; i < data.count % 5; ++i)
            {
                minsData = (Define.TTDX_MIN)Utility.BytesToStuct(data.buf, minsData.GetType(), pos);
                if ((data.count % 5) - 1 == i)
                {
                    currentMinsNode.close = minsData.Close;
                }
                currentMinsNode.high = Math.Max(currentMinsNode.high, minsData.Close);
                currentMinsNode.low = Math.Min(currentMinsNode.low, minsData.Close);
                currentMinsNode.volumn += minsData.Volume;

                ++dealtMinsData;
                pos += Marshal.SizeOf(minsData);
            }
            fiveMinsInit = true;
        }

        public override void InitDayData(Define.TTdxDllShareData data)
        {
            Define.TTDX_DAYInfo dayInfo = new Define.TTDX_DAYInfo();

            for (int pos = 0, readData = 0; readData < data.count; pos += Marshal.SizeOf(dayInfo), ++readData)
            {
                dayInfo = (Define.TTDX_DAYInfo)Utility.BytesToStuct(data.buf, dayInfo.GetType(), pos);
                StockNode node = new StockNode();
                node.time = dayInfo.DAY.ToString();
                node.open = dayInfo.Open;
                node.close = dayInfo.Close;
                node.low = dayInfo.Low;
                node.high = dayInfo.High;
                node.volumn = dayInfo.Volume;

                WriteToDayLine(node);
            }
            dayInit = true;
        }

        private void WriteToDayLine(StockNode node)
        {
            string str = node.time + "\t" + node.open + "\t" + node.high + "\t" +
                         node.low + "\t" + node.close + "\t" + node.volumn + "\n";

            FileStream fs = null;
            Encoding encoder = Encoding.UTF8;
            byte[] bytes = encoder.GetBytes(str.ToString());
            try
            {
                fs = File.OpenWrite(dayLineFileName);
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

        public override void UpdateCurrentDayNode(Define.TTdxDllShareData data)
        {
            Define.TTDX_PKBASE pkData = new Define.TTDX_PKBASE();
            pkData = (Define.TTDX_PKBASE)Utility.BytesToStuct(data.buf, pkData.GetType(), 0);
            currentDayNode.time = DateTime.Now.ToString("yyyyMMddhhmm");
            currentDayNode.open = pkData.Open;
            currentDayNode.close = pkData.Close;
            currentDayNode.high = pkData.High;
            currentDayNode.low = pkData.Low;
            currentDayNode.volumn = pkData.Volume;

            currentMinsNode.high = Math.Max(currentMinsNode.high, pkData.Close);
            currentMinsNode.low = Math.Min(currentMinsNode.low, pkData.Close);
            currentMinsNode.close = pkData.Close;
            currentMinsNode.volumn += pkData.LastVolume;

            lastDealTime = (int)pkData.LastDealTime;
        }
    }
}
