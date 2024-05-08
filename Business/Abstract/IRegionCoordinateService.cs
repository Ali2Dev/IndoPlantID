using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IRegionCoordinateService
    {
        List<RegionCoordinate> GetAll(Expression<Func<RegionCoordinate, bool>> filter = null);

        void Add(RegionCoordinate entity);
        void AddRange(IEnumerable<RegionCoordinate> entities);
    }
}
