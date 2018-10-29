using System.Configuration;

namespace BCL.Configuration
{
    public class DirectoryElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string Path => (string)this["path"];
    }
}
