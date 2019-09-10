using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QConsoleWeb.BLL.DTO;

namespace QConsoleWeb.BLL.Interfaces
{
    public interface ISessionService
    {
        IEnumerable<SessionDTO> GetSessions();
    }
}
