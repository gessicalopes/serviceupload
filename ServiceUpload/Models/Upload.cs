using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcServiceUpload.Models
{
    public class Upload
    {

        public int ID { get; set; }


        public byte[] Arquivo { get; set; }
 

        public string Descricao { get; set; }
    }
}