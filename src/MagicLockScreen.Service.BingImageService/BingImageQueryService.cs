using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_BingImageService.ApiParameters;
using MagicLockScreen_Service_BingImageService.Models;
using MagicLockScreen_Service_BingImageService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace MagicLockScreen_Service_BingImageService
{
    /// <summary>
    ///     ID is [Guid("5b986175-763c-4af4-b616-b96bd04743c0")]
    /// </summary>
    public class BingImageQueryService : Service
    {
        public BingImageQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public string BaseUrl { get; private set; }
        public string[] Markets { get; private set; }
        public string CurrentMarket { get; private set; }
        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("5b986175-763c-4af4-b616-b96bd04743c0")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                BaseUrl = configXml.GetAttribute("BaseUrl").Check();
                BingImage.BaseUrl = BaseUrl;

                Markets = configXml.GetAttribute("Markets").Check().StringToArray();
                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();

                // Set the current market
                var language = new Language(GlobalizationPreferences.Languages[0]);
                string currentLanaguageTag = language.LanguageTag.Replace(language.Script + "-", "");
                CurrentMarket = Markets[0];
                foreach (string market in Markets)
                {
                    if (market.ToUpperInvariant().Contains(currentLanaguageTag.ToUpperInvariant()))
                    {
                        CurrentMarket = market;
                        break;
                    }
                }
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
                (ServiceApiParameters as BingImageServiceApiParameter)
                    .Index = parameters[0].ToString().StringToInt();
                (ServiceApiParameters as BingImageServiceApiParameter)
                    .Number = parameters[1].ToString().StringToInt();
                (ServiceApiParameters as BingImageServiceApiParameter)
                    .Market = parameters[2].ToString();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #region Async Query API

        public IAsyncOperation<BingImage> QueryDataAsync()
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        BingImage bingImage = null;
                        InitializeParameters(new object[] {0, 1, CurrentMarket});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            bingImage = (new BingImageQueryResult(result)).Result as BingImage;
                        return bingImage;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncAction QueryDataAsync(BingImage bingImage)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return;

                        InitializeParameters(new object[] {0, 1, CurrentMarket});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                        {
                            var bingImageQueryResult = new BingImageQueryResult(bingImage);
                            bingImage = bingImageQueryResult.ParseQueryResult(result).Result as BingImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });
        }

        public IAsyncOperation<IList<BingImage>> QueryDataAsync(uint index, uint number)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        IList<BingImage> bingImages = null;
                        InitializeParameters(new object[] {index, number, CurrentMarket});
                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            bingImages =
                                (new BingImageQueryResult(result, QueryResultTypes.Multi)).Results as IList<BingImage>;
                        return bingImages;
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