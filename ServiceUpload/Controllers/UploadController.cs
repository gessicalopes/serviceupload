using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MvcServiceUpload.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;

namespace MvcApplication1.Controllers
{
    public class UploadController : ApiController
    {
            private ServiceContext db = new ServiceContext();
           
            // GET api/upload
            public List<Upload> Get()
            {
                List<Upload> uploads = db.Uploads.ToList();
                if (uploads == null)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }
                return uploads;
            }

            // POST api/upload
            [HttpPost] 
            public async Task<HttpResponseMessage> Upload()
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }

                try
                {
                    var provider = GetMultipartProvider();
                    var result = await Request.Content.ReadAsMultipartAsync(provider);
                                     
                    var imagem = new Upload();

                    if (result.FileData.Count() > 0)
                    {
                        //  ARQUIVO DE INFORMACAO
                        var arquivoParaInformacao = result.FileData[0].LocalFileName;

                        var fileExt = result.FileData[0].Headers.ContentDisposition.FileName.ToString().Split('.').Last().Replace("\\", "");
                        string nomeArquivoInformacao = result.FileData[0].Headers.ContentDisposition.FileName.ToString().Replace("\"", "");
                     
                        imagem.Arquivo = RetornaArquivoEmBytes(arquivoParaInformacao);
                        imagem.Descricao = nomeArquivoInformacao;

                        // verificar se os arquivos foram enviados novamente
                        db.Uploads.Add(imagem);
                        db.SaveChanges();

                        //RemoveArquivoTemporario(result.FileData[0].LocalFileName);
                    }
                }
                catch (Exception)
                {
                    string temErro = "Arquivo de informação não foi enviado para o servidor. Verifique as configurações iniciais para o funcionamento do sistema.";
                    return this.Request.CreateResponse(HttpStatusCode.OK, new { temErro });
                    throw;
                }

                string sucesso = "Arquivo Informação inserido";
                return this.Request.CreateResponse(HttpStatusCode.OK, new { sucesso });
            }
    
            private MultipartFormDataStreamProvider GetMultipartProvider()
            {
                // IMPORTANT: replace "(tilde)" with the real tilde character
                // (our editor doesn't allow it, so I just wrote "(tilde)" instead)
                var uploadFolder = "~/App_Data/Tmp/Uploads"; // you could put this to web.config
                var root = HttpContext.Current.Server.MapPath(uploadFolder);
                Directory.CreateDirectory(root);
                return new MultipartFormDataStreamProvider(root);
            }

            private byte[] RetornaArquivoEmBytes(string enderecoArquivo)
            {
                FileStream fs = new FileStream(enderecoArquivo, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                int numBytes = (int)new FileInfo(enderecoArquivo).Length;
                byte[] arquivo = br.ReadBytes(numBytes);
                return arquivo;
            }

            public HttpResponseMessage Delete(int id)
            {
                Upload usuario = db.Uploads.Find(id);
                if (usuario == null){
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                db.Uploads.Remove(usuario);
                try{
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
                return Request.CreateResponse(HttpStatusCode.OK, usuario);
            }
    }
}