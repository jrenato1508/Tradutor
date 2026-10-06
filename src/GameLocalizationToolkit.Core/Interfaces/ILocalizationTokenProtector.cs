using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationTokenProtector
    {
        ProtectedLocalizationText Protect(string text);

        string Restore(ProtectedLocalizationText protectedText);
    }
}
