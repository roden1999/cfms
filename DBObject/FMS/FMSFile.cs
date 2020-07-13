using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
namespace DBObject.FMS
{

    public class FMSFile
    {
        public static FMSFile Default { get; set; } = new FMSFile();

        public partial class FileModel
        {
            public long FileID { get; set; } = -1;
            public long CompanyID { get; set; } = -1;
            public string FileName { get; set; } = "";
            public int FileType { get; set; } = -1;
            public string RawFileName { get; set; } = "";
            public string Description { get; set; } = "";
            public int Size { get; set; } = -1;
            public DateTime UploadDate { get; set; } = DateTime.Now;
            public DateTime ModifiedDate { get; set; }
            public int Favorite { get; set; }
            public bool IsDeleted { get; set; } = false;
        }

        public static double getTotalFileSize(long companyID)
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var sum = dc.CompanyFile.Where(x => x.CompanyId == companyID & x.IsDeleted == false).Sum(x => x.Size);  
            return sum;
        }

        public static new List<SQLLINQ.Models.CompanyFile> List()
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var dbFile = dc.CompanyFile.Where(x => x.IsDeleted == false).ToList();
            var fileList = new List<SQLLINQ.Models.CompanyFile>();
            //var compId = dc.Company.Where(x => x.CompanyId);

            foreach (var compFile in dbFile)
            {
                var model = new SQLLINQ.Models.CompanyFile();
                model.FileId = compFile.FileId;
                model.CompanyId = compFile.CompanyId;
                model.FileName = compFile.FileName;
                model.FileType = compFile.FileType;
                model.RawFileName = compFile.RawFileName;
                model.Description = compFile.Description;
                model.Size = compFile.Size;
                model.UploadDate = compFile.UploadDate;
                model.ModifiedDate = compFile.ModifiedDate;
                model.Favorite = compFile.Favorite;
                fileList.Add(model);
            }
            return fileList.OrderBy(x => x.FileName).ToList();
        }

        public static new List<SQLLINQ.Models.CompanyFile> ListDeleted()
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var dbFile = dc.CompanyFile.Where(x => x.IsDeleted == true).ToList();
            var fileList = new List<SQLLINQ.Models.CompanyFile>();

            foreach (var compFile in dbFile)
            {
                var model = new SQLLINQ.Models.CompanyFile();
                model.FileId = compFile.FileId;
                model.CompanyId = compFile.CompanyId;
                model.FileName = compFile.FileName;
                model.FileType = compFile.FileType;
                model.RawFileName = compFile.RawFileName;
                model.Description = compFile.Description;
                model.Size = compFile.Size;
                model.UploadDate = compFile.UploadDate;
                model.ModifiedDate = compFile.ModifiedDate;
                model.Favorite = compFile.Favorite;
                fileList.Add(model);
            }
            return fileList.OrderBy(x => x.FileName).ToList();
        }

        public FileModel Save(FileModel file)
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var dbFile = new SQLLINQ.Models.CompanyFile();

            if (file.FileID == -1)
            {
                dbFile.CompanyId = file.CompanyID;
                dbFile.FileName = file.FileName;
                dbFile.FileType = file.FileType;
                dbFile.RawFileName = file.RawFileName;
                dbFile.Description = file.Description;
                dbFile.Size = file.Size;
                dbFile.UploadDate = file.UploadDate;
                dbFile.ModifiedDate = file.ModifiedDate;
                dbFile.Favorite = file.Favorite;
                dc.CompanyFile.Add(dbFile);
            }
            else
            {
                dbFile = dc.CompanyFile.Where(x => x.FileId == file.FileID).FirstOrDefault();
                if (dbFile != null)
                {
                    dbFile.FileId = file.FileID;
                    dbFile.CompanyId = file.CompanyID;
                    dbFile.FileName = file.FileName;
                    dbFile.FileType = file.FileType;
                    dbFile.RawFileName = file.RawFileName;
                    dbFile.Description = file.Description;
                    dbFile.Size = file.Size;
                    dbFile.UploadDate = file.UploadDate;
                    dbFile.ModifiedDate = file.ModifiedDate;
                    dbFile.Favorite = file.Favorite;
                }
            }
            dc.SaveChanges();

            return file;
        }

        public static Boolean upload(Stream obj, string folder, string fileWithExtension)
        {
            bool result = false;
            //string path = Microsoft.AspNetCore.Http.HttpContext.Server.MapPath("~/App_Data/" + folder);
            string path = "./App_Data/" + folder;
            if (!File.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            using (var fileStream = new FileStream(path + "/" + fileWithExtension, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                obj.CopyTo(fileStream);
                fileStream.Dispose();
            }
            return result;
        }

        public static bool removeFile(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.FMSContext();
                var dbFile = new SQLLINQ.Models.CompanyFile();

                dbFile = dc.CompanyFile.Where(x => x.FileId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbFile != null)
                    dc.Remove(dbFile);
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool setDeleted(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.FMSContext();
                var dbFile = new SQLLINQ.Models.CompanyFile();

                dbFile = dc.CompanyFile.Where(x => x.FileId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbFile != null)
                    dbFile.IsDeleted = true;
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(FileModel id)
        {
            throw new NotImplementedException();
        }

        public static bool setRestore(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.FMSContext();
                var dbFile = new SQLLINQ.Models.CompanyFile();

                dbFile = dc.CompanyFile.Where(x => x.FileId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbFile != null)
                    dbFile.IsDeleted = false;
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Restore(FileModel id)
        {
            throw new NotImplementedException();
        }

        public static bool setFavorite(string id)
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var dbFile = new SQLLINQ.Models.CompanyFile();
            var fav = 0;

            dbFile = dc.CompanyFile.Where(x => x.FileId == Convert.ToInt64(id)).SingleOrDefault();
            if (dbFile != null)
                if (dbFile.Favorite != 1)
                {
                    fav = 1;
                }
            if (dbFile.Favorite != 0)
            {
                fav = 0;
            }
            dbFile.Favorite = fav;
            dc.SaveChanges();

            return true;
        }

        public bool Favorite(FileModel id)
        {
            throw new NotImplementedException();
        }
    }
}