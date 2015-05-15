﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    public class FlickrImageCollection : ModelBase
    {
        private readonly uint _maxCount;
        private readonly string _queryText = string.Empty;
        private RangeIncrementalLoadingClass<FlickrSearchImage> _images;

        private bool _isLoading;

        private bool _isResultEmpty;

        public FlickrImageCollection(uint maxCount, IService[] services, string queryText)
            : base(services)
        {
            _queryText = queryText;
            _maxCount = maxCount;
            if (Images == null)
                Images = new RangeIncrementalLoadingClass<FlickrSearchImage>(maxCount,
                                                                             async (index, count) =>
                                                                             await LoadMoreFlickrImages(index, count));
        }

        public RangeIncrementalLoadingClass<FlickrSearchImage> Images
        {
            get { return _images; }
            private set { SetProperty(ref _images, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public bool IsResultEmpty
        {
            get { return _isResultEmpty; }
            private set
            {
                if (value)
                {
                    IsLoading = false;
                    Images = null;
                    new MessagePopup("No Results").Show(10);
                }
                SetProperty(ref _isResultEmpty, value);
            }
        }

        public void RefreshCollection()
        {
            Images = new RangeIncrementalLoadingClass<FlickrSearchImage>(_maxCount,
                                                                         async (index, count) =>
                                                                         await LoadMoreFlickrImages(index, count));
        }

        private async Task<IList<FlickrSearchImage>> LoadMoreFlickrImages(uint index, uint count)
        {
            IsLoading = true;
            IList<FlickrSearchImage> images = null;

            try
            {
                var flickrImageSearchService = this["FISS"] as FlickrImageSearchService;

                images = await flickrImageSearchService.QueryDataAsync(_queryText, index, count);

                if (index == 1 && images.Count == 0)
                {
                    IsResultEmpty = true;
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return images;
        }
    }
}