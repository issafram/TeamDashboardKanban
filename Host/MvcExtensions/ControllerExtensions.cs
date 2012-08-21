using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Host.MvcExtensions
{
    using System.IO;
    using System.Web.Mvc;

    public static class ControllerExtensions
    {
        public static ImageResult Image(this Controller controller, Stream imageStream, string contentType)
        {
            return new ImageResult(imageStream, contentType);
        }

        public static ImageResult Image(this Controller controller, byte[] imageBytes, string contentType)
        {
            return new ImageResult(new MemoryStream(imageBytes), contentType);
        }
    }
}