using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record RegisterRequest(string Username, string Password, string Email);
    public record RegisterResponse(string Username, string Email);
}
