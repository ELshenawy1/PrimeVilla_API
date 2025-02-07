using PrimeVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace PrimeVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);

    }
}


//!!!!!!!!!!!!!!! Can we call the repository "Data Layer"!!!!!!!!!!!!!!!! Remove Me