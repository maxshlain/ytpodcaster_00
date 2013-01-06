using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ytpodcaster_01
{
    internal class Settings
    {
        internal string appName;
        internal string username;
        internal string userShort;
        internal string password;
        internal string devKey;
        internal string urlFormatter;
        internal int feedDepth = 5;
        internal string rootSaveDir;

        internal Settings()
        {
            appName = "ytpodcaster";
            username = "";
            userShort = "";
            password = "";
            devKey = "";
            urlFormatter = "http://gdata.youtube.com/feeds/api/users/{0}/uploads?orderby=published";
            rootSaveDir = @"D:\Torrents\ytCasts";
        }
    }
}
