using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using wakeup.Properties;


namespace wakeup
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Application.Run(new MyCustormApplicationContext());
        }


    }

    public class MyCustormApplicationContext : ApplicationContext
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(int uAction, int uParam, ref int lpvParam, int flags);

        private NotifyIcon trayIcon;

        public MyCustormApplicationContext()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            trayIcon.MouseDoubleClick += trayIconMouseDoubleClickEventHandler;
            run();
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void trayIconMouseDoubleClickEventHandler(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("double clicked!");
        }

        private void run()
        {
            Debug.WriteLine("thread runs");
            new Thread(new ThreadStart(exeStateNotifier)).Start();
        }

        private void keepMoving()
        {
            while (true)
            {
                int curx = Cursor.Position.X;
                int cury = Cursor.Position.Y;

                Debug.WriteLine("Hello x=" + curx + " y=" + cury);
                Cursor.Position = new Point(curx + 100, cury + 100);
                Thread.Sleep(100);
                Cursor.Position = new Point(curx + 1, cury + 1);

                Thread.Sleep(5000);
            }
        }

        private int getSome()
        {
            int en = 0;
            SystemParametersInfo(16, 0, ref en, 0);
            return en;
        }

        private void exeStateNotifier()
        {
            while (true)
            { 

                Debug.WriteLine("wakeup! screen saver info="+ getSome());
                NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                Thread.Sleep(5000);
            }
        }
            

        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
        }

        internal class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }


    }
}
