using CoreLib;
using System.Collections.Generic;
using System.IO;

namespace PlanetApp
{
    internal class NebulaData
    {
        //  星雲・銀河などのデータ
        public record NEBULADATA(
            int messierNo,                  //  メシエ番号
            PointD coordinate,              //  赤道座標
            double magnitude,               //  視等級
            string NGCNo,                   //  NGC番号
            string name                     //  名称
        );
        public List<NEBULADATA> mNebulaData;        //  星雲・銀河データリスト
        public List<string[]> mNebulaStrData;       //  星雲・銀河データテキストリスト
        private string[] mTitleList = new string[] {
            "メシエ番号", "NGC番号", "名称", "タイプ", "距離(千光年)", "星座" ,
            "赤経：時分秒", "赤緯：±度分秒", "等級"
        };

        private string mNebulaFileName = "メシエ天体の一覧.csv";
        private string mAppFolder;
        private string mDataFolder;

        private AstroLib alib = new AstroLib();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appFolder">アプリケーションフォルダ</param>
        /// <param name="dataFolder">データフォルダ</param>
        public NebulaData(string appFolder, string dataFolder)
        {
            mAppFolder = appFolder;
            mDataFolder = dataFolder;
            mNebulaFileName = Path.Combine(mDataFolder, mNebulaFileName);
        }

        /// <summary>
        /// 星雲・銀河データの読込
        /// </summary>
        public void  loadData()
        {
            mNebulaStrData = alib.loadStarStrData(mNebulaFileName);
            mNebulaData = convData(mNebulaStrData);
        }

        /// <summary>
        /// 星雲・銀河データの読込ンでバイナリに変換
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns></returns>
        public List<NEBULADATA> loadData(string fileName)
        {
            List<string[]> nebulaStrData = alib.loadStarStrData(fileName);
            return convData(nebulaStrData);
        }

        /// <summary>
        /// 星雲・銀河データをバイナリに変換
        /// </summary>
        /// <param name="nebulaStrData">テキストデータ</param>
        /// <returns></returns>
        private List<NEBULADATA> convData(List<string[]> nebulaStrData)
        {
            List<NEBULADATA> nebulaData = new List<NEBULADATA>();
            if (0 < nebulaStrData.Count) {
                int[] convCol = alib.getConvCol(nebulaStrData[0], mTitleList);
                for (int row = 1; row < nebulaStrData.Count; row++) {
                    NEBULADATA buf = new NEBULADATA(
                        ylib.string2int(nebulaStrData[row][convCol[0]].Substring(1)),
                        new PointD(
                            ylib.H2R(ylib.HM2hour(nebulaStrData[row][convCol[6]])),    //  赤経(rad)
                            ylib.D2R(ylib.DM2deg(nebulaStrData[row][convCol[7]]))      //  赤緯(rad)
                        ),
                        ylib.string2double(nebulaStrData[row][convCol[8]]),
                        nebulaStrData[row][convCol[1]],
                        nebulaStrData[row][convCol[2]]
                        );
                    nebulaData.Add(buf);
                }
            }
            return nebulaData;
        }

    }
}
