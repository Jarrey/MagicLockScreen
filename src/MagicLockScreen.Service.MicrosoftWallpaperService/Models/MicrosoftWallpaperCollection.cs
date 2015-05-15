using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_MicrosoftWallpaperService.Models
{
    public class MicrosoftWallpaperCollection : ModelBase
    {
        private readonly uint _maxCount;
        private RangeIncrementalLoadingClass<MicrosoftWallpaper> _microsoftWallpapers;

        private bool _isLoading;

        public MicrosoftWallpaperCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (MicrosoftWallpapers == null)
                MicrosoftWallpapers = new RangeIncrementalLoadingClass<MicrosoftWallpaper>(maxCount,
                                                                   async (index, count) =>
                                                                   await LoadMoreMicrosoftWallpapers(index, count));
        }

        public RangeIncrementalLoadingClass<MicrosoftWallpaper> MicrosoftWallpapers
        {
            get { return _microsoftWallpapers; }
            private set { SetProperty(ref _microsoftWallpapers, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            MicrosoftWallpapers = new RangeIncrementalLoadingClass<MicrosoftWallpaper>(_maxCount,
                                                               async (index, count) =>
                                                               await LoadMoreMicrosoftWallpapers(index, count));
        }

        private async Task<IList<MicrosoftWallpaper>> LoadMoreMicrosoftWallpapers(uint index, uint count)
        {
            IsLoading = true;
            IList<MicrosoftWallpaper> microsoftWallpapers = null;
            try
            {
                var microsoftWallpaperQueryService = this["MSWQS"] as MicrosoftWallpaperQueryService;
                microsoftWallpapers = await microsoftWallpaperQueryService.QueryDataAsync(index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return microsoftWallpapers;
        }
    }
}