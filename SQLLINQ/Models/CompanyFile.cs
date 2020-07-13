using System;
using System.Collections.Generic;

namespace SQLLINQ.Models
{
    public partial class CompanyFile
    {
        public long FileId { get; set; }
        public long CompanyId { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }
        public string RawFileName { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Favorite { get; set; }
        public bool IsDeleted { get; set; }
    }
}
