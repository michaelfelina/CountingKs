using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CountingKs.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
        public string CurrentUser => Thread.CurrentPrincipal.Identity.Name;
    }
}