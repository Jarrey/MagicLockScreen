using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_GooglePicasaService.ApiParameters;
using MagicLockScreen_Service_GooglePicasaService.Models;
using MagicLockScreen_Service_GooglePicasaService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_GooglePicasaService
{
    /// <summary>
    ///     ID is [Guid("7C9D2FDB-D43E-4573-A6EF-BF609445B3F7")]
    /// </summary>
    public class GooglePicasaQueryService : Service
    {
        public GooglePicasaQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("7C9D2FDB-D43E-4573-A6EF-BF609445B3F7")) != 0)
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
                (ServiceApiParameters as GooglePicasaServiceApiParameter)
                    .KeyWord = parameters[0].ToString();
                (ServiceApiParameters as GooglePicasaServiceApiParameter)
                    .Count = parameters[1].ToString().StringToInt();
                (ServiceApiParameters as GooglePicasaServiceApiParameter)
                    .StartIndex = parameters[2].ToString().StringToInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<GooglePicasa> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        GooglePicasa googlePicasa = null;
                        InitializeParameters(new object[] {"nature", 1, 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            googlePicasa = (new GooglePicasaQueryResult(result)).Result as GooglePicasa;
                        return googlePicasa;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<GooglePicasa>> QueryDataAsync(string keyword, uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<GooglePicasa> googlePicasas = null;
                        InitializeParameters(new object[] {keyword, count, 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            googlePicasas =
                                (new GooglePicasaQueryResult(result, QueryResultTypes.Multi)).Results as
                                IList<GooglePicasa>;
                        return googlePicasas;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<GooglePicasa>> QueryDataAsync(uint index, uint count, string keyword)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<GooglePicasa> googlePicasas = null;
                        InitializeParameters(new object[] {keyword, (int) count, (int) index + 1});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            googlePicasas =
                                (new GooglePicasaQueryResult(result, QueryResultTypes.Multi)).Results as
                                IList<GooglePicasa>;
                        return googlePicasas;
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