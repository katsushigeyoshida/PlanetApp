using CoreLib;
using System.Collections.Generic;
using System.IO;

namespace PlanetApp
{
    class StarInfoData
    {
        // 恒星情報検索データ
        public record STARINFODATA(             //  恒星情報データ
            int hip,                            //  HIP番号
            string starName,                    //  恒星名
            string[] infoData                   //  恒星データ情報
            );
        //  恒星位置照合データ
        public record SEARCHDATA(               //  座標からの検索データ
            PointD coodinate,                   //  スクリーン座標
            string starName,                    //  恒星名
            int mesia,                          //  メシア番号
            int hip                             //  HIP番号
        );

        public List<SEARCHDATA> mSearchData = new List<SEARCHDATA>();   //  恒星データ検索リスト
        private List<STARINFODATA> mStarInfoData;                       //  恒星情報データリスト
        public List<string[]> mNebulaStrData;                           //  星雲・銀河データテキストリスト
        private string[] mInfoDataTitle;
        private string mStarInfoDataPath;
        private string mAppFolder;
        private string mDataFolder;

        private YLib ylib = new YLib();

        /// <summary>
        /// 初期設定
        /// </summary>
        /// <param name="dataFolder">データフォルダ</param>
        public StarInfoData(string appFolder,string dataFolder)
        {
            mAppFolder = appFolder;
            mDataFolder = dataFolder;
            mStarInfoDataPath = "hip_100(視等級が小さい（明るい）星を100個抽出).csv";
            mStarInfoDataPath = Path.Combine(mDataFolder, mStarInfoDataPath);
            //mStarInfoFile = mStarFileData[0];
        }


        /// <summary>
        /// 恒星データからインデックスデータとをマージして情報データを作成
        /// </summary>
        /// <param name="starInfoData">恒星文字データ</param>
        public void loadInfoData(List<string[]> starInfoData)
        {
            //  恒星データのHIPカラムと恒星名カラム位置を求める
            int hipCol = getTaitleCol(starInfoData[0], 0);   //  HIP番号
            int mesiaCol = getTaitleCol(starInfoData[0], 7);   //  メシエ番号
            int starNameCol = getTaitleCol(starInfoData[0], 4);    //  恒星名
                //);

            if (hipCol < 0 && starNameCol < 0) {
                //  HIPまたは恒星名データがない場合、標準のフィルデータを使う
                StarData starData = new StarData(mDataFolder, mStarInfoDataPath);
                if (!starData.loadStarData(mStarInfoDataPath))
                    return;
                starInfoData = starData.mOrgStarStrData;
                hipCol = getTaitleCol(starInfoData[0], 0);   //  HIP番号
                mesiaCol = getTaitleCol(starInfoData[0], 7);   //  メシエ番号
                starNameCol = getTaitleCol(starInfoData[0], 4);    //  恒星名
            }
            //  HIP番号または恒星名との情報データを作成
            mStarInfoData = new List<STARINFODATA>();
            if (1 < starInfoData.Count) {
                mInfoDataTitle = starInfoData[0];
                for (int i = 1; i < starInfoData.Count; i++) {
                    STARINFODATA buf = new STARINFODATA(
                        hipCol < 0 ? 0 : ylib.string2int(starInfoData[i][hipCol]),  //  HIP番号
                        starNameCol < 0 ? "" : starInfoData[i][starNameCol],        //  恒星名
                        starInfoData[i]                                                                         //  恒星データ情報
                    );
                    mStarInfoData.Add(buf);
                }
            }
        }

        /// <summary>
        /// タイトル位置の検索
        /// mStarSearchTitleで定義されているタイトル名の検索
        /// </summary>
        /// <param name="title">タイトルデータ</param>
        /// <param name="titleNo">検索タイトル</param>
        /// <returns>検索位置</returns>
        private int getTaitleCol(string[] title, int titleNo)
        {
            for (int i = 0; i < AstroLib.mStarSearchTitle[titleNo].Length; i++) {
                int n = title.FindIndex(p => p.CompareTo(AstroLib.mStarSearchTitle[titleNo][i]) == 0);
                if (0 <= n)
                    return n;
            }
            return -1;
        }

        /// <summary>
        /// HIP番号または恒星名から恒星情報を取得する
        /// </summary>
        /// <param name="hip">HIP番号</param>
        /// <param name="starName">恒星名</param>
        /// <returns>恒星情報</returns>
        public List<string> searchHipData(SEARCHDATA searchdata)
        {
            List<string> starData = new List<string>();
            STARINFODATA starInfo = null;
            string[] neburaData = null;
            //  データの検索
            if (searchdata.hip == 0) {
                //  HIP番号のない恒星データ
                starInfo = mStarInfoData.Find(p => p.starName.CompareTo(searchdata.starName) == 0);
            } else if (searchdata.hip < 0 && 0 <= searchdata.mesia) { 
                //  星雲・銀河などのデータ
                neburaData = mNebulaStrData.Find(p => p[0].CompareTo("M" + searchdata.mesia.ToString()) == 0);
            } else {
                //  HIP番号のある恒星データ
                starInfo = mStarInfoData.Find(p => p.hip == searchdata.hip);
            }

            //  データテキストリストの作成
            if (starInfo != null) {
                for (int i = 0; i < mInfoDataTitle.Length; i++) {
                    starData.Add( mInfoDataTitle[i] + ": " + starInfo.infoData[i]);
                }
            } else if (neburaData != null) {
                for (int i = 0; i < neburaData.Length; i++) {
                    starData.Add(mNebulaStrData[0][i] + ": " + neburaData[i]);
                }
            }
            return starData;
        }

        /// <summary>
        /// 恒星の座標(スクリーン座標)から恒星を特定する
        /// </summary>
        /// <param name="pos">スクリーン座標</param>
        /// <param name="dis">許容範囲(距離)</param>
        /// <returns>検索結果</returns>
        public SEARCHDATA searchPos(PointD pos, double dis)
        {
            if (0 < mSearchData.Count)
                foreach (var p in mSearchData) {
                    if (pos.length(p.coodinate) < dis) {
                        return p;
                    }
                }
            return null;
        }
    }
}
