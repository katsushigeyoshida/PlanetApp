using CoreLib;
using System;
using System.Collections.Generic;

namespace PlanetApp
{

    public class PlanetLib
    {
        //  「理科年表2021」のデータ
        public List<PLANETDATA> mPlanetData = new List<PLANETDATA>() {
            //  惑星名, 軌道長半径a, 離心率e, 黄道軌道傾斜i, 近日点黄経ω, 昇交点黄経Ω, 元期平均近点角M0, 公転周期, 元期(JD)
            new PLANETDATA("水星",    0.3871, 0.2056, 7.004,  77.490,  48.304, 282.128,  0.24085, 2459400.5),
            new PLANETDATA("金星",    0.7233, 0.0068, 3.694, 131.565,  76.620,  35.951,  0.61520, 2459400.5),
            new PLANETDATA("地球",    1.0000, 0.0167, 0.003, 103.007, 174.821, 179.912,  1.00002, 2459400.5),
            new PLANETDATA("火星",    1.5237, 0.0934, 1.848, 336.156,  49.495, 175.817,  1.88085, 2459400.5),
            new PLANETDATA("木星",    5.2026, 0.0485, 1.303,  14.378, 100.502, 312.697,  11.8620, 2459400.5),
            new PLANETDATA("土星",    9.5549, 0.0555, 2.489,  93.179, 113.610, 219.741,  29.4572, 2459400.5),
            new PLANETDATA("天王星", 19.2184, 0.0464, 0.773, 173.024,  74.022, 233.182,  84.0205, 2459400.5),
            new PLANETDATA("海王星", 30.1104, 0.0095, 1.770,  48.127, 131.783, 303.212, 164.7701, 2459400.5)
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
        public PLANETDATA getPlanetData(string planetName)
        {
            int index = mPlanetData.FindIndex((x) => x.name.CompareTo(planetName) == 0);
            if (0 <= index)
                return mPlanetData[index];
            else
                return null;
        }

        /// <summary>
        /// 惑星の赤経・赤緯を求める
        /// </summary>
        /// <param name="planetName">惑星名</param>
        /// <param name="jd">ユリウス日</param>
        /// <returns>赤道座標(rad)</returns>
        public PointD equatorialCoordeNate(string planetName, double jd)
        {
            double T = (jd - 2451545.0) / 36525;        //  ユリウス世紀
            //  黄道傾斜角
            double epsilon = (84381.406 - 46.836769 * T - 0.00059 * Math.Pow(T, 2.0) + 0.001813 * Math.Pow(T, 3.0)) / 3600.0;
            epsilon = ylib.D2R(epsilon);    //  歳差を考慮しない時は 84381.406 / 3600 (deg) = 0.409092600600583(rad)

            //  地球の黄道座標
            PLANETDATA earthData = getPlanetData("地球");
            Point3D earthPos = earthData.getPlanetPos(jd);

            //  対象惑星の黄道座標
            PLANETDATA planetData = getPlanetData(planetName);
            Point3D planetPos = planetData.getPlanetPos(jd);

            //  赤道座標に変換
            double x = planetPos.x - earthPos.x;
            double y = (planetPos.y - earthPos.y) * Math.Cos(epsilon) - (planetPos.z - earthPos.z) * Math.Sin(epsilon);
            double z = (planetPos.y - earthPos.y) * Math.Sin(epsilon) + (planetPos.z - earthPos.z) * Math.Cos(epsilon);
            double ra = y < 0 ? 2.0 * Math.PI + Math.Atan2(y, x) : Math.Atan2(y, x);
            double dec = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            //System.Diagnostics.Debug.WriteLine($"{planetName}: 赤経 {ylib.R2D(ra)},赤緯 {ylib.R2D(dec)}");
            return new PointD(ra, dec);     //  赤経(ra),赤緯(dec)
        }
    }
}
