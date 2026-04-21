using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capybara.Tool
{
    public class DescriptionAttribute : Attribute
    {
        public string description { get; set; } = string.Empty;
        public DescriptionAttribute(string description, bool isOSType = false)
        {
            this.description = description;
            if (isOSType)
            {
                this.description += (Environment.OSVersion.Platform == PlatformID.Win32NT ? "Windows系统." : "Linux系统.");
            }
        }
    }
}
