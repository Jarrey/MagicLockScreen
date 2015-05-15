using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_FlickrService.Models
{
    public class FlickrCollection : ModelBase
    {
        private readonly uint _maxCount;
        private RangeIncrementalLoadingClass<Flickr> _flickrs;

        private bool _isLoading;

        public FlickrCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (Flickrs == null)
                Flickrs = new RangeIncrementalLoadingClass<Flickr>(maxCount,
                                                                   async (index, count) =>
                                                                   await LoadMoreFlickrs(index, count));
        }

        public RangeIncrementalLoadingClass<Flickr> Flickrs
        {
            get { return _flickrs; }
            private set { SetProperty(ref _flickrs, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            Flickrs = new RangeIncrementalLoadingClass<Flickr>(_maxCount,
                                                               async (index, count) =>
                                                               await LoadMoreFlickrs(index, count));
        }

        private async Task<IList<Flickr>> LoadMoreFlickrs(uint index, uint count)
        {
            IsLoading = true;
            IList<Flickr> flickrs = null;
            try
            {
                var flickrQueryService = this["FQS"] as FlickrQueryService;
                flickrs = await flickrQueryService.QueryDataAsync(index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return flickrs;
        }
    }
}