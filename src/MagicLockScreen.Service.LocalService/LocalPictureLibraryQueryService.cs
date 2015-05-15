using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MagicLockScreen_Service_LocalService.Models;
using MagicLockScreen_Service_LocalService.Resources;
using MagicLockScreen_Service_LocalService.Results;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using NoteOne_Utility.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using ApplicationHelper = MagicLockScreen_Helper.ApplicationHelper;

namespace MagicLockScreen_Service_LocalService
{
    /// <summary>
    ///     ID is [Guid("b1943724-5c34-4267-959c-dc773ce93d00")]
    /// </summary>
    public class LocalPictureLibraryQueryService : Service
    {
        public static readonly AsyncLock FILES_COLLECTION_ASYNC_LOCKER = new AsyncLock();

        public LocalPictureLibraryQueryService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public IEnumerable<string> FileExtensions { get; private set; }
        public Dictionary<string, IList<StorageFile>> FilesCollection { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("b1943724-5c34-4267-959c-dc773ce93d00")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                FileExtensions = ApplicationHelper.SupportImageExtensions.Values;
                if (FilesCollection == null) FilesCollection = new Dictionary<string, IList<StorageFile>>();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Async Query API

        public IAsyncOperation<LocalPictureLibrary> QueryDataAsync(string path)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        var random = new Random(DateTime.Now.Millisecond);
                        LocalPictureLibrary LocalPictureLibrary = null;
                        IList<StorageFile> files = await GetFilesAsync(path);
                        if (files != null && files.Count > 0)
                        {
                            int index = random.Next(0, files.Count);
                            LocalPictureLibrary =
                                (new LocalPictureLibraryQueryResult(files[index])).Result as LocalPictureLibrary;
                        }
                        return LocalPictureLibrary;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        public IAsyncOperation<IList<LocalPictureLibrary>> QueryDataAsync(string path, uint index, uint count)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        IList<LocalPictureLibrary> LocalPictureLibrarys = null;
                        IList<StorageFile> files = null;
                        if (FilesCollection.ContainsKey(path))
                        {
                            files = FilesCollection[path];
                        }

                        if (files != null && files.Count >= index + count)
                        {
                            IEnumerable<StorageFile> results = from int i in Enumerable.Range((int)index, (int)count)
                                                               select files[i];
                            LocalPictureLibrarys =
                                (new LocalPictureLibraryQueryResult(results, QueryResultTypes.Multi)).Results as
                                IList<LocalPictureLibrary>;
                        }
                        return LocalPictureLibrarys;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        #endregion

        #region Helper Methods

        public async Task<IList<StorageFile>> GetFilesAsync(string path)
        {
            try
            {
                using (await FILES_COLLECTION_ASYNC_LOCKER.LockAsync())
                {
                    StorageFolder folder = null;
                    if (!string.IsNullOrEmpty(path) && path != "/")
                        folder = await StorageFolder.GetFolderFromPathAsync(path);
                    else
                        folder = KnownFolders.PicturesLibrary;

                    if (FilesCollection.ContainsKey(path) == false)
                        FilesCollection[path] = new List<StorageFile>();
                    else
                        FilesCollection[path].Clear();

                    foreach (IStorageItem item in await folder.GetStorageFiles(FileExtensions, FolderDepth.Shallow))
                    {
                        if (item.IsOfType(StorageItemTypes.File))
                        {
                            var localPictureLibraryStorageFile =
                                new LocalPictureLibraryStorageFile(item as StorageFile);

                            if (localPictureLibraryStorageFile.IsAvailableImageSize)
                            {
                                FilesCollection[path].Add(item as StorageFile);
                            }
                        }
                    }
                }

                return FilesCollection[path];
            }
            catch (FileNotFoundException ex)
            {
                ex.WriteLog();
                new MessagePopup(string.Format(ResourcesLoader.Loader["FolderNotFoundError"], path)).Show();
                return null;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        #endregion
    }
}