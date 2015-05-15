using System;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_WikiMediaPODService.Models
{
    public class WikiMediaPODCollection : ModelBase
    {
        private readonly uint _maxCount;
        private int _deviationDays;
        private bool _isLoading;
        private GeneratorIncrementalLoadingClass<WikiMediaPOD> _wikiMediaPODs;

        public WikiMediaPODCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (WikiMediaPODs == null)
                WikiMediaPODs = new GeneratorIncrementalLoadingClass<WikiMediaPOD>(maxCount,
                                                                                   async (index) =>
                                                                                   await LoadMoreWikiMediaPODs(index));
        }

        public GeneratorIncrementalLoadingClass<WikiMediaPOD> WikiMediaPODs
        {
            get { return _wikiMediaPODs; }
            private set { SetProperty(ref _wikiMediaPODs, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            _deviationDays = 0;
            WikiMediaPODs = new GeneratorIncrementalLoadingClass<WikiMediaPOD>(_maxCount,
                                                                               async (index) =>
                                                                               await LoadMoreWikiMediaPODs(index));
        }

        private async Task<WikiMediaPOD> LoadMoreWikiMediaPODs(int days)
        {
            IsLoading = true;
            WikiMediaPOD wikiMediaPOD = null;
            try
            {
                var wikiMediaPODQueryService = this["WMPQS"] as WikiMediaPODQueryService;

                wikiMediaPOD =
                    await
                    wikiMediaPODQueryService.QueryDataAsync(
                        DateTime.Now.Subtract(TimeSpan.FromDays(days + _deviationDays)));

                while (null == wikiMediaPOD || !wikiMediaPOD.IsAvailable)
                {
                    if (_deviationDays++ > 10) break;
                    wikiMediaPOD =
                        await
                        wikiMediaPODQueryService.QueryDataAsync(
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
            return wikiMediaPOD;
        }
    }
}