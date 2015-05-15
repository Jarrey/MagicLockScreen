using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MagicLockScreen_Service_WWFPictureService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_WWFPictureService.Results
{
    public class WWFPictureQueryResult : QueryResult
    {
        public WWFPictureQueryResult(object result,
                                        QueryResultTypes type = QueryResultTypes.Single,
                                        uint minIndex = 0,
                                        uint maxIndex = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Html;

            MinIndex = minIndex;
            MaxIndex = maxIndex;

            if (type == QueryResultTypes.Single)
                Result = new WWFPicture();
            else if (type == QueryResultTypes.Multi)
                Results = default(WWFPicture[]);

            ParseResponse();
        }

        public WWFPictureQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Html;
        }

        public WWFPictureQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Html;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                var wwfPictureHTML = (ResponseContent as string);

                MatchCollection imgsHTML = Regex.Matches(wwfPictureHTML, _imagesHTMLRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string[] itemParts = imgsHTML.Cast<Match>().Select(m => m.Groups[_contentGroupName].Value).ToArray();

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var wwfPicture = Result as WWFPicture;

                        if (itemParts.Length > MinIndex)
                        {
                            GenerateWWFPictureFromHTML(itemParts[MinIndex], wwfPicture);
                        }
                        break;

                    case QueryResultTypes.Multi:
                        var wwfPictures = new List<WWFPicture>();
                        if (itemParts.Length > MinIndex)
                        {
                            for (uint index = MinIndex; index <= Math.Min(itemParts.Length - 1, MaxIndex - 1); index++)
                            {
                                var img = new WWFPicture();
                                GenerateWWFPictureFromHTML(itemParts[index], img);
                                wwfPictures.Add(img);
                            }
                        }
                        Results = wwfPictures.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void GenerateWWFPictureFromHTML(string htmlPart, WWFPicture wwfPicture)
        {
            GroupCollection imageDetail =
                Regex.Match(htmlPart, _imageInfoRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups;
            
            wwfPicture.ThumbnailImageUrl = imageDetail[_imageUrlGroupName].Value;

            wwfPicture.OriginalImageUrl = Regex.Replace(wwfPicture.ThumbnailImageUrl, _imageScaleRegex, _imageLargeReplacement,
                                                           RegexOptions.IgnoreCase | RegexOptions.Singleline);

            wwfPicture.Explanation = imageDetail[_descriptionGroupName].Value;
            wwfPicture.Title = imageDetail[_titleGroupName].Value;
            wwfPicture.Copyright = imageDetail[_copyrightGroupName].Value;
            wwfPicture.PageUrl = WWFPicture.BaseUrl + imageDetail[_pageUrlGroupName].Value;
            wwfPicture.Date = DateTime.Now;
        }

        #region Regex

        private const string _contentGroupName = "content";
        private const string _pageUrlGroupName = "pageUrl";
        private const string _titleGroupName = "title";
        private const string _copyrightGroupName = "copyright";
        private const string _imageUrlGroupName = "imageUrl";
        private const string _descriptionGroupName = "description";

        private const string _imagesHTMLRegex = @"<li class=\""span4\"">\s*(?<" + _contentGroupName + @">.*?)\s*</li>";

        private const string _imageInfoRegex = @"<a href=\""(?<" + _pageUrlGroupName + @">.*?)\"".*" +
                                               @"<img alt=\""(?<" + _titleGroupName + @">.*?)\"" " +
                                               @"data-attribution=\""(?<" + _copyrightGroupName + @">.*?)\"".*" +
                                               @"src=\""(?<" + _imageUrlGroupName + @">.*?)\"".*" +
                                               @"<h2>(?<" + _descriptionGroupName + @">.*?)</h2>";

        private const string _imageScaleRegex = @"featured_story";
        private const string _imageLargeReplacement = @"story_full_width";

        #endregion

        #region Parameters

        private uint MinIndex { get; set; }
        private uint MaxIndex { get; set; }

        #endregion
    }
}