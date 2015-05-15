using System;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_OceanPODService.Models
{
    public class OceanPODCollection : ModelBase
    {
        private readonly uint _maxCount;
        private int _deviationDays;
        private bool _isLoading;
        private GeneratorIncrementalLoadingClass<OceanPOD> _oceanPODs;

        public OceanPODCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (OceanPODs == null)
                OceanPODs = new GeneratorIncrementalLoadingClass<OceanPOD>(maxCount,
                                                                           async (index) =>
                                                                           await LoadMoreOceanPODs(index));
        }

        public GeneratorIncrementalLoadingClass<OceanPOD> OceanPODs
        {
            get { return _oceanPODs; }
            private set { SetProperty(ref _oceanPODs, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            _deviationDays = 0;
            OceanPODs = new GeneratorIncrementalLoadingClass<OceanPOD>(_maxCount,
                                                                       async (index) => await LoadMoreOceanPODs(index));
        }

        private async Task<OceanPOD> LoadMoreOceanPODs(int days)
        {
            IsLoading = true;
            OceanPOD oceanPOD = null;
            try
            {
                var oceanPODQueryService = this["OPQS"] as OceanPODQueryService;

                oceanPOD =
                    await
                    oceanPODQueryService.QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(days + _deviationDays)));

                while (null == oceanPOD || !oceanPOD.IsAvailable)
                {
                    if (_deviationDays++ > 10) break;
                    oceanPOD =
                        await
                        oceanPODQueryService.QueryDataAsync(
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
            return oceanPOD;
        }
    }
}