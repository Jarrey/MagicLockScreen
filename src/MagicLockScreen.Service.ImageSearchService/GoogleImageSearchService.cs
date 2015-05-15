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
    ///     ID is [Guid("15E0288C-0B25-4D78-8132-1F85EEB52E87")]
    /// </summary>
    public class GoogleImageSearchService : Service
    {
        public GoogleImageSearchService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("15E0288C-0B25-4D78-8132-1F85EEB52E87")) != 0)
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
                (ServiceApiParameters as GoogleImageSearchServiceApiParameter)
                    .Keyword = Uri.EscapeUriString(parameters[0].ToString());
                (ServiceApiParameters as GoogleImageSearchServiceApiParameter)
                    .RecordNumber = parameters[1].ToString().StringToInt();
                (ServiceApiParameters as GoogleImageSearchServiceApiParameter)
                    .StartIndex = parameters[2].ToString().StringToInt();
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

                        SearchImage googleImage = null;
                        InitializeParameters(new object[] {queryText, 1, index});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            googleImage = (new GoogleImageSearchResult(result)).Result as SearchImage;
                        return googleImage;
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

                        IList<SearchImage> googleImages = null;
                        InitializeParameters(new object[] {queryText, number, index});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            googleImages =
                                (new GoogleImageSearchResult(result, QueryResultTypes.Multi, index, number)).Results as
                                IList<SearchImage>;
                        return googleImages;
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