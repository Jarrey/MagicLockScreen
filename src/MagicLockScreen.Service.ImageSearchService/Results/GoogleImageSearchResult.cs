using System;
using System.Collections.Generic;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_ImageSearchService.Results
{
    public class GoogleImageSearchResult : QueryResult
    {
        public GoogleImageSearchResult(object result,
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

        public GoogleImageSearchResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Json;
        }

        public GoogleImageSearchResult(IList<ModelBase> results)
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
                        if (d.responseData != null &&
                            d.responseData.results.Count != 0 &&
                            d.responseData.cursor.resultCount > 0)
                        {
                            var googleImage = Result as SearchImage;
                            googleImage.OriginalImageUrl = d.responseData.results[0].unescapedUrl.ToString().Trim();
                            googleImage.ThumbnailImageUrl = googleImage.OriginalImageUrl;
                            googleImage.Title = d.responseData.results[0].titleNoFormatting.ToString().Trim();
                            googleImage.Content = d.responseData.results[0].contentNoFormatting.ToString().Trim();
                            googleImage.Copyright = d.responseData.results[0].visibleUrl.ToString().Trim();
                            googleImage.CopyrightUrl = "http://" +
                                                       d.responseData.results[0].visibleUrl.ToString().Trim();
                            googleImage.PageUrl = d.responseData.results[0].originalContextUrl.ToString().Trim();
                            googleImage.Date = DateTime.Now;
                        }
                        break;

                    case QueryResultTypes.Multi:
                        var googleImages = new List<SearchImage>();
                        if (d.responseData != null &&
                            d.responseData.results.Count != 0 &&
                            d.responseData.cursor.resultCount > 0)
                        {
                            for (int i = 0; i < (int) Count; i++)
                            {
                                var img = new SearchImage();
                                img.OriginalImageUrl = d.responseData.results[i].unescapedUrl.ToString().Trim();
                                img.ThumbnailImageUrl = img.OriginalImageUrl;
                                img.Title = d.responseData.results[i].titleNoFormatting.ToString().Trim();
                                img.Content = d.responseData.results[i].contentNoFormatting.ToString().Trim();
                                img.Copyright = d.responseData.results[i].visibleUrl.ToString().Trim();
                                img.CopyrightUrl = "http://" + d.responseData.results[i].visibleUrl.ToString().Trim();
                                img.PageUrl = d.responseData.results[i].originalContextUrl.ToString().Trim();
                                img.Date = DateTime.Now;
                                googleImages.Add(img);
                            }
                        }
                        Results = googleImages.ToArray();
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