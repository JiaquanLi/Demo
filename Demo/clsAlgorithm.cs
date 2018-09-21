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
        //double[,] Matric = new double[4, 4] { { 0.997, 0.073, 0.001, -40.502 }, { -0.073, 0.997, -0, -12.458 }, { -0.001, 0, 1.0, -0.153 }, { 0.0, 0.0, 0.0, 1 } };
        

        public void MatricOperation(double[,] matricArry)
        {
            double[,] Point = new double[4,1] { { 1 }, { 2 }, { 3 },{ 4 } };
            var mb1 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var matric = mb1.DenseOfArray(matricArry);

            var mb2 = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var fromArray2 = mb1.DenseOfArray(Point);


        }
    }
}
