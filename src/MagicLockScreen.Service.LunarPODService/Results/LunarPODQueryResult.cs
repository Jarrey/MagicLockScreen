using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_LunarPODService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_LunarPODService.Results
{
    public class LunarPODQueryResult : QueryResult
    {
        public LunarPODQueryResult(object result, DateTime date)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            // For Parameters
            Date = date;

            Result = new LunarPOD();

            ParseResponse();
        }

        public LunarPODQueryResult(ModelBase result, DateTime date)
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

                var lunarPODHTML = (ResponseContent as string);

                var lunarPOD = Result as LunarPOD;

                lunarPOD.Title =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(lunarPODHTML, _titleRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value);

                var dateTimeFormatShortMonthNames = new[]
                    {
                        "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                    };
                var dateTimeFormatMonthNames = new[]
                    {
                        "", "January", "February", "March", "April", "May", "June", "July", "August", "September",
                        "October", "November", "December"
                    };

                lunarPOD.OriginalImageUrl =
                    LunarPOD.BaseUrl +
                    Regex.Match(lunarPODHTML,
                                string.Format(_originalImageUrlRegex, dateTimeFormatShortMonthNames[Date.Month], 
                                              Date.ToString(_datetimeDayFormat, DateTimeFormatInfo.InvariantInfo), 
                                              Date.ToString(_datetimeYearFormat, DateTimeFormatInfo.InvariantInfo)),
                                RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;
                lunarPOD.ThumbnailImageUrl = lunarPOD.OriginalImageUrl;

                lunarPOD.Explanation =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(lunarPODHTML, _explanationRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                             .Groups[_contentGroupName].Value).TrimStart('\r', '\n');

                lunarPOD.PageUrl = string.Format(LunarPOD.Url, dateTimeFormatMonthNames[Date.Month], Date);

                lunarPOD.Date = Date;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _datetimeYearFormat = "yy";
        private const string _datetimeDayFormat = "%d";

        private const string _originalImageUrlRegex = @"/file/view/LPOD-{0}{1}[\-]{{1,2}}{2}.jpg";
        private const string _titleRegex = @"<h1 id=""toc0"">.*?</h1>";

        private const string _explanationRegex =
            @"ws:end:WikiTextLocalImageRule:6 -->(?<" + _contentGroupName +
            @">.*?)<!-- ws:start:WikiTextHeadingRule:2:&lt;h3&gt;";

        #endregion

        #region Parameters

        private DateTime Date { get; set; }

        #endregion
    }
}