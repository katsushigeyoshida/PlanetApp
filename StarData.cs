using CoreLib;
using System.Collections.Generic;
using System.IO;

namespace PlanetApp
{
    class StarData
    {
        public string mAppFolder;
        public string mDataFolder;

        //  表示用恒星データ
        public record STARDATA(
            int hipNo,                      //  HIP番号
            PointD coordinate,              //  赤経(rad),赤緯(rad)
            double magnitude,               //  視等級
            string starName,                //  恒星名
            string starNameJp,              //  恒星名(日本語)
            string constellation            //  星座名
        );
        public List<STARDATA> mStarData;            //  表示用恒星データ
        public string[] mStarStrDataTitle = {       //  標準恒星データ表タイトル
            "HIP番号", "赤経：時分秒", "赤緯：±度分秒", "視等級", "恒星名", "恒星名(日本語)", "星座名"
        };
        public string mStarTableTitle;              //  標準データ表タイトル
        public List<string[]> mStarStrData;         //  標準構成データ

        //  ファイル恒星データ
        public string mOrgStarTableTitle;           //  ファイル読込データ表タイトル(ファイル名)
        public List<string[]> mOrgStarStrData;      //  ファイル読込恒星データ

        private AstroLib alib = new AstroLib();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appFolder">アプリケーションのフォルダ</param>
        /// <param name="dataFolder">データフォルダ</param>
        public StarData(string appFolder, string dataFolder)
        {
            mAppFolder = appFolder;
            mDataFolder = dataFolder;
        }

        /// <summary>
        /// ファイルデータの読込
        /// データの加工なし mOrgStarStrDataTitle/mOrgStarStrData に設定
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public bool loadOrgStarData(string filePath)
        {
            mOrgStarTableTitle = Path.GetFileNameWithoutExtension(filePath);    //  テーブル名
            List<string[]> starDataList = alib.loadData(filePath);              //  データの読込
            mOrgStarStrData = starDataList;
            return mOrgStarStrData.Count < 1 ? false : true;
        }

        /// <summary>
        /// ファイルデータの読込(タイトルとデータを分ける)
        /// 赤経、赤緯で時分秒または度分秒が分かれている場合には結合して一つの項目にする
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public bool loadStarData(string filePath)
        {
            mOrgStarTableTitle = Path.GetFileNameWithoutExtension(filePath);
            mOrgStarStrData = alib.loadStarStrData(filePath);
            return mOrgStarStrData.Count < 1 ? false : true;
        }

        /// <summary>
        /// バイナリ変換
        /// 恒星表の文字データをSTARDATA形式の構造体データに変換
        /// </summary>
        public void convStarData()
        {
            if (mStarStrData != null && 0 < mStarStrData.Count)
                mStarData = convStarData(mStarStrData);
        }

        /// <summary>
        /// 恒星データを標準パターンに変換
        /// (未使用部分を削除))
        /// </summary>
        public void convStanderdData()
        {
            mStarStrData = alib.convStanderdData(mOrgStarStrData);
            mStarTableTitle = mOrgStarTableTitle;                   //  データタイトル(ファイルタイトル)
        }

        /// <summary>
        /// 標準データの保存
        /// </summary>
        /// <param name="fileName">ファイル名(拡張子なし)</param>
        /// <returns></returns>
        public bool saveStandardData(string fileName)
        {
            if (0 < mStarStrData.Count) {
                string path = Path.Combine(mAppFolder, fileName+".csv");
                ylib.saveCsvData(path, mStarStrDataTitle, mStarStrData);
                return true;
            } else {
                return false;
            }
        }



        /// <summary>
        /// 既存のデータに新たなデータをマージする(列追加)
        /// </summary>
        /// <param name="mergeData">マージデータ</param>
        /// <param name="mergeTitle">マージタイトル</param>
        /// <param name="mergeKey">マージの共通タイトル</param>
        /// <param name="orgKey">既存のマージの共通タイトル</param>
        public void fileMerge(List<string[]> mergeData, string[] mergeTitle, string mergeKey, string orgKey ="")
        {
            //  マージの共通タイトルの列番を求める
            int keyCol = mOrgStarStrData[0].FindIndex(x => x.CompareTo(mergeKey) == 0);
            if (orgKey.Length == 0)
                orgKey = mergeKey;
            int mergeCol = mergeTitle.FindIndex(x => x.CompareTo(orgKey) == 0);
            if (mergeCol < 0 || keyCol < 0)
                return;

            //  タイトルをマージする
            List<string> tempTitle = new List<string>();
            for (int i = 0; i < mOrgStarStrData[0].Length; i++)
                tempTitle.Add(mOrgStarStrData[0][i]);
            for (int i = 0; i < mergeTitle.Length; i++)
                if (i != mergeCol)
                    tempTitle.Add(mergeTitle[i]);

            //  マージするデータをハッシュテーブルに置換える
            Dictionary<string, string[]> hashMergeData = new Dictionary<string, string[]>();
            for (int i = 0; i < mergeData.Count; i++)
                hashMergeData.Add(mergeData[i][mergeCol], mergeData[i]);

            //  データをマージする
            List<string[]> tempStarData = new List<string[]>();
            for (int i = 1; i < mOrgStarStrData.Count; i++) {
                List<string> buf = new List<string>();
                for (int j = 0; j < mOrgStarStrData[0].Length; j++) {
                    if (j < mOrgStarStrData[i].Length)
                        buf.Add(mOrgStarStrData[i][j]);
                    else
                        buf.Add("");
                }
                if (hashMergeData.ContainsKey(mOrgStarStrData[i][keyCol])) {
                    string[] mdata = hashMergeData[mOrgStarStrData[i][keyCol]];
                    for (int j = 0; j < mdata.Length; j++) {
                        if (j != mergeCol)
                            buf.Add(mdata[j]);
                    }
                } else {
                    string[] mergeDmy = new string[mergeData[0].Length - 1];
                    buf.AddRange(mergeDmy);
                }
                tempStarData.Add(buf.ToArray());
            }
            //  結果を設定
            mOrgStarStrData = tempStarData;
            mOrgStarStrData.Insert(0, tempTitle.ToArray());
        }

        /// <summary>
        /// 恒星表の文字データをSTARDATA形式の構造体データに変換
        /// </summary>
        /// <param name="strListData">リストデータ</param>
        /// <returns>構造体データリスト</returns>
        private List<STARDATA> convStarData(List<string[]> strListData)
        {
            List<STARDATA> starDatas = new List<STARDATA>();
            for (int row =1; row < strListData.Count; row++) {
                if (0 < strListData[row][1].Length && 0 < strListData[row][2].Length) {
                    STARDATA star = new STARDATA(
                        (int)ylib.doubleParse(strListData[row][0]),              //  HIP番号
                        new PointD(
                            ylib.H2R(ylib.HMS2hour(strListData[row][1])),        //  赤経
                            ylib.D2R(ylib.DMS2deg(strListData[row][2]))),        //  赤緯
                        ylib.string2double(strListData[row][3]),                 //  視等級
                        strListData[row][4],                                     //  恒星名
                        strListData[row].Length <= 5 ? "" : strListData[row][5], //  恒星名(日本語)
                        strListData[row].Length <= 6 ? "" : strListData[row][6]  //  星座名
                        );
                    starDatas.Add(star);
                }
            }
            return starDatas;
        }

    }
}
