using NoteOne_Utility.Attributes;

namespace MagicLockScreen_Helper
{
    public enum SearchProvider
    {
        [Description("Baidu")] 
        Baidu = 1,

        [Description("Google")]
        Google = 2,

        [Description("Flickr")]
        Flickr = 3,

        [Description("Info.com")]
        InfoDotCom = 4
    }
}