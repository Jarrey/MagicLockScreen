using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_WWFPictureService.Models
{
    public class WWFPictureCollection : ModelBase
    {
        private readonly uint _maxCount;
        private RangeIncrementalLoadingClass<WWFPicture> _wwfPictures;

        private bool _isLoading;

        public WWFPictureCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (WWFPictures == null)
                WWFPictures = new RangeIncrementalLoadingClass<WWFPicture>(maxCount,
                                                                                 async (index, count) =>
                                                                                 await
                                                                                 LoadMoreWWFPictures(index, count));
        }

        public RangeIncrementalLoadingClass<WWFPicture> WWFPictures
        {
            get { return _wwfPictures; }
            private set { SetProperty(ref _wwfPictures, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            WWFPictures = new RangeIncrementalLoadingClass<WWFPicture>(_maxCount,
                                                                             async (index, count) =>
                                                                             await LoadMoreWWFPictures(index, count));
        }

        private async Task<IList<WWFPicture>> LoadMoreWWFPictures(uint index, uint count)
        {
            IsLoading = true;
            IList<WWFPicture> wwfPictures = null;
            try
            {
                var wwfPictureQueryService = this["WWFPQS"] as WWFPictureQueryService;
                wwfPictures = await wwfPictureQueryService.QueryDataAsync(index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return wwfPictures;
        }
    }
}