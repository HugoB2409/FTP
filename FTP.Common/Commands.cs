using System;
using System.IO;

namespace FTP.Common
{
    public static class Commands
    {
        public static string Pwd(string path)
        {
            return path;
        }
        
        public static string Ls(string path)
        {
            var pathLenght = path.Length;
            var files = "";
            foreach (var file in Directory.GetFiles(path))
            {
                files += ($"{file.Remove(0, pathLenght)} \n");
            }
            foreach (var directory in Directory.GetDirectories(path))
            {
                files += ($"{directory.Remove(0, pathLenght)} \n");
            }

            if (files == "")
            {
                files = "This directory is empty.";
            }
            return files;
        }
        
        public static string Cd(string path, string currentPath)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
            
            Console.WriteLine("The specified directory does not exist.");
            return currentPath;
        }
        
        public static string CdWithRestriction(string path, string rootFolder)
        {
            var di1 = new DirectoryInfo(path);
            var di2 = new DirectoryInfo(rootFolder);
            var isParent = false;
            while (di2.Parent != null)
            {
                if (di2.Parent.FullName == di1.FullName)
                {
                    isParent = true;
                    break;
                }
                else di2 = di2.Parent;
            }

            if (!isParent)
            {
                return path;
            }
            else
            {
                Console.WriteLine("You dont have permission.");
                return rootFolder;
            }
        }
    }
}