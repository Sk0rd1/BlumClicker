using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MessageBox = System.Windows.MessageBox;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace BlumClicker
{
    internal class ClickEngine
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwflags, uint dx, uint dy, uint dwData, uint ExtraInf);

        [DllImport("user32.dll")]
        private static extern void SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        int[] colorMin = { 40, 200, -1 };
        int[] colorMax = { 120, 256, 80 };

        bool isSearch = true;
        Thread searchThread;

        public void Start()
        {
            isSearch = true;

            searchThread = new Thread(SearchPoints);
            searchThread.Start();
        }

        private void SearchPoints()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            while (isSearch)
            {
                System.Threading.Thread.Sleep(1);

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 470; x++)
                    {
                        int yValue = 370 + y * 100;
                        Color currentPixelColor = bitmap.GetPixel(x, yValue);

                        if (!(currentPixelColor.R < 40 && currentPixelColor.G < 40 && currentPixelColor.B < 40))
                        {
                            Click(x, yValue);
                            x += 20;
                        }
                    }
                }

                Color p1 = bitmap.GetPixel(80, 750);
                Color p2 = bitmap.GetPixel(380, 750);

                if (p1.R > 200 && p1.G > 200 && p1.B > 200 && p2.R > 200 && p2.G > 200 && p2.B > 200)
                    Click(80, 750);

            }
                bitmap.Dispose();
        }
        private void Click(int x, int y)
        {
            POINT currentPosition;
            GetCursorPos(out currentPosition);

            SetCursorPos(x, y);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            System.Threading.Thread.Sleep(1);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            //SetCursorPos(currentPosition.X, currentPosition.Y);

            System.Threading.Thread.Sleep(1);
        }

        public void Stop() 
        {
            isSearch = false;
            if (searchThread != null && searchThread.IsAlive)
            {
                searchThread.Join();
            }
        }
    }
}
