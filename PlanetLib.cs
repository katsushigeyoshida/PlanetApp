using CoreLib;
using System;
using System.Collections.Generic;

namespace PlanetApp
{

    public class PlanetLib
    {
        //  「理科年表2021」のデータ
        // Π 近日点黄経 = ω 近日点引数 + Ω 昇交点経度 
        public List<PLANETDATA> mPlanetData = new List<PLANETDATA>() {
            //  惑星名, 軌道長半径a, 離心率e, 黄道軌道傾斜i, 近日点黄経ω, 昇交点黄経Ω, 元期平均近点角M0, 公転周期, 元期(JD)
            new PLANETDATA("水星",    0.3871, 0.2056, 7.004,  77.490,  48.304, 282.128,  0.24085, 2459400.5),
            new PLANETDATA("金星",    0.7233, 0.0068, 3.694, 131.565,  76.620,  35.951,  0.61520, 2459400.5),
            new PLANETDATA("地球",    1.0000, 0.0167, 0.003, 103.007, 174.821, 179.912,  1.00002, 2459400.5),
            new PLANETDATA("火星",    1.5237, 0.0934, 1.848, 336.156,  49.495, 175.817,  1.88085, 2459400.5),
            new PLANETDATA("木星",    5.2026, 0.0485, 1.303,  14.378, 100.502, 312.697,  11.8620, 2459400.5),
            new PLANETDATA("土星",    9.5549, 0.0555, 2.489,  93.179, 113.610, 219.741,  29.4572, 2459400.5),
            new PLANETDATA("天王星", 19.2184, 0.0464, 0.773, 173.024,  74.022, 233.182,  84.0205, 2459400.5),
            new PLANETDATA("海王星", 30.1104, 0.0095, 1.770,  48.127, 131.783, 303.212, 164.7701, 2459400.5),
            new PLANETDATA("冥王星", 39.4451, 0.2502,17.089, 222.974, 110.377, 25.2472, 247.7407, 2454000.5),  //  Wikipediaより
            new PLANETDATA("月",      0.00257,0.0555, 5.157,  -0.011,   0.002,  0.0,      0.0745, 0.0)
        };

        //  AstroCommons 惑星データ(http://astro.starfree.jp/commons/astrometry/orbit.html)
        public List<PLANETDATA2> mPlanetData2 = new List<PLANETDATA2>() {
            //          惑星名, a 軌道長半径(au), e 離心率, i 軌道傾斜角(deg), 
            //          Ω Omega 昇交点経度(deg), ω varpi 近日点経度(deg), L 平均経度(deg)
            new PLANETDATA2(
                "水星", 0.38709927, 3.7e-7, 0.20563593, 0.00001906, 7.00497902, -0.00594749,
                48.33076593, -0.12534081, 77.45779628, 0.16047689, 252.2503235, 149472.67411175, 0.240847),
            new PLANETDATA2(
                "金星", 0.72333566, 0.0000039, 0.00677672, -0.00004107, 3.39467605, -0.0007889,
                76.67984255, -0.27769418, 131.60246718, 0.00268329, 181.9790995, 58517.81538729,0.615197),
            //new PLANETDATA2(
            //    "地球", 1.00000, 0.00000, 0.01670, 0.000000, 0.0026, 0.00000,
            //    174.8250, 0.00000, 103.0029, 0.00000, 99.8482, 0.00000, 1.000017),
            new PLANETDATA2(
                "火星", 1.52371034, 0.00001847, 0.0933941, 0.00007882, 1.84969142, -0.00813131,
                49.55953891, -0.29257343, -23.94362959, 0.44441088, -4.55343205, 19140.30268499, 1.880848),
            new PLANETDATA2(
                "木星", 5.202887, -0.00011607, 0.04838624, -0.00013253, 1.30439695, -0.00183714,
                100.47390909, 0.20469106, 14.72847983, 0.21252668, 34.39644051, 3034.74612775, 11.861983),
            new PLANETDATA2(
                "土星", 9.53667594, -0.0012506, 0.05386179, -0.00050991, 2.48599187, 0.00193609,
                113.66242448, -0.28867794, 92.59887831, -0.41897216, 49.95424423, 1222.49362201,29.457159),
            new PLANETDATA2(
                "天王星", 19.18916464, -0.00196176, 0.04725744, -0.00004397, 0.77263783, -0.00242939,
                74.01692503, 0.04240589, 170.9542763, 0.40805281, 313.23810451, 428.48202785,84.020473),
            new PLANETDATA2(
                "海王星", 30.06992276, 0.00026291, 0.00859048, 0.00005105, 1.77004347, 0.00035372,
                131.78422574, -0.00508664, 44.96476227, -0.32241464, -55.12002969, 218.45945325,164.770132),
            new PLANETDATA2(
                "冥王星", 39.48211675, -0.00031596,0.2488273, 0.0000517, 17.14001206, 0.00004818,
                110.30393684, -0.01183482, 224.06891629, -0.04062942, 238.92903833, 145.20780515, 247.7406624),
        };

