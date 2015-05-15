using MagicLockScreen_Helper.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace MagicLockScreen_Helper.Models
{
    public class SaveImagePosition
    {
        public static List<SaveImagePosition> SaveImagePositions
        {
            get
            {
                return new List<SaveImagePosition>
                {
                    new SaveImagePosition(PickerLocationId.PicturesLibrary, ResourcesLoader.Loader["PicturesLibrary"]),
                    new SaveImagePosition(PickerLocationId.Desktop, ResourcesLoader.Loader["Desktop"]),
                    new SaveImagePosition(PickerLocationId.ComputerFolder, ResourcesLoader.Loader["ComputerFolder"]),
                    new SaveImagePosition(PickerLocationId.DocumentsLibrary, ResourcesLoader.Loader["DocumentsLibrary"]),
                    new SaveImagePosition(PickerLocationId.Downloads, ResourcesLoader.Loader["Downloads"])
                };
            }
        }

        public SaveImagePosition(PickerLocationId id, string name)
        {
            LocationId = (int)id;
            DisplayName = name;
        }

        public int LocationId { get; private set; }
        public string DisplayName { get; private set; }
    }
}
