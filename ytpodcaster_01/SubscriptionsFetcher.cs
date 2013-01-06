using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.YouTube;
using Google.GData.Client;

namespace ytpodcaster_01
{
    public class SubscriptionsFetcher
    {
        internal List<KeyValuePair<string, List<Video>>> GetListOfSubscriptions(Settings i_settings)
        {
            List<KeyValuePair<string, List<Video>>> resObj = new  List<KeyValuePair<string, List<Video>>> ();

            YouTubeRequestSettings settings = new YouTubeRequestSettings(
                  i_settings.appName
                , i_settings.devKey
                , i_settings.username
                , i_settings.password);

            YouTubeRequest request = new YouTubeRequest(settings);
            Feed<Subscription> feedOfSubcr = request.GetSubscriptionsFeed(i_settings.userShort);

            string[] stringSeparators = new string[] { "Activity of:" };

            foreach (Subscription subItem in feedOfSubcr.Entries)
            {
                string keyStr = subItem.ToString().Split(stringSeparators, StringSplitOptions.None)[1].Trim();
                List<Video> listOfVideos = new List<Video>();

                string userName = subItem.UserName;
                string url = string.Format(i_settings.urlFormatter, userName);

                Feed<Video> videoFeed = request.Get<Video>(new Uri(url));

                int depth = 0;
                foreach (Video entry in videoFeed.Entries)
                {
                    //strBuilder.AppendLine("    " + entry.Title);
                    listOfVideos.Add(entry);
                    depth++;
                    if (depth >= i_settings.feedDepth)
                    {
                        break;
                    }
                }

                if (listOfVideos.Count > 0)
                {
                    KeyValuePair<string, List<Video>> subscriptionO = new KeyValuePair<string, List<Video>>(keyStr, listOfVideos);
                    resObj.Add(subscriptionO);
                }

            }

            return resObj;
        }

        internal string GetTextSubscriptions(List<KeyValuePair<string, List<Video>>> strO)
        {
            StringBuilder resBuilder = new StringBuilder();

            foreach (var kvPair in strO)
            {
                resBuilder.AppendLine(kvPair.Key);

                foreach (Video videoItem in kvPair.Value)
                {
                    resBuilder.AppendLine("   " + videoItem.Title);
                }
                
            }

            return resBuilder.ToString();
        }
    }
}
