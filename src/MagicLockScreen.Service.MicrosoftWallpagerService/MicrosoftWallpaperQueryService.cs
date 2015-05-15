using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MagicLockScreen_Service_MicrosoftWallpaperService.ApiParameters;
using MagicLockScreen_Service_MicrosoftWallpaperService.Models;
using MagicLockScreen_Service_MicrosoftWallpaperService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace MagicLockScreen_Service_MicrosoftWallpaperService
{
    /// <summary>
    ///     ID is [Guid("E46696EB-BB08-404D-8E64-6AE59316B159")]
    /// </summary>
    public class MicrosoftWallpaperQueryService : Service
    {
        public MicrosoftWallpaperQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public uint MaxItemCount { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("E46696EB-BB08-404D-8E64-6AE59316B159")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                MicrosoftWallpaper.Url = ServiceApiUri;

                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Async Query API

        public IAsyncOperation<MicrosoftWallpaper> QueryDataAsync(uint index)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        if (!CheckNetworkStatus()) return null;

                        if (MicrosoftWallpaperQueryResult.MicrosoftWallpapers.Count > 0)
                        {
                            return (new MicrosoftWallpaperQueryResult(null, QueryResultTypes.Single, index, 1U)).Result as
                                    MicrosoftWallpaper;
                        }
                        else
                        {
                            MicrosoftWallpaper microsoftWallpaper = null;

                            object queryResult = await QueryDataAsyncInternal();
                            if (null != queryResult)
                                microsoftWallpaper =
                                    (new MicrosoftWallpaperQueryResult(queryResult, QueryResultTypes.Single, index, 1U)).Result as MicrosoftWallpaper;

                            return microsoftWallpaper;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<MicrosoftWallpaper>> QueryDataAsync(uint index, uint count)
        {
            return AsyncInfo.Run(async (token) =>
            {
                try
                {
                    if (!CheckNetworkStatus()) return null;

                    if (MicrosoftWallpaperQueryResult.MicrosoftWallpapers.Count > 0)
                    {
                        return (new MicrosoftWallpaperQueryResult(null, QueryResultTypes.Multi, index, count)).Results as
                               IList<MicrosoftWallpaper>;
                    }
                    else
                    {
                        IList<MicrosoftWallpaper> microsoftWallpapers = null;

                        object result = await QueryDataAsyncInternal();
                        if (result != null)
                            microsoftWallpapers =
                                (new MicrosoftWallpaperQueryResult(result, QueryResultTypes.Multi, index, count)).Results as
                                IList<MicrosoftWallpaper>;
                        return microsoftWallpapers;
                    }
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