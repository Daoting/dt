#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatrixMock
    {
        const int c_identityHashCode = 0;
        static MatrixMock s_identity;
        internal double _m11;
        internal double _m12;
        internal double _m21;
        internal double _m22;
        internal double _offsetX;
        internal double _offsetY;
        internal MatrixTypesMock _type;
        internal int _padding;
        public MatrixMock(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = MatrixTypesMock.TRANSFORM_IS_UNKNOWN;
            this._padding = 0;
            this.DeriveMatrixType();
        }

        public static MatrixMock Identity
        {
            get { return  s_identity; }
        }
        public void SetIdentity()
        {
            this._type = MatrixTypesMock.TRANSFORM_IS_IDENTITY;
        }

        public bool IsIdentity
        {
            get { return  ((this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY) || (((((this._m11 == 1.0) && (this._m12 == 0.0)) && ((this._m21 == 0.0) && (this._m22 == 1.0))) && (this._offsetX == 0.0)) && (this._offsetY == 0.0))); }
        }
        public static MatrixMock operator *(MatrixMock trans1, MatrixMock trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public static MatrixMock Multiply(MatrixMock trans1, MatrixMock trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public void Append(MatrixMock matrix)
        {
            this *= matrix;
        }

        public void Prepend(MatrixMock matrix)
        {
            this = matrix * this;
        }

        public void Rotate(double angle)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295);
        }

        public void RotatePrepend(double angle)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295) * this;
        }

        public void RotateAt(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY);
        }

        public void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY) * this;
        }

        public void Scale(double scaleX, double scaleY)
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        public void ScalePrepend(double scaleX, double scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        public void Skew(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this *= CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295);
        }

        public void SkewPrepend(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this = CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295) * this;
        }

        public void Translate(double offsetX, double offsetY)
        {
            if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
            {
                this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypesMock.TRANSFORM_IS_TRANSLATION);
            }
            else if (this._type == MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
            {
                this._offsetX += offsetX;
                this._offsetY += offsetY;
            }
            else
            {
                this._offsetX += offsetX;
                this._offsetY += offsetY;
                this._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
            }
        }

        public void TranslatePrepend(double offsetX, double offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        public Windows.Foundation.Point Transform(Windows.Foundation.Point point)
        {
            double x = point.X;
            double y = point.Y;
            this.MultiplyPoint(ref x, ref y);
            return new Windows.Foundation.Point(x, y);
        }

        public void Transform(Windows.Foundation.Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    this.MultiplyPoint(ref x, ref y);
                    points[i].X = x;
                    points[i].Y = y;
                }
            }
        }

        public double Determinant
        {
            get
            {
                switch (this._type)
                {
                    case MatrixTypesMock.TRANSFORM_IS_IDENTITY:
                    case MatrixTypesMock.TRANSFORM_IS_TRANSLATION:
                        return 1.0;

                    case MatrixTypesMock.TRANSFORM_IS_SCALING:
                    case (MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION):
                        return (this._m11 * this._m22);
                }
                return ((this._m11 * this._m22) - (this._m12 * this._m21));
            }
        }
        public bool HasInverse
        {
            get { return  (Math.Abs(this.Determinant) >= 2.2204460492503131E-15); }
        }
        public void Invert()
        {
            double determinant = this.Determinant;
            if (Math.Abs(determinant) < 2.2204460492503131E-15)
            {
                throw new InvalidOperationException(ResourceStrings.PdfInvertError);
            }
            switch (this._type)
            {
                case MatrixTypesMock.TRANSFORM_IS_IDENTITY:
                    break;

                case MatrixTypesMock.TRANSFORM_IS_TRANSLATION:
                    this._offsetX = -this._offsetX;
                    this._offsetY = -this._offsetY;
                    return;

                case MatrixTypesMock.TRANSFORM_IS_SCALING:
                    this._m11 = 1.0 / this._m11;
                    this._m22 = 1.0 / this._m22;
                    return;

                case (MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION):
                    this._m11 = 1.0 / this._m11;
                    this._m22 = 1.0 / this._m22;
                    this._offsetX = -this._offsetX * this._m11;
                    this._offsetY = -this._offsetY * this._m22;
                    return;

                default:
                {
                    double num2 = 1.0 / determinant;
                    this.SetMatrix(this._m22 * num2, -this._m12 * num2, -this._m21 * num2, this._m11 * num2, ((this._m21 * this._offsetY) - (this._offsetX * this._m22)) * num2, ((this._offsetX * this._m12) - (this._m11 * this._offsetY)) * num2, MatrixTypesMock.TRANSFORM_IS_UNKNOWN);
                    break;
                }
            }
        }

        public double M11
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0;
                }
                return this._m11;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_SCALING);
                }
                else
                {
                    this._m11 = value;
                    if (this._type != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypesMock.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }
        public double M12
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._m12;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    this._m12 = value;
                    this._type = MatrixTypesMock.TRANSFORM_IS_UNKNOWN;
                }
            }
        }
        public double M21
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._m21;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    this._m21 = value;
                    this._type = MatrixTypesMock.TRANSFORM_IS_UNKNOWN;
                }
            }
        }
        public double M22
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0;
                }
                return this._m22;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_SCALING);
                }
                else
                {
                    this._m22 = value;
                    if (this._type != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypesMock.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }
        public double OffsetX
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._offsetX;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, MatrixTypesMock.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    this._offsetX = value;
                    if (this._type != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }
        public double OffsetY
        {
            get
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._offsetY;
            }
            set
            {
                if (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, MatrixTypesMock.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    this._offsetY = value;
                    if (this._type != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }
        internal void MultiplyVector(ref double x, ref double y)
        {
            switch (this._type)
            {
                case MatrixTypesMock.TRANSFORM_IS_IDENTITY:
                case MatrixTypesMock.TRANSFORM_IS_TRANSLATION:
                    return;

                case MatrixTypesMock.TRANSFORM_IS_SCALING:
                case (MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION):
                    x *= this._m11;
                    y *= this._m22;
                    return;
            }
            double num = y * this._m21;
            double num2 = x * this._m12;
            x *= this._m11;
            x += num;
            y *= this._m22;
            y += num2;
        }

        internal void MultiplyPoint(ref double x, ref double y)
        {
            switch (this._type)
            {
                case MatrixTypesMock.TRANSFORM_IS_IDENTITY:
                    return;

                case MatrixTypesMock.TRANSFORM_IS_TRANSLATION:
                    x += this._offsetX;
                    y += this._offsetY;
                    return;

                case MatrixTypesMock.TRANSFORM_IS_SCALING:
                    x *= this._m11;
                    y *= this._m22;
                    return;

                case (MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION):
                    x *= this._m11;
                    x += this._offsetX;
                    y *= this._m22;
                    y += this._offsetY;
                    return;
            }
            double num = (y * this._m21) + this._offsetX;
            double num2 = (x * this._m12) + this._offsetY;
            x *= this._m11;
            x += num;
            y *= this._m22;
            y += num2;
        }

        internal static MatrixMock CreateRotationRadians(double angle)
        {
            return CreateRotationRadians(angle, 0.0, 0.0);
        }

        internal static MatrixMock CreateRotationRadians(double angle, double centerX, double centerY)
        {
            MatrixMock mock = new MatrixMock();
            double num = Math.Sin(angle);
            double num2 = Math.Cos(angle);
            double offsetX = (centerX * (1.0 - num2)) + (centerY * num);
            double offsetY = (centerY * (1.0 - num2)) - (centerX * num);
            mock.SetMatrix(num2, num, -num, num2, offsetX, offsetY, MatrixTypesMock.TRANSFORM_IS_UNKNOWN);
            return mock;
        }

        internal static MatrixMock CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            MatrixMock mock = new MatrixMock();
            mock.SetMatrix(scaleX, 0.0, 0.0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY), MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION);
            return mock;
        }

        internal static MatrixMock CreateScaling(double scaleX, double scaleY)
        {
            MatrixMock mock = new MatrixMock();
            mock.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_SCALING);
            return mock;
        }

        internal static MatrixMock CreateSkewRadians(double skewX, double skewY)
        {
            MatrixMock mock = new MatrixMock();
            mock.SetMatrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_UNKNOWN);
            return mock;
        }

        internal static MatrixMock CreateTranslation(double offsetX, double offsetY)
        {
            MatrixMock mock = new MatrixMock();
            mock.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypesMock.TRANSFORM_IS_TRANSLATION);
            return mock;
        }

        static MatrixMock CreateIdentity()
        {
            MatrixMock mock = new MatrixMock();
            mock.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypesMock.TRANSFORM_IS_IDENTITY);
            return mock;
        }

        void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, MatrixTypesMock type)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = type;
        }

        void DeriveMatrixType()
        {
            this._type = MatrixTypesMock.TRANSFORM_IS_IDENTITY;
            if ((this._m21 != 0.0) || (this._m12 != 0.0))
            {
                this._type = MatrixTypesMock.TRANSFORM_IS_UNKNOWN;
            }
            else
            {
                if ((this._m11 != 1.0) || (this._m22 != 1.0))
                {
                    this._type = MatrixTypesMock.TRANSFORM_IS_SCALING;
                }
                if ((this._offsetX != 0.0) || (this._offsetY != 0.0))
                {
                    this._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                }
                if ((this._type & (MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION)) == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    this._type = MatrixTypesMock.TRANSFORM_IS_IDENTITY;
                }
            }
        }

        void Debug_CheckType()
        {
        }

        bool IsDistinguishedIdentity
        {
            get { return  (this._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY); }
        }
        public static bool operator ==(MatrixMock matrix1, MatrixMock matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) && ((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22))) && (matrix1.OffsetX == matrix2.OffsetX)) && (matrix1.OffsetY == matrix2.OffsetY));
        }

        public static bool operator !=(MatrixMock matrix1, MatrixMock matrix2)
        {
            return !(matrix1 == matrix2);
        }

        public static bool Equals(MatrixMock matrix1, MatrixMock matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return ((((((double) matrix1.M11).Equals(matrix2.M11) && ((double) matrix1.M12).Equals(matrix2.M12)) && (((double) matrix1.M21).Equals(matrix2.M21) && ((double) matrix1.M22).Equals(matrix2.M22))) && ((double) matrix1.OffsetX).Equals(matrix2.OffsetX)) && ((double) matrix1.OffsetY).Equals(matrix2.OffsetY));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is MatrixMock))
            {
                return false;
            }
            MatrixMock mock = (MatrixMock) o;
            return Equals(this, mock);
        }

        public bool Equals(MatrixMock value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            if (this.IsDistinguishedIdentity)
            {
                return 0;
            }
            return (((((((double) this.M11).GetHashCode() ^ ((double) this.M12).GetHashCode()) ^ ((double) this.M21).GetHashCode()) ^ ((double) this.M22).GetHashCode()) ^ ((double) this.OffsetX).GetHashCode()) ^ ((double) this.OffsetY).GetHashCode());
        }

        static MatrixMock()
        {
            s_identity = CreateIdentity();
        }
    }
}

