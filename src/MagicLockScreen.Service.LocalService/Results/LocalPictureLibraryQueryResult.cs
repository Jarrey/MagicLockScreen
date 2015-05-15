using System;
using System.Collections.Generic;
using MagicLockScreen_Service_LocalService.Models;
using MagicLockScreen_Service_LocalService.Resources;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MagicLockScreen_Service_LocalService.Results
{
    public class LocalPictureLibraryQueryResult : QueryResult
    {
        public LocalPictureLibraryQueryResult(object result,
                                              QueryResultTypes type = QueryResultTypes.Single)
            : base(result, type)
        {
            ResponseType = ResponseTypes.Object;

            if (type == QueryResultTypes.Single)
                Result = new LocalPictureLibrary();
            else if (type == QueryResultTypes.Multi)
                Results = default(LocalPictureLibrary[]);

            ParseResponse();
        }

        public LocalPictureLibraryQueryResult(ModelBase result)
            : base(result)
        {
            ResponseType = ResponseTypes.Object;
        }

        public LocalPictureLibraryQueryResult(IList<ModelBase> results)
            : base(results)
        {
            ResponseType = ResponseTypes.Object;
        }

        protected override void ParseResponse()
        {
            try
            {
                base.ParseResponse();

                switch (QueryResultType)
                {
                    case QueryResultTypes.Single:
                        if (ResponseContent is StorageFile)
                        {
                            var file = ResponseContent as StorageFile;
                            GenerateLocalPictureLibrary(file, Result as LocalPictureLibrary);
                        }
                        break;

                    case QueryResultTypes.Multi:
                        if (ResponseContent is IEnumerable<StorageFile>)
                        {
                            var localPictureLibraries = new List<LocalPictureLibrary>();
                            var files = ResponseContent as IEnumerable<StorageFile>;
                            foreach (StorageFile file in files)
                            {
                                var img = new LocalPictureLibrary();
                                GenerateLocalPictureLibrary(file, img);
                                localPictureLibraries.Add(img);
                            }
                            Results = localPictureLibraries.ToArray();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private async void GenerateLocalPictureLibrary(StorageFile file, LocalPictureLibrary localPictureLibrary)
        {
            try
            {
                localPictureLibrary.Title = file.Name;
                localPictureLibrary.DateCreated = file.DateCreated.DateTime;
                localPictureLibrary.LocalImagePath = file.Path;
                localPictureLibrary.Explanation = ResourcesLoader.Loader["StoreLocaton"] + file.Path;
                ImageProperties imageProperties = await file.Properties.GetImagePropertiesAsync();
                if (imageProperties != null)
                {
                    if (!string.IsNullOrEmpty(imageProperties.Title))
                    {
                        localPictureLibrary.Title = imageProperties.Title;
                        localPictureLibrary.Explanation += "\n" + ResourcesLoader.Loader["Title"] +
                                                           imageProperties.Title;
                    }
                    if (!string.IsNullOrEmpty(imageProperties.CameraManufacturer))
                        localPictureLibrary.Explanation += "\n" + ResourcesLoader.Loader["CameraManufacturer"] +
                                                           imageProperties.CameraManufacturer;
                    if (!string.IsNullOrEmpty(imageProperties.CameraModel))
                        localPictureLibrary.Explanation += "\n" + ResourcesLoader.Loader["CameraModel"] +
                                                           imageProperties.CameraModel;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #region Parameters

        #endregion
    }
}