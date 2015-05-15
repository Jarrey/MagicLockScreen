using System;
using System.Collections.Generic;
using MagicLockScreen_Service_BingImageService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_BingImageService.Results
{
    public class BingImageQueryResult : QueryResult
    {
        public BingImageQueryResult(object result, QueryResultTypes type = QueryResultTypes.Single)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Xml;

            if (type == QueryResultTypes.Single)
                Result = new BingImage();
            else if (type == QueryResultTypes.Multi)
                Results = default(BingImage[]);

            ParseResponse();
        }

        public BingImageQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Xml;
        }

        public BingImageQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Xml;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                XmlElement bingImageElement = null;
                if (ResponseContent != null && ResponseContent is XmlDocument)
                    bingImageElement = (ResponseContent as XmlDocument).DocumentElement;
                else return;

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var bingImage = Result as BingImage;
                        bingImage.OriginalImageUrl = BingImage.BaseUrl +
                                                     bingImageElement.SelectSingleNode("/images/image/url").InnerText;
                        bingImage.ThumbnailImageUrl = bingImage.OriginalImageUrl;
                        break;

                    case QueryResultTypes.Multi:
                        var bingImages = new List<BingImage>();
                        foreach (IXmlNode xmlNode in bingImageElement.SelectNodes("/images/image"))
                        {
                            var img = new BingImage();
                            img.OriginalImageUrl = BingImage.BaseUrl + xmlNode.SelectSingleNode("url").InnerText;
                            img.ThumbnailImageUrl = img.OriginalImageUrl;
                            img.Copyright = xmlNode.SelectSingleNode("copyright").InnerText;
                            img.CopyrightLink = xmlNode.SelectSingleNode("copyrightlink").InnerText;
                            img.Date = xmlNode.SelectSingleNode("startdate").InnerText.StringToDateTime();
                            bingImages.Add(img);
                        }
                        Results = bingImages.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }
    }
}