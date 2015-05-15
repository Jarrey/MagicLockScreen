using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_BingImageService.Models
{
    public class BingImageCollection : ModelBase
    {
        private readonly uint _maxCount;
        private RangeIncrementalLoadingClass<BingImage> _bingImages;

        private bool _isLoading;

        public BingImageCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (BingImages == null)
                BingImages = new RangeIncrementalLoadingClass<BingImage>(maxCount,
                                                                         async (index, count) =>
                                                                         await LoadMoreBingImages(index, count));
        }

        public RangeIncrementalLoadingClass<BingImage> BingImages
        {
            get { return _bingImages; }
            private set { SetProperty(ref _bingImages, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            BingImages = new RangeIncrementalLoadingClass<BingImage>(_maxCount,
                                                                     async (index, count) =>
                                                                     await LoadMoreBingImages(index, count));
        }

        private async Task<IList<BingImage>> LoadMoreBingImages(uint index, uint count)
        {
            IsLoading = true;
            IList<BingImage> bingImages = null;

            try
            {
                var bingImageQueryService = this["BIQS"] as BingImageQueryService;

                bingImages = await bingImageQueryService.QueryDataAsync(index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return bingImages;
        }
    }
}