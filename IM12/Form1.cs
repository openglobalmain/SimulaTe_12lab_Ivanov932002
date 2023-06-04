using System;
using System.Windows.Forms;
using System.IO;

namespace IM12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string Forecast;
        private void Start_Click(object sender, EventArgs e)
        {
            String filename = "User.txt";
            Imitation model = new Imitation((int)N.Value, (int)T.Value);
            model.GetValue();
            model.GetFreq();
            Sunny.Text = Convert.ToString(Math.Round(model.freq[0], 3));
            Windy.Text = Convert.ToString(Math.Round(model.freq[1], 3));
            Rainy.Text = Convert.ToString(Math.Round(model.freq[2], 3));
            
            System.Diagnostics.Process.Start(filename);

        }
        class Imitation
        {
            int N;
            int T;
            int i;
            double Time = 0;
            double t_new;
            double t0 = 0;
            int day = 1;
            int month = 06;
            int hour = 0;
            int minute;
            int c = 0;
            int c1 = 0;
            public string[] Weather = new string[] { "Солнечно", "Ветренно", "Дождливо" };
            public double[] freq = new double[3] { 0, 0, 0 };
            double[,] Matrix = new double[3, 3] { { -0.4, 0.3, 0.1 }, { 0.4, -0.8, 0.4 }, { 0.1, 0.4, -0.5 } };
            double[] P = new double[] { 0.0, 0.0, 0.0 };
            Random rnd = new Random();
            public Imitation(int n, int t)
            {
                N = n;
                T = t;
            }

            public void GetStart()
            {
                i = 0;
                while (Time < T)
                {
                    double a = rnd.NextDouble();
                    t0 = Math.Log(a) / Matrix[i, i];
                    Time += t0;
                    int k = -1;
                    do
                    {
                        k++;
                        if (k != i)
                        {
                            a -= -Matrix[i, k] / Matrix[i, i];
                        }
                    } while (a > 0);
                    i = k;
                }
            }

            public void GetValue()
            {
                GetStart();
                int quantity = 0;
                Time = 0;
                string writePath = "User.txt";
                try
                {
                    using (StreamWriter output = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                        while (quantity < N)
                        {

                            double a = rnd.NextDouble();
                            t_new = Math.Log(a) / Matrix[i, i];
                            int i_new = -1;
                            do
                            {
                                i_new++;
                                if (i_new != i)
                                {
                                    a -= -Matrix[i, i_new] / Matrix[i, i];
                                }
                            } while (a > 0);
                            P[i] += Math.Abs(t0 - t_new);
                            Time += t0;
                            output.WriteLine(GetTime(Time, i));
                            t0 = t_new;
                            i = i_new;
                            quantity++;
                        }
                }
                catch (Exception e)
                {
                }
            }

            public string GetTime(double t, int i)
            {
                string data = "";
                t -= 30 * c1;
                if (Math.Truncate(t) != day)
                {
                    day = (int)Math.Truncate(t);
                    t -= Math.Truncate(t);
                    if (day > 30)
                    {
                        month++;
                        day -= 30;
                        c1++;
                        if (month == 13)
                        {
                            month = 1;
                        }
                    }
                    data += day + "." + month + "   ";
                }
                else
                {
                    if (c == 0)
                    {
                        data += "1.6    ";
                    }
                    else
                    {
                        data += "       ";
                    }
                    t -= Math.Truncate(t);
                }
                hour = (int)Math.Truncate(24 * t);
                data += hour + ":";
                minute = (int)Math.Truncate(60 * (24 * t - Math.Truncate(24 * t)));
                data += minute + "  ";
                data += Weather[i];
                c++;
                return data;
            }
            public void GetFreq()
            {
                double longTime = 0;
                for (int j = 0; j < P.Length; j++)
                {
                    longTime += P[j];
                }

                for (int j = 0; j < freq.Length; j++)
                {
                    freq[j] = P[j] / longTime;
                }
            }
        }

        private void Sunny_TextChanged(object sender, EventArgs e)
        {

        }

        private void Windy_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
