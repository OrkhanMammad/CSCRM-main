using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models.ResponseTypes;

namespace CSCRM.Concretes
{
    public class ClientService : IClientService
    {
        readonly AppDbContext _context;
        public ClientService(AppDbContext context)
        {
         _context = context;
        }
        public Task<BaseResponse> GetClientsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
