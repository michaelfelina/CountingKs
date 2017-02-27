using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CountingKs.Data;
using CountingKs.Models;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly ICountingKsRepository _repo;
        private ModelFactory _modelFactory;
        public BaseApiController(ICountingKsRepository repo)
        {
            _repo = repo;
        }

        protected ModelFactory modelFactory
        {
            get
            {
                if (_modelFactory == null) _modelFactory = new ModelFactory(this.Request, repo);
                return _modelFactory;
            }
            
        }

        protected ICountingKsRepository repo
        {
            get { return _repo; }
        }
    }
}
