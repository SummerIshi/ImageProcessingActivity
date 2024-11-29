using ImageProcess2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Rodrigo_ImageProcessingActivity
{
    public partial class Form2 : Form
    {
        Bitmap loaded, processed;
        List<int> coinsArea = new List<int>();
        bool[,] visited_pixels;
        double TotalValue = 0;
        int peso_5 = 0, peso_1 = 0, cent_25 = 0, cent_10 = 0, cent_5 = 0;
        public Form2()
        {
            InitializeComponent();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private int getCoinArea(Point start_point)
        {
            int area = 1;

            Queue<Point> myQueue = new Queue<Point>();
            myQueue.Enqueue(start_point);
            visited_pixels[start_point.X, start_point.Y] = true;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (myQueue.Count > 0)
            {
                Point currentPoint = myQueue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int newX = currentPoint.X + dx[i];
                    int newY = currentPoint.Y + dy[i];

                    if (newX >= 0 && newX < loaded.Width && newY >= 0 && newY < loaded.Height && !visited_pixels[newX, newY])
                    {
                        
                        Color pixelColor = loaded.GetPixel(newX, newY);
                        if (isBlack(pixelColor))
                        {
                            visited_pixels[newX, newY] = true;
                            myQueue.Enqueue(new Point(newX, newY));

                            area++;
                        }
                    }
                }

            }

            return area;
        }


        private bool isBlack(Color p)
        {
            return p.R == 0 && p.G == 0 && p.B == 0;
        }

        private void getCoinAreaValue(int coinSizeThreshold)
        {
            coinsArea.Sort();

            List<List<int>> groupedCoins = new List<List<int>>();
            List<int> currentGroup = new List<int> { coinsArea[0] };


            for (int i = 1; i < coinsArea.Count; i++)
            {
                if (coinsArea[i] - currentGroup[0] <= coinSizeThreshold)
                {
                    currentGroup.Add(coinsArea[i]);
                }
                else
                {
                 
                    groupedCoins.Add(new List<int>(currentGroup));
                    currentGroup.Clear();
                    currentGroup.Add(coinsArea[i]);
                }
            }


            groupedCoins.Add(new List<int>(currentGroup));



            peso_5 = groupedCoins[4].Count;
            peso_1 = groupedCoins[3].Count;
            cent_25 = groupedCoins[2].Count;
            cent_10 = groupedCoins[1].Count;
            cent_5 = groupedCoins[0].Count;

            label1.Text = "5 Peso coin count (" + peso_5.ToString() + " pcs): ₱" + (5 * peso_5).ToString();
            label2.Text = "1 Peso coin count (" + peso_1.ToString() + " pcs): ₱" + peso_1.ToString();
            label3.Text = "25 Centavos coin count (" + cent_25.ToString() + " pcs): ₱" + (0.25 * cent_25).ToString();
            label4.Text = "10 Centavos coin count (" + cent_10.ToString() + " pcs): ₱" + (0.10 * cent_10).ToString();
            label5.Text = "5 Centavos coin count (" + cent_5.ToString() + " pcs): ₱" + (0.05 * cent_5).ToString();

            TotalValue = 5 * peso_5 + peso_1 + 0.25 * cent_25 + 0.10 * cent_10 + 0.05 * cent_5;
            label6.Text = "Total Coin Amount: ₱" + TotalValue.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            BitmapFilter.GaussianBlur(loaded, 10);

            BitmapFilter.GrayScale(loaded);

            BitmapFilter.Binary(loaded, 200);

            visited_pixels = new bool[loaded.Width, loaded.Height];

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {

                    Color current_pixel = loaded.GetPixel(x, y);
                    Point current_point = new Point(x, y);

                    if (isBlack(current_pixel) && !visited_pixels[current_point.X, current_point.Y])
                    {
                        int new_coin_area = getCoinArea(current_point);

                        if (new_coin_area > 50)
                        {
                            coinsArea.Add(new_coin_area);
                        }


                    }
                }
            }

            int coinSizeThreshold = 300;
            getCoinAreaValue(coinSizeThreshold);

        }
    }
}
