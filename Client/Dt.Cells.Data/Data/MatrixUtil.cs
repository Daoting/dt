#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal static class MatrixUtil
    {
        internal static void MultiplyMatrix(ref MatrixMock matrix1, ref MatrixMock matrix2)
        {
            MatrixTypesMock mock = matrix1._type;
            MatrixTypesMock mock2 = matrix2._type;
            if (mock2 != MatrixTypesMock.TRANSFORM_IS_IDENTITY)
            {
                if (mock == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
                {
                    matrix1 = matrix2;
                }
                else if (mock2 == MatrixTypesMock.TRANSFORM_IS_TRANSLATION)
                {
                    matrix1._offsetX += matrix2._offsetX;
                    matrix1._offsetY += matrix2._offsetY;
                    if (mock != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                    }
                }
                else if (mock == MatrixTypesMock.TRANSFORM_IS_TRANSLATION)
                {
                    double num = matrix1._offsetX;
                    double num2 = matrix1._offsetY;
                    matrix1 = matrix2;
                    matrix1._offsetX = ((num * matrix2._m11) + (num2 * matrix2._m21)) + matrix2._offsetX;
                    matrix1._offsetY = ((num * matrix2._m12) + (num2 * matrix2._m22)) + matrix2._offsetY;
                    if (mock2 == MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type = MatrixTypesMock.TRANSFORM_IS_UNKNOWN;
                    }
                    else
                    {
                        matrix1._type = MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                    }
                }
                else
                {
                    switch (((int) (((MatrixTypesMock) (((int) mock) << 4)) | mock2)))
                    {
                        case 0x22:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            return;

                        case 0x23:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX = matrix2._offsetX;
                            matrix1._offsetY = matrix2._offsetY;
                            matrix1._type = MatrixTypesMock.TRANSFORM_IS_SCALING | MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                            return;

                        case 0x24:
                        case 0x34:
                        case 0x42:
                        case 0x43:
                        case 0x44:
                            matrix1 = new MatrixMock((matrix1._m11 * matrix2._m11) + (matrix1._m12 * matrix2._m21), (matrix1._m11 * matrix2._m12) + (matrix1._m12 * matrix2._m22), (matrix1._m21 * matrix2._m11) + (matrix1._m22 * matrix2._m21), (matrix1._m21 * matrix2._m12) + (matrix1._m22 * matrix2._m22), ((matrix1._offsetX * matrix2._m11) + (matrix1._offsetY * matrix2._m21)) + matrix2._offsetX, ((matrix1._offsetX * matrix2._m12) + (matrix1._offsetY * matrix2._m22)) + matrix2._offsetY);
                            return;

                        case 50:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX *= matrix2._m11;
                            matrix1._offsetY *= matrix2._m22;
                            return;

                        case 0x33:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX = (matrix2._m11 * matrix1._offsetX) + matrix2._offsetX;
                            matrix1._offsetY = (matrix2._m22 * matrix1._offsetY) + matrix2._offsetY;
                            break;

                        default:
                            return;
                    }
                }
            }
        }

        internal static void PrependOffset(ref MatrixMock matrix, double offsetX, double offsetY)
        {
            if (matrix._type == MatrixTypesMock.TRANSFORM_IS_IDENTITY)
            {
                matrix = new MatrixMock(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
                matrix._type = MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
            }
            else
            {
                matrix._offsetX += (matrix._m11 * offsetX) + (matrix._m21 * offsetY);
                matrix._offsetY += (matrix._m12 * offsetX) + (matrix._m22 * offsetY);
                if (matrix._type != MatrixTypesMock.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypesMock.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }
}

