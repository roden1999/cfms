using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
namespace DBObject.FMS {

    public class Company {
        public static Company Default { get; set; } = new Company();

        public partial class CompanyModel
        {
            public long CompanyID { get; set; } = -1;
            public string CompanyName { get; set; } = "";
            public string CompanyLogo { get; set; } = "";
            public string Address { get; set; } = "";
            public string ContactNumber { get; set; } = "";
            public string Email { get; set; } = "";
            public bool IsDeleted { get; set; } = false;
        }

            public CompanyModel Save (CompanyModel company) 
            {
                var dc = new SQLLINQ.Models.FMSContext();
                var dbCompany = new SQLLINQ.Models.Company();

                if (company.CompanyID == -1)
                {
                    dbCompany.CompanyName = company.CompanyName;
                    dbCompany.CompanyLogo = company.CompanyLogo;
                    dbCompany.Address = company.Address;
                    dbCompany.ContactNumber = company.ContactNumber;
                    dbCompany.Email = company.Email;
                    dc.Company.Add(dbCompany);
                }
                else
                { 
                    dbCompany = dc.Company.Where(x => x.CompanyId == company.CompanyID).SingleOrDefault();
                    if (dbCompany != null)
                    {
                        dbCompany.CompanyId = company.CompanyID;  
                        dbCompany.CompanyName = company.CompanyName;
                        dbCompany.CompanyLogo = company.CompanyLogo;
                        dbCompany.Address = company.Address;
                        dbCompany.ContactNumber = company.ContactNumber;
                        dbCompany.Email = company.Email;
                    }
                }
                dc.SaveChanges();    

                return company;
            }

        public static new List<SQLLINQ.Models.Company> List() 
        {
            var dc = new SQLLINQ.Models.FMSContext();
            var dbCompany = dc.Company.Where(x => x.IsDeleted == false).ToList();
            var companyList = new List<SQLLINQ.Models.Company>();

            foreach (var comp in dbCompany)
            {
                var model = new SQLLINQ.Models.Company();
                model.CompanyId = comp.CompanyId;
                model.CompanyName = comp.CompanyName;
                model.CompanyLogo = comp.CompanyLogo;
                model.Address = comp.Address;
                model.ContactNumber = comp.ContactNumber;
                model.Email = comp.Email;
                companyList.Add(model);
            }
            return companyList.OrderBy(x => x.CompanyName).ToList();
        }

        public static bool setDeleted(string id)
        {
            try
            {
                var dc = new SQLLINQ.Models.FMSContext();
                var dbCompany = new SQLLINQ.Models.Company();

                dbCompany = dc.Company.Where(x => x.CompanyId == Convert.ToInt64(id)).SingleOrDefault();
                if (dbCompany != null)
                    dbCompany.IsDeleted = true;
                dc.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(CompanyModel id)
        {
            throw new NotImplementedException();
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
    }
}