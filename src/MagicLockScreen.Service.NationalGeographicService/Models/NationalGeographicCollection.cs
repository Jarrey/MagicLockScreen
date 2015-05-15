using System;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_NationalGeographicService.Models
{
    public class NationalGeographicCollection : ModelBase
    {
        private readonly uint _maxCount;
        private bool _isLoading;
        private GeneratorIncrementalLoadingClass<NationalGeographic> _nationalGeographics;
        private string _previousPhotoLink = string.Empty;

        public NationalGeographicCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (NationalGeographics == null)
                NationalGeographics = new GeneratorIncrementalLoadingClass<NationalGeographic>(maxCount,
                                                                                               async (index) =>
                                                                                               await
                                                                                               LoadMoreNationalGeographics
                                                                                                   (index));
        }

        public GeneratorIncrementalLoadingClass<NationalGeographic> NationalGeographics
        {
            get { return _nationalGeographics; }
            private set { SetProperty(ref _nationalGeographics, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            NationalGeographics = new GeneratorIncrementalLoadingClass<NationalGeographic>(_maxCount,
                                                                                           async (index) =>
                                                                                           await
                                                                                           LoadMoreNationalGeographics(
                                                                                               index));
        }

        private async Task<NationalGeographic> LoadMoreNationalGeographics(int days)
        {
            IsLoading = true;
            NationalGeographic nationalGeographic = null;
            try
            {
                var nationalGeographicQueryService = this["NGQS"] as NationalGeographicQueryService;

                if (days == 0 || string.IsNullOrEmpty(_previousPhotoLink))
                    nationalGeographic = await nationalGeographicQueryService.QueryDataAsync();
                else nationalGeographic = await nationalGeographicQueryService.QueryDataAsync(_previousPhotoLink);

                _previousPhotoLink = nationalGeographic.PreviousPhotoLink;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return nationalGeographic;
        }
    }
}