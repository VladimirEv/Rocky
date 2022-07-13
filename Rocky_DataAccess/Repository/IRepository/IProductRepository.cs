using Rocky_Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    { 
        void Update(Product obj);

        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
