using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Google.YouTube;
using System.Net;
using System.Web;

namespace ytpodcaster_01
{
    class VideoDownloader
    {
        internal void StartDownloading(List<KeyValuePair<string, List<Google.YouTube.Video>>> i_videosCollection, Settings i_settingsObj)
        {
            string rootPath = i_settingsObj.rootSaveDir;
            string nextSaveDirName = "";
            string nextSaveDirPath = "";

            foreach (var kvPair in i_videosCollection)
            {
                nextSaveDirName = getDirName(kvPair.Key);
                nextSaveDirPath = Path.Combine(rootPath, nextSaveDirName);

                if (!Directory.Exists(nextSaveDirPath)) { Directory.CreateDirectory(nextSaveDirPath); }

                foreach (Video videoItem in kvPair.Value)
                {
                    saveVideo(videoItem, nextSaveDirPath);
                }
            }
        }

        private string getDirName(string i_str)
        {
            return i_str;
        }

        private void saveVideo(Video i_videoItem, string i_nextSaveDirPath)
        {
            string vidName = i_videoItem.Title;
            string videoId = i_videoItem.VideoId;
            string getVideoInfoPattern = @"www.youtube.com/get_video_info?video_id={0}";
            string getVideoInfoUrl = NormalizeYoutubeUrl(string.Format(getVideoInfoPattern, videoId));

            string videoInfoStr = getPageSource(getVideoInfoUrl);

            IEnumerable<Uri> urisColl = ExtractDownloadUrls(videoInfoStr);

            Uri videoUri = urisColl.Last();

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(videoUri.AbsoluteUri, Path.Combine(i_nextSaveDirPath, (i_videoItem.VideoId + ".mp4")));
            }

            return;
        }

        private string getPageSource(string i_videoUrl)
        {
            string pageSource;
            var req = WebRequest.Create( new Uri(i_videoUrl));

            using (var resp = req.GetResponse())
            {
                pageSource = new StreamReader(resp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            }

            return pageSource;
        }

        private static IEnumerable<Uri> ExtractDownloadUrls(string source)
        {
            var t = HttpUtility.ParseQueryString(source);

            string[] mp4Itags = new string[] { "itag=18", "itag=22", "itag=37", "itag=38"};
            string urlMap = t.Get("url_encoded_fmt_stream_map");

            string[] splitByUrls = urlMap.Split(',');

            foreach (string s in splitByUrls)
            {
                var queries = HttpUtility.ParseQueryString(s);

                string url = queries.Get("url") + "&fallback_host=" + queries.Get("fallback_host") + "&signature=" + queries.Get("sig");

                url = HttpUtility.UrlDecode(url);
                url = HttpUtility.UrlDecode(url);

                foreach (var tagItem in mp4Itags)
                {
                    if (url.Contains(tagItem))
                    {
                        yield return new Uri(url);       
                    }
                }
            }
        }

        private static string NormalizeYoutubeUrl(string url)
        {
            url = url.Trim();

            if (url.StartsWith("https://"))
            {
                url = "http://" + url.Substring(8);
            }

            else if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube.com", "youtube.com");

            if (url.StartsWith("http://youtube.com/v/"))
            {
                url = url.Replace("youtube.com/v/", "youtube.com/watch?v=");
            }

            else if (url.StartsWith("http://youtube.com/watch#"))
            {
                url = url.Replace("youtube.com/watch#", "youtube.com/watch?");
            }

            //if (!url.StartsWith("http://youtube.com/watch"))
            //{
            //    throw new ArgumentException("URL is not a valid youtube URL!");
            //}

            return url;
        }
    }
}
