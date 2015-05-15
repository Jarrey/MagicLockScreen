using System;
using System.Collections.Generic;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_ImageSearchService.Results
{
    public class FlickrImageSearchResult : QueryResult
    {
        public FlickrImageSearchResult(object result,
                                       QueryResultTypes type = QueryResultTypes.Single,
                                       uint index = 0,
                                       uint count = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Xml;

            if (type == QueryResultTypes.Single)
                Result = new FlickrSearchImage();
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(FlickrSearchImage[]);
                Index = index;
                Count = count;
            }

            ParseResponse();
        }

        public FlickrImageSearchResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Xml;
        }

        public FlickrImageSearchResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Xml;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                XmlElement flickrElement = (ResponseContent as XmlDocument).DocumentElement;

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var flickr = Result as FlickrSearchImage;
                        GenerateFlickrFromXml(flickrElement.SelectSingleNode("/rsp/photos/photo"), flickr);
                        break;

                    case QueryResultTypes.Multi:
                        var flickrs = new List<FlickrSearchImage>();
                        XmlNodeList photosElements = flickrElement.SelectNodes("/rsp/photos/photo");
                        for (uint i = 0; i < Count; i++)
                        {
                            var img = new FlickrSearchImage();
                            GenerateFlickrFromXml(photosElements[(int) i], img);
                            flickrs.Add(img);
                        }
                        Results = flickrs.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void GenerateFlickrFromXml(IXmlNode xmlelement, FlickrSearchImage flickr)
        {
            try
            {
                flickr.Id = xmlelement.SelectSingleNode("@id").InnerText;
                flickr.Owner = xmlelement.SelectSingleNode("@owner").InnerText;
                flickr.Secret = xmlelement.SelectSingleNode("@secret").InnerText;
                flickr.Server = xmlelement.SelectSingleNode("@server").InnerText;
                flickr.Farm = xmlelement.SelectSingleNode("@farm").InnerText;
                flickr.Title = xmlelement.SelectSingleNode("@title").InnerText;
                flickr.Copyright = flickr.UserProfileUrl;
                flickr.PageUrl = flickr.PhotoProfileUrl;
                flickr.CopyrightUrl = flickr.UserProfileUrl;
                flickr.Date = DateTime.Now;

                flickr.Content = xmlelement.SelectSingleNode("@title").InnerText;
                flickr.SetImageUrl();
                flickr.SetBigImageUrl();
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