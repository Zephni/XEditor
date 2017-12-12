using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace XEditor
{
    public static class StaticUpdate
    {
        private static Action Update;

        public static void Start(Action update)
        {
            Update = update;
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += StaticUpdate.Tick;
            dt.Interval = new TimeSpan(1);
            dt.Start();
        }

        private static void Tick(object sender, EventArgs e)
        {
            Update();
        }
    }
}