        public List<PLANETDATA3> mPlanetData3 = new List<PLANETDATA3>() {
            //          惑星名, L 平均経度(deg), ω varpi 近日点経度(deg),Ω Omega 昇交点経度(deg),
            //          i 軌道傾斜角(deg), e 離心率,  a 軌道長半径(au), p 公転周期
            new PLANETDATA3(
                "太陽", 280.4665,0.985647358, 0.000303, 282.9373, 0.00004707, 0.000457,
                0.0, 0.0, 0.0, 0.0, 0.0, 0.016709, -0.00004204, 1.000001, 0.0, 1.000017),
            new PLANETDATA3(
                "月", 218.3166, 13.17639647, -0.001466, 83.3532, 0.111403522, -0.010321,
                125.0446, -0.052953765, 0.002075, 5.1566898, 0.0, 0.0555455, 0.0, 60.11257, 0.0, 0.0),
            new PLANETDATA3(
                "水星", 252.2509, 4.092377062, 0.000303, 77.4561, 1.556401, 0.000295,
                48.3309, 1.186112, 0.000175, 7.0050, 0.001821, 0.205632, 0.0002040, 0.0387098, 0.0, 0.240847),
            new PLANETDATA3(
                "金星", 181.9798, 1.602168732, 0.000310, 131.5637, 1.402152, -0.001076,
                76.6799, 0.901044, 0.000310, 3.3947, 0.001004, 0.006772, -0.00004778, 0.723330, 0.0, 0.615197),
            new PLANETDATA3(
                "火星", 355.4330, 0.524071085, 0.000311, 336.0602, 1.840968, 0.000135,
                49.5581, 0.772019, 0.0, 1.8497, -0.000601, 0.093401, 0.00009048, 1.523679, 0.0, 1.880848),
            new PLANETDATA3(
                "木星", 34.3515, 0.083129439, 0.000223, 14.3312, 1.612635, 0.001030,
                100.4644, 1.020977, 0.000403, 1.3033, -0.005496, 0.048498, 0.00016323, 5.202603, 0.0, 11.861983),
            new PLANETDATA3(
                "土星", 50.0774, 0.033497907, 0.000519, 93.0572, 1.963761, 0.000838,
                113.6655, 0.877088, -0.000121, 2.4889, -0.003736, 0.055548, -0.0003466, 9.554909,-0.0000021, 29.457159),
            new PLANETDATA3(
                "天王星", 314.0550, 0.011769036, 0.000304, 173.0053, 1.486378, 0.000214,
                74.0060,0.521127, 0.001339, 0.7732, 0.000774, 0.046381, -0.00002729, 19.218446, -0.000003, 84.020473),
            new PLANETDATA3(
                "海王星", 304.3487, 0.006020077, 0.000309, 48.1203, 1.426296, 0.000384,
                131.7841, 1.102204, 0.000260, 1.7700, -0.009308, 0.009456, 0.00000603, 30.110387, 0.0, 164.770132)
        };

        private double AU = 149597870691;           //  1天文単位(m)
        //  太陽中心重力定数 GM_Sun[m^3*s^-2]は万有引力定数G[m^3*kg^-1*s^-2]と太陽の質量M_Sun[kg]の積
        private double GM_Sun = 1.32712440018e+20;  //  太陽中心重力定数(m^3*s^-2)
        private double K0;                          //  ガウスの引力定数

