﻿using Windows.ApplicationModel.Resources;

namespace MagicLockScreen_UI.Resources
{
    public class ResourcesLoader
    {
        private static ResourcesLoader _loader;

        private readonly ResourceLoader resourceLoader;

        private ResourcesLoader()
        {
            resourceLoader = ResourceLoader.GetForViewIndependentUse(@"Resources");
        }

        public static ResourcesLoader Loader
        {
            get
            {
                if (_loader == null)
                    _loader = new ResourcesLoader();
                return _loader;
            }
        }

        public string this[string name]
        {
            get
            {
                string result = resourceLoader.GetString(name);
                if (!string.IsNullOrEmpty(result))
                    return result;
                else
                    return name;
            }
        }
    }
}