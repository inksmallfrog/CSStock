using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEditor
{
    class Function {
        public static Decimal pow(Decimal o1, Decimal o2) {
            int t = (int)o2;
            Decimal result = 1;
            while (t > 0) {
                result *= o1;
                t--;
            }
            return result;
        }
        public static decimal ACOS(decimal x) {
            return (decimal)Math.Acos((double)x);
        }
        public static decimal ASIN(decimal x) {
            return (decimal)Math.Asin((double)x);
        }
        public static decimal ATAN(decimal x) {
            return (decimal)Math.Atan((double)x);
        }
        public static decimal COS(decimal x) {
            return (decimal)Math.Cos((double)x);
        }
        public static decimal SIN(decimal x) {
            return (decimal)Math.Sin((double)x);
        }
        public static decimal TAN(decimal x) {
            return (decimal)Math.Tan((double)x);
        }
        public static decimal EXP(decimal x) {
            return (decimal)Math.Exp((double)x);
        }
        public static decimal LN(decimal x) {
            return (decimal)Math.Log((double)x, Math.E);
        }
        public static decimal LOG(decimal x) {
            return (decimal)Math.Log10((double)x);
        }
        public static decimal SQRT(decimal x) {
            return (decimal)Math.Sqrt((double)x);
        }
        public static decimal ABS(decimal x) {
            return (decimal)Math.Abs((double)x);
        }
        public static decimal CEILING(decimal x) {
            return Math.Ceiling(x);
        }
        public static decimal FLOOR(decimal x) {
            return Math.Floor(x);
        }
        public static decimal INTPART(decimal x) {
            if (x <= 0) {
                return -Math.Floor(ABS(x));
            }
            else {
                return Math.Floor(x);
            }
        }
        public static decimal FRACPART(decimal x) {
            return (x - INTPART(x));
        }
        public static decimal ROUND(decimal x) {
            return Math.Round(x, MidpointRounding.AwayFromZero);
        }
        public static decimal SIGN(decimal x) {
            if (x > 0) {
                return 1;
            }
            if (x == 0) {
                return 0;
            }
            else {
                return -1;
            }
        }
        public static decimal MOD(decimal a, decimal b) {
            return a % b;
        }
        public static decimal RAND(decimal x) {
            Random rd = new Random();
            return rd.Next(1, (int)x);
        }
        public static decimal MAX(decimal a, decimal b) {
            return Math.Max(a, b);
        }
        public static decimal MIN(decimal a, decimal b) {
            return Math.Min(a, b);
        }
    }
}