        private YLib ylib = new YLib();

        public PlanetLib()
        {
            //  ガウスの引力定数
            K0 = Math.Sqrt(GM_Sun / (AU * AU * AU)) * 86400;
        }

        /// <summary>
        /// 惑星データの取得
        /// </summary>
        /// <param name="planetName">惑星名</param>
        /// <returns>惑星データ</returns>
        public PLANETDATA getPlanetData(string planetName, double jd = 0.0)
        {
            int index = mPlanetData.FindIndex((x) => x.name.CompareTo(planetName) == 0);
            if (0 <= index)
                return mPlanetData[index].getPlanetData(jd);
            else
                return null;
        }

        public PLANETDATA getPlanetData2(string planetName, double jd)
        {
            int index = mPlanetData2.FindIndex((x) => x.name.CompareTo(planetName) == 0);
            if (0 <= index)
                return mPlanetData2[index].getPlanetData(jd);
            else
                return null;
        }

        public PLANETDATA getPlanetData3(string planetName, double jd)
        {
            int index = mPlanetData3.FindIndex((x) => x.name.CompareTo(planetName) == 0);
            if (0 <= index)
                return mPlanetData3[index].getPlanetData(jd);
            else
                return null;
        }

        /// <summary>
        /// 惑星の赤経・赤緯を求める
        /// </summary>
        /// <param name="planetName">惑星名</param>
        /// <param name="jd">ユリウス日</param>
        /// <returns>赤道座標(rad)</returns>
        public PointD equatorialCoordinate(string planetName, double jd)
        {
            double T = (jd - 2451545.0) / 36525;        //  ユリウス世紀

            //  黄道傾斜角
            double epsilon = (84381.406 - 46.836769 * T - 0.00059 * Math.Pow(T, 2.0) + 0.001813 * Math.Pow(T, 3.0)) / 3600.0;
            epsilon = ylib.D2R(epsilon);    //  歳差を考慮しない時は 84381.406 / 3600 (deg) = 0.409092600600583(rad)

            //  地球の黄道座標
            PLANETDATA earthData = getPlanetData("地球");
            Point3D earthPos = earthData.getPlanetPos(jd);

            //  対象惑星の黄道座標
            PLANETDATA planetData = getPlanetData(planetName);      //  惑星データ
            Point3D planetPos = planetData.getPlanetPos(jd);        //  日心黄道座標(xyz)

            //  赤道座標に変換
            return ecliptic2equatorial(planetPos.sub(earthPos), epsilon);
        }

        /// <summary>
        /// 黄道座標を赤道座標に変換
        /// </summary>
        /// <param name="ecliptic">黄道座標(xyz)</param>
        /// <param name="epsilon">黄道傾斜角(rad)</param>
        /// <returns>赤道座標(赤経ra、赤緯dec)(rad)</returns>
        public PointD ecliptic2equatorial(Point3D ecliptic, double epsilon)
        {
            //  赤道座標に変換
            double x = ecliptic.x;
            double y = ecliptic.y * Math.Cos(epsilon) - ecliptic.z * Math.Sin(epsilon);
            double z = ecliptic.y * Math.Sin(epsilon) + ecliptic.z * Math.Cos(epsilon);
            double ra = y < 0 ? 2.0 * Math.PI + Math.Atan2(y, x) : Math.Atan2(y, x);
            double dec = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            //System.Diagnostics.Debug.WriteLine($"{planetName}: 赤経 {ylib.R2D(ra)},赤緯 {ylib.R2D(dec)}");
            return new PointD(ra, dec);     //  赤経(ra),赤緯(dec)
        }

        /// <summary>
        /// 地球の自転軸の傾き
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>傾き(rad)</returns>
        public double getEpslion(double jd)
        {
            double T = (jd - ylib.getJD(1899, 12, 31, 9)) / 365.25;
            return ylib.D2R(23.452294 - 0.0130125 * T - 0.00000164 * T * T + 0.000000503 * T * T * T);
        }

