using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_AstronomyPictureService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_AstronomyPictureService.Results
{
    public class AstronomyPictureQueryResult : QueryResult
    {
        public AstronomyPictureQueryResult(object result, DateTime date)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            // For Parameters
            Date = date;

            Result = new AstronomyPicture();

            ParseResponse();
        }

        public AstronomyPictureQueryResult(ModelBase result, DateTime date)
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

                var astronomyPictureHTML = (ResponseContent as string);

                var astronomyPicture = Result as AstronomyPicture;

                astronomyPicture.Title =
                    Regex.Match(astronomyPictureHTML, _titleRegex, RegexOptions.IgnoreCase)
                         .Groups[_contentGroupName].Value;

                astronomyPicture.OriginalImageUrl =
                    AstronomyPicture.BaseUrl +
                    Regex.Match(astronomyPictureHTML, string.Format(_originalImageUrlRegex, Date.ToString(_datetimeFormat, DateTimeFormatInfo.InvariantInfo)),
                                RegexOptions.IgnoreCase)
                         .Groups[_contentGroupName].Value;

                astronomyPicture.ThumbnailImageUrl =
                    AstronomyPicture.BaseUrl +
                    Regex.Match(astronomyPictureHTML, string.Format(_thumbnailImageUrlRegex, Date.ToString(_datetimeFormat, DateTimeFormatInfo.InvariantInfo)),
                                RegexOptions.IgnoreCase)
                         .Groups[_contentGroupName].Value;

                astronomyPicture.Explanation =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(astronomyPictureHTML, _explanationRegex, RegexOptions.IgnoreCase).Groups[
                            _contentGroupName].Value);

                astronomyPicture.Copyright =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(astronomyPictureHTML, _copyrightRegex,
                                    RegexOptions.IgnoreCase | RegexOptions.Singleline).Value);

                astronomyPicture.PageUrl = string.Format(AstronomyPicture.Url, Date);

                astronomyPicture.Date = Date;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _datetimeFormat = "yyMM";

        private const string _originalImageUrlRegex =
            @"<a href=\""(?<" + _contentGroupName + @">image\/{0}\/.+\.jpg?)\"">";

        private const string _thumbnailImageUrlRegex =
            @"<img src=\""(?<" + _contentGroupName + @">image\/{0}\/.+\.jpg?)\""";

        private const string _titleRegex = @"<title>(?<" + _contentGroupName + @">.*?)\s<\/title>";

        private const string _explanationRegex =
            @"<p>\s*<b>\s*explanation:\s*<\/b>\s*(?<" + _contentGroupName + @">(.*\s*)*?)\s*<p>";

        private const string _copyrightRegex =
            @"<b>(.*copyright.*?|.*image creadit.*?|.*credit.*?)<\/b>.*<\/center>\s*<p>";

        #endregion

        #region Parameters

        private DateTime Date { get; set; }

        #endregion
    }
}