
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChandrimERP.Models
{
    public class Student
    {
        [Key]
        public int studentID { get; set; }
        public string studentName { get; set; }
        public string studentAddress { get; set; }
        public string studentNote { get; set; }

    }

    public class FileUpload1
    {
        public string ErrorMessage { get; set; }
        public decimal filesize { get; set; }
        public string UploadUserFile(HttpPostedFileBase file)
        {
            try
            {
                var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx", };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    ErrorMessage = "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT File";
                    return ErrorMessage;
                }
                else if (file.ContentLength > (filesize * 1024))
                {
                    ErrorMessage = "File size Should Be UpTo " + filesize + "KB";
                    return ErrorMessage;
                }
                else
                {
                    ErrorMessage = "File Is Successfully Uploaded";
                    return ErrorMessage;
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Upload Container Should Not Be Empty or Contact Admin";
                return ErrorMessage;
            }
        }
    }
}