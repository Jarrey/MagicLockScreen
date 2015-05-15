using System;
using System.Collections.Generic;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_GooglePicasaService.Models;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Contract;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Globalization;

namespace MagicLockScreen_Service_GooglePicasaService.UI.ViewModels
{
    public class GooglePicasaSingleViewPageViewModel : ViewModelBase
    {
        public GooglePicasaSingleViewPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            this["GooglePicasaSelectedItem"] = null;

            #region Commands

            #region SetAsLockScreenCommand

            this["SetAsLockScreenCommand"] = ApplicationHelper.SetAsLockScreenCommand;

            #endregion

            #region SetAppBackgroundCommand

            this["SetAppBackgroundCommand"] = ApplicationHelper.SetAppBackgroundCommand;

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["GooglePicasaSelectedItem"] as GooglePicasa).Title,
                                                        (this["GooglePicasaSelectedItem"] as GooglePicasa).Summary);
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion

            #region ZoomInCommand

            this["ZoomInCommand"] = new RelayCommand<object>(p =>
                {
                    var imageScrollViewer = p as ScrollViewer;
                    if (imageScrollViewer != null)
                    {
                        float zoomInterval =
                            AppSettings.Instance[AppSettings.ZOOM_FACTORY_INTERVAL].ToString(CultureInfo.InvariantCulture).StringToFloat();
                        imageScrollViewer.ChangeView(null, null, imageScrollViewer.ZoomFactor + zoomInterval);
                    }
                });

            #endregion

            #region ZoomOutCommand

            this["ZoomOutCommand"] = new RelayCommand<object>(p =>
                {
                    var imageScrollViewer = p as ScrollViewer;
                    if (imageScrollViewer != null)
                    {
                        float zoomInterval =
                            AppSettings.Instance[AppSettings.ZOOM_FACTORY_INTERVAL].ToString(CultureInfo.InvariantCulture).StringToFloat();
                        imageScrollViewer.ChangeView(null, null, imageScrollViewer.ZoomFactor - zoomInterval);
                    }
                });

            #endregion

            #endregion
        }

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }
    }
}