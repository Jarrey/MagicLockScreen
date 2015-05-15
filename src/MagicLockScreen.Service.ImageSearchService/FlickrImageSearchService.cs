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
    ///     ID is [Guid("F9B6617A-B4DF-4BE6-8CE1-2DE544A5285D")]
    /// </summary>
    public class FlickrImageSearchService : Service
    {
        public FlickrImageSearchService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public string BaseImageUrl { get; private set; }
        public string BaseUserProfileUrl { get; private set; }
        public string BasePhotoProfileUrl { get; private set; }
        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("F9B6617A-B4DF-4BE6-8CE1-2DE544A5285D")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseImageUrl = configXml.GetAttribute("BaseImageUrl").Check();
                FlickrSearchImage.BaseImageUrl = BaseImageUrl;
                BaseUserProfileUrl = configXml.GetAttribute("BaseUserProfileUrl").Check();
                FlickrSearchImage.BaseUserProfileUrl = BaseUserProfileUrl;
                BasePhotoProfileUrl = configXml.GetAttribute("BasePhotoProfileUrl").Check();
                FlickrSearchImage.BasePhotoProfileUrl = BasePhotoProfileUrl;

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
                (ServiceApiParameters as FlickrImageSearchServiceApiParameter)
                    .Keyword = Uri.EscapeUriString(parameters[0].ToString());
                (ServiceApiParameters as FlickrImageSearchServiceApiParameter)
                    .RecordPerPageNumber = parameters[1].ToString().StringToInt();
                (ServiceApiParameters as FlickrImageSearchServiceApiParameter)
                    .PageNumber = parameters[2].ToString().StringToInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<FlickrSearchImage> QueryDataAsync(string queryText, int index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        FlickrSearchImage flickrImage = null;
                        InitializeParameters(new object[] {queryText, 1, index + 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            flickrImage = (new FlickrImageSearchResult(result)).Result as FlickrSearchImage;
                        return flickrImage;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<FlickrSearchImage>> QueryDataAsync(string queryText, uint index, uint number)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<FlickrSearchImage> flickrImages = null;
                        InitializeParameters(new object[] {queryText, number, index + 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            flickrImages =
                                (new FlickrImageSearchResult(result, QueryResultTypes.Multi, index, number)).Results as
                                IList<FlickrSearchImage>;
                        return flickrImages;
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