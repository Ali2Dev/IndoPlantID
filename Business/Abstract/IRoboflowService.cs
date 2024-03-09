using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IRoboflowService
    {
        List<RoboflowResponse> GetAll();

        RoboflowResponse GetById(int id);

        //List<RoboflowResponse> GetByUserId(string userId);
        void Add(RoboflowResponse entity);

        void Delete(RoboflowResponse entity);

        RoboflowResponse GetResponse(byte[] image, string fileName);
    }
}
