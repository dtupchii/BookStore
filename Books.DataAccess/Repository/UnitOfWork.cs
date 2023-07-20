using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfwork
    {
        private ApplicationDBContext _db;
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
