using System;
using System.Collections.Generic;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;

namespace MagicLockScreen_Service_ImageSearchService.Results
{
    public class BaiduImageSearchResult : QueryResult
    {
        public BaiduImageSearchResult(object result,
                                      QueryResultTypes type = QueryResultTypes.Single,
                                      uint index = 0,
                                      uint count = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Json;

            if (type == QueryResultTypes.Single)
                Result = new SearchImage();
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(SearchImage[]);
                Index = index;
                Count = count;
            }

            ParseResponse();
        }

        public BaiduImageSearchResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Json;
        }

        public BaiduImageSearchResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Json;
        }


        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                dynamic d = ResponseContent;

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        if (d.displayNum > 0)
                        {
                            var baiduImage = Result as SearchImage;
                            baiduImage.OriginalImageUrl = d.data[0].objURL.ToString().Trim();
                            baiduImage.ThumbnailImageUrl = baiduImage.OriginalImageUrl;
                            string title = HtmlUtilities.ConvertToText(d.data[0].fromPageTitleEnc.ToString().Trim());
                            baiduImage.Title = title;
                            baiduImage.Content = title;
                            baiduImage.Copyright = d.data[0].fromURLHost.ToString().Trim();
                            baiduImage.CopyrightUrl = d.data[0].fromURLHost.ToString().Trim();
                            baiduImage.PageUrl = d.data[0].fromURL.ToString().Trim();
                            baiduImage.Date =
                                ParseValueConverterHelper.StringToDateTime(d.data[0].bdImgnewsDate.ToString().Trim(),
                                                                           "yyyy-MM-dd hh:mm");
                        }
                        break;

                    case QueryResultTypes.Multi:
                        var baiduImages = new List<SearchImage>();
                        if (d.displayNum > 0)
                        {
                            for (int i = 0; i < (int) Count; i++)
                            {
                                var img = new SearchImage();
                                img.ThumbnailImageUrl = img.OriginalImageUrl = d.data[i].objURL.ToString().Trim();
                                img.Title = img.Content = HtmlUtilities.ConvertToText(d.data[i].fromPageTitleEnc.ToString().Trim());
                                img.Copyright = img.CopyrightUrl = d.data[i].fromURLHost.ToString().Trim();
                                img.PageUrl = d.data[i].fromURL.ToString().Trim();
                                img.Date = ParseValueConverterHelper.StringToDateTime(
                                        d.data[i].bdImgnewsDate.ToString().Trim(), "yyyy-MM-dd hh:mm");
                                baiduImages.Add(img);
                            }
                        }
                        Results = baiduImages.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Parameters

        private uint Index { get; set; }
        private uint Count { get; set; }

        #endregion
    }
}