using IntXLib;
using System.Text;

namespace BigRationalLib
{
    public struct BigRational :
        IEquatable<BigRational>, IComparable<BigRational>,
        IEquatable<IntX>, IComparable<IntX>

    {
        #region IntXExtended
        private static IntX IntXAbs(in IntX a)
        {
            return a < 0 ? -a : a;
        }

        private static IntX GCD(IntX a, IntX b)
        {
            while (b != 0)
            {
                IntX t = a % b;
                b = a;
                a = t;
            }
            return a;
        }
        #endregion

        #region Constructors
        public BigRational()
        {
            Sign = 0;
            Numerator = 0;
            Denominator = 1;
        }

        public BigRational(in IntX numerator, in IntX denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator can't be 0.");

            Sign = numerator.CompareTo(0) * denominator.CompareTo(0);
            Numerator = Sign * numerator;
            Denominator = IntXAbs(denominator);
            Factor();
        }

        public BigRational(in IntX numerator)
        {
            Sign = numerator.CompareTo(0);
            Numerator = numerator;
            Denominator = 1;
        }
        #endregion

        #region Members
        public int Sign { get; private set; }
        public IntX Numerator { get; private set; }
        public IntX Denominator { get; private set; }
        #endregion

        #region Private Methods
        private void Factor()
        {
            IntX gcd = GCD(Numerator, Denominator);
            Numerator /= gcd;
            Denominator /= gcd;
        }
        #endregion

        #region Operators
        public static implicit operator BigRational(int n) => new(n);
        public static implicit operator BigRational(long n) => new(n);
        public static implicit operator BigRational(in IntX n) => new(n);
        public static explicit operator IntX(in BigRational n) => n.Numerator / n.Denominator;

        public static BigRational operator +(in BigRational a, in BigRational b) => Add(a, b);
        public static BigRational operator -(in BigRational a, in BigRational b) => Subtract(a, b);
        public static BigRational operator *(in BigRational a, in BigRational b) => Multiply(a, b);
        public static BigRational operator /(in BigRational a, in BigRational b) => Divide(a, b);
        public static bool operator ==(in BigRational a, in BigRational b) => a.Equals(b);
        public static bool operator !=(in BigRational a, in BigRational b) => !a.Equals(b);
        public static bool operator >(in BigRational a, in BigRational b) => a.CompareTo(b) > 0;
        public static bool operator <(in BigRational a, in BigRational b) => a.CompareTo(b) < 0;
        public static bool operator >=(in BigRational a, in BigRational b) => a.CompareTo(b) >= 0;
        public static bool operator <=(in BigRational a, in BigRational b) => a.CompareTo(b) <= 0;
        public static BigRational operator ++(in BigRational a) => Add(a, 1);
        public static BigRational operator --(in BigRational a) => Subtract(a, 1);
        #endregion

        #region Static Methods
        public static BigRational Add(in BigRational a, in BigRational b)
        {
            return new BigRational(
                a.Numerator * b.Denominator + b.Numerator * a.Denominator,
                a.Denominator * b.Denominator);
        }

        public static BigRational Subtract(in BigRational a, in BigRational b)
        {
            return new BigRational(
                a.Numerator * b.Denominator - b.Numerator * a.Denominator,
                a.Denominator * b.Denominator);
        }

        public static BigRational Multiply(in BigRational a, in BigRational b)
        {
            return new BigRational(
                a.Numerator * b.Numerator,
                a.Denominator * b.Denominator);
        }

        public static BigRational Divide(in BigRational a, in BigRational b)
        {
            return new BigRational(
                a.Numerator * b.Denominator,
                b.Numerator * a.Denominator);
        }

        public static BigRational Abs(in BigRational a)
        {
            return new BigRational(IntXAbs(a.Numerator), a.Denominator);
        }

        public static BigRational Parse(string s)
        {
            string[] fraction = s.Split(" / ");
            if (fraction.Length != 2)
                throw new ArgumentException("string is not a valid fraction.");
            IntX numerator = IntX.Parse(fraction[0]);
            IntX denominator = IntX.Parse(fraction[1]);
            return new BigRational(numerator, denominator);
        }
        #endregion

        #region Public Methods
        public string ToString(long decimals)
        {
            StringBuilder sb = new();
            if (Sign == -1)
                sb.Append('-');

            IntX tNum = IntXAbs(Numerator);
            IntX tDen = Denominator;

            IntX whole = IntX.DivideModulo(tNum, tDen, out IntX rem);
            tNum = rem;
            sb.Append(whole.ToString());

            if (rem == 0) return sb.ToString();

            sb.Append('.');
            for (long i = 0; i < decimals; ++i)
            {
                tNum *= 10;
                sb.Append(IntX.DivideModulo(tNum, tDen, out IntX rem2));
                tNum = rem2;
                if (tNum == 0)
                    break;
            }

            return sb.ToString();
        }
        
        public override string ToString()
        {
            return ToString(100);
        }

        public string ToRationalString()
        {
            return string.Concat(Numerator.ToString(), " / ", Denominator.ToString());
        }
        #endregion

        #region Interface Implementations
        public bool Equals(BigRational other)
        {
            return (Numerator == other.Numerator) && (Denominator == other.Denominator);
        }

        public int CompareTo(BigRational other)
        {
            IntX w1 = IntX.DivideModulo(Numerator, Denominator, out IntX r1);
            IntX w2 = IntX.DivideModulo(other.Numerator, other.Denominator, out IntX r2);

            if (w1 > w2) return 1;
            else if (w1 < w2) return -1;

            if (r1 > r2) return 1;
            else if (r1 < r2) return -1;

            return 0;
        }

        public bool Equals(IntX? other)
        {
            return (other is not null) && Denominator == 1 && Numerator == other;
        }

        public int CompareTo(IntX? other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            return CompareTo((BigRational)other);
        }

        public override bool Equals(object? obj)
        {
            if (obj is BigRational r)
                return Equals(r);
            else if (obj is IntX i)
                return Equals(i);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode() ^ Denominator.GetHashCode();
        }
        #endregion
    }
}