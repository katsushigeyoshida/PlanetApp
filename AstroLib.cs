using CoreLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanetApp
{
    public class AstroLib
    {
        private YLib ylib = new YLib();

        //  標準データへの変換用タイトル抽出テーブル
        public static List<string[]> mStarSearchTitle = new List<string[]>() {
            new string[] { "HIP番号", "HIP" },
            new string[] { "赤経：時分秒", "赤経", "RA" },
            new string[] { "赤緯：±度分秒", "赤緯", "Dec" },
            new string[] { "視等級", "Vmag", "V等級" },
            new string[] { "Name/ASCII", "Name/Diacritics", "固有名", "固有名（英語）", "恒星名", "星名", "IAU Name" },
            new string[] { "カタカナ表記", "固有名（日本語）", "恒星名(日本語)" },
            new string[] { "Constellation", "Con", "星座" },
            new string[] { "メシエ番号" },
        };
        //  恒星表タイトル置換えタイトル元データ
        private string[] mConvTitle = {
            "赤経：時（整数）","赤経：分（整数）","赤経：秒（小数）",                           //  0 hip_100,hip_lite...
            "赤緯：符号（0：- 1：+）","赤緯：度（整数）","赤緯：分（整数）","赤緯：秒（小数）", //  3 hip_100,hip_lite...
            "HIP番号",                                                                          //  7 hip_100,hip_lite...
            "赤経:時", "赤経:分（整数）", "赤経:分（小数第一位）",                              //  8 メシエカタログ
            "赤緯:符号 0:-、1:+", "赤緯:度 0°～90°", "赤緯:分 1分=1/60°",                    //  11 メシエカタログ
            "天体種別 1:銀河、2:星団、3:星雲、0:その他(M40:二重星、M73:星群)",                  //  14 メシエカタログ
            "RA(J2000)", "Dec(J2000)",                                                         //  15 IAU Catalog
            "赤経(hhmmss)", "赤緯(±ddmm.m)",                                                   //  17 天文年恒星表
            "赤経(hhmm.m)", "赤緯(±ddmm)",                                                     //  18 理科年表恒星表
            "赤経(hhmm)", "赤緯(±dd)",                                                         //  20 理科年表星座  
            "赤経(h:m)", "赤緯(deg:m)"                                                          //  22 メシエ天体の一覧
        };
        //  恒星表タイトル置換えタイトルデータ
        private string[] mArrangeTitle = { "赤経：時分秒", "赤緯：±度分秒", "HIP番号", "天体種別" };
        //  星雲や銀河などの種類
        private string[] mAstroType = { "その他(M40:二重星、M73:星群)", "銀河", "星団", "星雲" };

        /// <summary>
        /// ファイルデータの読み込み
        /// (赤経・赤緯が分割されているデータは毛号する)
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>データリスト</returns>
        public List<string[]> loadStarStrData(string filePath)
        {
            List<string[]> starStrData = new List<string[]>();
            List<string[]> starDataList = loadData(filePath);                               //  ファイルデータの読込
            if (1 < starDataList.Count) {
                int[] convCol = getConvCol(starDataList[0], mConvTitle);                    //  赤経、赤緯の変換リスト
                string[] starStrDataTitle = getConvTitle(starDataList, convCol).ToArray();  //  赤経、赤緯のタイトル結合
                starStrDataTitle = convCoordinateTitle(starStrDataTitle);                   //  タイトル名の置換え
                starStrData = getConvData(starDataList, convCol, starStrDataTitle);         //  赤経、赤緯のデータ結合
                starStrData = dataNormalized(starStrData);                                  //  列数を揃える
            }
            return starStrData;
        }

        /// <summary>
        /// ファイルデータからタイトルの抽出と変換したカラム位置を求める
        /// 赤経、赤緯で時分秒または度分秒が分かれている場合には結合して一つの項目にする
        /// </summary>
        /// <param name="starData">ファイルデータ</param>
        /// <returns>(タイトルリスト,変換列リスト)</returns>
        public List<string> getConvTitle(List<string[]> starData, int[] convCol)
        {
            //  赤経、赤緯の分割タイトルを結合したタイトルに変更
            List<string> titleList = new List<string>();
            for (int i = 0; i < starData[0].Length; i++) {
                if (i == convCol[0]) {                 //  赤経：時（整数）
                    titleList.Add(mArrangeTitle[0]);
                } else if (i == convCol[1] || i == convCol[2]) {
                } else if (i == convCol[3] || (convCol[3] < 0 && i == convCol[4])) { //  赤緯：符号（0：- 1：+）
                    titleList.Add(mArrangeTitle[1]);
                } else if ((0 <= convCol[3] && i == convCol[4]) || i == convCol[5] || i == convCol[6]) {
                } else if (i == convCol[7]) {          //  HIP番号
                    titleList.Add(mArrangeTitle[2]);
                } else if (i == convCol[8]) {          //  赤経:時
                    titleList.Add(mArrangeTitle[0]);
                } else if (i == convCol[9]) {
                } else if (i == convCol[10]) {          //  赤緯:符号 0:-、1:+
                    titleList.Add(mArrangeTitle[1]);
                } else if (i == convCol[11] || i == convCol[12] || i == convCol[13]) {
                } else if (i == convCol[14]) {          //  天体種別 1:銀河、2:星団、3:星雲、0:その他(M40:二重星、M73:星群
                    titleList.Add(mArrangeTitle[3]);
                } else {
                    titleList.Add(starData[0][i]);
                }
            }
            return titleList;
        }

        /// <summary>
        /// 赤経・赤緯のタイトル変換に(mStarSearchTitleに合わせてタイトルを変更)
        /// </summary>
        /// <param name="srcTitle">元のタイトルリスト</param>
        /// <returns>変換後のタイトルリスト</returns>
        public string[] convCoordinateTitle(string[] srcTitle)
        {
            string[] destTitle = new string[srcTitle.Length];
            //  タイトルで抽出列のリストを作成
            int[] convCol = getConvCol2(srcTitle, mStarSearchTitle);
            for (int i = 0; i < srcTitle.Length; i++) {
                if (i == convCol[1]) {
                    destTitle[i] = mArrangeTitle[0];    //  赤経：時分秒
                } else if (i == convCol[2]) {
                    destTitle[i] = mArrangeTitle[1];    //  赤緯：±度分秒
                } else {
                    destTitle[i] = srcTitle[i].Trim();
                }
            }
            return destTitle;
        }

        /// <summary>
        /// 数値結合処理
        /// ファイルデータで分割された赤経、赤緯のデータを結合したデータリスを作成
        /// </summary>
        /// <param name="starData">データリスト</param>
        /// <param name="convCol">変換カラム位置(getConvTitlewで作成)</param>
        /// <returns>変換データ</returns>
        public List<string[]> getConvData(List<string[]> starData, int[] convCol, string[] title)
        {
            //  HIP番号の最大文字長さ(数値の桁を揃えるため))
            int maxHipStrLen = 0;
            if (0 <= convCol[7]) {
                for (int row = 1; row < starData.Count; row++)
                    maxHipStrLen = Math.Max(maxHipStrLen, starData[row][convCol[7]].Length);
            }
            //  データの登録
            List<string[]> starStrData = new List<string[]>();
            starStrData.Add(title);
            for (int row = 1; row < starData.Count; row++) {
                List<string> buf = new List<string>();
                string ra = "";
                string dec = "";
                for (int col = 0; col < starData[row].Length; col++) {
                    if (convCol[0] == col) {           //  赤経
                        ra = starData[row][col].PadLeft(2, '0') + "h";
                    } else if (convCol[1] == col) {
                        ra += starData[row][col].PadLeft(2, '0') + "m";
                    } else if (convCol[2] == col) {
                        ra += starData[row][col].PadLeft(2, '0') + "s";
                        buf.Add(ra);
                    } else if (convCol[3] == col) {    //  赤緯
                        dec = starData[row][col].CompareTo("0") == 0 ? "-" : "+";
                    } else if (convCol[4] == col) {
                        if (convCol[3] < 0) {
                            dec += (starData[row][col][0] == '-' ? "" :
                                (starData[row][col][0] == '+' ? "" : "+")) +
                                 starData[row][col] + "°";
                        } else {
                            dec += starData[row][col].PadLeft(2, '0') + "°";
                        }
                    } else if (convCol[5] == col) {
                        dec += starData[row][col].PadLeft(2, '0') + "′";
                    } else if (convCol[6] == col) {
                        int l = 2;
                        if (0 <= starData[row][col].IndexOf('.'))
                            l += starData[row][col].Length - starData[row][col].IndexOf('.');
                        dec += starData[row][col].PadLeft(l, '0') + "″";
                        buf.Add(dec);
                    } else if (convCol[7] == col ||
                        0 <= starData[0][col].IndexOf(mArrangeTitle[2])) {      //  HIP番号
                        buf.Add(starData[row][col].PadLeft(maxHipStrLen, '0'));
                    } else if (convCol[8] == col) {     //  "赤経:時
                        ra = starData[row][col].PadLeft(2, '0') + "h";
                    } else if (convCol[9] == col) {
                        ra += starData[row][col].PadLeft(2, '0') + "m";
                    } else if (convCol[10] == col) {
                        ra += (ylib.string2double(starData[row][col]) * 6.0).ToString("00") + "s";
                        buf.Add(ra);
                    } else if (convCol[11] == col) {        //  赤緯:符号 0:-、1:+
                        dec += starData[row][col][0] == '0' ? "-" : "+";
                    } else if (convCol[12] == col) {
                        dec += starData[row][col].PadLeft(2, '0') + "°";
                    } else if (convCol[13] == col) {
                        dec += starData[row][col].PadLeft(2, '0') + "′";
                        buf.Add(dec);
                    } else if (convCol[14] == col) {        //  天体種別
                        buf.Add(mAstroType[ylib.string2int(mAstroType.Length <= ylib.string2int(starData[row][col]) ? "0" : starData[row][col])]);
                    } else if (convCol[15] == col) {        //  RA(J2000)度(ddd.dddd)
                        buf.Add(ylib.hour2HMS(ylib.D2H(ylib.doubleParse(starData[row][col]))));
                    } else if (convCol[16] == col) {        //  Dec(J2000)度(ddd.dddd)
                        buf.Add(ylib.deg2DMS(ylib.doubleParse(starData[row][col])));
                    } else if (convCol[17] == col) {        //  赤経(hhmmss)
                        buf.Add(ylib.hour2HMS(ylib.hhmmss2hour(starData[row][col])));
                    } else if (convCol[18] == col) {        //  赤緯(±ddmm.m)
                        buf.Add(ylib.deg2DMS(ylib.DM2deg(starData[row][col])));
                    } else if (convCol[19] == col) {        //  赤経(hhmm.m)
                        buf.Add(ylib.hour2HMS(ylib.HM2hour(starData[row][col])));
                    } else if (convCol[20] == col) {        //  赤緯(±ddmm)
                        buf.Add(ylib.deg2DMS(ylib.DM2deg(starData[row][col])));
                    } else if (convCol[21] == col) {        //  赤経(hhmm)
                        buf.Add(ylib.hour2HMS(ylib.HM2hour(starData[row][col])));
                    } else if (convCol[22] == col) {        //  赤緯(±dd)
                        buf.Add(ylib.deg2DMS(ylib.DM2deg(starData[row][col])));
                    } else if (convCol[23] == col) {        //  赤経(h:m)
                        buf.Add(ylib.hour2HMS(ylib.hm2hour(starData[row][col])));
                    } else if (convCol[24] == col) {        //  赤緯(deg:m)
                        buf.Add(ylib.deg2DMS(ylib.dm2deg(starData[row][col])));
                    } else {
                        buf.Add(starData[row][col]);
                    }
                }
                starStrData.Add(buf.ToArray());
            }
            return starStrData;
        }

        /// <summary>
        /// タイトル列の変換テーブルの取得
        /// </summary>
        /// <param name="starDataTitle">変換データタイトル</param>
        /// <param name="convTitle">変換タイトルリスト</param>
        /// <returns>変換テーブル</returns>
        public int[] getConvCol(string[] starDataTitle, string[] convTitle)
        {
            //  タイトル行から赤経、赤緯の分割データ位置の検索
            int[] convCol = new int[convTitle.Length];
            for (int i = 0; i < convTitle.Length; i++) {
                int n = Array.FindIndex(starDataTitle, x => x.CompareTo(convTitle[i]) == 0);
                if (0 <= n) {
                    convCol[i] = n;
                } else {
                    convCol[i] = -1;
                }
            }
            return convCol;
        }

        /// <summary>
        /// タイトルの変換リストを作成
        /// </summary>
        /// <param name="srcTitle">ソースリストタイトル</param>
        /// <param name="convTitle">変換タイトルリスト(2次元)</param>
        /// <returns></returns>
        public int[] getConvCol2(string[] srcTitle, List<string[]> convTitle)
        {
            //  タイトルで抽出列のリストを作成
            int[] convCol = (new int[convTitle.Count]).Select(v => -1).ToArray();    //  初期化
            for (int i = 0; i < convTitle.Count; i++) {
                for (int j = 0; j < convTitle[i].Length; j++) {
                    for (int k = 0; k < srcTitle.Length; k++) {
                        if (0 <= srcTitle[k].IndexOf(convTitle[i][j])) {
                            convCol[i] = k;
                            break;
                        }
                    }
                    if (0 <= convCol[i])
                        break;
                }
            }
            return convCol;
        }

        /// <summary>
        /// リストの列数を最大数に揃える
        /// </summary>
        /// <param name="listData">リストデータ</param>
        /// <returns>リストデータ</returns>
        public List<string[]> dataNormalized(List<string[]> listData)
        {
            int maxLength = 0;
            foreach (string[] data in listData) {
                maxLength = Math.Max(maxLength, data.Length);
            }
            List<string[]> normalList = new List<string[]>();
            foreach (string[] data in listData) {
                if (maxLength > data.Length) {
                    string[] buf = new string[maxLength];
                    for (int i = 0; i < data.Length; i++)
                        buf[i] = data[i].Trim();
                    normalList.Add(buf);
                } else {
                    normalList.Add(data);
                }
            }
            return normalList;
        }

        /// <summary>
        /// ファイルデータの読込
        /// データの加工なし、コメント行削除
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>データリスト</returns>
        public List<string[]> loadData(string filePath)
        {
            ylib.mEncordingType = 1;    //  SJIS
            List<string[]> dataList = ylib.loadCsvData(filePath);
            List<string[]> starData = new List<string[]>();
            for (int i = 0; i < dataList.Count; i++) {
                if (0 < dataList[i][0].Length && dataList[i][0][0] == '#')   //  コメント行削除
                    continue;
                string[] buf = new string[dataList[i].Length];
                for (int j = 0; j <dataList[i].Length; j++)
                    buf[j]= dataList[i][j].Trim();
                starData.Add(buf);
            }
            return starData;
        }


        //  ---  座標変換関連  ------

        /// <summary>
        /// 赤道座標(equatorial coordinates)から地平座標(horizon coordinates)の高度(height)(0～90)を求める
        /// </summary>
        /// <param name="hourAngle">時角t(rad)</param>
        /// <param name="declination">赤緯δ(rad)</param>
        /// <param name="latitude">観測点の緯度φ(rad)</param>
        /// <returns>高度h(rad)</returns>
        public double horizonHeight(double hourAngle, double declination, double latitude)
        {
            double sinh = Math.Sin(declination) * Math.Sin(latitude) +
                    Math.Cos(declination) * Math.Cos(latitude) * Math.Cos(hourAngle);
            return Math.Asin(sinh);
        }

        /// <summary>
        /// 赤道座標(equatorial coordinates)から地平座標(horizon coordinates)の方位(azimuth)を求める
        /// </summary>
        /// <param name="hourAngle">時角t(rad)</param>
        /// <param name="declination">赤緯δ(rad)</param>
        /// <param name="latitude">観測点の緯度φ(rad)</param>
        /// <returns>方位A(rad)</returns>
        public double horizonAzimuth(double hourAngle, double declination, double latitude)
        {
            double y = Math.Cos(declination) * Math.Sin(hourAngle);
            double x = -Math.Cos(latitude) * Math.Sin(declination) +
                        Math.Sin(latitude) * Math.Cos(declination) * Math.Cos(hourAngle);
            return y < 0 ? 2.0 * Math.PI + Math.Atan2(y, x) : Math.Atan2(y, x);  // 0 - 2π
        }

        /// <summary>
        /// 地平座標(horizon coordinates)から赤道座標(equatorial coordinates)の赤緯(δ:declination)を求める
        /// </summary>
        /// <param name="azimuth">方位(A: rad)</param>
        /// <param name="height">高度(h: rad)</param>
        /// <param name="latitude">観測点緯度(φ: rad)</param>
        /// <returns>赤緯(δ: rad -π～π)</returns>
        public double equatorialDeclination(double azimuth, double height, double latitude)
        {
            double sind = Math.Sin(height) * Math.Sin(latitude) - Math.Cos(height) * Math.Cos(latitude) * Math.Cos(azimuth);
            return Math.Asin(sind);
        }

        /// <summary>
        /// 地平座標(horizon coordinates)から赤道座標(equatorial coordinates)の時角(t: hourangle)を求める
        /// </summary>
        /// <param name="azimuth">方位(A: rad)</param>
        /// <param name="height">高度(h: rad)</param>
        /// <param name="latitude">観測点緯度(φ: rad)</param>
        /// <returns>時角(t: rad 0～2π)</returns>
        public double equatorialHourAngle(double azimuth, double height, double latitude)
        {
            double x = Math.Cos(latitude) * Math.Sin(height) + Math.Sin(latitude) * Math.Cos(height) * Math.Cos(azimuth);
            double y = Math.Cos(height) * Math.Sin(azimuth);
            return y < 0 ? 2.0 * Math.PI + Math.Atan2(y, x) : Math.Atan2(y, x);  // 0 - 2π
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="azimuth">方位(A: rad)</param>
        /// <param name="height">高度(h: rad)</param>
        /// <param name="latitude">観測点緯度(φ: rad)</param>
        /// <param name="lst">地方恒星時(lst: rad)</param>
        /// <returns>赤経(ra: rad))</returns>
        public double equatorialRightAscension(double azimuth, double height, double latitude, double lst)
        {
            double t = equatorialHourAngle(azimuth, height, latitude);
            return lst - t;
        }

        /// <summary>
        /// 地平座標系の座標を全天表示の直交座標に変換
        /// </summary>
        /// <param name="azimuth">恒星の方位(rad)</param>
        /// <param name="height">恒星の高さ(rad)</param>
        /// <param name="direction">表示盤の方位(hour)</param>
        /// <param name="radius">表示盤の半径</param>
        /// <returns>表示の直交座標</returns>
        public PointD cnvFullHorizontal(double azimuth, double height, double direction, double radius)
        {
            PointD p = new PointD();
            p.fromPoler(radius * (1.0 - height * 2.0 / Math.PI), azimuth);
            p.rotate(ylib.H2R((direction + 6) % 24));
            return p;
        }


        /// <summary>
        /// 地平座標系の投影のため半円表示の直交座標に変換
        /// </summary>
        /// <param name="azimuth">恒星の方位(rad)</param>
        /// <param name="height">恒星の高さ(rad)</param>
        /// <param name="direction">表示盤の方位(hour)</param>
        /// <param name="radius">表示盤の半径</param>
        /// <returns>表示の直交座標</returns>
        public PointD cnvHorizontal(double azimuth, double height, double direction, double radius)
        {
            return new PointD(radius * Math.Sin(azimuth - ylib.H2R(direction)) * Math.Cos(height),
                                radius * Math.Sin(height));
        }


        /// <summary>
        /// 赤道座標(赤経・赤緯)から表示の直交座標に変換
        /// </summary>
        /// <param name="coordinate">恒星の赤道座標(赤経(rad),赤緯(rad))</param>
        /// <param name="direction">表示盤の方位(hour)</param>
        /// <param name="radius">表示盤の半径</param>
        /// <returns>表示の直交座標</returns>
        public PointD equatorial2orthogonal(PointD coordinate, double direction, double radius)
        {
            PointD p = polar2orthogonal(declinationLength(coordinate.y, radius), Math.PI * 2 - coordinate.x);
            p.rotate(ylib.H2R(direction));
            return p;
        }

        /// <summary>
        /// 赤緯から天の天頂からの距離を求める
        /// </summary>
        /// <param name="declination">赤緯(rad)</param>
        /// <param name="radius">表示領域の半径</param>
        /// <returns>距離</returns>
        public double declinationLength(double declination, double radius)
        {
            return radius * (0.5 - declination / Math.PI);
        }

        /// <summary>
        /// 恒星データを標準パターンに変換
        /// mSearchTitle でパターン変換
        /// (HIP,赤経<赤緯,視等級,恒星名,恒星名(日本語),星座,メシエNo)に変換する
        /// </summary>
        /// <param name="srcData">元データ</param>
        /// <param name="srcTitle">元データタイトル</param>
        /// <param name="stdTitle">変換タイトル</param>
        /// <returns>変換データ</returns>
        public List<string[]> convStanderdData(List<string[]> srcData)
        {
            List<string[]> stdTitle = mStarSearchTitle;
            List<string[]> starData = new List<string[]>();
            if (srcData.Count <= 0)
                return starData;

            //  タイトルで抽出列のリストを作成
            int[] convCol = getConvCol2(srcData[0], stdTitle);

            for (int row = 0; row < srcData.Count; row++) {
                string[] buf = new string[convCol.Length];
                for (int col = 0; col < convCol.Length; col++) {
                    if (0 <= convCol[col] && convCol[col] < srcData[row].Length) {
                        buf[col] = srcData[row][convCol[col]];
                    } else {    //  対象外
                        buf[col] = "";
                    }
                }
                starData.Add(buf);
            }
            return starData;
        }

        /// <summary>
        /// 極座標から直交座標に変換
        /// </summary>
        /// <param name="r">原点からの距離</param>
        /// <param name="th">角度(rad)</param>
        /// <returns>直交座標</returns>
        public PointD polar2orthogonal(double r, double th)
        {
            return new PointD(r * Math.Cos(th), r * Math.Sin(th));
        }

    }
}
