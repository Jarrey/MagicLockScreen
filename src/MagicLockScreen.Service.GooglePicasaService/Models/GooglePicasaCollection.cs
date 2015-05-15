using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_GooglePicasaService.Models
{
    public class GooglePicasaCollection : ModelBase
    {
        private RangeIncrementalLoadingClass<GooglePicasa> _googlePicasas;

        private bool _isLoading;

        public GooglePicasaCollection(uint maxCount, string keyword, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            _keyword = keyword;
            if (GooglePicasas == null)
                GooglePicasas = new RangeIncrementalLoadingClass<GooglePicasa>(maxCount,
                                                                               async (index, count) =>
                                                                               await LoadMoreGooglePicasas(index, count));
        }

        public RangeIncrementalLoadingClass<GooglePicasa> GooglePicasas
        {
            get { return _googlePicasas; }
            private set { SetProperty(ref _googlePicasas, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            GooglePicasas = new RangeIncrementalLoadingClass<GooglePicasa>(_maxCount,
                                                                           async (index, count) =>
                                                                           await LoadMoreGooglePicasas(index, count));
        }

        private async Task<IList<GooglePicasa>> LoadMoreGooglePicasas(uint index, uint count)
        {
            IsLoading = true;
            IList<GooglePicasa> googlePicasas = null;
            try
            {
                var googlePicasaQueryService = this["GPQS"] as GooglePicasaQueryService;
                googlePicasas = await googlePicasaQueryService.QueryDataAsync(index, count, _keyword);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return googlePicasas;
        }

        #region Parameters

        private readonly string _keyword = "";
        private readonly uint _maxCount;

        #endregion
    }
}