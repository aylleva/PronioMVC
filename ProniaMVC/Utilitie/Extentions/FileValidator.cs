using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProniaMVC.Models;
using ProniaMVC.Utilitie.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace ProniaMVC.Utilitie.Extentions
{
    public static  class FileValidator
    {
        public static bool CheckType(this IFormFile file,string type)
        {
            if(file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }

        public static bool CheckFileSize(this IFormFile file,FileSize filesize,int size)
        {
            switch (filesize)
            
            { 
                case FileSize.KB:
                    return file.Length <= size * 1024;
                case FileSize.MB:
                    return file.Length <= size * 1024 * 1024;
            }
            return false;   

        }

         private  static string CreatePath(this string filename,params string[] roots)
        {
            string path= string.Empty;
            for(int i = 0; i < roots.Length; i++)
            {
                path=Path.Combine(path, roots[i]);
            }
            path = Path.Combine(path, filename);
            return path;
        }



        public static async Task<string> CreateFileAsync(this IFormFile File, params string[] roots)
        {
            string file = File.FileName;

            string Filename = string.Concat(Guid.NewGuid().ToString(), file.Substring(file.LastIndexOf(".")));

            using (FileStream fileStream = new(Filename.CreatePath(roots), FileMode.Create))
            {
                await File.CopyToAsync(fileStream);
            }
          
            return Filename;

        
           
        }

        public static void DeleteFile(this string  file,params string[] roots)
        {
            File.Delete(file.CreatePath(roots));
        }
    }
}
