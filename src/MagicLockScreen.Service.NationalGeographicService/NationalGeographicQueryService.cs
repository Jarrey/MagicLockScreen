using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_NationalGeographicService.Models;
using MagicLockScreen_Service_NationalGeographicService.Results;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_NationalGeographicService
{
    /// <summary>
    ///     ID is [Guid("9F59EF49-8861-4DE9-B3AE-E82233CF0A95")]
    /// </summary>
    public class NationalGeographicQueryService : Service
    {
        public NationalGeographicQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
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

            if (ID.CompareTo(new Guid("0cec6bd0-8612-4aac-9f33-3c2fa5c59207")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                NationalGeographic.BaseUrl = BaseUrl;

                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Async Query API

        public IAsyncOperation<NationalGeographic> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        NationalGeographic nationalGeographic = null;

                        object queryResult = await QueryDataAsyncInternal();
                        if (null != queryResult)
                            nationalGeographic =
                                (new NationalGeographicQueryResult(queryResult)).Result as NationalGeographic;

                        return nationalGeographic;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<NationalGeographic> QueryDataAsync(string link)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        object queryResult = await QueryDataAsyncInternal(link);
                        if (queryResult != null)
                        {
                            var nationalGeographic =
                                (new NationalGeographicQueryResult(queryResult)).Result as NationalGeographic;
                            return nationalGeographic;
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

        public IAsyncOperation<IList<NationalGeographic>> QueryDataAsync(uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        count = Math.Min(count, MaxItemCount); // should be less than MaxItemCount
                        string _previousPhotoLink = string.Empty;
                        IList<NationalGeographic> nationalGeographics = null;

                        for (uint index = 0; index < count; index++)
                        {
                            NationalGeographic nationalGeographic = null;
                            if (index == 0 || string.IsNullOrEmpty(_previousPhotoLink))
                                nationalGeographic = await QueryDataAsync();
                            else nationalGeographic = await QueryDataAsync(_previousPhotoLink);
                            if (nationalGeographic != null)
                            {
                                if (nationalGeographics == null) nationalGeographics = new List<NationalGeographic>();
                                nationalGeographics.Add(nationalGeographic);
                                _previousPhotoLink = nationalGeographic.PreviousPhotoLink;
                            }
                        }
                        if (nationalGeographics != null)
                            return nationalGeographics.ToArray() as IList<NationalGeographic>;
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