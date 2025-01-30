using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ExternalSystems.Fizikal
{
    public interface IFizikalHandler
    {
        Task<string> GetClassScheduleAsync();
        Task<string> GetClassCategories();
    }
}
