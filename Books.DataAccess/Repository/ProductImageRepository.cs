using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAccess.Repository
{
    public class ProductImageRepository :  Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDBContext _db;
        public ProductImageRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }


        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
