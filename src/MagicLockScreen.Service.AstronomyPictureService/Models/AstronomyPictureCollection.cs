using System;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_AstronomyPictureService.Models
{
    public class AstronomyPictureCollection : ModelBase
    {
        private readonly uint _maxCount;
        private GeneratorIncrementalLoadingClass<AstronomyPicture> _astronomyPictures;
        private int _deviationDays;

        private bool _isLoading;

        public AstronomyPictureCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (AstronomyPictures == null)
                AstronomyPictures = new GeneratorIncrementalLoadingClass<AstronomyPicture>(maxCount,
                                                                                           async (index) =>
                                                                                           await
                                                                                           LoadMoreAstronomyPictures(
                                                                                               index));
        }

        public GeneratorIncrementalLoadingClass<AstronomyPicture> AstronomyPictures
        {
            get { return _astronomyPictures; }
            private set { SetProperty(ref _astronomyPictures, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            _deviationDays = 0;
            AstronomyPictures = new GeneratorIncrementalLoadingClass<AstronomyPicture>(_maxCount,
                                                                                       async (index) =>
                                                                                       await
                                                                                       LoadMoreAstronomyPictures(index));
        }

        private async Task<AstronomyPicture> LoadMoreAstronomyPictures(int days)
        {
            IsLoading = true;
            AstronomyPicture astronomyPicture = null;
            try
            {
                var astronomyPictureQueryService = this["APQS"] as AstronomyPictureQueryService;

                astronomyPicture =
                    await
                    astronomyPictureQueryService.QueryDataAsync(
                        DateTime.Now.Subtract(TimeSpan.FromDays(days + _deviationDays)));

                while (null == astronomyPicture || !astronomyPicture.IsAvailable)
                {
                    if (_deviationDays++ > 10) break;
                    astronomyPicture =
                        await
                        astronomyPictureQueryService.QueryDataAsync(
                            DateTime.Now.Subtract(TimeSpan.FromDays(days + _deviationDays)));
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
            return astronomyPicture;
        }
    }
}