        /// <summary>
        /// 黄道座標を赤道座標に変換
        /// </summary>
        /// <param name="ecliptic">黄道座標(λ,β)(rad)</param>
        /// <param name="epslion">黄道傾斜角(rad)</param>
        /// <returns>赤道座標(ra,dec)(rad)</returns>
        public PointD ecliptic2equatorial(PointD ecliptic, double epslion)
        {
            double U = Math.Cos(ecliptic.y) * Math.Cos(ecliptic.x);
            double V = Math.Cos(ecliptic.y) * Math.Sin(ecliptic.x);
            double W = Math.Sin(ecliptic.y);

            double L = U;
            double M = V * Math.Cos(epslion) - W * Math.Sin(epslion);
            double N = V * Math.Sin(epslion) + W * Math.Cos(epslion);

            double ra = Math.Atan2(M, L);
            double dec = Math.Asin(N);

            if (ra < 0.0)
                ra += 2.0 * Math.PI;
            if (dec < 0.0)
                dec *= -1.0;
            return new PointD(ra, dec);
        }

        /// <summary>
        /// 月の黄経・黄緯を求める
        /// Tは1975年1月0日9時0分(世界時0時)とした日数を365.25日で割った値
        /// 「天体の位置計算」長沢工緒201pより「月の位置の略算法」
        /// </summary>
        /// <param name="T"></param>
        /// <returns>黄経・黄緯(λ,β)(rad)</returns>
        public PointD moonEclipticCoordinate(double jd)
        {
            double T = (jd - ylib.getJD(1974, 12, 31, 0)) / 365.25;
            double A;
            A  = 0.0040 * Math.Sin(ylib.D2R( 93.8 -  1.33 * T));
            A += 0.0020 * Math.Sin(ylib.D2R(248.6 - 19.34 * T));
            A += 0.0006 * Math.Sin(ylib.D2R( 66.0 +  0.2  * T));
            A += 0.0006 * Math.Sin(ylib.D2R(249.0 - 19.3  * T));

            double ramuda = 124.8754 + 4812.67881 * T;
            ramuda += 6.2887 * Math.Sin(ylib.D2R(338.915 + 4771.9886 * T + A));
            ramuda += 1.2740 * Math.Sin(ylib.D2R(107.248 - 4133.3536 * T));
            ramuda += 0.6583 * Math.Sin(ylib.D2R( 51.668 + 8905.3422 * T));
            ramuda += 0.2136 * Math.Sin(ylib.D2R(317.831 + 9543.9773 * T));
            ramuda += 0.1856 * Math.Sin(ylib.D2R(176.531 +  359.9905 * T));

            ramuda += 0.1143 * Math.Sin(ylib.D2R(292.463 +  9664.0404 * T));
            ramuda += 0.0588 * Math.Sin(ylib.D2R( 86.161 +   638.635  * T));
            ramuda += 0.0572 * Math.Sin(ylib.D2R(103.781 -  3773.363  * T));
            ramuda += 0.0533 * Math.Sin(ylib.D2R( 30.581 + 13677.331  * T));
            ramuda += 0.0459 * Math.Sin(ylib.D2R(124.861 -  8545.352  * T));

            ramuda += 0.0410 * Math.Sin(ylib.D2R(342.38  +  4411.998 * T));
            ramuda += 0.0348 * Math.Sin(ylib.D2R( 25.83  +  4452.671 * T));
            ramuda += 0.0305 * Math.Sin(ylib.D2R(155.45  +  5131.979 * T));
            ramuda += 0.0153 * Math.Sin(ylib.D2R(240.79  +   758.698 * T));
            ramuda += 0.0125 * Math.Sin(ylib.D2R(271.38  + 14436.029 * T));

            ramuda += 0.0110 * Math.Sin(ylib.D2R(226.45 -  4892.052 * T));
            ramuda += 0.0107 * Math.Sin(ylib.D2R( 55.58 - 13038.696 * T));
            ramuda += 0.0100 * Math.Sin(ylib.D2R(296.75 + 14315.966 * T));
            ramuda += 0.0085 * Math.Sin(ylib.D2R( 34.5  -  8266.71  * T));
            ramuda += 0.0079 * Math.Sin(ylib.D2R(290.7  -  4493.34  * T));

            ramuda += 0.0068 * Math.Sin(ylib.D2R(228.2  +  9265.33  * T));
            ramuda += 0.0052 * Math.Sin(ylib.D2R(133.1  +   319.32  * T));
            ramuda += 0.0050 * Math.Sin(ylib.D2R(202.4  +  4812.66  * T));
            ramuda += 0.0048 * Math.Sin(ylib.D2R( 68.6  -    19.34  * T));
            ramuda += 0.0040 * Math.Sin(ylib.D2R( 34.1  + 13317.34  * T));

            ramuda += 0.0040 * Math.Sin(ylib.D2R(  9.5 + 18449.32 * T));
            ramuda += 0.0040 * Math.Sin(ylib.D2R( 93.8 -     1.33 * T));
            ramuda += 0.0039 * Math.Sin(ylib.D2R(103.3 + 17810.68 * T));
            ramuda += 0.0037 * Math.Sin(ylib.D2R( 65.1 +  5410.62 * T));
            ramuda += 0.0027 * Math.Sin(ylib.D2R(321.3 +  9183.99 * T));

            ramuda += 0.0026 * Math.Sin(ylib.D2R(174.8 - 13797.39 * T));
            ramuda += 0.0024 * Math.Sin(ylib.D2R( 82.7 +   998.63 * T));
            ramuda += 0.0024 * Math.Sin(ylib.D2R(  4.7 +  9224.66 * T));
            ramuda += 0.0022 * Math.Sin(ylib.D2R(121.4 -  8185.36 * T));
            ramuda += 0.0021 * Math.Sin(ylib.D2R(134.4 +  9903.97 * T));

            ramuda += 0.0021 * Math.Sin(ylib.D2R(173.1 +   719.98 * T));
            ramuda += 0.0021 * Math.Sin(ylib.D2R(100.3 -  3413.37 * T));
            ramuda += 0.0020 * Math.Sin(ylib.D2R(248.6 -    19.34 * T));
            ramuda += 0.0018 * Math.Sin(ylib.D2R( 98.1 +  4013.29 * T));
            ramuda += 0.0016 * Math.Sin(ylib.D2R(344.1 + 18569.38 * T));

            ramuda += 0.0012 * Math.Sin(ylib.D2R( 52.1 - 12678.71 * T));
            ramuda += 0.0011 * Math.Sin(ylib.D2R(250.3 + 19208.02 * T));
            ramuda += 0.0009 * Math.Sin(ylib.D2R( 81.0 -  8586.0  * T));
            ramuda += 0.0008 * Math.Sin(ylib.D2R(207.0 + 14037.3  * T));
            ramuda += 0.0008 * Math.Sin(ylib.D2R( 31.0 -  7906.7  * T));

            ramuda += 0.0007 * Math.Sin(ylib.D2R(346.0 +  4052.0  * T));
            ramuda += 0.0007 * Math.Sin(ylib.D2R(294.0 -  4853.3  * T));
            ramuda += 0.0007 * Math.Sin(ylib.D2R( 90.0 +   278.6  * T));
            ramuda += 0.0006 * Math.Sin(ylib.D2R(237.0 +  1118.7  * T));
            ramuda += 0.0005 * Math.Sin(ylib.D2R( 82.0 + 22582.7  * T));

            ramuda += 0.0005 * Math.Sin(ylib.D2R(276.0 + 19088.0 * T));
            ramuda += 0.0005 * Math.Sin(ylib.D2R( 73.0 - 17450.7 * T));
            ramuda += 0.0005 * Math.Sin(ylib.D2R(112.0 +  5091.3 * T));
            ramuda += 0.0004 * Math.Sin(ylib.D2R(116.0 -   398.7 * T));
            ramuda += 0.0004 * Math.Sin(ylib.D2R( 25.0 -   120.1 * T));

            ramuda += 0.0004 * Math.Sin(ylib.D2R(181.0 +  9584.7 * T));
            ramuda += 0.0004 * Math.Sin(ylib.D2R( 18.0 +   720.0 * T));
            ramuda += 0.0003 * Math.Sin(ylib.D2R( 60.0 -  3814.0 * T));
            ramuda += 0.0003 * Math.Sin(ylib.D2R( 13.0 -  3494.7 * T));
            ramuda += 0.0003 * Math.Sin(ylib.D2R( 13.0 + 18089.3 * T));

            ramuda += 0.0003 * Math.Sin(ylib.D2R(152.0 +  5492.0 * T));
            ramuda += 0.0003 * Math.Sin(ylib.D2R(317.0 -    40.7 * T));
            ramuda += 0.0003 * Math.Sin(ylib.D2R(348.0 + 23221.3 * T));

            double B = 0.0267 * Math.Sin(ylib.D2R(68.64 - 19.341 * T));
            B += 0.0043 * Math.Sin(ylib.D2R(342.0 - 19.36 * T));
            B += 0.0040 * Math.Sin(ylib.D2R( 93.8 -  1.33 * T));
            B += 0.0020 * Math.Sin(ylib.D2R(248.6 - 19.34 * T));
            B += 0.0005 * Math.Sin(ylib.D2R(358.0 - 19.40 * T));

            double beta = 5.1282 * Math.Sin(ylib.D2R(236.231 + 4832.0202 * T + B));
            beta += 0.2806 * Math.Sin(ylib.D2R(215.147 + 9604.0088 * T));
            beta += 0.2777 * Math.Sin(ylib.D2R( 77.316 +   60.0316 * T));
            beta += 0.1732 * Math.Sin(ylib.D2R(  4.563 - 4073.3220 * T));
            beta += 0.0554 * Math.Sin(ylib.D2R(308.98  + 8965.374  * T));

            beta += 0.0463 * Math.Sin(ylib.D2R(343.48 +   698.667 * T));
            beta += 0.0326 * Math.Sin(ylib.D2R(287.90 + 13737.362 * T));
            beta += 0.0172 * Math.Sin(ylib.D2R(194.06 + 14375.997 * T));
            beta += 0.0093 * Math.Sin(ylib.D2R( 25.6  -  8845.31  * T));
            beta += 0.0088 * Math.Sin(ylib.D2R( 98.4  -  4711.96  * T));

            beta += 0.0082 * Math.Sin(ylib.D2R(  1.1  -  3713.33  * T));
            beta += 0.0043 * Math.Sin(ylib.D2R(322.4  +  5470.66 * T));
            beta += 0.0042 * Math.Sin(ylib.D2R(266.8  + 18509.35 * T));
            beta += 0.0034 * Math.Sin(ylib.D2R(188.0  -  4433.31 * T));
            beta += 0.0025 * Math.Sin(ylib.D2R(312.5  +  8605.38 * T));

            beta += 0.0022 * Math.Sin(ylib.D2R(291.4 + 13377.37 * T));
            beta += 0.0021 * Math.Sin(ylib.D2R(340.0 +  1058.66 * T));
            beta += 0.0019 * Math.Sin(ylib.D2R(218.6 +  9244.02 * T));
            beta += 0.0018 * Math.Sin(ylib.D2R(291.8 -  8206.68 * T));
            beta += 0.0018 * Math.Sin(ylib.D2R( 52.8 +  5192.01 * T));

            beta += 0.0017 * Math.Sin(ylib.D2R(168.7 + 14496.06 * T));
            beta += 0.0016 * Math.Sin(ylib.D2R( 73.8 +   420.02 * T));
            beta += 0.0015 * Math.Sin(ylib.D2R(262.1 +  9284.69 * T));
            beta += 0.0015 * Math.Sin(ylib.D2R( 31.7 +  9964.00 * T));
            beta += 0.0014 * Math.Sin(ylib.D2R(260.8 -   299.96 * T));

            beta += 0.0013 * Math.Sin(ylib.D2R(239.7 +  4472.03 * T));
            beta += 0.0013 * Math.Sin(ylib.D2R( 30.4 +   379.35 * T));
            beta += 0.0012 * Math.Sin(ylib.D2R(304.9 +  4812.68 * T));
            beta += 0.0012 * Math.Sin(ylib.D2R( 12.4 -  4851.36 * T));
            beta += 0.0011 * Math.Sin(ylib.D2R(173.0 + 19147.99 * T));

            beta += 0.0010 * Math.Sin(ylib.D2R(312.9 - 12978.66 * T));
            beta += 0.0008 * Math.Sin(ylib.D2R(  1.0 + 17870.70 * T));
            beta += 0.0008 * Math.Sin(ylib.D2R(190.0 +  9724.10 * T));
            beta += 0.0007 * Math.Sin(ylib.D2R( 22.0 + 13098.70 * T));
            beta += 0.0006 * Math.Sin(ylib.D2R(117.0 +  5590.70 * T));

            beta += 0.0006 * Math.Sin(ylib.D2R( 47.0 - 13617.30 * T));
            beta += 0.0005 * Math.Sin(ylib.D2R( 22.0 -  8465.30 * T));
            beta += 0.0005 * Math.Sin(ylib.D2R(150.0 +  4193.40 * T));
            beta += 0.0004 * Math.Sin(ylib.D2R(119.0 -  9483.90 * T));
            beta += 0.0004 * Math.Sin(ylib.D2R(246.0 + 23281.30 * T));

            beta += 0.0004 * Math.Sin(ylib.D2R(301.0 + 10242.60 * T));
            beta += 0.0004 * Math.Sin(ylib.D2R(126.0 +  9325.40 * T));
            beta += 0.0004 * Math.Sin(ylib.D2R(104.0 + 14097.40 * T));
            beta += 0.0003 * Math.Sin(ylib.D2R(340.0 + 22642.70 * T));
            beta += 0.0003 * Math.Sin(ylib.D2R(270.0 + 18149.40 * T));

            beta += 0.0003 * Math.Sin(ylib.D2R(358.0 -  3353.30 * T));
            beta += 0.0003 * Math.Sin(ylib.D2R(148.0 + 19268.00 * T));

            return new PointD(ylib.D2R(ramuda), ylib.D2R(beta));
        }

