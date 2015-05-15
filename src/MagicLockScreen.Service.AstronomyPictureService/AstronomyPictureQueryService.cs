using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_AstronomyPictureService.ApiParameters;
using MagicLockScreen_Service_AstronomyPictureService.Models;
using MagicLockScreen_Service_AstronomyPictureService.Results;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_AstronomyPictureService
{
    /// <summary>
    ///     ID is [Guid("9F59EF49-8861-4DE9-B3AE-E82233CF0A95")]
    /// </summary>
    public class AstronomyPictureQueryService : Service
    {
        public AstronomyPictureQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("9F59EF49-8861-4DE9-B3AE-E82233CF0A95")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                AstronomyPicture.BaseUrl = BaseUrl;
                AstronomyPicture.Url = ServiceApiUri;

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
                (ServiceApiParameters as AstronomyPictureServiceApiParameter)
                    .Date = (DateTime) parameters[0];
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<AstronomyPicture> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        AstronomyPicture astronomyPicture = null;
                        int days = 0;

                        while ((astronomyPicture == null || !astronomyPicture.IsAvailable) && days < 10)
                        {
                            object queryResult = null;
                            DateTime date = DateTime.Now;
                            while (null == queryResult && days < 10)
                            {
                                date = date.Subtract(TimeSpan.FromDays(days));
                                InitializeParameters(new object[] {date});
                                queryResult = await QueryDataAsyncInternal();
                                days++;
                            }
                            if (null != queryResult)
                                astronomyPicture =
                                    (new AstronomyPictureQueryResult(queryResult, date)).Result as AstronomyPicture;
                        }
                        return astronomyPicture;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<AstronomyPicture> QueryDataAsync(DateTime date)
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
                            var astronomyPicture =
                                (new AstronomyPictureQueryResult(queryResult, date)).Result as AstronomyPicture;
                            return astronomyPicture;
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

        public IAsyncOperation<IList<AstronomyPicture>> QueryDataAsync(uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        count = Math.Min(count, MaxItemCount); // should be less than MaxItemCount
                        uint _deviationDays = 0;
                        IList<AstronomyPicture> astronomyPictures = null;
                        for (uint index = 0; index < count; index++)
                        {
                            AstronomyPicture astronomyPicture =
                                await QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            while (null == astronomyPicture || !astronomyPicture.IsAvailable)
                            {
                                if (_deviationDays++ > 10) break;
                                astronomyPicture =
                                    await
                                    QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(index + _deviationDays)));
                            }
                            if (astronomyPictures == null) astronomyPictures = new List<AstronomyPicture>();
                            if (astronomyPicture != null)
                                astronomyPictures.Add(astronomyPicture);
                        }
                        if (astronomyPictures != null)
                            return astronomyPictures.ToArray() as IList<AstronomyPicture>;
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