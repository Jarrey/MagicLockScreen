using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteOne.Core.Common;

namespace MagicLockScreen.Service.AstronomyPictureService.ApiParameters
{
    public class AstronomyPictureServiceApiParameter : ApiParameter
    {
        public AstronomyPictureServiceApiParameter()
        {
            this.Parameters.Add(0, () =>
                this.Date);
        }
        public DateTime Date { get; set; }
    }
}
