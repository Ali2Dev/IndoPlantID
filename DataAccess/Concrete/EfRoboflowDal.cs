using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;

namespace DataAccess.Concrete
{
    public class EfRoboflowDal : EfEntityRepositoryBase<RoboflowResponse, IndoPlantDb>, IRoboflowDal
    {
    }
}
