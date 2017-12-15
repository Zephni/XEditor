using System;

namespace XEditor
{
    public class Updater
    {
        public System.Windows.Forms.Timer UpdateTimer;
        private Action UpdateAction;

        public void Start(int millisecondUpdateGap, Action update)
        {
            UpdateAction = update;
            UpdateTimer = new System.Windows.Forms.Timer();
            UpdateTimer.Tick += Update;
            UpdateTimer.Interval = millisecondUpdateGap;
            UpdateTimer.Start();
        }

        public void Update(object sender, EventArgs e)
        {
            UpdateAction();
        }
    }
}
