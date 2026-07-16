using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationMerger
    {
        LocalizationFile Merge(LocalizationFile source, LocalizationFile target);
    }
}
