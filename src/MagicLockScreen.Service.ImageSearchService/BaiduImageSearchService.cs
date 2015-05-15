using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_ImageSearchService.ApiParameters;
using MagicLockScreen_Service_ImageSearchService.Models;
using MagicLockScreen_Service_ImageSearchService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_ImageSearchService
{
    /// <summary>
    ///     ID is [Guid("1270A168-641F-48BD-95CB-544D49985764")]
    /// </summary>
    public class BaiduImageSearchService : Service
    {
        public BaiduImageSearchService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("1270A168-641F-48BD-95CB-544D49985764")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        protected override void InitializeParameters(object[] parameters)
        {
            try
            {
                base.InitializeParameters(parameters);
                (ServiceApiParameters as BaiduImageSearchServiceApiParameter)
                    .Keyword = Uri.EscapeUriString(parameters[0].ToString());
                (ServiceApiParameters as BaiduImageSearchServiceApiParameter)
                    .RecordNumber = parameters[1].ToString().StringToInt();
                (ServiceApiParameters as BaiduImageSearchServiceApiParameter)
                    .PageNumber = parameters[2].ToString().StringToInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<SearchImage> QueryDataAsync(string queryText, int index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        SearchImage baiduImage = null;
                        InitializeParameters(new object[] {queryText, 1, index});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            baiduImage = (new BaiduImageSearchResult(result)).Result as SearchImage;
                        return baiduImage;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<SearchImage>> QueryDataAsync(string queryText, uint index, uint number)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<SearchImage> baiduImages = null;
                        InitializeParameters(new object[] {queryText, number, index});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            baiduImages =
                                (new BaiduImageSearchResult(result, QueryResultTypes.Multi, index, number)).Results as
                                IList<SearchImage>;
                        return baiduImages;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        #endregion
    }
}