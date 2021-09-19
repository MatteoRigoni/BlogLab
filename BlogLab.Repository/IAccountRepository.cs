using BlogLab.Core.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CrateASync(ApplicationUserIdentity user, CancellationToken cancellationToken);
        Task<ApplicationUserIdentity> GetByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken);
    }
}
