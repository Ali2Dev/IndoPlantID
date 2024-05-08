using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RegionCoordinateManager : IRegionCoordinateService
    {
        IRegionCoordinateDal _regionCoordinateDal;

        public RegionCoordinateManager(IRegionCoordinateDal regionCoordinateDal)
        {
            _regionCoordinateDal = regionCoordinateDal;
        }

        public void Add(RegionCoordinate entity)
        {
            _regionCoordinateDal.Add(entity);
        }

        public void AddRange(IEnumerable<RegionCoordinate> entities)
        {
            _regionCoordinateDal.AddRange(entities);
        }

        public List<RegionCoordinate> GetAll(Expression<Func<RegionCoordinate, bool>> filter = null)
        {
            return _regionCoordinateDal.GetAll(filter);
        }
    }

}
