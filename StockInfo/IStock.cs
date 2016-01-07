using System;
namespace StockInfo
{
    public struct StockNode
    {
        public string time;
        public float open;
        public float close;
        public float high;
        public float low;
        public float volumn;
    }

    public interface IStock
    {
        void Connect();
        StockNode CurrentDayNode { get; }
        StockNode CurrentMinsNode { get; }
        bool DayInit { get; }
        System.Collections.Generic.List<StockNode> DayLine { get; }
        bool FiveMinsInit { get; }
        System.Collections.Generic.List<StockNode> FiveMinsLine { get; }
        void InitDayData(Define.TTdxDllShareData data);
        void InitMinsData(Define.TTdxDllShareData data);
        int LastDealTime { get; }
        void LoadDayLine();
        void LoadMins();
        void LoadPK();
        void LoadTestPK();
        string StkCode { get; }
        string StkName { get; }
        void UpdateCurrentDayNode(Define.TTdxDllShareData data);
        void UpdateFiveMinsLine(Define.TTdxDllShareData data);
    }
}
