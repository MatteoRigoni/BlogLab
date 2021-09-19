using BlogLab.Core.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogLab.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUserIdentity user);
    }
}
