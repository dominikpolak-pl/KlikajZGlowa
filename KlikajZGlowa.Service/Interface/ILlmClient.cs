using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface ILlmClient
    {
        Task<string> GetStructuredCompletionAsync(string systemPrompt, string userPrompt);
    }
}
