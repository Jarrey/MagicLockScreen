using System;
using System.Collections.Generic;
using MagicLockScreen_Service_LocalService.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MagicLockScreen_Service_LocalService.UI.ViewModels
{
    public class LocalPictureFolderSelectionPageViewModel : ViewModelBase
    {
        private readonly AddLocalPictureFolderSelectionService addLocalPictureFolderSelectionService;
        private readonly AddLocalPictureServiceChannel addLocalPictureServiceChannel;
        private readonly LocalPictureLibraryServiceChannel localPictureLibraryServiceChannel;
        private LocalPictureLibraryQueryService localPictureLibraryQueryService;

        public LocalPictureFolderSelectionPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            addLocalPictureServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["ALPSC"] as AddLocalPictureServiceChannel;
            addLocalPictureFolderSelectionService =
                addLocalPictureServiceChannel["ALPFSS"] as AddLocalPictureFolderSelectionService;

            localPictureLibraryServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["LPLSC"] as LocalPictureLibraryServiceChannel;
            localPictureLibraryQueryService =
                localPictureLibraryServiceChannel["LPLQS"] as LocalPictureLibraryQueryService;

            this["CanGoUp"] = false;
            this["HasImages"] = false;
            this["CurrentPath"] = addLocalPictureFolderSelectionService.PopPath();
            GetStorageItems(this["CurrentPath"].ToString());
            UpdateView(this["CurrentPath"].ToString());

            #region Commands

            this["ItemClickCommand"] = new RelayCommand<object[]>(p =>
                {
                    if (p != null && p.Length == 2 && p[1] is ItemClickEventArgs)
                    {
                        var localStorageItem =
                            (p[1] as ItemClickEventArgs).ClickedItem as LocalPictureLibraryStorageItem;
                        if (localStorageItem != null &&
                            localStorageItem.Type == StorageItemTypes.Folder)
                        {
                            var localStorageFolder = localStorageItem as LocalPictureLibraryStorageFolder;
                            this["CurrentPath"] = localStorageFolder.Path;
                            GetStorageItems(this["CurrentPath"].ToString());
                            UpdateView(this["CurrentPath"].ToString());
                        }
                    }
                });

            this["GoUpCommand"] = new RelayCommand(() =>
                {
                    this["CurrentPath"] = addLocalPictureFolderSelectionService.PopPath(2);
                    GetStorageItems(this["CurrentPath"].ToString());
                    UpdateView(this["CurrentPath"].ToString());
                });

            this["SelectFolderCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        localPictureLibraryServiceChannel.AddModel(localPictureLibraryServiceChannel,
                                                                   this["CurrentPath"].ToString());
                        localPictureLibraryServiceChannel.SaveModels();
                        (View as LayoutAwarePage).GoBack(View, new RoutedEventArgs());
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion
        }

        private async void GetStorageItems(string path)
        {
            if (ContainsKey("StorageItems") && this["StorageItems"] is LocalPictureLibraryStorageItemGroup)
                (this["StorageItems"] as LocalPictureLibraryStorageItemGroup).Clear();
            LocalPictureLibraryStorageItemGroup itemGroup =
                await addLocalPictureFolderSelectionService.GetStorageItemsAsync(path);
            if (itemGroup != null && itemGroup[StorageItemTypes.File].Count > 0)
                this["HasImages"] = true;
            else
                this["HasImages"] = false;
            this["StorageItems"] = itemGroup;
            await addLocalPictureFolderSelectionService.PushPath(path);
        }

        private void UpdateView(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
            {
                this["CanGoUp"] = false;
            }
            else
            {
                this["CanGoUp"] = true;
            }
        }

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }
    }
}