using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace StockInfo
{
    public class StockDependsOnDatabase : StockBase
    {
        private SqlConnection con;
        private string dayLineTB;
        private string minsLineTB;

        public override List<StockNode> FiveMinsLine
        {
            get
            {
                return GetFiveMinsLineFromDatabase();
            }
        }

        public override List<StockNode> DayLine
        {
            get
            {
                return GetDayLineFromDatabase();
            }
        }

        public StockDependsOnDatabase(IntPtr handle, string _stkCode, string _stkName) : 
            base(handle, _stkCode, _stkName)
        {
            dayLineTB = "dayLineTB" + stkCode;
            minsLineTB = "minsLineTB" + stkCode;

            con = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");
            con.Open();

            InitDatabase();
        }
        ~StockDependsOnDatabase()
        {
            try
            {
                con.Close();
            }
            catch (Exception e)
            {
            }
            finally
            {
                con.Dispose();
            }
        }

        private void InitDatabase(){
            string sql = "IF not EXISTS(SELECT * FROM sysdatabases " +
                         "WHERE name = 'stock')" +
                         "CREATE DATABASE stock ON PRIMARY" +
                         "(name=stock_data, filename='D:\\stock_data.mdf'," +
                         "size = 10, maxsize = 50, filegrowth=10%)log on" +
                         "(name = stock_data_log, filename='D:\\stock_data.ldf'," +
                         "size = 10, maxsize=20, filegrowth=1);";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            con.Close();

            con = new SqlConnection("Integrated Security=SSPI;Initial Catalog=stock;Data Source=localhost;");
            con.Open();

            sql = "IF OBJECT_ID('" + dayLineTB + "', 'U') IS NOT NULL " + 
                  "DROP TABLE " + dayLineTB + ";"; 
            cmd.Connection = con;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "create table " + dayLineTB +
                  "(dataTime CHAR(12), dataOpen float(6), dataHigh float(6), dataLow float(6), dataClose float(6), dataVolumn float(6));";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "IF OBJECT_ID('" + minsLineTB + "', 'U') IS NOT NULL " +
                  "DROP TABLE " + minsLineTB + ";"; 
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "create table " + minsLineTB +
                  "(dataTime CHAR(12), dataOpen float(6), dataHigh float(6), dataLow float(6), dataClose float(6), dataVolumn float(6));";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private List<StockNode> GetFiveMinsLineFromDatabase()
        {
            List<StockNode> fiveMinsLine = new List<StockNode>();
            string str = "Select * from " + minsLineTB;
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                StockNode node = new StockNode();
                node.time = (string)reader["dataTime"];
                node.open = (float)reader["dataOpen"];
                node.high = (float)reader["dataHigh"];
                node.low = (float)reader["dataLow"];
                node.close = (float)reader["dataClose"];
                node.volumn = (float)reader["dataVolumn"];
                fiveMinsLine.Add(node);
            }
            reader.Close();
            return fiveMinsLine;
        }

        private List<StockNode> GetDayLineFromDatabase()
        {
            List<StockNode> dayLine = new List<StockNode>();
            string str = "Select * from " + dayLineTB;
            SqlCommand command = new SqlCommand(str, con);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                StockNode node = new StockNode();
                node.time = (string)reader["dataTime"];
                node.open = (float)reader["dataOpen"];
                node.high = (float)reader["dataHigh"];
                node.low = (float)reader["dataLow"];
                node.close = (float)reader["dataClose"];
                node.volumn = (float)reader["dataVolumn"];
                dayLine.Add(node);
            }
            reader.Close();
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
            string str = "INSERT INTO " + minsLineTB + "(dataTime,dataOpen,dataHigh,dataLow,dataClose,dataVolumn)" +
                         "VALUES ('" + node.time + "'," + node.open + "," + node.high + "," + node.low + "," + node.close + "," + node.volumn + ");";
            SqlCommand cmd = new SqlCommand(str, con);
            cmd.ExecuteNonQuery();
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
            string str = "INSERT INTO " + dayLineTB + "(dataTime,dataOpen,dataHigh,dataLow,dataClose,dataVolumn)" +
                         "VALUES ('" + node.time + "'," + node.open + "," + node.high + "," + node.low + "," + node.close + "," + node.volumn + ");";
            SqlCommand cmd = new SqlCommand(str, con);
            cmd.ExecuteNonQuery();
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
