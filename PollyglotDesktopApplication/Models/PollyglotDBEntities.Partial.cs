using System.Data.Entity;

namespace PollyglotDesktopApp.Models
{
    public partial class PollyglotDBEntities
    {
        public new DbSet<UczenKurs> UczenKurs => Set<UczenKurs>();
    }
}
