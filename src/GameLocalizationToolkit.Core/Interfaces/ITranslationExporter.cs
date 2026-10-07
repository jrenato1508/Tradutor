using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ITranslationExporter
    {
      TranslationExportResult Export(IEnumerable<LocalizationEntry> entries,string outputDirectoryPath,int entriesPerFile = 500);
    }
}
