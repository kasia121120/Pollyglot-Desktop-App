using PollyglotDesktopApp.Models.ForAllView;
using System.Linq;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    public class PlatnoscB : DatabaseClass
    {
        public PlatnoscB(PollyglotDBEntities db) : base(db) { }

        public IQueryable<KeyAndValue<string>> GetOkresyKeyAndValueItems()
        {
            var okresy = (
                from p in db.Platnosc
                where p.Okres != null && p.Okres != ""
                select p.Okres
            )
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

            return okresy
                .Select(o => new KeyAndValue<string>
                {
                    Key = o,
                    Value = o
                })
                .AsQueryable();
        }

        public IQueryable<KeyAndValue<int>> GetGrupyKeyAndValueItems()
        {
            var grupy = (
                from g in db.Grupa
                where g.Status == "aktywna"
                select new { g.GrupaId, g.Nazwa }
            ).ToList();

            return grupy
                .Select(g => new KeyAndValue<int>
                {
                    Key = g.GrupaId,
                    Value = g.Nazwa
                })
                .AsQueryable();
        }
    }
}
