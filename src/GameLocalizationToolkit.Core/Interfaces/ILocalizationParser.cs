using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationParser
    {
        LocalizationFile Parse(string filePath, IEnumerable<string> lines);
    }
}
