using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;

namespace MagicLockScreen_Service_LocalService.Models
{
    public class LocalPictureLibraryCollection : ModelBase
    {
        private bool _isLoading;
        private RangeIncrementalLoadingClass<LocalPictureLibrary> _localPictureLibrarys;
        private string _path;

        public LocalPictureLibraryCollection(uint maxCount, IService[] services, string path)
            : base(services)
        {
            Path = path;
            if (LocalPictureLibrarys == null)
                LocalPictureLibrarys = new RangeIncrementalLoadingClass<LocalPictureLibrary>(maxCount,
                                                                                             async (index, count) =>
                                                                                             await
                                                                                             LoadMoreLocalPictureLibrarys
                                                                                                 (index, count));
        }

        public RangeIncrementalLoadingClass<LocalPictureLibrary> LocalPictureLibrarys
        {
            get { return _localPictureLibrarys; }
            private set { SetProperty(ref _localPictureLibrarys, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public string Path
        {
            get { return _path; }
            private set { SetProperty(ref _path, value); }
        }

        public void RefreshCollection(uint maxCount)
        {
            LocalPictureLibrarys = new RangeIncrementalLoadingClass<LocalPictureLibrary>(maxCount,
                                                                                         async (index, count) =>
                                                                                         await
                                                                                         LoadMoreLocalPictureLibrarys(
                                                                                             index, count));
        }

        private async Task<IList<LocalPictureLibrary>> LoadMoreLocalPictureLibrarys(uint index, uint count)
        {
            IsLoading = true;
            IList<LocalPictureLibrary> localPictureLibrarys = null;
            try
            {
                var localPictureLibraryQueryService = this["LPLQS"] as LocalPictureLibraryQueryService;
                localPictureLibrarys = await localPictureLibraryQueryService.QueryDataAsync(Path, index, count);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            finally
            {
                IsLoading = false;
            }
            return localPictureLibrarys;
        }
    }
}