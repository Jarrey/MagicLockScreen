using System;
using System.Collections.Generic;
using MagicLockScreen_Service_GooglePicasaService.Models;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_GooglePicasaService.Results
{
    public class GooglePicasaQueryResult : QueryResult
    {
        public GooglePicasaQueryResult(object result,
                                       QueryResultTypes type = QueryResultTypes.Single)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Xml;

            if (type == QueryResultTypes.Single)
                Result = new GooglePicasa();
            else if (type == QueryResultTypes.Multi)
            {
                Results = default(GooglePicasa[]);
            }

            ParseResponse();
        }

        public GooglePicasaQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Xml;
        }

        public GooglePicasaQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Xml;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                XmlElement googlePicasaElement = (ResponseContent as XmlDocument).DocumentElement;

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        var googlePicasa = Result as GooglePicasa;
                        GenerateGooglePicasaFromXml(
                            googlePicasaElement.SelectSingleNodeNS(
                                "/" + DefalutPrefix + ":feed/" + DefalutPrefix + ":entry", XmlNamespace), googlePicasa);
                        break;

                    case QueryResultTypes.Multi:
                        var googlePicasas = new List<GooglePicasa>();
                        XmlNodeList photosElements =
                            googlePicasaElement.SelectNodesNS(
                                "/" + DefalutPrefix + ":feed/" + DefalutPrefix + ":entry", XmlNamespace);
                        foreach (XmlElement xmlElement in photosElements)
                        {
                            var img = new GooglePicasa();
                            GenerateGooglePicasaFromXml(xmlElement, img);
                            googlePicasas.Add(img);
                        }
                        Results = googlePicasas.ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void GenerateGooglePicasaFromXml(IXmlNode xmlelement, GooglePicasa googlePicasa)
        {
            try
            {
                googlePicasa.Id = xmlelement.SelectSingleNodeNS(DefalutPrefix + ":id", XmlNamespace).InnerText;
                googlePicasa.Summary = xmlelement.SelectSingleNodeNS(DefalutPrefix + ":summary", XmlNamespace).InnerText;
                googlePicasa.Title = xmlelement.SelectSingleNodeNS(DefalutPrefix + ":title", XmlNamespace).InnerText;
                googlePicasa.OriginalImageUrl =
                    xmlelement.SelectSingleNodeNS(DefalutPrefix + ":content/@src", XmlNamespace).InnerText;
                googlePicasa.Owner =
                    xmlelement.SelectSingleNodeNS(DefalutPrefix + ":author/" + DefalutPrefix + ":name", XmlNamespace)
                              .InnerText;
                googlePicasa.OwnerUrl =
                    xmlelement.SelectSingleNodeNS(DefalutPrefix + ":author/" + DefalutPrefix + ":uri", XmlNamespace)
                              .InnerText;
                googlePicasa.Updated =
                    xmlelement.SelectSingleNodeNS(DefalutPrefix + ":updated", XmlNamespace)
                              .InnerText.StringToDateTime(DateTimeFormat);
                googlePicasa.Published =
                    xmlelement.SelectSingleNodeNS(DefalutPrefix + ":published", XmlNamespace)
                              .InnerText.StringToDateTime(DateTimeFormat);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region XML Namespace

        private const string DefalutPrefix = "default";
        private const string XmlNamespace = @"xmlns:" + DefalutPrefix + "='http://www.w3.org/2005/Atom'";
        private const string DateTimeFormat = @"yyyy-MM-ddTHH:mm:ss.fffZ";

        #endregion

        #region Parameters

        #endregion
    }
}