using Chameleon.UpdateWallpage.Service.Services;
using log4net;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Chameleon.UpdateWallpaper.Service.Controllers
{
    [ChameExceptionFilter]
    [RoutePrefix("api")]
    public class UpdateWallpageController : ApiController
    {
        private readonly ILog logger = LogManager.GetLogger("Controller");

        [HttpGet, Route("IsValid")]
        public async Task<HttpResponseMessage> IsValid([FromUri]string key)
        {
            if (key != MD5Encryptor.GetMD5("chameleon"))
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
            logger.Info("Request from chameleon, to check service status");

            // create and return http response
            var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent("Bomb") };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }

        [HttpPost, Route("update")]
        public async Task<HttpResponseMessage> UpdateWallpaper([FromUri]string key)
        {
            if (key != MD5Encryptor.GetMD5("chameleon"))
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var wallpaperFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ChameDesk");
                if (!Directory.Exists(wallpaperFolder))
                {
                    Directory.CreateDirectory(wallpaperFolder);
                }

                var wallpaperFile = Path.Combine(wallpaperFolder, "wallpaper.chamedesk");
                byte[] bytes = Convert.FromBase64String(await Request.Content.ReadAsStringAsync());
                logger.InfoFormat("Request from chameleon to update wallpaper, byte length: {0}", bytes.Length);

                File.WriteAllBytes(wallpaperFile, bytes);
                WallpageService.SetWallpaper(wallpaperFile, WallpaperStyle.Fill);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to update wallpaper.", ex);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
            }

            // create and return http response
            var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent("Bomb") };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
    }
}
