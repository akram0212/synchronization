using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    public class FTPServer
    {

        Page p = new Page();
        public FTPServer()
        {

        }

        public bool UploadFile(String sourceFilePath, String destinationFilePath, ref String retunMessage)
        {
            WebClient ftpConnection = new WebClient();
            ftpConnection.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);

            try
            {
                ftpConnection.UploadFile("ftp://" + BASIC_MACROS.FTP_SERVER_IP + destinationFilePath, sourceFilePath);
            }
            catch (WebException exception)
            {
                retunMessage = exception.Message;
                return false;
            }

            return true;
        }
        public bool UploadFile(String sourceFilePath, String destinationFilePath, int severityLevel, ref String retunMessage)
        {
            WebClient ftpConnection = new WebClient();
            ftpConnection.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);

            try
            {
                ftpConnection.UploadFile("ftp://" + BASIC_MACROS.FTP_SERVER_IP + destinationFilePath, sourceFilePath);
            }
            catch (WebException exception)
            {
                retunMessage = exception.Message;
                return false;
            }

            return true;
        }

        public bool DownloadFile(String sourceFilePath, String destinationFilePath, ref String retunMessage)
        {
            WebClient ftpConnection = new WebClient();
            ftpConnection.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);

            try
            {
                ftpConnection.DownloadFile("ftp://" + BASIC_MACROS.FTP_SERVER_IP + sourceFilePath, destinationFilePath);
            }
            catch (WebException exception)
            {
                retunMessage = exception.Message;
                return false;
            }



            return true;
        }

        public bool DownloadFolder(String sourceFolderPath, String destinationFolderPath, ref String retunMessage)
        {
            List<String> FoldersAndFiles = new List<string>();

            Directory.CreateDirectory(destinationFolderPath);

            if (!ListFilesInFolder(sourceFolderPath, ref FoldersAndFiles, ref retunMessage))
                return false;

            for (int k = 0; k < FoldersAndFiles.Count; k++)
            {
                if (!FoldersAndFiles[k].Contains("."))
                {
                    if (!DownloadFolder(sourceFolderPath + FoldersAndFiles[k] + "/", destinationFolderPath + FoldersAndFiles[k] + "\\", ref retunMessage))
                        return false;
                }

                else
                {

                    if (!DownloadFile(sourceFolderPath + FoldersAndFiles[k], destinationFolderPath + FoldersAndFiles[k], ref retunMessage))
                        return false;
                }
            }

            return true;
        }

        public bool DownloadFile(String sourceFilePath, String destinationFilePath, int severityLevel, ref String retunMessage)
        {
            WebClient ftpConnection = new WebClient();
            ftpConnection.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);

            try
            {
                ftpConnection.DownloadFile("ftp://" + BASIC_MACROS.FTP_SERVER_IP + sourceFilePath, destinationFilePath);
            }
            catch (WebException exception)
            {
                retunMessage = exception.Message;
                return false;
            }
            return true;
        }

        public bool CheckExistingFolder(String directoryPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);

                request.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Timeout = 10000;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {

                }
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    FtpWebResponse exceptionResponse = (FtpWebResponse)exception.Response;

                    if (exceptionResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        return false;
                }
            }

            return true;
        }

        public bool CreateNewFolder(String directoryPath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
                request.KeepAlive = false;
                request.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception exception)
            {
                return false;
            }

            return true;
        }

        public bool ListDirectory(String directoryPath, ref List<string> mFiles, ref String retunMessage)
        {
            try
            {
                //var request = createRequest(WebRequestMethods.Ftp.ListDirectory);
                var ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
                ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                //ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                string temp = reader.ReadLine();
                                if (temp.Contains(".pdf") || temp.Contains(".doc") || temp.Contains(".docs"))
                                    mFiles.Add(temp);
                            }
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                retunMessage = "File download failed! Please check your internet connection and try again.";
                return false;
            }

            return true;
        }

        public bool ListFilesInFolder(String directoryPath, ref List<string> mFiles, ref String retunMessage)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
                ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Timeout = 10000;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());


                    string line = streamReader.ReadLine();

                    while (!string.IsNullOrEmpty(line))
                    {
                        if (!line.Contains("/.") && !line.Contains("..") && line != ".")
                            mFiles.Add(line);
                        line = streamReader.ReadLine();
                    }

                    streamReader.Dispose();
                    //streamReader.Close();
                    //response.Dispose();
                    //response.Close();
                    //ftpRequest.Abort();
                }
            }
            catch (WebException exception)
            {
                //retunMessage = "File download failed! Please check your internet connection and try again.";
                retunMessage = exception.Message;
                return false;
            }

            return true;
        }


        public void DeleteFtpDirectory(String directoryPath)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
            ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
            ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

            ftpRequest.GetResponse().Close();

        }
        public bool DeleteFtpDirectory(String directoryPath, int severityLevel, ref String retunMessage)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
                ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

                ftpRequest.GetResponse().Close();
            }
            catch (WebException exception)
            {
                if (severityLevel == BASIC_MACROS.SEVERITY_HIGH)
                {
                    retunMessage = "File download failed! Please check your internet connection and try again.";
                    return false;
                }
            }

            return true;
        }
        public void DeleteFtpFile(String directoryPath)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
            ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            ftpRequest.GetResponse().Close();
        }

        public bool DeleteFtpFile(String directoryPath, int severityLevel, ref String retunMessage)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + directoryPath);
                ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                ftpRequest.GetResponse().Close();
            }
            catch (WebException exception)
            {
                if (severityLevel == BASIC_MACROS.SEVERITY_HIGH)
                {
                    retunMessage = "File download failed! Please check your internet connection and try again.";
                    return false;
                }
            }

            return true;
        }

        public bool CheckIfFileExistsOnServer(string fileName)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + fileName);
            ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
            ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

        //public void GetModificationTime(String directoryPath = "")
        //{

        //    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + "/erp_system/products_photos/Async");

        //    ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
        //    ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;

        //    FtpWebResponse Response = (FtpWebResponse)ftpRequest.GetResponse();


        //    DateTime LastModified = Response.LastModified;
        //    Response.Close();

        //    File.SetLastWriteTime(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source\01electronics_crm\Track.txt", LastModified);

        //}


        //public bool CheckDateChanged() {

        //    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + BASIC_MACROS.FTP_SERVER_IP + "/erp_system/products_photos/Async");

        //    ftpRequest.Credentials = new NetworkCredential(BASIC_MACROS.FTP_SERVER_USERNAME, BASIC_MACROS.FTP_SERVER_PASSWORD);
        //    ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;

        //    FtpWebResponse Response = (FtpWebResponse)ftpRequest.GetResponse();


        //    DateTime LastModified = Response.LastModified;

        //    DateTime localDateModify=File.GetLastWriteTime(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+@"\source\01electronics_crm\Track.txt");

        //    Response.Close();

        //    if (LastModified == localDateModify)
        //        return false;
        //    else
        //        return true;

        //}


        //public void GetFileParsing() {
        //    //check the number of lines retrieved with the current to check what to update

        //    string error = "";
        //    DownloadFile("/erp_system/products_photos/Async", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\Async.txt", ref error);

        //    File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\Async.txt","Path"+","+"Action"+","+DateTime.Now+'\n');
        //    File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\Async.txt", "Path" + "," + "Action" + "," + DateTime.Now + '\n');
        //    File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\Async.txt", "Path" + "," + "Action" + "," + DateTime.Now + '\n');

        //  string[]serverLines=File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\01 Electronics\\products_photos\\Async.txt");
        //  string[]localLines=File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source\01electronics_crm\Track.txt");

        //   int rows=serverLines.Length - localLines.Length;

        //    for (int i = 0; i < rows; i++) {
        //        File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\source\01electronics_crm\Track.txt", serverLines[(serverLines.Length - 1) - i]);
        //    }

        //    GetModificationTime();



        //}





    }
}
