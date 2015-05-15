using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_InterfaceLIFTService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_InterfaceLIFTService.Results
{
    public class InterfaceLIFTQueryResult : QueryResult
    {
        public InterfaceLIFTQueryResult(object result,
                                        QueryResultTypes type = QueryResultTypes.Single,
                                        uint minIndex = 0,
                                        uint maxIndex = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Html;

            MinIndex = minIndex;
            MaxIndex = maxIndex;

            if (type == QueryResultTypes.Single)
                Result = new InterfaceLIFT();
            else if (type == QueryResultTypes.Multi)
                Results = default(InterfaceLIFT[]);

            ParseResponse();
        }

        public InterfaceLIFTQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;
        }

        public InterfaceLIFTQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Html;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var interfaceLIFTHTML = (ResponseContent as string);

                // split image items, one page should contains 10 image parts, and the code before wallpapers part
                string[] itemParts =
                    Regex.Split(interfaceLIFTHTML, _imagePartSplit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var interfaceLIFT = Result as InterfaceLIFT;

                        if (itemParts.Length > 1 + MinIndex)
                        {
                            GenerateInterfaceLIFTFromHTML(itemParts[1 + MinIndex], interfaceLIFT);
                        }
                        break;

                    case QueryResultTypes.Multi:
                        var interfaceLIFTs = new List<InterfaceLIFT>();
                        if (itemParts.Length > 1 + MinIndex)
                        {
                            for (uint index = MinIndex + 1; index <= Math.Min(itemParts.Length - 1, MaxIndex); index++)
                            {
                                var img = new InterfaceLIFT();
                                GenerateInterfaceLIFTFromHTML(itemParts[index], img);
                                interfaceLIFTs.Add(img);
                            }
                        }
                        Results = interfaceLIFTs.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void GenerateInterfaceLIFTFromHTML(string htmlPart, InterfaceLIFT interfaceLIFT)
        {
            GroupCollection imageDetailUrl =
                Regex.Match(htmlPart, _imgaeDetailUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups;

            string imageDetail = imageDetailUrl[_contentGroupName1].Value;
            string imageUrl = imageDetailUrl[_contentGroupName2].Value;
            string imageDetails =
                Regex.Match(htmlPart, _imageDetailsRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                     .Groups[_contentGroupName].Value;

            interfaceLIFT.PageUrl = InterfaceLIFT.BaseUrl + imageDetail;
            interfaceLIFT.ImageId =
                Regex.Match(imageDetail, _imageIdRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                     .Groups[_contentGroupName].Value.Check().StringToInt();

            interfaceLIFT.ThumbnailImageUrl = imageUrl;

            interfaceLIFT.OriginalImageUrl = Regex.Replace(imageUrl, _imageScaleLevelRegex, _imageLargeReplacement,
                                                           RegexOptions.IgnoreCase | RegexOptions.Singleline);

            interfaceLIFT.Explanation = HtmlUtilities.ConvertToText(
                Regex.Match(imageDetails, _imageExplanationRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                     .Groups[_contentGroupName].Value).Trim();

            interfaceLIFT.Title = HtmlUtilities.ConvertToText(
                Regex.Match(imageDetails, _imageTitleRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value);

            GroupCollection imageAuthorInfo =
                Regex.Match(imageDetails, _imageAuthorRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline)
                     .Groups;
            interfaceLIFT.AuthorUrl = InterfaceLIFT.BaseUrl + imageAuthorInfo[_contentGroupName1].Value;
            interfaceLIFT.Copyright = imageAuthorInfo[_contentGroupName2].Value;
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _contentGroupName1 = "content1";
        private const string _contentGroupName2 = "content2";

        private const string _imgaeDetailUrlRegex =
            @"div  class=\""preview\"".*?<a href=\""(?<" + _contentGroupName1 + @">.*?)\"".*?<img src=\""(?<" +
            _contentGroupName2 + @">.*?)\""";

        private const string _imageIdRegex = @"/wallpaper/details/(?<" + _contentGroupName + @">\d*?)/";
        private const string _imageScaleLevelRegex = @"previews/(?<" + _contentGroupName + @">.*?)_[\d|x]*?.jpg";

        private const string _imageDetailsRegex =
            @"<div class=\""details\"".*?>\s*(?<" + _contentGroupName + @">.*?)<div class=\""display_actions\""";

        private const string _imageExplanationRegex = @"<p>(?<" + _contentGroupName + @">.*)</p>";
        private const string _imageTitleRegex = @"<h1.*?</h1>";

        private const string _imageAuthorRegex =
            @">By\s*<a href=\""(?<" + _contentGroupName1 + @">.*?)\"".*?>(?<" + _contentGroupName2 + @">.*?)</a>";

        private const string _imagePartSplit = @"<div class=""item"">";

        // "7yz4ma1" id is from http://interfacelift.com/inc_NEW/jscript002.js, imgload() funtion
        private const string _imageLargeReplacement = @"7yz4ma1/${" + _contentGroupName + @"}_1920x1200.jpg";

        #endregion

        #region Parameters

        private uint MinIndex { get; set; }
        private uint MaxIndex { get; set; }

        #endregion
    }
}