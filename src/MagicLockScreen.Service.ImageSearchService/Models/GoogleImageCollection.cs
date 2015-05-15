using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    public class GoogleImageCollection : ModelBase
    {
        private readonly uint _maxCount;
        private readonly string _queryText = string.Empty;
        private RangeIncrementalLoadingClass<SearchImage> _images;

        private bool _isLoading;

        private bool _isResultEmpty;

        public GoogleImageCollection(uint maxCount, IService[] services, string queryText)
            : base(services)
        {
            _queryText = queryText;
            _maxCount = maxCount;
            if (Images == null)
                Images = new RangeIncrementalLoadingClass<SearchImage>(maxCount,
                                                                       async (index, count) =>
                                                                       await LoadMoreGoogleImages(index, count));
        }

        public RangeIncrementalLoadingClass<SearchImage> Images
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
            Images = new RangeIncrementalLoadingClass<SearchImage>(_maxCount,
                                                                   async (index, count) =>
                                                                   await LoadMoreGoogleImages(index, count));
        }

        private async Task<IList<SearchImage>> LoadMoreGoogleImages(uint index, uint count)
        {
            IsLoading = true;
            IList<SearchImage> images = null;

            try
            {
                var googleImageSearchService = this["GISS"] as GoogleImageSearchService;

                do
                {
                    uint c = Math.Min(count, 8);
                    IList<SearchImage> imgs = await googleImageSearchService.QueryDataAsync(_queryText, index, c);
                    if (images == null) images = new List<SearchImage>();
                    if (imgs != null)
                        foreach (SearchImage img in imgs)
                            images.Add(img);
                    index += c;
                    count -= c;
                } while (count > 0);

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
            return images.ToArray();
        }
    }
}