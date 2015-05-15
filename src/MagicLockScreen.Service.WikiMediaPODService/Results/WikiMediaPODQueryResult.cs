using System;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_WikiMediaPODService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_WikiMediaPODService.Results
{
    public class WikiMediaPODQueryResult : QueryResult
    {
        public WikiMediaPODQueryResult(object result, DateTime date)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            // For Parameters
            Date = date;

            Result = new WikiMediaPOD();

            ParseResponse();
        }

        public WikiMediaPODQueryResult(ModelBase result, DateTime date)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            // For Parameters
            Date = date;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var wikiMediaPODHTML = (ResponseContent as string);

                var wikiMediaPOD = Result as WikiMediaPOD;

                string imageFileName =
                    Regex.Match(wikiMediaPODHTML, _imageFileRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;

                wikiMediaPOD.Title =
                    Regex.Match(imageFileName, _titleRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;

                string thumbnailImageUrl =
                    Regex.Match(wikiMediaPODHTML, _thumbnailImageUrlRegex,
                                RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

                if (!string.IsNullOrEmpty(thumbnailImageUrl))
                {
                    thumbnailImageUrl = @"http://" + thumbnailImageUrl;
                    wikiMediaPOD.OriginalImageUrl = thumbnailImageUrl + @"/1000px-" + imageFileName;
                    wikiMediaPOD.ThumbnailImageUrl = thumbnailImageUrl + @"/300px-" + imageFileName;
                }

                wikiMediaPOD.Explanation =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(wikiMediaPODHTML, _explanationRegex,
                                    RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[_contentGroupName].Value)
                                 .TrimStart('\r', '\n');

                wikiMediaPOD.PageUrl =
                    WikiMediaPOD.BaseUrl + @"/wiki/File:" + imageFileName;

                wikiMediaPOD.Date = Date;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _contentGroupName = "content";

        private const string _imageFileRegex =
            @"\""/wiki/File:(?<" + _contentGroupName + @">.*?\.(jpg|png|bmp|jpeg|tif))\""";

        private const string _thumbnailImageUrlRegex =
            @"upload.wikimedia.org/wikipedia/commons/thumb/[\w]+/[\w]+/.*?\.(jpg|png|bmp|jpeg|tif)\/";

        private const string _titleRegex = @"(?<" + _contentGroupName + @">.*?)\.(jpg|png|bmp|jpeg|tif)";
        private const string _explanationRegex = @"class=""description.*?>(?<" + _contentGroupName + @">.*?)<\/span>";

        #endregion

        #region Parameters

        private DateTime Date { get; set; }

        #endregion
    }
}