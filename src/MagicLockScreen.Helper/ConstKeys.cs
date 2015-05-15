using NoteOne_Utility.Helpers;

namespace MagicLockScreen_Helper
{
    public class ConstKeys : NoteOne_Utility.ConstKeys
    {
        public const string IMAGE_SEARCH_PAGE_CACHE_KEY = "ImageSearchPageCacheKey";
        public const string LOCAL_PICTURE_LIBRARY_VIEW_PAGE_KEY = "LocalPictureLibraryViewPageKey";
        public const string ADD_SERVICE_CHANNEL_MODEL_KEY = "$AddServiceChannelModelKey$";
        public const string LOCAL_FOLDER_PATH_KEY = "LocalFolderPathKey";

        public static readonly AsyncLock LOCAL_PICTURE_LIBRARY_READ_ASYNC_LOCKER = new AsyncLock();
    }
}