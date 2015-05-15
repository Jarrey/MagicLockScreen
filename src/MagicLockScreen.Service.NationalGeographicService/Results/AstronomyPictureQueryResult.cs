using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteOne.Core;
using NoteOne.Core.Common;
using NoteOne.Utility;
using Windows.Data.Xml.Dom;
using NoteOne.Utility.Converters;
using MagicLockScreen.Service.AstronomyPictureService.Models;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;

namespace MagicLockScreen.Service.AstronomyPictureService.Results
{
    public class AstronomyPictureQueryResult : QueryResult
    {
        public AstronomyPictureQueryResult(object result, DateTime date)
            : base(result)
        {
            ResponseType = NoteOne.Core.Common.ResponseTypes.HTML;

            // For Parameters
            this.Date = date;

            Result = new AstronomyPicture();

            ParseResponse();
        }

        public AstronomyPictureQueryResult(ModelBase result, DateTime date)
            : base(result)
        {
            ResponseType = NoteOne.Core.Common.ResponseTypes.HTML;

            // For Parameters
            this.Date = date;
        }

        protected override void ParseResponse()
        {
            base.ParseResponse();

            string astronomyPictureHTML = (this.ResponseContent as string);

            AstronomyPicture astronomyPicture = Result as AstronomyPicture;

            astronomyPicture.Title =
                Regex.Match(astronomyPictureHTML, _titleRegex, RegexOptions.IgnoreCase)
                    .Groups[_contentGroupName].Value;

            astronomyPicture.OriginalImageUrl =
                AstronomyPicture.BaseUrl +
                Regex.Match(astronomyPictureHTML, string.Format(_originalImageUrlRegex, Date), RegexOptions.IgnoreCase)
                    .Groups[_contentGroupName].Value;

            astronomyPicture.ThumbnailImageUrl =
                AstronomyPicture.BaseUrl +
                Regex.Match(astronomyPictureHTML, string.Format(_thumbnailImageUrlRegex, Date), RegexOptions.IgnoreCase)
                    .Groups[_contentGroupName].Value;
            
            astronomyPicture.Explanation=
                Windows.Data.Html.HtmlUtilities.ConvertToText(
                    Regex.Match(astronomyPictureHTML, _explanationRegex, RegexOptions.IgnoreCase).Groups[_contentGroupName].Value);

            astronomyPicture.Copyright=
                Windows.Data.Html.HtmlUtilities.ConvertToText(
                    Regex.Match(astronomyPictureHTML, _copyrightRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value);

            astronomyPicture.Date = Date;
        }

        #region Regex
        private const string _contentGroupName = "content";
        private const string _originalImageUrlRegex = @"<a href=\""(?<" + _contentGroupName + @">image\/{0:yyMM}\/.+\.jpg?)\"">";
        private const string _thumbnailImageUrlRegex = @"<img src=\""(?<" + _contentGroupName + @">image\/{0:yyMM}\/.+\.jpg?)\""";
        private const string _titleRegex = @"<title>(?<" + _contentGroupName + @">.*?)\s<\/title>";
        private const string _explanationRegex = @"<p>\s*<b>\s*explanation:\s*<\/b>\s*(?<" + _contentGroupName + @">(.*\s*)*?)\s*<p>";
        private const string _copyrightRegex = @"<b>(.*copyright.*?|.*image creadit.*?|.*credit.*?)<\/b>.*<\/center>\s*<p>";
        #endregion

        #region Parameters
        private DateTime Date { get; set; }
        #endregion
    }
}
