using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace StockInfo
{
    public class StockDependsOnMemory : StockBase
    {
        private List<StockNode> fiveMinsLine = new List<StockNode>();
        public override List<StockNode> FiveMinsLine
        {
            get
            {
                return fiveMinsLine;
            }
        }

        private List<StockNode> dayLine = new List<StockNode>();
        public override List<StockNode> DayLine
        {
            get
            {
                return dayLine;
            }
        }

        public StockDependsOnMemory(IntPtr handle, string _stkCode, string _stkName) : 
            base(handle, _stkCode, _stkName)
        {
        }
        ~StockDependsOnMemory()
        {    
        }

        public override void UpdateFiveMinsLine(Define.TTdxDllShareData data)
        {
            if (data.count > dealtMinsData)
            {
                Define.TTDX_MIN minsData = new Define.TTDX_MIN();
                int pos = (data.count - 1) * Marshal.SizeOf(minsData);
                minsData = (Define.TTDX_MIN)Utility.BytesToStuct(data.buf, minsData.GetType(), pos);
                currentMinsNode.high = Math.Max(currentMinsNode.high, minsData.Close);
                currentMinsNode.low = Math.Min(currentMinsNode.low, minsData.Close);
                currentMinsNode.close = minsData.Close;

                if (data.count % 5 == 0)
                {
                    float lastClose = currentMinsNode.close;
                    fiveMinsLine.Add(currentMinsNode);
                    currentMinsNode = new StockNode();
                    currentMinsNode.time = DateTime.Now.ToString("yyyyMMddhhmm");
                    currentMinsNode.open = lastClose;
                    currentMinsNode.high = lastClose;
                    currentMinsNode.low = lastClose;
                    currentMinsNode.close = lastClose;
                }
                ++dealtMinsData;
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
                fiveMinsLine.Add(node);
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

                dayLine.Add(node);
            }
            dayInit = true;
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
