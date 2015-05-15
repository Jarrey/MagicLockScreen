using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_MicrosoftWallpaperService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_MicrosoftWallpaperService.Results
{
    public class MicrosoftWallpaperQueryResult : QueryResult
    {
        /// <summary>
        /// static constructor, for initializing internal image collection
        /// </summary>
        static MicrosoftWallpaperQueryResult()
        {
            // initialize internal image collection
            if (MicrosoftWallpapers == null)
                MicrosoftWallpapers = new List<MicrosoftWallpaper>();
        }

        public MicrosoftWallpaperQueryResult(object result,
                                             QueryResultTypes type = QueryResultTypes.Single,
                                             uint index = 0,
                                             uint count = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Html;

            if (type == QueryResultTypes.Single)
            {
                Result = new MicrosoftWallpaper();
                Index = index;
            }
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(MicrosoftWallpaper[]);
                Index = index;
                Count = count;
            }

            ParseResponse();
        }

        public MicrosoftWallpaperQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;
        }

        public MicrosoftWallpaperQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Html;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var microsoftWallpaperHTML = (ResponseContent as string);

                // prepare the image collection from first query
                if (!string.IsNullOrEmpty(microsoftWallpaperHTML))
                {
                    var imageInfos = Regex.Matches(microsoftWallpaperHTML, _imageRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match imageInfo in imageInfos)
                    {
                        var microsoftWallpaper = new MicrosoftWallpaper()
                            {
                                Title = imageInfo.Groups[_titleGroupName].Value,
                                OriginalImageUrl = imageInfo.Groups[_originalImageGroupName].Value,
                                ThumbnailImageUrl = imageInfo.Groups[_thumbnailImageGroupName].Value,
                                PageUrl = @"http://windows.microsoft.com/en-us/windows/wallpaper",
                                Date = DateTime.Now,
                                Explanation=imageInfo.Groups[_titleGroupName].Value
                            };
                        MicrosoftWallpapers.Add(microsoftWallpaper);
                    }
                }

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        if (MicrosoftWallpapers.Count > Index)
                            Result = MicrosoftWallpapers.ElementAt((int)Index);
                        else if (MicrosoftWallpapers.Count > 0)
                            Result = MicrosoftWallpapers.First();
                        break;

                    case QueryResultTypes.Multi:

                        var microsoftWallpapers = new List<MicrosoftWallpaper>();
                        for (var i = (int)Index; i < Index + Count; i++)
                        {
                            if (MicrosoftWallpapers.Count > i)
                                microsoftWallpapers.Add(MicrosoftWallpapers[i]);
                        }
                        Results = microsoftWallpapers.ToArray();
                        break;
                }

            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _titleGroupName = "title";
        private const string _originalImageGroupName = "original";
        private const string _thumbnailImageGroupName = "thumbnail";

        private const string _imageRegex = @"class=""prodPaneImage""><a href=""(?<" + _originalImageGroupName +
                                              @">.*?)""(.*?)src=""(?<" + _thumbnailImageGroupName +
                                              @">.*?)""(.*?)alt=""(?<" + _titleGroupName +
                                              @">.*?)""";

        #endregion

        #region Parameters

        private uint Index { get; set; }
        private uint Count { get; set; }

        #endregion

        #region Properties

        public static List<MicrosoftWallpaper> MicrosoftWallpapers { get; private set; }

        #endregion
    }
}