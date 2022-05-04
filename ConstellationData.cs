using CoreLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace PlanetApp
{
    /// <summary>
    /// 星座データ
    /// </summary>
    class ConstellationData
    {
        /// <summary>
        /// 星座の腺データ HIP番号(構造体)
        /// </summary>
        public record CONSTELLATIONLINE (
            int sHip,                   //  HIP番号(始点)
            int eHip,                   //  HIP番号(終点)
            string constrationName      //  星座名
        );
        /// <summary>
        /// 星座線の起点座標(赤経赤緯)の構造体
        /// </summary>
        public record CONSTELLATIONSTAR (
            int hip,                    //  HIP番号
            PointD coordinate            //  赤経(rad),赤緯(rad)
        );
        public record CONSTELLATIONNAME(
            string constrationName,     //  星座名
            string constrationNameJpn,  //  星座名(日本語)
            string constrationNameMono, //  略符
            PointD coordinate            //  赤経(rad),赤緯(rad)
        );
        public List<CONSTELLATIONLINE> mConstellaLineLineList;          //  星座線リスト
        public Dictionary<int, CONSTELLATIONSTAR> mConstellaStarList;   //  座標データのハッシュリスト
        public List<CONSTELLATIONNAME> mConstellaNameList;              //  星座名リスト

        public string mAppFolder;       //  アプリケーションフォルダ
        public string mDataFolder;      //  データフォルダ
        public string mConstellationLineFile = "hip_constellation_line(星座線データ).csv";
        public string mConstellationStarFile = "hip_constellation_line_star(星座線恒星データ).csv";
        public string mConstellationNameFile = "理科年表2021星座.csv";
        private string[] mConstellaLineTitle = { "星座", "HIP", "HIP" };
        private string[] mConstellaStarTitle = { "HIP", "赤経", "赤緯" };
        private string[] mConstellaNameTitle = { "星座名", "学名", "略符", "赤経", "赤緯" };

        private AstroLib alib = new AstroLib();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appFolder">APPフォルダ</param>
        /// <param name="dataFolder">データフォルダ</param>
        public ConstellationData(string appFolder, string dataFolder)
        {
            mAppFolder = appFolder;
            mDataFolder = dataFolder;
            mConstellationLineFile = Path.Combine(mDataFolder, mConstellationLineFile);
            mConstellationStarFile = Path.Combine(mDataFolder, mConstellationStarFile);
            mConstellationNameFile = Path.Combine(mDataFolder, mConstellationNameFile);
        }

        /// <summary>
        /// 星座線データの読込
        /// </summary>
        public void loadConstellationLine()
        {
            mConstellaLineLineList = loadConstellationLine(mConstellationLineFile);
        }

        /// <summary>
        /// 星座線データの読込
        /// </summary>
        /// <param name="filePath">データファイルパス</param>
        /// <returns>星座線リスト</returns>
        public List<CONSTELLATIONLINE> loadConstellationLine(string filePath)
        {
            List<CONSTELLATIONLINE> constrationLineList = new List<CONSTELLATIONLINE>();

            //  ファイルデータの読込
            ylib.mEncordingType = 1;        //  SJIS
            List<string[]> constellaLineData = alib.loadData(filePath);
            if (constellaLineData.Count <= 1)
                return constrationLineList;


            //  タイトル位置を求める
            int[] convCol = getTitleColumn(mConstellaLineTitle, constellaLineData[0]);
            //  タイトル位置からデータリストを求める
            for (int row = 1; row < constellaLineData.Count; row++) {
                CONSTELLATIONLINE buf = new CONSTELLATIONLINE(
                    ylib.intParse(constellaLineData[row][convCol[1]]),
                    ylib.intParse(constellaLineData[row][convCol[2]]),
                    constellaLineData[row][convCol[0]]
                    );
                constrationLineList.Add(buf);
            }
            return constrationLineList;
        }

        /// <summary>
        /// 星座線の座標データの読込
        /// </summary>
        public void loadConstellationStar()
        {
            mConstellaStarList = loadConstellationStar(mConstellationStarFile);
        }

        /// <summary>
        /// 星座線の座標データ読込
        /// </summary>
        /// <param name="filePath">データファイルパス</param>
        /// <returns>座標データリスト</returns>
        public Dictionary<int, CONSTELLATIONSTAR> loadConstellationStar(string filePath)
        {
            Dictionary<int, CONSTELLATIONSTAR> constrationStarList = new Dictionary<int, CONSTELLATIONSTAR>();

            //  ファイルデータの読込
            ylib.mEncordingType = 1;        //  SJIS
            List<string[]> constellaStarData = alib.loadStarStrData(filePath);
            if (constellaStarData.Count <= 1)
                return constrationStarList;

            //  タイトル位置を求める
            //convCol = getTitleColumn(mConstellaStarTitle, titleList);
            int[] convCol = getTitleColumn(mConstellaStarTitle, constellaStarData[0]);

            //  座標データをハッシュリストに設定
            for (int row = 1; row < constellaStarData.Count; row++) {
                int hip = ylib.intParse(constellaStarData[row][convCol[0]]);
                CONSTELLATIONSTAR buf = new CONSTELLATIONSTAR(
                    hip,
                    new PointD(
                            ylib.H2R(ylib.HMS2hour(constellaStarData[row][convCol[1]])),    //  赤経(rad)
                            ylib.D2R(ylib.DMS2deg(constellaStarData[row][convCol[2]]))      //  赤緯(rad)
                        )
                    );
                constrationStarList.Add(hip, buf);
            }
            return constrationStarList;
        }

        /// <summary>
        /// 星座名の読込
        /// </summary>
        public void loadConstellationName()
        {
            mConstellaNameList = loadConstellaNameSub(mConstellationNameFile);
        }

        /// <summary>
        /// 星座名の読込
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>星座名リスト</returns>
        public List<CONSTELLATIONNAME> loadConstellaNameSub(string filePath)
        {
            List<CONSTELLATIONNAME> constellaNameList = new List<CONSTELLATIONNAME>();
            //  ファイルデータの読込
            ylib.mEncordingType = 1;        //  SJIS
            List<string[]> constellaNameData = alib.loadStarStrData(filePath);
            if (constellaNameData.Count <= 1)
                return constellaNameList;

            //  タイトル位置を求める
            int[] convCol = getTitleColumn(mConstellaNameTitle, constellaNameData[0]);
            for (int row = 1; row < constellaNameData.Count; row++) {
                CONSTELLATIONNAME constName = new CONSTELLATIONNAME(
                    constellaNameData[row][convCol[2]],
                    constellaNameData[row][convCol[0]],
                    constellaNameData[row][convCol[1]],
                    new PointD(
                            ylib.H2R(ylib.HM2hour(constellaNameData[row][convCol[3]])),    //  赤経(rad)
                            ylib.D2R(ylib.DM2deg(constellaNameData[row][convCol[4]]))      //  赤緯(rad)
                        )
                    );
                constellaNameList.Add(constName);
            }
            return constellaNameList;
        }

        /// <summary>
        /// タイトル位置を求める
        /// </summary>
        /// <param name="titleList">取込み側タイトルリスト</param>
        /// <param name="dataTitleList">データのタイトルリスト</param>
        /// <returns>カラム位置リスト</returns>
        private int[] getTitleColumn(string[] titleList, string[] dataTitleList)
        {
            //  タイトル位置を求める
            int[] convCol = (new int[titleList.Length]).Select(v => -1).ToArray();    //  初期化
            for (int i = 0; i < convCol.Length; i++) {
                for (int j = 0; j < dataTitleList.Length; j++) {
                    if (0 <= dataTitleList[j].IndexOf(titleList[i])) {
                        if (convCol.Exists(j)) {
                            continue;
                        } else {
                            convCol[i] = j;
                            break;
                        }
                    }
                }
            }
            return convCol;
        }
    }
}
