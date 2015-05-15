using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_FlickrService.ApiParameters;
using MagicLockScreen_Service_FlickrService.Models;
using MagicLockScreen_Service_FlickrService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_FlickrService
{
    /// <summary>
    ///     ID is [Guid("ecdba335-b6bc-4348-be6d-2679c273b7ee")]
    /// </summary>
    public class FlickrQueryService : Service
    {
        public FlickrQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("ecdba335-b6bc-4348-be6d-2679c273b7ee")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseImageUrl = configXml.GetAttribute("BaseImageUrl").Check();
                Flickr.BaseImageUrl = BaseImageUrl;
                BaseUserProfileUrl = configXml.GetAttribute("BaseUserProfileUrl").Check();
                Flickr.BaseUserProfileUrl = BaseUserProfileUrl;
                BasePhotoProfileUrl = configXml.GetAttribute("BasePhotoProfileUrl").Check();
                Flickr.BasePhotoProfileUrl = BasePhotoProfileUrl;

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
                (ServiceApiParameters as FlickrServiceApiParameter)
                    .PerPage = parameters[0].ToString().StringToInt();
                (ServiceApiParameters as FlickrServiceApiParameter)
                    .Page = parameters[1].ToString().StringToInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<Flickr> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        Flickr flickr = null;
                        InitializeParameters(new object[] {1, 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            flickr = (new FlickrQueryResult(result)).Result as Flickr;
                        return flickr;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<Flickr> QueryDataAsync(int index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        Flickr flickr = null;
                        InitializeParameters(new object[] {1, index + 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            flickr = (new FlickrQueryResult(result)).Result as Flickr;
                        return flickr;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<Flickr>> QueryDataAsync(uint index, uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<Flickr> flickrs = null;
                        InitializeParameters(new object[] {MaxItemCount, 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            flickrs =
                                (new FlickrQueryResult(result, QueryResultTypes.Multi, index, count)).Results as
                                IList<Flickr>;
                        return flickrs;
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