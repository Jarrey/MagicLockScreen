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
    ///     ID is [Guid("EA4132D3-5361-46C8-BD31-A952454945D5")]
    /// </summary>
    public class InfoImageSearchService : Service
    {
        public InfoImageSearchService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public uint MaxItemCount { get; private set; }
        public uint NumberInPage { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("EA4132D3-5361-46C8-BD31-A952454945D5")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();
                NumberInPage = configXml.GetAttribute("NumberInPage").Check().StringToUInt();
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
                (ServiceApiParameters as InfoImageSearchServiceApiParameter)
                    .PageNumber = parameters[0].ToString().StringToInt();
                (ServiceApiParameters as InfoImageSearchServiceApiParameter)
                    .Keyword = Uri.EscapeUriString(parameters[1].ToString());
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<SearchImage> QueryDataAsync(string queryText, uint index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        SearchImage infoImage = null;

                        uint pageIndex = index / NumberInPage + 1;
                        uint imgIndex = index % NumberInPage;

                        InitializeParameters(new object[] { pageIndex, queryText });
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            infoImage = (new InfoImageSearchResult(result, QueryResultTypes.Single, imgIndex, imgIndex)).Result as SearchImage;
                        return infoImage;
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

                        IList<SearchImage> infoImages = null;

                        uint pageIndex = index / NumberInPage + 1;
                        uint minIndex = index % NumberInPage;
                        uint maxIndex = NumberInPage - minIndex > number ? minIndex + number : NumberInPage;

                        InitializeParameters(new object[] { pageIndex, queryText });
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            infoImages =
                                (new InfoImageSearchResult(result, QueryResultTypes.Multi, minIndex, maxIndex)).Results as
                                IList<SearchImage>;
                        return infoImages;
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