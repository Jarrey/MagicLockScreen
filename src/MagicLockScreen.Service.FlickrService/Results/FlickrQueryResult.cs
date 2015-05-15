using System;
using System.Collections.Generic;
using MagicLockScreen_Service_FlickrService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Html;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_FlickrService.Results
{
    public class FlickrQueryResult : QueryResult
    {
        public FlickrQueryResult(object result,
                                 QueryResultTypes type = QueryResultTypes.Single,
                                 uint index = 0,
                                 uint count = 0)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Xml;

            if (type == QueryResultTypes.Single)
                Result = new Flickr();
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(Flickr[]);
                Index = index;
                Count = count;
            }

            ParseResponse();
        }

        public FlickrQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Xml;
        }

        public FlickrQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Xml;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                XmlElement flickrElement = null;
                if (ResponseContent != null && ResponseContent is XmlDocument)
                    flickrElement = (ResponseContent as XmlDocument).DocumentElement;
                else return;

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var flickr = Result as Flickr;
                        GenerateFlickrFromXml(flickrElement.SelectSingleNode("/rsp/photos/photo"), flickr);
                        break;

                    case QueryResultTypes.Multi:
                        var flickrs = new List<Flickr>();
                        XmlNodeList photosElements = flickrElement.SelectNodes("/rsp/photos/photo");
                        for (uint i = Index; i < Index + Count; i++)
                        {
                            var img = new Flickr();
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

        private void GenerateFlickrFromXml(IXmlNode xmlelement, Flickr flickr)
        {
            try
            {
                flickr.Id = xmlelement.SelectSingleNode("@id").InnerText;
                flickr.Owner = xmlelement.SelectSingleNode("@owner").InnerText;
                flickr.Secret = xmlelement.SelectSingleNode("@secret").InnerText;
                flickr.Server = xmlelement.SelectSingleNode("@server").InnerText;
                flickr.Farm = xmlelement.SelectSingleNode("@farm").InnerText;
                flickr.Title = xmlelement.SelectSingleNode("@title").InnerText;
                flickr.OwnerName = xmlelement.SelectSingleNode("@ownername").InnerText;
                flickr.DateUpload = xmlelement.SelectSingleNode("@dateupload").InnerText.JSStringToDateTime();
                flickr.Description = HtmlUtilities.ConvertToText(xmlelement.SelectSingleNode("description").InnerText);
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