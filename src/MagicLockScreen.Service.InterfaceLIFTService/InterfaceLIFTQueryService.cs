using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_InterfaceLIFTService.ApiParameters;
using MagicLockScreen_Service_InterfaceLIFTService.Models;
using MagicLockScreen_Service_InterfaceLIFTService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_InterfaceLIFTService
{
    /// <summary>
    ///     ID is [Guid("5F9D0BDF-26B2-49CD-80BD-62861D29B308")]
    /// </summary>
    public class InterfaceLIFTQueryService : Service
    {
        public InterfaceLIFTQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("5F9D0BDF-26B2-49CD-80BD-62861D29B308")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                NumberInPage = configXml.GetAttribute("NumberInPage").Check().StringToUInt();
                InterfaceLIFT.BaseUrl = BaseUrl;
                InterfaceLIFT.Url = ServiceApiUri;

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
                (ServiceApiParameters as InterfaceLIFTServiceApiParameter)
                    .PageIndex = (uint) parameters[0];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<InterfaceLIFT> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        InterfaceLIFT interfaceLIFT = null;

                        object queryResult = await QueryDataAsyncInternal();
                        if (null != queryResult)
                            interfaceLIFT = (new InterfaceLIFTQueryResult(queryResult)).Result as InterfaceLIFT;

                        return interfaceLIFT;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<InterfaceLIFT> QueryDataAsync(uint index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        uint pageIndex = index/NumberInPage + 1;
                        uint imgIndex = index%NumberInPage;
                        InitializeParameters(new object[] {pageIndex});
                        object queryResult = await QueryDataAsyncInternal();
                        if (queryResult != null)
                        {
                            var interfaceLIFT =
                                (new InterfaceLIFTQueryResult(queryResult, QueryResultTypes.Single, imgIndex, imgIndex))
                                    .Result as InterfaceLIFT;
                            return interfaceLIFT;
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

        public IAsyncOperation<IList<InterfaceLIFT>> QueryDataAsync(uint index, uint count)
        {
            return AsyncInfo.Run<IList<InterfaceLIFT>>(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        List<InterfaceLIFT> interfaceLIFTs = null;

                        uint pageIndex = index/NumberInPage + 1;
                        uint minIndex = index%NumberInPage;
                        uint maxIndex = NumberInPage - minIndex > count ? minIndex + count : NumberInPage;

                        while (count > 0)
                        {
                            InitializeParameters(new object[] {pageIndex});
                            object result = await QueryDataAsyncInternal();
                            if (result != null)
                            {
                                if (interfaceLIFTs == null)
                                    interfaceLIFTs = new List<InterfaceLIFT>();

                                var list =
                                    (new InterfaceLIFTQueryResult(result, QueryResultTypes.Multi, minIndex, maxIndex))
                                        .Results as IList<InterfaceLIFT>;
                                interfaceLIFTs.AddRange(list);
                            }

                            count = count - (maxIndex - minIndex);
                            pageIndex = pageIndex + (maxIndex < NumberInPage ? 0U : 1U);
                            minIndex = (maxIndex < NumberInPage ? maxIndex : 0U);
                            maxIndex = Math.Min(count, NumberInPage);
                        }

                        if (interfaceLIFTs != null)
                            return interfaceLIFTs.ToArray();
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