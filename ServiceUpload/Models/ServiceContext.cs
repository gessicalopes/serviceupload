using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcServiceUpload.Models
{
    public class ServiceContext : DbContext
    {
        public ServiceContext() : base("name=ServiceContext")
        {
        }
        public DbSet<Upload> Uploads { get; set; }
    }
}