using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ytpodcaster_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //textBox1.Text = tester.Tester();
            SubscriptionsFetcher subFetcherObj = new SubscriptionsFetcher();
            Settings settingsObj = new Settings();
            var strO = subFetcherObj.GetListOfSubscriptions(settingsObj);
            textBox1.Text = subFetcherObj.GetTextSubscriptions(strO);

            VideoDownloader vDownObj = new VideoDownloader();
            vDownObj.StartDownloading(strO, settingsObj);
        }
    }
}
