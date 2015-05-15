using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_WWFPictureService.ApiParameters;
using MagicLockScreen_Service_WWFPictureService.Models;
using MagicLockScreen_Service_WWFPictureService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_WWFPictureService
{
    /// <summary>
    ///     ID is [Guid("F3C6C37A-D595-4933-8908-ECF28CADB49C")]
    /// </summary>
    public class WWFPictureQueryService : Service
    {
        public WWFPictureQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties
        public string BaseUrl { get; private set; }
        public uint MaxItemCount { get; private set; }
        public uint NumberInPage { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("F3C6C37A-D595-4933-8908-ECF28CADB49C")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                WWFPicture.BaseUrl = BaseUrl;

                NumberInPage = configXml.GetAttribute("NumberInPage").Check().StringToUInt();
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
                (ServiceApiParameters as WWFPictureServiceApiParameter)
                    .PageIndex = (uint)parameters[0];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<WWFPicture> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        WWFPicture wwfPicture = null;

                        object queryResult = await QueryDataAsyncInternal();
                        if (null != queryResult)
                            wwfPicture = (new WWFPictureQueryResult(queryResult)).Result as WWFPicture;

                        return wwfPicture;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<WWFPicture> QueryDataAsync(uint index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        uint pageIndex = index / NumberInPage + 1;
                        uint imgIndex = index % NumberInPage;
                        InitializeParameters(new object[] { pageIndex });
                        object queryResult = await QueryDataAsyncInternal();
                        if (queryResult != null)
                        {
                            var wwfPicture =
                                (new WWFPictureQueryResult(queryResult, QueryResultTypes.Single, imgIndex, imgIndex))
                                    .Result as WWFPicture;
                            return wwfPicture;
                        }
                        else return null;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<WWFPicture>> QueryDataAsync(uint index, uint count)
        {
            return AsyncInfo.Run<IList<WWFPicture>>(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        List<WWFPicture> wwfPictures = null;

                        uint pageIndex = index / NumberInPage + 1;
                        uint minIndex = index % NumberInPage;
                        uint maxIndex = NumberInPage - minIndex > count ? minIndex + count : NumberInPage;

                        while (count > 0)
                        {
                            InitializeParameters(new object[] { pageIndex });
                            object result = await QueryDataAsyncInternal();
                            if (result != null)
                            {
                                if (wwfPictures == null)
                                    wwfPictures = new List<WWFPicture>();

                                var list =
                                    (new WWFPictureQueryResult(result, QueryResultTypes.Multi, minIndex, maxIndex))
                                        .Results as IList<WWFPicture>;
                                wwfPictures.AddRange(list);
                            }

                            count = count - (maxIndex - minIndex);
                            pageIndex = pageIndex + (maxIndex < NumberInPage ? 0U : 1U);
                            minIndex = (maxIndex < NumberInPage ? maxIndex : 0U);
                            maxIndex = Math.Min(count, NumberInPage);
                        }

                        if (wwfPictures != null)
                            return wwfPictures.ToArray();
                        else return null;
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