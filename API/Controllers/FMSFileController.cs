using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers {
    [Route ("[controller]/[action]")]
    [ApiController]
    public class FMSFileController : ControllerBase {
        private DBObject.FMS.FMSFile.FileModel DBObjectInstance()
        {
            return new DBObject.FMS.FMSFile.FileModel();
        }

        [HttpGet]
        public ActionResult avatarImageByFileName(string filename)
        {
            var defaultImage = new FileStream("./App_Data/default.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = new FileStream("./App_Data/default.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            var path = "./App_Data/CompanyLogo/"+ filename;
            if (System.IO.File.Exists(path))
            {
                    var fileStream = new FileStream("./App_Data/CompanyLogo/" + filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(fileStream, "image/jpg");
            }

            return File(defaultImage, "image/png");
        }

        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        
        [HttpPost]
        public ActionResult Save () {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader(stream)) {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<DBObject.FMS.FMSFile.FileModel>>(reader.ReadToEnd());

                var id = result.Payload.FileID;
                var fileData = result.Payload.RawFileName;
                if (!string.IsNullOrEmpty(fileData))
                {
                     var file = fileData.Split(';')[1].Split(',')[1];
                     var bytes = Convert.FromBase64String(file);
                     var content = new MemoryStream(bytes);
                     var uniqueName = Guid.NewGuid();
                     var fileName = uniqueName + "." + GetFileExtension(file);
                     result.Payload.RawFileName = fileName;
                     DBObject.FMS.FMSFile.upload(content, "Files", fileName);
                }

                var companyFile =  DBObject.FMS.FMSFile.Default.Save(result.Payload);

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(companyFile)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult FileList () {
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var list = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.FMS.FMSFile.List());
                
                var rc = new {
                    Status = "OK",
                    JsonData = list
                };
                return Content(JsonConvert.SerializeObject(rc, Formatting.Indented), "application/json");
            }
        }

        [HttpPost]
        public ActionResult totalFileSize(){
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var compId = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = compId.Payload;
                var total = DBObject.FMS.FMSFile.getTotalFileSize(id);

                var rc = new {
                    Status = "OK",
                    JsonData = total
                };
                return Content(JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult ListDeleted () {
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var list = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.FMS.FMSFile.ListDeleted());

                var rc = new {
                    Status = "OK",
                    JsonData = list
                };
                return Content(JsonConvert.SerializeObject(rc, Formatting.Indented), "application/json");
            }
        }

        [HttpPost]
        public ActionResult Remove() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var delete = DBObject.FMS.FMSFile.removeFile(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(delete)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

        [HttpPost]
        public ActionResult DeleteFile() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var delete = DBObject.FMS.FMSFile.setDeleted(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(delete)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

            [HttpPost]
            public ActionResult RestoreFile() {
                var stream = HttpContext.Request.Body;

                using (var reader = new System.IO.StreamReader (stream)) {
                    
                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                    var id = value.Payload;
                    var restore = DBObject.FMS.FMSFile.setRestore(id.ToString());

                    // create object without creating class
                    var rc = new {
                        Status = "OK",
                        JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(restore)
                    };
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
                }
            }

        [HttpPost]
        public ActionResult Favorite() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var fav = DBObject.FMS.FMSFile.setFavorite(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(fav)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }

        [HttpGet]
        public ActionResult download(string fileName, string originalName)
        {
            if (System.IO.File.Exists("./App_Data/Files/" + fileName))
            {
                var fileStream = new FileStream("./App_Data/Files/" + fileName, FileMode.Open);
                return File(fileStream, "application/*", originalName);
            }
            return Content(JsonConvert.SerializeObject("File Not Found"));
        }

    }
}