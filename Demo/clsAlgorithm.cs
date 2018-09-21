using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Demo
{
    class clsAlgorithm
    {
        double[,] Matric = new double[4, 4] { { 0.997, 0.073, 0.001, -40.502 }, { -0.073, 0.997, -0, -12.458 }, { -0.001, 0, 1.0, -0.153 }, { 0.0, 0.0, 0.0, 1 } };
        

        public void MatricOperation()
        {
            double[,] Point = new double[4,1] { { 1 }, { 2 }, { 3 },{ 4 } };
            var mb1 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var fromArray = mb1.DenseOfArray(Matric);

            var mb2 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var fromArray2 = mb1.DenseOfArray(Point);


            var res = fromArray * fromArray2;
            double x = 0.997 * 1 + 0.073 * 2 + 0.001 * 3 - 40.502 * 4;
        }
    }
}
