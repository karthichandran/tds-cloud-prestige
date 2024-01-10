using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReProServices.Infrastructure.MegaDrive
{
    public class MegaDriveService
    {
        const string megaEmail = "karthikbalaj.c@gmail.com";
        const string megaPwd = "Matrix@mega";

        public MegaDriveService() { 
        
        }

        public async Task<bool> UploadFile(MemoryStream blob,string fileName) {
            try
            {
                var client = new MegaApiClient();
                var session= client.Login(megaEmail, megaPwd);
                var status = client.IsLoggedIn;
                var nodes = client.GetNodes();
                var root = nodes.First(x => x.Type == NodeType.Directory);
                //Uri fileLink = new Uri("https://mega.nz/folder/DJxCVA6Z#ekiHMRqfEUjp3DN0-R1sfg");
                //https://mega.nz/folder/DJxCVA6Z#2FtTNYl_4OfvS2pCG3I6Lg
                //var node = client.GetNodeFromLink(fileLink);
                //var linl = client.GetDownloadLink(root);
                //var childNodes = client.GetNodes(root);
                //var fileNode = childNodes.First(x => x.Name.Contains("ABWxxxxx9L_2024-25_AK11138746-1"));
                // var file =client.Download(fileNode);

                await client.UploadAsync(blob, fileName, root);
                await client.LogoutAsync();
                return true;
            }
            catch (Exception ex) {
                return false;
            }

        }

        public async Task<MemoryStream> DownloadFile(string fileName) {
            try
            {
                var client = new MegaApiClient();
                var session = client.Login(megaEmail, megaPwd);
                var nodes = client.GetNodes();
                var root = nodes.First(x => x.Type == NodeType.Directory);
                var childNodes = client.GetNodes(root);
                var fileNode = childNodes.First(x => x.Name.Contains(fileName));
                var file = client.Download(fileNode);
                await client.LogoutAsync();

                var ms = new MemoryStream();
                file.CopyTo(ms);
                return ms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteFile(string fileName)
        {
            try
            {
                var client = new MegaApiClient();
                var session = client.Login(megaEmail, megaPwd);
                var nodes = client.GetNodes();
                var root = nodes.First(x => x.Type == NodeType.Directory);
                var childNodes = client.GetNodes(root);
                var fileNode = childNodes.First(x => x.Name.Contains(fileName));
                client.Delete(fileNode);
                await client.LogoutAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
