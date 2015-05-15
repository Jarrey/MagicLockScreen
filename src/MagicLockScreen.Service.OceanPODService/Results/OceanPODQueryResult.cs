using System;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_OceanPODService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_OceanPODService.Results
{
    public class OceanPODQueryResult : QueryResult
    {
        public OceanPODQueryResult(object result, DateTime date)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;

            // For Parameters
            Date = date;

            Result = new OceanPOD();

            ParseResponse();
        }

        public OceanPODQueryResult(ModelBase result, DateTime date)
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

                var oceanPODHTML = (ResponseContent as string);

                var oceanPOD = Result as OceanPOD;

                oceanPOD.Title =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(oceanPODHTML, _titleRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups
                            [_contentGroupName].Value).Trim();

                oceanPOD.OriginalImageUrl =
                    Regex.Match(oceanPODHTML, _originalImageUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Groups[_contentGroupName].Value;
                oceanPOD.ThumbnailImageUrl = oceanPOD.OriginalImageUrl;

                oceanPOD.Explanation =
                    HtmlUtilities.ConvertToText(
                        Regex.Match(oceanPODHTML, _explanationRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                             .Groups[_contentGroupName].Value).TrimStart('\r', '\n');

                oceanPOD.PageUrl = string.Format(OceanPOD.Url, Date);

                oceanPOD.Date = Date;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Regex

        private const string _contentGroupName = "content";

        private const string _originalImageUrlRegex =
            @"(?<" + _contentGroupName + @">http:\/\/www\.theoceanproject\.org\/opod\/upload.*?)\""";

        private const string _titleRegex = @"prev</a>(?<" + _contentGroupName + @">.*?)<";
        private const string _explanationRegex = @"<center>(?<" + _contentGroupName + @">.*?)<a href=\""index\.php";

        #endregion

        #region Parameters

        private DateTime Date { get; set; }

        #endregion
    }
}