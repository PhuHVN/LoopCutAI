using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IKeyService
    {
        Task<string> GetApiKeyAsync();
        Task RefreshApiKeysAsync();
        void ReportUsedKey(string key);
    }
}
