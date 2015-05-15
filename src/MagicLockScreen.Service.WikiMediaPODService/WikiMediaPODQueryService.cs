using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_WikiMediaPODService.ApiParameters;
using MagicLockScreen_Service_WikiMediaPODService.Models;
using MagicLockScreen_Service_WikiMediaPODService.Results;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_WikiMediaPODService
{
    /// <summary>
    ///     ID is [Guid("887236a1-ea41-4f16-b3e8-ac0e399861ab")]
    /// </summary>
    public class WikiMediaPODQueryService : Service
    {
        public WikiMediaPODQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("887236a1-ea41-4f16-b3e8-ac0e399861ab")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                WikiMediaPOD.BaseUrl = BaseUrl;
                WikiMediaPOD.Url = ServiceApiUri;

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
                (ServiceApiParameters as WikiMediaPODServiceApiParameter)
                    .Date = (DateTime) parameters[0];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<WikiMediaPOD> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        WikiMediaPOD wikiMediaPOD = null;
                        int days = 0;

                        while ((wikiMediaPOD == null || !wikiMediaPOD.IsAvailable) && days < 99)
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
                                wikiMediaPOD =
                                    (new WikiMediaPODQueryResult(queryResult, date)).Result as WikiMediaPOD;
                        }
                        return wikiMediaPOD;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<WikiMediaPOD> QueryDataAsync(DateTime date)
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
                            var wikiMediaPOD =
                                (new WikiMediaPODQueryResult(queryResult, date)).Result as WikiMediaPOD;
                            return wikiMediaPOD;
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

        public IAsyncOperation<IList<WikiMediaPOD>> QueryDataAsync(uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        count = Math.Min(count, MaxItemCount); // should be less than MaxItemCount
                        uint _deviationDays = 0;
                        IList<WikiMediaPOD> wikiMediaPODs = null;
                        for (uint index = 0; index < count; index++)
                        {
                            WikiMediaPOD wikiMediaPOD =
                                await QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            while (null == wikiMediaPOD || !wikiMediaPOD.IsAvailable)
                            {
                                if (_deviationDays++ > 10) break;
                                wikiMediaPOD =
                                    await
                                    QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            }
                            if (wikiMediaPODs == null) wikiMediaPODs = new List<WikiMediaPOD>();
                            if (wikiMediaPOD != null)
                                wikiMediaPODs.Add(wikiMediaPOD);
                        }
                        if (wikiMediaPODs != null)
                            return wikiMediaPODs.ToArray() as IList<WikiMediaPOD>;
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