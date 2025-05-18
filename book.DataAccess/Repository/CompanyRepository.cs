using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using book.DataAccess.Data;
using book.DataAccess.Repository.IRepository;
using book.Models;

namespace book.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void Update(Company obj)
        {
            _db.Companies.Update(obj);
        }
    }
}
