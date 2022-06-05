using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetApp
{
    //  天文年鑑2020のデータをベースにした惑星データ構造
    public class PLANETDATA3
    {
        public string name;     //  惑星名
        public double L0;       //  L 平均黄経(平均近点角M = 平均黄経L - 近日点黄経π)
        public double L1;
        public double L2;
        public double w0;       //  π 近地点黄経/近日点黄経(近日点引数ω = 近日点黄経π - 昇交点黄経Ω)
        public double w1;
        public double w2;
        public double W0;       //  Ω 昇交点黄経
        public double W1;
        public double W2;
        public double i0;       //  i 軌道傾斜角
        public double i1;
        public double e0;       //  e 離心率
        public double e1;
        public double a0;       //  a 軌道長半径
        public double a1;
        public double p;        //  p 公転周期

        public PLANETDATA3(string name, double l0, double l1, double l2, double w0, double w1, double w2,
                    double W0, double W1, double W2, double i0, double i1, double e0, double e1, 
                    double a0, double a1, double p)
        {
            this.name = name;
            this.L0 = l0;
            this.L1 = l1;
            this.L2 = l2;
            this.w0 = w0;
            this.w1 = w1;
            this.w2 = w2;
            this.W0 = W0;
            this.W1 = W1;
            this.W2 = W2;
            this.i0 = i0;
            this.i1 = i1;
            this.e0 = e0;
            this.e1 = e1;
            this.a0 = a0;
            this.a1 = a1;
            this.p = p;
        }

        public PLANETDATA getPlanetData(double jd)
        {
            double d = jd - 2451545.0;
            double T = d / 36525;
            double L = L0 + L1 * d + L2 * T * T;
            double w = w0 + w1 * T + w2 * T * T;
            double W = W0 + W1 * T + W2 * T * T;
            double i = i0 + i1 * T;
            double e = e0 + e1 * T;
            double a = a0 + a1 * T;
            if (name.CompareTo("月") == 0) {
                a = a * 6378136.0 / 1.495978707e11;
            }
            return new PLANETDATA(name, a, e, i, w, W, L - w, p, jd);
        }

    }
}