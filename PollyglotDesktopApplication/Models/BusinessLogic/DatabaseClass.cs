using System;

namespace PollyglotDesktopApp.Models.BusinessLogic
{
    // Klasa bazowa dla logiki biznesowej
    public class DatabaseClass
    {
        #region Pola
        protected PollyglotDBEntities db;
        #endregion

        #region Konstruktor
        public DatabaseClass(PollyglotDBEntities db)
        {
            this.db = db;
        }
        #endregion
    }
}
