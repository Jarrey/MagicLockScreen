using System;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_LunarPODService.Models
{
    public class LunarPODCollection : ModelBase
    {
        private readonly uint _maxCount;
        private int _deviationDays;
        private bool _isLoading;
        private GeneratorIncrementalLoadingClass<LunarPOD> _lunarPODs;

        public LunarPODCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (LunarPODs == null)
                LunarPODs = new GeneratorIncrementalLoadingClass<LunarPOD>(maxCount,
                                                                           async (index) =>
                                                                           await LoadMoreLunarPODs(index));
        }

        public GeneratorIncrementalLoadingClass<LunarPOD> LunarPODs
        {
            get { return _lunarPODs; }
            private set { SetProperty(ref _lunarPODs, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            _deviationDays = 0;
            LunarPODs = new GeneratorIncrementalLoadingClass<LunarPOD>(_maxCount,
                                                                       async (index) => await LoadMoreLunarPODs(index));
        }

        private async Task<LunarPOD> LoadMoreLunarPODs(int days)
        {
            IsLoading = true;
            LunarPOD lunarPOD = null;
            try
            {
                var lunarPODQueryService = this["LPQS"] as LunarPODQueryService;

                lunarPOD =
                    await
                    lunarPODQueryService.QueryDataAsync(DateTime.Now.Subtract(TimeSpan.FromDays(days + _deviationDays)));

                while (null == lunarPOD || !lunarPOD.IsAvailable)
                {
                    if (_deviationDays++ > 10) break;
                    lunarPOD =
                        await
                        lunarPODQueryService.QueryDataAsync(
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
            return lunarPOD;
        }
    }
}