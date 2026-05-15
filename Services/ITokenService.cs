using System.Collections.Generic;

namespace ExampleOrderService.Services
{
    public interface ITokenService
    {
        string CreateToken(string userName, IEnumerable<string>? roles = null);
    }
}