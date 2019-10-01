using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Graphics g;
        double level = 0,x1,x2;
        int F, A, I, M;
        int ScrollZoom = 1;
        int mult = 20;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            ScrollZoom = trackBar1.Value;
            button1_Click(sender, e);
        }

        float W, H;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Form1()
        {
            InitializeComponent();
            W = pictureBox1.Width;
            H = pictureBox1.Height;           
            g = pictureBox1.CreateGraphics();     
        }
            
        private void button1_Click(object sender, EventArgs e)
        {          
            try
            {           
                string str = "";
                //массив для хранения точек для построениея
                List<PointF> points = new List<PointF>();
                List<float> lambdas = new List<float>();
                List<PointF> grads = new List<PointF>();
                //ввод искходных данных
                F = GetNum(textBox1.Text[0]) ;
                A = GetNum(textBox1.Text[1]);
                I = GetNum(textBox2.Text[0]);
                M = GetNum(textBox2.Text[1]);
                x1 = int.Parse(textBox3.Text);
                x2 = int.Parse(textBox4.Text);
                level = F * I;

                double[] input = { F, -1 * A, I, M };
                double[] point = { x1, x2 };
                double[] grad = methodSteepestDescent.Grad(point, input);
                int i = 0;

                
                while (!(methodSteepestDescent.Stop(grad)))
                {
                    PointF pointG = new PointF((float)point[0], (float)point[1]); 
                    
                    points.Add(pointG);
                    grad = methodSteepestDescent.Grad(point, input);
                    double lambda = methodSteepestDescent.Step(grad, point, input);

                    str += "Шаг " + (i + 1) + Environment.NewLine + "X" + i + "(" + Math.Round(point[0], 6) + "; " + Math.Round(point[1], 6) + ")  " + Environment.NewLine +  "grad(X" + i + ") = " + "(" + Math.Round(grad[0], 6) + "; " + Math.Round(grad[1], 6) + ")  " + Environment.NewLine + "lambda = " + lambda + Environment.NewLine + Environment.NewLine;

                    point = methodSteepestDescent.NextPoint(point, grad, lambda);
                    grads.Add(new PointF((float)grad[0], (float)grad[1]));
                    lambdas.Add((float)lambda);
                    i++;
                }

                textBox5.Text = str;
                DrawCoord(points[points.Count - 1]);
                //рисование графика
                DrawXY(points, grads, lambdas);

                label5.Text = "F(X) = " + F + "(X1-" + A + ")^2+" + I + "(X2+" + M + ")^2";
                label3.Text = "Xmin( " + Math.Round(point[0], 6) + "; " + Math.Round(point[1], 6) + " )";
                
            }
            catch
            {               
                MessageBox.Show("Данные введены неверно!");
            }
        }

        private void DrawCoord(PointF point)
        {
            g.Clear(BackColor);
            g.ResetTransform();
            g.MultiplyTransform(new System.Drawing.Drawing2D.Matrix(1 * mult * ScrollZoom, 0, 0, -1 * mult * ScrollZoom, W / 2 - point.X * mult * ScrollZoom, H / 2 + point.Y *mult* ScrollZoom));

            g.DrawLine(new Pen(Color.Black, 0.03f), -W/2, 0, W/2, 0);
            g.DrawLine(new Pen(Color.Black, 0.03f), 0, -H/2, 0, H/2);
            float size = 0.12f;
            for (int i = (int)-W / 2; i < W / 2; i++)
            {
                g.FillEllipse(new SolidBrush(Color.Black), new RectangleF(i - size / 2, -size / 2, size, size));
            }
            for (int i = (int)-H / 2; i < H / 2; i++)
                g.FillEllipse(new SolidBrush(Color.Black), new RectangleF( -size/2, i-size/2, size, size));
            g.MultiplyTransform(new System.Drawing.Drawing2D.Matrix(1,0,0,-1,0,0));
            for (int i = (int)-W/2; i < W/2; i++)
            {
                if (i!=0)
                {
                g.DrawString(i.ToString(), new Font("Arial", 0.5f), new SolidBrush(Color.Black), i - 0.25f, 0.25f);
                }
            }
            for (int i = (int)-H / 2; i < H / 2; i++)
            {
                if (i != 0)
                {
                    g.DrawString((-i).ToString(), new Font("Arial", 0.5f), new SolidBrush(Color.Black), 0.25f,i - 0.25f );
                }
            }
            g.MultiplyTransform(new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 0));
        }

        private void DrawXY(List<PointF> points, List<PointF> lambda, List<float> grad)
        {
            float width, height;

            for (int i = 0; i < points.Count ; i++)
            {
                
                width = (float)Math.Sqrt(level / F) ;
                height = (float)Math.Sqrt(level / I) ;
                level = F * (points[i].X - A) * (points[i].X - A) + I * (points[i].Y + M) * (points[i].Y + M);
                
                g.DrawEllipse(new Pen(Color.Black, 0.03f / ScrollZoom), (float)A - width, - (float)M - height , width * 2f  , height * 2f );
                
                if (i != points.Count - 1)
                {               
                    g.DrawLine(new Pen(Color.Blue, 0.03f / ScrollZoom), points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
                }

            }
            
        }

        private int GetNum(char letter)
        {
            int number = 0;
            if (letter == 'ё')
                number = 7;
            else {
                number = letter - 'а' + 1;
                if (letter >= 'ж')
                    number++;
            }  
            return number;
        }
    }

    internal class methodSteepestDescent
    {
        //храниение шага
        private static double _lambda;
        //точность вычислений
        private static double epsl = 0.001;

        //проверка критерия остановки
        static public bool Stop(double[] grad)
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
            double lambda = 
            (grad[0] * data[0] * (point[0] + data[1]) + data[2] * grad[1] * (point[1] + data[3])) / (data[0] * grad[0] * grad[0] + data[2] * grad[1] * grad[1]);
            _lambda = lambda;
            return lambda;
        }
    }
}
