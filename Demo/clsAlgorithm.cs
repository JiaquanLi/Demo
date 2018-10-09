using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Demo
{
    class clsAlgorithm
    {
        //double[,] Matrix = new double[4, 4] { { 0.997, 0.073, 0.001, -40.502 }, { -0.073, 0.997, -0, -12.458 }, { -0.001, 0, 1.0, -0.153 }, { 0.0, 0.0, 0.0, 1 } };

        public static double[,] MatrixOperation(double[,] MatrixArry, double[,] fromData)
        {
            //double[,] Point = new double[4, 1] { { 1 }, { 2 }, { 3 }, { 4 } };
            var mb1 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var Matrix1 = mb1.DenseOfArray(MatrixArry);

            var mb2 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var Matrix2 = mb1.DenseOfArray(fromData);

            var dataGet = Matrix1 * Matrix2;

            return dataGet.ToArray();

        }
    }
}
