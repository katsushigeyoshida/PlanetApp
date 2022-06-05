using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetApp
{
    //  AstroCommonsの惑星データの保存構造体
    //  AstroCommons 惑星データ(http://astro.starfree.jp/commons/astrometry/orbit.html)
    //  2000年1月1日正午を起点としたユリウス世紀
    public class PLANETDATA2
    {
        public string name = "";    //  惑星名
        double a0 = 0.0;            //  軌道長半径(au)
        double a1 = 0.0;
        double e0 = 0.0;            //  離心率
        double e1 = 0.0;
        double i0 = 0.0;            //  軌道傾斜角(deg)
        double i1 = 0.0;
        double Omega0 = 0.0;        //  昇交点経度(黄経)(deg)
        double Omega1 = 0.0;
        double varpi0 = 0.0;        //  近日点経度(deg)
        double varpi1 = 0.0;
        double L0 = 0.0;            //  平均経度(deg)
        double L1 = 0.0;
        double p = 0.0;             //  公転周期(年)

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="omgea0"></param>
        /// <param name="omega1"></param>
        /// <param name="varpi0"></param>
        /// <param name="varpi1"></param>
        /// <param name="L0"></param>
        /// <param name="L1"></param>
        /// <param name="p"></param>
        public PLANETDATA2(string name, double a0, double a1, double e0, double e1, double i0, double i1,
                    double omgea0, double omega1, double varpi0, double varpi1, double L0, double L1, double p)
        {
            this.name = name;
            this.a0 = a0;
            this.a1 = a1;
            this.e0 = e0;
            this.e1 = e1;
            this.i0 = i0;
            this.i1 = i1;
            this.varpi0 = varpi0;
            this.varpi1 = varpi1;
            this.Omega0 = omgea0;
            this.Omega1 = omega1;
            this.L0 = L0;
            this.L1 = L1;
            this.p = p;
        }

        /// <summary>
        /// PLANETDATA形式に変換
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public PLANETDATA getPlanetData(double jd)
        {
            //double jd = ylib.getJD(2000, 1, 1, 12);
            double T_century = (jd - 2451545.0) / 36525;
            double a = a0 + a1 * T_century;
            double e = e0 + e1 * T_century;
            double i = i0 + i1 * T_century;
            double varpi = varpi0 + varpi1 * T_century;
            double Omega = Omega0 + Omega1 * T_century;
            double L = L0 + L1 * T_century;
            double M = L - varpi;
            PLANETDATA planetData = new PLANETDATA(
            //  惑星名,軌道長半径,軌道傾斜角,近日点経度,昇交点経度,平均近点離角,公転周期,元期
                name, a, e, i, varpi, Omega, M, p, jd
            );
            return planetData;
        }
    }
}
