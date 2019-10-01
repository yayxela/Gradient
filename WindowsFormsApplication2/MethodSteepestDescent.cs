using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class MethodSteepestDescent
    {
        //храниение шага
        private static double _lambda;
        //точность вычислений
        private static double epsl = 0.001;

        //проверка критерия остановки
        static public  bool Stop(double[] grad)
        {
            return (Math.Sqrt(grad[0] * grad[0] + grad[1] * grad[1])) < epsl;
        }

        //вычисление градиента
        public static double[] Grad(double[] point, double[] data)
        {
            double[] grad = new double[2];

            grad[0] = data[0] * 2 * (point[0] + data[1]);
            grad[1] = data[2] * 2 * (point[1] + data[3]);

            return grad;
        }

        //вычисление следующей точке
        public static double[] NextPoint(double[] point, double[] grad, double lambda)
        {
            double[] newPoint = new double[2];
            newPoint[0] = point[0] - grad[0] * _lambda;
            newPoint[1] = point[1] - grad[1] * _lambda;

            return newPoint;
        }

        //вычисление шага
        public static double Step(double[] grad, double[] point, double[] data)
        {
            double a = 2 * data[0] * -1 * grad[0];
            double b = 2 * data[3] * -1 * grad[1];

            double lambda = (-1 * (a * (point[0] + data[1]) + b * (point[1] + data[3]))) / (-1 * a * grad[0] + -1 * b * grad[1]);

            _lambda = lambda;
            return lambda;
        }


    }
}
