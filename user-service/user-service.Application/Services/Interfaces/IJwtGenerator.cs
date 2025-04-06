using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace user_service.Application.Services.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(Guid userId, string email, IList<string> roles);
    }
}