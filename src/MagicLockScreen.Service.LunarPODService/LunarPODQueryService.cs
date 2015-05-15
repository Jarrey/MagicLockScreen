using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_LunarPODService.ApiParameters;
using MagicLockScreen_Service_LunarPODService.Models;
using MagicLockScreen_Service_LunarPODService.Results;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_LunarPODService
{
    /// <summary>
    ///     ID is [Guid("e35a00b4-f6cf-478d-80c3-1043954dcba8")]
    /// </summary>
    public class LunarPODQueryService : Service
    {
        public LunarPODQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("e35a00b4-f6cf-478d-80c3-1043954dcba8")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                LunarPOD.BaseUrl = BaseUrl;
                LunarPOD.Url = ServiceApiUri;

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
                (ServiceApiParameters as LunarPODServiceApiParameter)
                    .Month = (int) parameters[0];
                (ServiceApiParameters as LunarPODServiceApiParameter)
                    .Date = (DateTime) parameters[1];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<LunarPOD> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        LunarPOD lunarPOD = null;
                        int days = 0;

                        while ((lunarPOD == null || !lunarPOD.IsAvailable) && days < 99)
                        {
                            object queryResult = null;
                            DateTime date = DateTime.Now;
                            while (null == queryResult && days < 99)
                            {
                                date = date.Subtract(TimeSpan.FromDays(days));
                                InitializeParameters(new object[] {date.Month, date});
                                queryResult = await QueryDataAsyncInternal();
                                days++;
                            }
                            if (null != queryResult)
                                lunarPOD =
                                    (new LunarPODQueryResult(queryResult, date)).Result as LunarPOD;
                        }
                        return lunarPOD;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<LunarPOD> QueryDataAsync(DateTime date)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        InitializeParameters(new object[] {date.Month, date});
                        object queryResult = await QueryDataAsyncInternal();
                        if (queryResult != null)
                        {
                            var lunarPOD =
                                (new LunarPODQueryResult(queryResult, date)).Result as LunarPOD;
                            return lunarPOD;
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

        public IAsyncOperation<IList<LunarPOD>> QueryDataAsync(uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        count = Math.Min(count, MaxItemCount); // should be less than MaxItemCount
                        uint _deviationDays = 0;
                        IList<LunarPOD> lunarPODs = null;
                        for (uint index = 0; index < count; index++)
                        {
                            LunarPOD lunarPOD =
                                await QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));

                            while (null == lunarPOD || !lunarPOD.IsAvailable)
                            {
                                if (_deviationDays++ > 10) break;
                                lunarPOD =
                                    await
                                    QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            }
                            if (lunarPODs == null) lunarPODs = new List<LunarPOD>();
                            if (lunarPOD != null)
                                lunarPODs.Add(lunarPOD);
                        }
                        if (lunarPODs != null)
                            return lunarPODs.ToArray() as IList<LunarPOD>;
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