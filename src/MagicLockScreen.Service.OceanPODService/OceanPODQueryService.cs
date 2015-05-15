using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_OceanPODService.ApiParameters;
using MagicLockScreen_Service_OceanPODService.Models;
using MagicLockScreen_Service_OceanPODService.Results;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_OceanPODService
{
    /// <summary>
    ///     ID is [Guid("25D5FC07-74BE-478A-8234-7A6D90E64A3B")]
    /// </summary>
    public class OceanPODQueryService : Service
    {
        public OceanPODQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public string BaseUrl { get; private set; }
        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("25D5FC07-74BE-478A-8234-7A6D90E64A3B")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                OceanPOD.Url = ServiceApiUri;

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
                (ServiceApiParameters as OceanPODServiceApiParameter)
                    .Date = (DateTime) parameters[0];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<OceanPOD> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        OceanPOD oceanPOD = null;
                        int days = 0;

                        while ((oceanPOD == null || !oceanPOD.IsAvailable) && days < 99)
                        {
                            object queryResult = null;
                            DateTime date = DateTime.Now;
                            while (null == queryResult && days < 99)
                            {
                                date = date.Subtract(TimeSpan.FromDays(days));
                                InitializeParameters(new object[] {date});
                                queryResult = await QueryDataAsyncInternal();
                                days++;
                            }
                            if (null != queryResult)
                                oceanPOD =
                                    (new OceanPODQueryResult(queryResult, date)).Result as OceanPOD;
                        }
                        return oceanPOD;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<OceanPOD> QueryDataAsync(DateTime date)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        InitializeParameters(new object[] {date});
                        object queryResult = await QueryDataAsyncInternal();
                        if (queryResult != null)
                        {
                            var oceanPOD =
                                (new OceanPODQueryResult(queryResult, date)).Result as OceanPOD;
                            return oceanPOD;
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

        public IAsyncOperation<IList<OceanPOD>> QueryDataAsync(uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        count = Math.Min(count, MaxItemCount); // should be less than MaxItemCount
                        uint _deviationDays = 0;
                        IList<OceanPOD> oceanPODs = null;
                        for (uint index = 0; index < count; index++)
                        {
                            OceanPOD oceanPOD =
                                await QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            while (null == oceanPOD || !oceanPOD.IsAvailable)
                            {
                                if (_deviationDays++ > 10) break;
                                oceanPOD =
                                    await
                                    QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            }
                            if (oceanPODs == null) oceanPODs = new List<OceanPOD>();
                            if (oceanPOD != null)
                                oceanPODs.Add(oceanPOD);
                        }
                        if (oceanPODs != null)
                            return oceanPODs.ToArray() as IList<OceanPOD>;
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