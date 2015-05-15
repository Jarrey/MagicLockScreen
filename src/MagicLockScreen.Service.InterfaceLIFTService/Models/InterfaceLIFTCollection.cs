using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_InterfaceLIFTService.Models
{
    public class InterfaceLIFTCollection : ModelBase
    {
        private readonly uint _maxCount;
        private RangeIncrementalLoadingClass<InterfaceLIFT> _interfaceLIFTs;

        private bool _isLoading;

        public InterfaceLIFTCollection(uint maxCount, IService[] services)
            : base(services)
        {
            _maxCount = maxCount;
            if (InterfaceLIFTs == null)
                InterfaceLIFTs = new RangeIncrementalLoadingClass<InterfaceLIFT>(maxCount,
                                                                                 async (index, count) =>
                                                                                 await
                                                                                 LoadMoreInterfaceLIFTs(index, count));
        }

        public RangeIncrementalLoadingClass<InterfaceLIFT> InterfaceLIFTs
        {
            get { return _interfaceLIFTs; }
            private set { SetProperty(ref _interfaceLIFTs, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public void RefreshCollection()
        {
            InterfaceLIFTs = new RangeIncrementalLoadingClass<InterfaceLIFT>(_maxCount,
                                                                             async (index, count) =>
                                                                             await LoadMoreInterfaceLIFTs(index, count));
        }

        private async Task<IList<InterfaceLIFT>> LoadMoreInterfaceLIFTs(uint index, uint count)
        {
            IsLoading = true;
            IList<InterfaceLIFT> interfaceLIFTs = null;
            try
            {
                var interfaceLIFTQueryService = this["ILQS"] as InterfaceLIFTQueryService;
                interfaceLIFTs = await interfaceLIFTQueryService.QueryDataAsync(index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return interfaceLIFTs;
        }
    }
}