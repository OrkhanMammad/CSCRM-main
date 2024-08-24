using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.InvoiceVMs;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Concretes
{
    public class InvoiceService : IInvoiceService
    {
        readonly AppDbContext _context;
        readonly ILogger<InvoiceService> _logger;
        public InvoiceService(AppDbContext context, ILogger<InvoiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<InvoicePageMainVm> GetInvoiceAsync(int clientId)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve invoice details for client ID: {ClientId}", clientId);

                // Get client orders
                var clientOrders = await _context.Clients
                    .Where(c => c.Id == clientId)
                    .Select(c => new GetClientOrdersForInvoiceVM
                    {
                        ClientId = c.Id,
                        InvCode = c.InvCode,
                        Name = c.Name,
                        Surname = c.Surname,                        
                        CompanyName = c.Company,
                        SalesAmount = c.SalesAmount
                    }).FirstOrDefaultAsync();

                if (clientOrders == null)
                {
                    _logger.LogWarning("Client orders not found for client ID: {ClientId}", clientId);
                    return new InvoicePageMainVm
                    {
                        ClientOrders = null,
                        Company = null
                    };
                }

                _logger.LogInformation("Client orders retrieved successfully for client ID: {ClientId}", clientId);

                // Get company details
                var company = await _context.Companies
                    .Where(c => !c.IsDeleted && c.Name == clientOrders.CompanyName)
                    .Select(c => new GetCompanyVM
                    {
                        Name = c.Name,
                        Address = c.Address,
                        Email = c.Email,
                        Id = c.Id,
                        Phone = c.Phone
                    }).FirstOrDefaultAsync();

                if (company == null)
                {
                    _logger.LogWarning("Company not found for company name: {CompanyName}", clientOrders.CompanyName);
                }
                else
                {
                    _logger.LogInformation("Company details retrieved successfully for company name: {CompanyName}", clientOrders.CompanyName);
                }

              

                // Construct response object
                var invoicePageMainVm = new InvoicePageMainVm
                {
                    ClientOrders = clientOrders,
                    Company = company
                };

                _logger.LogInformation("Invoice details successfully assembled for client ID: {ClientId}", clientId);

                return invoicePageMainVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving invoice details for client ID: {ClientId}", clientId);
                return new InvoicePageMainVm
                {
                    ClientOrders = null,
                    Company = null
                };
            }
        }

    }
}
