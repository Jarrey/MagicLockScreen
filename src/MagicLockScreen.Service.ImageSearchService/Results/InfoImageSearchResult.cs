using System;
using System.Linq;
using System.Collections.Generic;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;
using System.Text.RegularExpressions;

namespace MagicLockScreen_Service_ImageSearchService.Results
{
    public class InfoImageSearchResult : QueryResult
    {
        public InfoImageSearchResult(object result,
                                      QueryResultTypes type = QueryResultTypes.Single,
                                      uint min = 0,
                                      uint max = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Html;

            if (type == QueryResultTypes.Single)
                Result = new SearchImage();
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(SearchImage[]);
                MinIndex = min;
                MaxIndex = max;
            }

            ParseResponse();
        }

        public InfoImageSearchResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;
        }

        public InfoImageSearchResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Html;
        }


        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var infoImageHTML = (ResponseContent as string);

                var imagesPart = Regex.Match(infoImageHTML, _imagesPartRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                uint start = imagesPart.Groups[_startGroupName].Value.StringToUInt();
                uint end = imagesPart.Groups[_endGroupName].Value.StringToUInt();
                int total = imagesPart.Groups[_totalGroupName].Value.StringToInt();
                string imagesPartHTML = imagesPart.Groups[_imagesGroupName].Value;
                var images = Regex.Matches(imagesPartHTML, _imagesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                var imagesHTML = new List<string>();
                foreach (Match matchImage in images)
                {
                    imagesHTML.Add(matchImage.Value);
                }

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var infoImage = Result as SearchImage;
                        GenerateInfoImageFromHTML(imagesHTML[(int)MinIndex], infoImage);
                        break;

                    case QueryResultTypes.Multi:
                        var infoImages = new List<SearchImage>();

                        for (int i = (int)MinIndex; i < (int)Math.Min(MaxIndex, end - start); i++)
                        {
                            var img = new SearchImage();
                            GenerateInfoImageFromHTML(imagesHTML[i], img);
                            infoImages.Add(img);
                        }

                        Results = infoImages.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void GenerateInfoImageFromHTML(string htmlPart, SearchImage image)
        {
            image.OriginalImageUrl = image.ThumbnailImageUrl =
                Regex.Match(htmlPart, _imageUrlRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[_contentGroupName].Value;

            var imageInfo = Regex.Match(htmlPart, _imageInfoRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            image.Content = image.Title = imageInfo.Groups[_imageName].Value;
            image.Copyright = image.CopyrightUrl = image.PageUrl = imageInfo.Groups[_imageCopyright].Value;
            image.Date = DateTime.Now;
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _startGroupName = "start";
        private const string _endGroupName = "end";
        private const string _totalGroupName = "total";
        private const string _imagesGroupName = "images";
        private const string _imageName = "name";
        private const string _imageCopyright = "copyright";

        private const string _imagesPartRegex = @"<div class=\""right\"">Showing <b>(?<" + _startGroupName +
                                                @">[\d]*)</b> to <b>(?<" + _endGroupName +
                                                @">[\d]*)</b> of <b>(?<" + _totalGroupName +
                                                @">[\d\,]*)</b></div>\s*Images</div>(?<" + _imagesGroupName +
                                                @">.*?)</table>";

        private const string _imagesRegex = @"<a href=.*?<img.*?</a>";

        private const string _imageUrlRegex = @"href=\""(?<" + _contentGroupName + @">.*?)\""";

        private const string _imageInfoRegex = @"title=\""(?<" + _imageName + @">.*?)\s+(?<" + _imageCopyright + @">.*?)\""";

        #endregion

        #region Parameters

        private uint MinIndex { get; set; }
        private uint MaxIndex { get; set; }

        #endregion
    }
}