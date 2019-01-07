using ServiceDeskApplication.Models;
using System;
using System.Web;

namespace ServiceDeskApplication.Services
{
    public static class FileHelper
    {
        public static AttachedFile GetAttachedFile(HttpPostedFileBase file)
        {
            AttachedFile attachedFile = new AttachedFile
            {
                Id = Guid.NewGuid(),
                ContentType = file.ContentType,
            };

            byte[] data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);
            attachedFile.Data = data;

            return attachedFile;
        }
    }
}