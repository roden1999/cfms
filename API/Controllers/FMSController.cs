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
    public class FMSController : ControllerBase {

        private DBObject.FMS.Company.CompanyModel DBObjectInstance()
        {
            return new DBObject.FMS.Company.CompanyModel();
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

            //using (StreamReader reader = new StreamReader (this.HttpContext.Request.Body, Encoding.UTF8)) {
            using (var reader = new System.IO.StreamReader(stream)) {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<DBObject.FMS.Company.CompanyModel>>(reader.ReadToEnd());

                var id = result.Payload.CompanyID;
                var imageData = result.Payload.CompanyLogo;
                if (!string.IsNullOrEmpty(imageData))
                {
                     var file = imageData.Split(';')[1].Split(',')[1];
                     var bytes = Convert.FromBase64String(file);
                     var content = new MemoryStream(bytes);
                     var uniqueName = Guid.NewGuid();
                     var fileName = uniqueName + "." + GetFileExtension(file);
                     result.Payload.CompanyLogo = fileName;
                     DBObject.FMS.Company.upload(content, "CompanyLogo", fileName);
                }

                var company =  DBObject.FMS.Company.Default.Save(result.Payload);

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(company)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc), "application/json");
            }
        }

        [HttpPost]
        public ActionResult CompanyList () {
            using (StreamReader reader = new StreamReader(this.HttpContext.Request.Body, Encoding.UTF8)) {
                var list = Newtonsoft.Json.JsonConvert.SerializeObject(DBObject.FMS.Company.List());

                var rc = new {
                    Status = "OK",
                    JsonData = list
                };
                return Content(JsonConvert.SerializeObject(rc, Formatting.Indented), "application/json");
            }
        }

        [HttpPost]
        public ActionResult DeleteCompany() {
            var stream = HttpContext.Request.Body;

            using (var reader = new System.IO.StreamReader (stream)) {
                
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject<DBObject.Common.RequestClass<long>>(reader.ReadToEnd());
                var id = value.Payload;
                var delete = DBObject.FMS.Company.setDeleted(id.ToString());

                // create object without creating class
                var rc = new {
                    Status = "OK",
                    JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(delete)
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rc));
            }
        }
    }
}