        /// <summary>
        /// 視差の正弦定数(the constant of sine parallax)
        /// 地球の赤道半径をae,地球と月との重心間距離rとすると sinπ =ae / r となり月までの距離が求められる
        /// 
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>視差の正弦定数(rad)</returns>
        public double constantSineParallax(double jd)
        {
            double T = (jd - ylib.getJD(1975, 12, 31, 9)) / 365.25;
            double sinePai = 0.9507;
            sinePai += 0.0518 * Math.Cos(ylib.D2R(338.92 +  4771.989 * T));
            sinePai += 0.0095 * Math.Cos(ylib.D2R(287.2  -  4133.35  * T));
            sinePai += 0.0078 * Math.Cos(ylib.D2R( 51.7  +  8905.34  * T));
            sinePai += 0.0028 * Math.Cos(ylib.D2R(317.8  +  9543.98  * T));
            sinePai += 0.0009 * Math.Cos(ylib.D2R( 31.0  + 13677.3   * T));

            sinePai += 0.0005 * Math.Cos(ylib.D2R(305.0 -  8545.4  * T));
            sinePai += 0.0004 * Math.Cos(ylib.D2R(284.0 -  3773.4  * T));
            sinePai += 0.0003 * Math.Cos(ylib.D2R(342.0 +  4412.0  * T));

            return ylib.D2R(sinePai);
        }
        
        /// <summary>
        /// 地球と月との重心距離
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>距離(km)</returns>
        public double moonDistance(double jd)
        {
            double ae = 6378.140;       //  地球の赤道半径
            return ae / constantSineParallax(jd);
        }

        /// <summary>
        /// kmを天文単位ら変換
        /// </summary>
        /// <param name="km">km</param>
        /// <returns>AU</returns>
        public double km2AU(double km)
        {
            return km / AU * 1000.0;
        }

        /// <summary>
        /// 天文単位をkm に変換
        /// </summary>
        /// <param name="au">AU</param>
        /// <returns>km</returns>
        public double AU2km(double au)
        {
            return au * AU / 1000.0;
        }
    }
}
