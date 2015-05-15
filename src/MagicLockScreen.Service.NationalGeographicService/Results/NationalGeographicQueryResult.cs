using System;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_NationalGeographicService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_NationalGeographicService.Results
{
    public class NationalGeographicQueryResult : QueryResult
    {
        public NationalGeographicQueryResult(object result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            Result = new NationalGeographic();

            ParseResponse();
        }

        public NationalGeographicQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var nationalGeographicHTML = (ResponseContent as string);

                var nationalGeographic = Result as NationalGeographic;

                string primaryPhoto =
                    Regex.Match(nationalGeographicHTML, _primaryPhotoRegex,
                                RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

                nationalGeographic.PreviousPhotoLink =
                    NationalGeographic.BaseUrl +
                    Regex.Match(primaryPhoto, _previousPhotoLinkRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;

                var thumbnailImageUrl = Regex.Match(primaryPhoto, _thumbnailImageUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                        .Groups[_contentGroupName].Value;
                nationalGeographic.ThumbnailImageUrl = (thumbnailImageUrl.StartsWith("http:") ? string.Empty : "http:") + thumbnailImageUrl;

                nationalGeographic.Title =
                    Regex.Match(primaryPhoto, _titleRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;

                string articleText =
                    Regex.Match(nationalGeographicHTML, _articleTextRegex,
                                RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

                var originalImageUrl = Regex.Match(articleText, _originalImageUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                     .Groups[_contentGroupName].Value;

                if (string.IsNullOrEmpty(originalImageUrl))
                    nationalGeographic.OriginalImageUrl = nationalGeographic.ThumbnailImageUrl;
                else nationalGeographic.OriginalImageUrl = (originalImageUrl.StartsWith("http:") ? string.Empty : "http:") + originalImageUrl;

                nationalGeographic.Copyright =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(articleText, _copyrightRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                             .Value);

                nationalGeographic.Date =
                    Regex.Match(articleText, _publishDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value.StringToDateTime("MMMM d, yyyy");

                nationalGeographic.Explanation =
                    Regex.Match(nationalGeographicHTML, _explanationRegex,
                                RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;

                nationalGeographic.Url =
                    Regex.Match(nationalGeographicHTML, _siteUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _primaryPhotoRegex = @"<div class=\""primary_photo\"">.*\.primary_photo";
        private const string _previousPhotoLinkRegex = @"<a href=\""(?<" + _contentGroupName + @">.*?)\""";
        private const string _thumbnailImageUrlRegex = @"<img src=\""(?<" + _contentGroupName + @">.*?)\""";
        private const string _titleRegex = @"alt=\""(?<" + _contentGroupName + @">.*?)\""";

        private const string _articleTextRegex = @"<div class=\""article_text\"">.*\.article_text";

        private const string _originalImageUrlRegex =
            @"<div class=""download_link"">\s*<a href=\""(?<" + _contentGroupName + @">.*?)\""";

        private const string _publishDateRegex =
            @"<p class=\""publication_time\"">(?<" + _contentGroupName + @">[\w\,\s]*?)</p>";

        private const string _explanationRegex =
            @"<meta name=\""description\"" content=\""(?<" + _contentGroupName + @">.*?)\""";

        private const string _copyrightRegex = @"<p class=\""credit\"">.*?</p>";

        private const string _siteUrlRegex = @"<link rel=\""canonical\"" href=\""(?<" + _contentGroupName + @">.*?)\""";

        #endregion

        #region Parameters

        #endregion
    }
}