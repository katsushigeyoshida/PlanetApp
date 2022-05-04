using CoreLib;
using System;

namespace PlanetApp
{
    public class PLANETDATA
    {
        //  軌道要素 2000年月1月1.5日(JD=2451545.0) 理科年表2021
        public string name = "地球";          //  惑星名
        public double a = 1.0;                  //  軌道長半径 semi-major axis(au)
        double e = 0.0167;                      //  離心率 eccentricity
        double i = 0.003 * Math.PI / 180.0;     //  軌道傾斜 orbital inclination (黄道面)(rad)
        double w = 103.007 * Math.PI / 180.0;   //  ω 近日点黄経 longitude of periheion (rad)
        double W = 174.821 * Math.PI / 180.0;   //  Ω 昇交点黄経 ascending node (rad)
        double M0 = 179.912 * Math.PI / 180.0;  //  元期平均近点角(rad)
        double P = 1.0;                         //  公転周期 orbital period (ユリウス年365.25日))
        double TT = 2459400.5;                  //  元期(epoch) 2021年7月5日のユリウス日

        double[,] M = new double[3, 2];
        YLib ylib = new YLib();

        public PLANETDATA(string name, double a, double e, double i, double w, double W, double m0, double p, double tt)
        {
            this.name = name;
            this.a = a;
            this.e = e;
            this.i = ylib.D2R(i);
            this.w = ylib.D2R(w);
            this.W = ylib.D2R(W);
            this.M0 = ylib.D2R(m0);
            this.P = p;
            this.TT = tt;
            //this.w -= this.W;   //  近日点黄経を近日点引数に変換

            setMatrixPara();
        }

        /// <summary>
        /// 公転周期
        /// </summary>
        /// <returns>公転周期(日)</returns>
        public double periodDays()
        {
            return P * 365.25;
        }

        /// <summary>
        /// M 平均近点角
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>平均近点角(rad)(</returns>
        public double meanAnomary(double jd)
        {
            //double n = 0.9856076700000015 / Math.Pow(a, 1.5); //  deg
            double n = 0.01720209895 / Math.Pow(a, 1.5);        //  rad (0.01720209895 ガウス引力定数)
            return Math.Abs(M0 + n * (jd - TT));
        }

        /// <summary>
        /// E 離心近点角(rad)を求める
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>離心近点角(rad)</returns>
        public double eccentricAnomary(double jd)
        {
            double M = meanAnomary(jd);
            return kepler(M, e);
        }

        /// <summary>
        /// 日心軌道面座標を求める
        /// </summary>
        /// <param name="E">離心近点角(rad)</param>
        /// <returns>日心軌道面座標</returns>
        public PointD heriocentricOrbit(double E)
        {
            double b = a * Math.Sqrt(1 - e * e);
            return new PointD(a * (Math.Cos(E) - e), b * Math.Sin(E));
        }

        /// <summary>
        /// 日心軌道面座標から日心黄道座標に変換する
        /// </summary>
        /// <param name="op">日心軌道面座標</param>
        /// <returns>日心黄道座標</returns>
        public Point3D heriocentricOrbit2Ecliptic(PointD op)
        {
            Point3D ep = new Point3D();
            ep.x = M[0, 0] * op.x + M[0, 1] * op.y;
            ep.y = M[1, 0] * op.x + M[1, 1] * op.y;
            ep.z = M[2, 0] * op.x + M[2, 1] * op.y;
            return ep;
        }

        /// <summary>
        /// 惑星の位置(日心黄道座標)
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>黄道座標</returns>
        public Point3D getPlanetPos(double jd)
        {
            double M = meanAnomary(jd);
            double E = eccentricAnomary(jd);
            PointD op = heriocentricOrbit(E);

            return heriocentricOrbit2Ecliptic(op);
        }

        /// <summary>
        /// 軌道面座標を黄道面座標に変換するパラメータをセット
        /// </summary>
        private void setMatrixPara()
        {
            double cos_i = Math.Cos(i);             //  軌道傾斜角
            double sin_i = Math.Sin(i);
            double cos_Omega = Math.Cos(W);         //  昇交点経度
            double sin_Omega = Math.Sin(W);
            double cos_omega = Math.Cos(w - W);     //  近日点経度(近日点黄経w - 昇交点経度W)
            double sin_omega = Math.Sin(w - W);
            M[0, 0] =  cos_omega * cos_Omega - sin_omega * sin_Omega * cos_i;
            M[0, 1] = -sin_omega * cos_Omega - cos_omega * sin_Omega * cos_i;
            M[1, 0] =  cos_omega * sin_Omega + sin_omega * cos_Omega * cos_i;
            M[1, 1] = -sin_omega * sin_Omega + cos_omega * cos_Omega * cos_i;
            M[2, 0] =  sin_omega * sin_i;
            M[2, 1] =  cos_omega * sin_i;
        }

        /// <summary>
        /// ケプラーの方程式で離心近点角を求める
        /// </summary>
        /// <param name="M">平均近点角(rad)</param>
        /// <param name="e">離心率</param>
        /// <returns>E 離心近点角(rad)</returns>
        double kepler(double M, double e)
        {
            double E0 = M;      //  M 平均近点角(rad)
            double delta_E;
            double E;           //  E 離心近点角(rad)
            do {
                delta_E = (M - E0 + e * Math.Sin(E0)) / (1 - e * Math.Cos(E0));
                E = E0 + delta_E;
                E0 = E;
                // Console.WriteLine($"離心近点角[rad] {E}");
            } while (Math.Abs(delta_E) > 0.00001);
            return E;
        }
    }
}
