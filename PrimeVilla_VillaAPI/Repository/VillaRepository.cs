using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrimeVilla_VillaAPI.Controllers;
using PrimeVilla_VillaAPI.Data;
using PrimeVilla_VillaAPI.Models;
using PrimeVilla_VillaAPI.Repository.IRepository;
using System.Linq.Expressions;

namespace PrimeVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }   
        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
