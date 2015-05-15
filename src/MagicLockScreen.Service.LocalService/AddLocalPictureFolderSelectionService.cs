using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_LocalService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using AppSettings = NoteOne_Utility.AppSettings;

namespace MagicLockScreen_Service_LocalService
{
    /// <summary>
    ///     ID is [Guid("9CD9312C-428A-4061-BCF4-A221B63DB213")]
    /// </summary>
    public class AddLocalPictureFolderSelectionService : Service
    {
        public AddLocalPictureFolderSelectionService(ServiceChannel serviceChannel, XmlElement configXml) :
            base(serviceChannel, configXml)
        {
        }

        #region Properties

        public Stack<string> PathStack { get; private set; }
        public uint MaxItemCount { get; private set; }
        public IEnumerable<string> FileExtensions { get; private set; }

        #endregion

        protected override void InitializeService(XmlElement configXml)
        {
            base.InitializeService(configXml);

            if (ID.CompareTo(new Guid("9CD9312C-428A-4061-BCF4-A221B63DB213")) != 0)
                throw new InvalidOperationException("The Service ID is incorrect.");

            try
            {
                if (PathStack == null)
                {
                    PathStack = new Stack<string>();
                    GeneratePathStack();
                }
                FileExtensions = ApplicationHelper.SupportImageExtensions.Values;
                MaxItemCount = configXml.GetAttribute("MaxItemCount").Check().StringToUInt();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Async Query API

        public IAsyncOperation<LocalPictureLibraryStorageItemGroup> GetStorageItemsAsync(string path)
        {
            return AsyncInfo.Run(async (token) =>
                {
                    try
                    {
                        StorageFolder folder = null;
                        if (!string.IsNullOrEmpty(path) && path != "/")
                            folder = await StorageFolder.GetFolderFromPathAsync(path);
                        else
                            folder = KnownFolders.PicturesLibrary;

                        var itemGroupCollection = new LocalPictureLibraryStorageItemGroup();

                        LocalPictureLibraryStorageItemCollection folderCollection =
                            itemGroupCollection.Add(StorageItemTypes.Folder);
                        foreach (IStorageItem item in await folder.GetFoldersAsync())
                        {
                            if (item.IsOfType(StorageItemTypes.Folder))
                                folderCollection.Add(new LocalPictureLibraryStorageFolder(item as StorageFolder, path));
                        }

                        LocalPictureLibraryStorageItemCollection fileCollection =
                            itemGroupCollection.Add(StorageItemTypes.File);
                        foreach (IStorageItem item in await folder.GetStorageFiles(FileExtensions, FolderDepth.Shallow, 0, 100))
                        {
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                var localPictureLibraryStorageFile =
                                    new LocalPictureLibraryStorageFile(item as StorageFile);

                                if (localPictureLibraryStorageFile.IsAvailableImageSize)
                                    fileCollection.Add(localPictureLibraryStorageFile);
                            }
                        }

                        return itemGroupCollection;
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                        return null;
                    }
                });
        }

        #endregion

        #region Path Helper Methods

        private void GeneratePathStack()
        {
            string[] paths =
                LocalAppSettings.Instance[LocalAppSettings.RECENT_LOCAL_SELECTED_PICTURE_FOLDER_PATH].ToString()
                                                                                                     .Split(
                                                                                                         new[] { '|' },
                                                                                                         StringSplitOptions
                                                                                                             .RemoveEmptyEntries);
            foreach (string path in paths)
            {
                PathStack.Push(path);
            }
        }

        private async Task StorePathStackAsync()
        {
            string paths = string.Empty;
            foreach (string path in PathStack)
            {
                paths = path + "|" + paths;
            }
            LocalAppSettings.Instance[LocalAppSettings.RECENT_LOCAL_SELECTED_PICTURE_FOLDER_PATH] = paths;
            await AppSettings.SaveSettings(LocalAppSettings.Instance);
        }

        public string PopPath(int round = 1)
        {
            if (round < 1) round = 1;
            string path = string.Empty;
            while (round > 0)
            {
                if (PathStack.Count > 0)
                {
                    path = PathStack.Pop();
                }
                round--;
            }
            return path;
        }

        public async Task PushPath(string path)
        {
            PathStack.Push(path);
            await StorePathStackAsync();
        }

        #endregion
    }
}