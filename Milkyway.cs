using CoreLib;
using System.Collections.Generic;
using System.IO;

namespace PlanetApp
{
    /// <summary>
    /// 天の川データ
    /// </summary>
    public class Milkyway
    {
        public string mAppFolder;
        public string mDataFolder;

        public record MILKYWAYDATA(
            PointD coordinate,              //  赤経(rad),赤緯(rad)
            int density                     //  濃度(0 ～255)
        );
        public List<MILKYWAYDATA> mMilkywayData;
        public List<string[]> mMilkywayStrList;
        private string mMilkywayFileName = "milkyway(天の川).csv";


        private AstroLib alib = new AstroLib();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appFolder">アプリフォルダ</param>
        /// <param name="dataFolder">データフォルダ</param>
        public Milkyway(string appFolder, string dataFolder)
        {
            mAppFolder = appFolder;
            mDataFolder = dataFolder;
            mMilkywayFileName = Path.Combine(mDataFolder, mMilkywayFileName);
        }

        /// <summary>
        /// データの読込
        /// </summary>
        public void loadData()
        {
            loadData(mMilkywayFileName);
        }

        /// <summary>
        /// CSVデータを読み込んでバイナリデータに変換
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public void loadData(string filePath)
        {
            mMilkywayStrList = alib.loadData(filePath);
            mMilkywayData = string2binaryData(mMilkywayStrList);
        }

        /// <summary>
        /// CSVデーからバイナリデータに変換
        /// </summary>
        /// <param name="strListData"></param>
        /// <returns></returns>
        public List<MILKYWAYDATA> string2binaryData(List<string[]> strListData)
        {
            List<MILKYWAYDATA> milkywayList = new List<MILKYWAYDATA>();
            for (int row = 1; row < strListData.Count; row++) {
                if (0 < strListData[row][0].Length && 0 < strListData[row][1].Length) {
                    MILKYWAYDATA milkyway = new MILKYWAYDATA(
                        new PointD(
                            ylib.H2R(ylib.doubleParse(strListData[row][0])),    //  赤経
                            ylib.D2R(ylib.doubleParse(strListData[row][1]))),   //  赤緯
                        ylib.intParse(strListData[row][2])                      //  濃度
                    );
                    milkywayList.Add(milkyway);
                }
            }
            return milkywayList;
        }
    }
}
